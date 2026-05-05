# Current-State Analysis: Assumptions About RimWorld Pawn Types

**Scope:** Full codebase analysis of explicit and implicit assumptions made about the kind of `Pawn` being processed by MoreInjuries.  
**Related issue:** [#167](https://github.com/frederik-hoeft/rimworld-more-injuries/issues/167)  
**Base assumption for this analysis:** All modded races use `Pawn` as their base type. Any other polymorphic base types are out of scope.

---

## Table of Contents

1. [Phase 1 — XML-Based Assumptions](#phase-1--xml-based-assumptions)
   - [Patch_Core.xml](#patch_corexml)
   - [Patch_BodyParts.xml](#patch_bodypartsxml)
   - [Patch_Surgeries.xml](#patch_surgeriesxml)
   - [Patch_BloodLoss.xml](#patch_bloodlossxml)
   - [Patch_Hypothermia.xml](#patch_hypothermiaxml)
   - [Patch_Bruise.xml and Patch_HearingLoss.xml](#patch_bruisexml-and-patch_hearinglossxml)
   - [Mod_HumanoidAlienRaces.xml](#mod_humanoidalienracesxml)
   - [Mod_CombatExtended.xml](#mod_combatextendedxml)
   - [Dlc_Biotech.xml](#dlc_biotechxml)
2. [Phase 2 — C#-Based Assumptions](#phase-2--c-based-assumptions)
   - [Component Bootstrap (MoreInjuryComp)](#component-bootstrap-moreinjurycomp)
   - [Injury Workers](#injury-workers)
   - [Hediff Comps and Secondary Condition Modifiers](#hediff-comps-and-secondary-condition-modifiers)
   - [Tourniquet System](#tourniquet-system)
   - [WorkGivers and AI Scheduling](#workgivers-and-ai-scheduling)
   - [Medical Device Helpers and Job Drivers](#medical-device-helpers-and-job-drivers)
   - [Extensions and Caching Layers](#extensions-and-caching-layers)
3. [Summary Table](#summary-table)

---

## Phase 1 — XML-Based Assumptions

### Patch_Core.xml

**File:** `Patches/Patch_Core.xml`

#### 1. `MoreInjuryCompProperties` is only injected into `ThingDef[defName="Human"]`

```xml
<xpath>/Defs/ThingDef[defName="Human"]/comps</xpath>
```

The entire MoreInjuries injury pipeline (`MoreInjuryComp`) is bootstrapped exclusively by patching the vanilla `Human` `ThingDef`. Any modded pawn race that does **not** also have this comp injected (either by MoreInjuries itself via mod-specific patches, or by the third-party mod) will be **completely excluded** from the injury pipeline. This is the single most important gating assumption in the entire mod.

See also: [Mod_HumanoidAlienRaces.xml](#mod_humanoidalienracesxml) for the partial mitigation for HAR races.

#### 2. All `Hediff_Injury` instances are globally replaced with `BetterInjury`

```xml
<xpath>/Defs/HediffDef[hediffClass="Hediff_Injury"]/hediffClass</xpath>
```

This replacement is applied to **every** `HediffDef` whose `hediffClass` is `Hediff_Injury`, regardless of race. As a consequence, `BetterInjury` is active for **all** pawns in the game — including animals, robots, and non-human races — even if they do not have `MoreInjuryComp`. The only race-specific logic inside `BetterInjury` is governed by settings and hediff state flags; there are no explicit race checks in the `BetterInjury` class itself, so this is generally safe.

#### 3. All `Hediff_MissingPart` instances are globally replaced with `BetterMissingPart`

Same scope as the `BetterInjury` replacement above.

---

### Patch_BodyParts.xml

**File:** `Patches/Patch_BodyParts.xml`

All six operations in this file target **only** `BodyDef[defName="Human"]`:

| Added Body Part | Parent in Human Body | Notes |
|---|---|---|
| `Abdomen` | core part (`Torso`) | Container for intestines |
| `SmallIntestine` | `Abdomen` | MoreInjuries-specific |
| `LargeIntestine` | `Abdomen` | MoreInjuries-specific |
| `FemoralArtery` | left/right `Leg` | MoreInjuries-specific |
| `PoplitealArtery` | left/right `Leg` | MoreInjuries-specific |
| `SpinalCord` | `Spine` | MoreInjuries-specific |

**Implication:** Any C# worker that checks for these body parts on a non-human pawn will gracefully skip (no match found), but the corresponding injuries — intestinal spill, paralysis, and artery lacerations — will **never fire** for non-human races. This is a feature gap, not a crash risk.

---

### Patch_Surgeries.xml

**File:** `Patches/Patch_Surgeries.xml`

Surgery recipes are added exclusively to `ThingDef[defName="Human"]`:

```xml
<xpath>/Defs/ThingDef[defName="Human"]/recipes</xpath>
```

Recipes added: `RepairCollapsedLung_Industrial`, `RepairCollapsedLung_Ultratech`, `ExtractWholeBloodBag`, `RepairFracture`, `SplintFracture`, `InstallBionicSmallIntestine`, `RemoveHemorrhagicStroke_*`, `RestoreHearing`, `TreatParalysis`, `TreatBrainDamage`.

**Implication:** These surgeries are unavailable for non-human races even if they carry the equivalent hediffs. A modded race with a spinal cord would not have `TreatParalysis` available by default.

---

### Patch_BloodLoss.xml

**File:** `Patches/Patch_BloodLoss.xml`

The `BloodLoss` hediff is a vanilla hediff that is modified globally (not tied to a specific race). The modifications are:

1. Forces `hediffClass` to `HediffWithComps` (globally).
2. Adds secondary condition handlers for hypovolemic shock and hypoxia.
3. The hypoxia secondary condition targets `Brain` by name via `BodyPartHediffTargetEvaluator_Single`:

   ```xml
   <targetEvaluator Class="...BodyPartHediffTargetEvaluator_Single">
     <target>Brain</target>
   </targetEvaluator>
   ```

   If the affected pawn has no `Brain` body part, the `BodyPartHediffTargetEvaluator_Single.GetTargetBodyPart` method returns `null` and the hypoxia effect is silently skipped. This is **graceful**.

4. Both the hypovolemic shock and hypoxia handlers include `HediffModifier_DisallowUntracked`, which checks `pawn.HasComp<MoreInjuryComp>()` and returns `0` (no effect) for pawns not tracked by MoreInjuries. This is the primary **safety gate** preventing secondary blood loss effects from firing on unmanaged pawns.

5. The `lethalSeverity` of `BloodLoss` is set to `1.001` globally, which makes blood loss non-lethal by default for **all** pawns. `HediffCompHandler_SecondaryCondition_BloodLossDeath` manually bypasses this to kill tracked pawns. Untracked pawns are protected by `HediffModifier_RequireUntracked` in the backup handler.

---

### Patch_Hypothermia.xml

**File:** `Patches/Patch_Hypothermia.xml`

The `Hypothermia` hediff is modified globally. Secondary conditions include:

1. **Coagulopathy** (via `LinkedSeverityProperties_ModExtension`): Applied to any pawn suffering from hypothermia, no race filter.
2. **Cardiac arrest** targeting `Heart` by name:

   ```xml
   <targetEvaluator Class="...BodyPartHediffTargetEvaluator_Single">
     <target>Heart</target>
   </targetEvaluator>
   ```

   If the pawn has no `Heart` body part, the evaluator returns `null` and the cardiac arrest is silently skipped. This is **graceful**.

3. Handlers use `HediffModifier_DisallowHidden` and `HediffModifier_DisallowUntracked` where appropriate, but the coagulopathy severity link (via `LinkedSeverityProperties_ModExtension`) does **not** use `HediffModifier_DisallowUntracked`. Coagulopathy severity is thus adjusted for **any** pawn with hypothermia, including untracked ones.

---

### Patch_Bruise.xml and Patch_HearingLoss.xml

**Files:** `Patches/Patch_Bruise.xml`, `Patches/Patch_HearingLoss.xml`

Both add `HediffCompProperties_CausedBy` to their respective vanilla hediffs globally. This is purely metadata tracking (recording the cause of a secondary injury) and has no race-specific behavior.

---

### Mod_HumanoidAlienRaces.xml

**File:** `Patches/Mod_HumanoidAlienRaces.xml`

When the *Humanoid Alien Races* (HAR) mod is active, `MoreInjuryCompProperties` is injected into:
- `AlienRace.ThingDef_AlienRace` defs where `alienRace/compatibility[isFlesh="true"]` is specified, **or**
- `AlienRace.ThingDef_AlienRace` defs that have **no** compatibility node (implicitly assumed to be flesh).

**Explicit assumption:** HAR races with `isFlesh=false` are excluded and receive no MoreInjuries processing.

**Implicit assumption:** Any HAR flesh race is assumed to have a compatible body structure for the injury workers. Specifically:
- The race needs at least some of the standard vanilla body parts (e.g., `Lung`, `Brain`, `Heart`, `Torso`) for the relevant workers to do anything useful.
- MoreInjuries-specific body parts (SpinalCord, FemoralArtery, etc.) are **not** patched into HAR body defs. Those workers will silently skip.
- The surgery recipes are **not** added to HAR race defs. Surgeries remain unavailable.
- The HAR body def coverage values are **not** adjusted (only `BodyDef[defName="Human"]` is adjusted by `Mod_CombatExtended.xml`).

---

### Mod_CombatExtended.xml

**File:** `Patches/Mod_CombatExtended.xml`

All body part coverage adjustments target **only** `BodyDef[defName="Human"]`. Non-human races using CE will not have their coverage values adjusted by MoreInjuries.

---

### Dlc_Biotech.xml

**File:** `Patches/Dlc_Biotech.xml`

- `HediffCompProperties_DeathrestIntegration` is added to `HediffDef[defName="Deathrest"]` globally. Any pawn that can enter Deathrest (a Biotech mechanic) will have this comp, regardless of race. The comp cancels hypovolemic shock and cardiac arrest when the pawn enters Deathrest.
- `HemogenPack` is given `TransfusionProperties_ModExtension` and outcome doer properties. This affects the item def, not any specific race.

---

## Phase 2 — C#-Based Assumptions

### Component Bootstrap (MoreInjuryComp)

**File:** `Source/.../HealthConditions/MoreInjuryComp.cs`

```csharp
public Pawn Pawn => (Pawn)parent;
```

**Hard assumption:** The `Thing` that holds this comp **must** be a `Pawn`. Since `ThingComp` is generic and can be attached to any `ThingWithComps`, this cast would throw if the comp were accidentally attached to a non-`Pawn` thing. In practice this is safe as long as only `ThingDef`s backed by `Pawn` ever carry this comp.

---

### Injury Workers

**Base:** `Source/.../HealthConditions/InjuryWorker.cs` (accesses `MoreInjuryComp.Pawn`, inheriting the `Pawn` cast assumption).

#### AdrenalineWorker

**File:** `Source/.../HealthConditions/AdrenalineRush/AdrenalineWorker.cs`

```csharp
public override bool IsEnabled => MoreInjuriesMod.Settings.EnableAdrenaline && !Pawn.IsShambler;
```

**Assumption:** `Pawn.IsShambler` is expected to be a valid property on the pawn. This is a standard RimWorld property (introduced in 1.5 with Anomaly) and is safe for any `Pawn` subtype. Shamblers will never receive adrenaline effects.

#### FractureWorker

**File:** `Source/.../HealthConditions/Fractures/FractureWorker.cs`

The static `s_lacerationRegistry` dictionary is the authoritative list of breakable bones and their laceration targets. It is hard-coded with Human-specific body part defs:

```
Head:    Skull, Jaw, Nose
Arms:    Clavicle, Humerus, Radius, Hand, Finger
Torso:   Ribcage, Sternum, Spine, Pelvis
Legs:    Femur, Tibia, Foot, Toe
```

Additionally, bone-fragment laceration targets reference Human-specific body parts: `FemoralArtery`, `PoplitealArtery`, `LargeIntestine`, `SmallIntestine`, `Liver`, `Stomach`, `Kidney`, `SpinalCord`.

**Implication:** Fractures only occur on body parts registered in `s_lacerationRegistry`. The check:

```csharp
IEnumerable<BodyPartRecord> affectedBones = damage.parts.Where(bodyPart =>
    s_lacerationRegistry.ContainsKey(bodyPart.def) && ...);
```

...acts as a graceful gate — non-human body parts that are not in the dictionary produce no fractures. The behavior is correct but incomplete for races with anatomy mapped to equivalent structural bones under different `BodyPartDef` names.

#### HeadInjuryWorker

**File:** `Source/.../HealthConditions/HeadInjury/HeadInjuryWorker.cs`

Two assumptions:

1. **`BodyPartGroupDefOf.FullHead` group existence:**

   ```csharp
   if (bodyPart.groups.Contains(BodyPartGroupDefOf.FullHead))
   ```

   This is a standard vanilla group. Any pawn race without a `FullHead` group on its head parts will never trigger head injuries. For races that do define `FullHead`, this works correctly.

2. **Hard-coded severity multipliers per body part def:**

   ```csharp
   _ when bodyPart == KnownBodyPartDefOf.Brain => 3.0f,
   _ when bodyPart == KnownBodyPartDefOf.Skull => 1.5f,
   _ when bodyPart == KnownBodyPartDefOf.Ear   => 1f,
   _ when bodyPart == BodyPartDefOf.Eye        => 0.7f,
   _ when bodyPart == KnownBodyPartDefOf.Nose  => 0.5f,
   _ => 0.75f
   ```

   The fallback of `0.75f` means unknown head body parts still contribute to head injury severity, making this **partially graceful** for non-standard head anatomy.

#### HydrostaticShockWorker

**File:** `Source/.../HealthConditions/HydrostaticShock/HydrostaticShockWorker.cs`

```csharp
&& patient.health.hediffSet.GetBrain() is BodyPartRecord brain
```

**Graceful:** Returns `null` if the pawn has no `Brain` body part and silently skips the hydrostatic shock application. No effect on brainless races.

#### ParalysisWorker

**File:** `Source/.../HealthConditions/Paralysis/ParalysisWorker.cs`

```csharp
if (bodyParts.FirstOrDefault(static bodyPart => bodyPart.def == KnownBodyPartDefOf.SpinalCord) is BodyPartRecord spinalCord
    && !patient.health.hediffSet.PartIsMissing(spinalCord))
```

**Assumption:** `SpinalCord` is a MoreInjuries-specific body part added only to `BodyDef[defName="Human"]` via `Patch_BodyParts.xml`. Any non-human race without a body part of def `SpinalCord` will never suffer paralysis. This is **graceful** (no crash) but is a feature gap for any race that should logically have a spinal cord.

#### IntestinalSpillWorker

**File:** `Source/.../HealthConditions/IntestinalSpill/IntestinalSpillWorker.cs`

```csharp
private static readonly HashSet<BodyPartDef> s_afflictedOrgans =
[
    KnownBodyPartDefOf.SmallIntestine,
    KnownBodyPartDefOf.LargeIntestine,
    KnownBodyPartDefOf.Stomach,
    KnownBodyPartDefOf.Kidney,
    KnownBodyPartDefOf.Liver
];
```

`SmallIntestine` and `LargeIntestine` are MoreInjuries-specific parts added only to the Human body. `Stomach`, `Kidney`, and `Liver` are vanilla parts that may or may not exist on modded races.

**Implication:** The acid-burn spill effect will only fire if the affected body part matches one of the listed defs. For non-human races missing these parts, the effect silently skips. **Graceful**, but incomplete for races with analogous organs under different `BodyPartDef` names.

#### SpallingInjuryWorker

**File:** `Source/.../HealthConditions/SpallingInjury/SpallingWorker.cs`

Two assumptions:

1. **Apparel tracker:**

   ```csharp
   if (patient.apparel?.WornApparel.Count is not > 0)
       return;
   ```

   **Graceful:** Null-safe. If a pawn has no `apparel` component (e.g., a non-humanlike pawn), spalling simply never fires.

2. **Torso body part:**

   ```csharp
   patient.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Torso)
   ```

   Used to filter armor vests by coverage. For any race that defines a `Torso` part (which is standard in vanilla and most modded races), this works correctly.

#### LungCollapsePerforationWorker / LungCollapseThermobaricWorker

**Files:** `Source/.../HealthConditions/LungCollapse/LungCollapsePerforationWorker.cs`, `LungCollapseThermobaricWorker.cs`

Both explicitly check for `BodyPartDefOf.Lung`:

```csharp
|| lung.def != BodyPartDefOf.Lung
```

**Graceful:** No lung → no lung collapse. Silently skips for non-humanlike races or races without lungs.

#### InhalationInjuryWorker

**File:** `Source/.../HealthConditions/InhalationInjury/InhalationInjuryWorker.cs`

```csharp
List<BodyPartRecord> lungs = [.. patient.health.hediffSet.GetNonMissingPartsOfType(BodyPartDefOf.Lung)];
```

**Graceful:** If the race has no lungs, the list is empty and the foreach loop does nothing. Stats used (`StatDefOf.Flammability`, `StatDefOf.ToxicEnvironmentResistance`) are standard RimWorld stats applicable to any pawn.

#### HearingLossWorker

**File:** `Source/.../HealthConditions/HearingLoss/HearingLossWorker.cs`

Two assumptions:

1. **Apparel tracker for ear protection:**

   ```csharp
   if (pawn.apparel is { WornApparel.Count: > 0 })
   ```

   **Graceful:** Null-safe. Without an apparel component, protection factor remains `1.0` (no protection, max hearing damage).

2. **Ear body part groups (injected via `HearingLossWorkerFactory`):**

   The `earGroups` parameter is populated by the factory from the `HearingLossVerbInfoProperties_ModExtension`. Hearing damage is applied to **any** pawn, but ear protection calculation depends on apparel covering the `earGroups`. For non-human races, the `HearingLoss` hediff is still applied (globally patched by `Patch_HearingLoss.xml`) regardless of whether that race has ears; however, the underlying biology (what constitutes an "ear") is only defined for the Human body.

   **Implication:** Non-human races may receive hearing damage (via the globally active `HearingLossWorker`), but ear protection from apparel will only work if the apparel covers one of the configured ear body part groups.

#### ChokingWorker

**File:** `Source/.../HealthConditions/Choking/ChokingWorker.cs`

```csharp
private static bool AffectsBreathing(BodyPartRecord bodyPart)
{
    foreach (BodyPartTagDef tag in bodyPart.def.tags)
    {
        if (tag == BodyPartTagDefOf.BreathingSource || tag == BodyPartTagDefOf.BreathingPathway)
        { ... }
    }
}
```

**Data-driven:** Uses standard vanilla `BodyPartTagDef`s (`BreathingSource`, `BreathingPathway`). Works correctly for any race that properly tags its respiratory body parts with these tags. No hard-coded body part names.

#### EmpShutdownWorker

**File:** `Source/.../HealthConditions/EmpShutdown/EmpShutdownWorker.cs`

```csharp
patient.health.hediffSet.hediffs.Where(static hediff =>
    hediff is { Part: not null, def.addedPartProps.betterThanNatural: true })
```

**No race-specific assumption:** Works for any pawn with bionic hediffs. The `betterThanNatural` flag is a standard `HediffDef` property.

---

### Hediff Comps and Secondary Condition Modifiers

#### HediffComp_PersonalityShift (BrainDamage)

**File:** `Source/.../HealthConditions/BrainDamage/HediffComp_PersonalityShift.cs`

```csharp
if (parent.pawn.skills?.skills is null)
{
    return;
}
```

**Graceful:** Null-safe. If a pawn has no `skills` tracker (e.g., animals or non-humanlike pawns), the skill reshuffling is silently skipped.

#### HediffComp_CardiacArrest

**File:** `Source/.../HealthConditions/CardiacArrest/HediffComp_CardiacArrest.cs`

```csharp
if (ModLister.BiotechInstalled && parent.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
{
    parent.pawn.health.RemoveHediff(parent);
}
```

**Assumption:** Checks for the `Deathrest` hediff (Biotech-specific). For non-Biotech races or non-deathresting pawns, this check always returns `false` and the cardiac arrest proceeds normally. Safe.

#### HediffComp_Choking

**File:** `Source/.../HealthConditions/Choking/HediffComp_Choking.cs`

```csharp
SoundDef soundDef = (coughing, patient.gender) switch
{
    (true, Gender.Female) => KnownSoundDefOf.ChokingCoughFemale,
    (true, _)             => KnownSoundDefOf.ChokingCoughMale,
    _                     => KnownSoundDefOf.Choking,
};
```

**Assumption:** Uses `Pawn.gender` for sound selection. This is a standard RimWorld pawn property. Non-female genders (including `Gender.None` for animals) fall through to the male cough sound. No crash risk.

#### HediffModifier_DisallowDeathResting

**File:** `Source/.../HealthConditions/Secondary/Handlers/Modifiers/HediffModifier_DisallowDeathResting.cs`

```csharp
if (ModLister.BiotechInstalled && hediff.pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
    return 0f;
```

**Assumption:** Checks for Biotech's `Deathrest` hediff. Biotech-specific, no crash risk for non-Biotech pawns.

#### HediffModifier_DisallowUntracked

**File:** `Source/.../HealthConditions/Secondary/Handlers/Modifiers/HediffModifier_DisallowUntracked.cs`

```csharp
if (!hediff.pawn.HasComp<MoreInjuryComp>())
    return 0f;
```

**Role:** This is the primary safety gate for secondary condition handlers. Any pawn without `MoreInjuryComp` is immune to secondary conditions guarded by this modifier. It is used on the most dangerous secondary conditions (blood loss → hypovolemic shock, blood loss → hypoxia, hypothermia → cardiac arrest) to ensure they only affect managed pawns.

**Coverage gap:** Not all secondary condition handlers use this modifier. Coagulopathy severity linking via `LinkedSeverityProperties_ModExtension` in `Patch_Hypothermia.xml` does **not** include this guard, meaning coagulopathy is adjusted for any pawn with hypothermia.

#### BodyPartHediffTargetEvaluator_Single

**File:** `Source/.../HealthConditions/Secondary/Handlers/TargetEvaluators/BodyPartHediffTargetEvaluator_Single.cs`

```csharp
if (hediffs.GetBodyPartRecord(target) is BodyPartRecord targetRecord && !hediffs.PartIsMissing(targetRecord))
    return targetRecord;
return null;
```

**Graceful:** Returns `null` if the target body part (e.g., `Brain`, `Heart`) does not exist on the pawn. The caller then skips the hediff application.

---

### Tourniquet System

#### BleedRateByLimbEnumerable

**File:** `Source/.../HealthConditions/HeavyBleeding/Tourniquets/BleedRateByLimbEnumerable.cs`

```csharp
private static IEnumerable<BodyPartRecord> GetLimbs(Pawn patient) => patient.health.hediffSet.GetNotMissingParts()
    .Where(bodyPart => (bodyPart.def == BodyPartDefOf.Shoulder
        || bodyPart.def == BodyPartDefOf.Leg
        || bodyPart.def == KnownBodyPartDefOf.Neck) ...);
```

**Hard assumption:** Tourniquets can only be applied to body parts of type `Shoulder`, `Leg`, or `Neck`. Any modded race with differently-named or differently-structured limbs will have **no tourniquet attachment points**. The tourniquet UI will show no options for such races even if they have bleeding injuries.

#### TourniquetHediffComp (Gangrene)

**File:** `Source/.../HealthConditions/HeavyBleeding/Tourniquets/TourniquetHediffComp.cs`

```csharp
private static bool CanAddGangrene(Pawn pawn, BodyPartRecord part) =>
    part is not null
    && part.def != KnownBodyPartDefOf.FemoralArtery
    && part.def != KnownBodyPartDefOf.PoplitealArtery
    && !part.def.IsSolid(part, ...)
    && ...
```

**Hard assumption:** `FemoralArtery` and `PoplitealArtery` are explicitly excluded from gangrene — these are MoreInjuries-specific Human body parts. For non-human races, these defs will never match (graceful no-op for the exclusion logic).

#### JobDriver_TourniquetBase — `PawnKnowsWhatTheyreDoing`

**File:** `Source/.../HealthConditions/HeavyBleeding/Tourniquets/JobDriver_TourniquetBase.cs`

```csharp
public static bool PawnKnowsWhatTheyreDoing(Pawn pawn)
{
    if (pawn.story.traits.HasTrait(KnownTraitDefOf.SlowLearner))
    { ... }
    foreach (SkillRecord skill in pawn.skills.skills)
    { ... }
}
```

**Hard assumptions (no null-checks):**
- `pawn.story` is accessed without null-guard → if a pawn has no `story` tracker (e.g., mechanoids, some animals), this **throws a `NullReferenceException`**.
- `pawn.skills` is accessed without null-guard → same crash risk for pawns without a `skills` tracker.

In practice this method is only called from `TourniquetFloatOptionProvider` for the selected pawn (who is always a humanlike colonist), and from the tourniquet gizmo (also only for humanlike pawns). However, it is a latent crash risk if invoked on a non-humanlike pawn.

---

### WorkGivers and AI Scheduling

#### WorkGiver_MoreInjuriesTreatmentBase

**File:** `Source/.../AI/WorkGivers/WorkGiver_MoreInjuriesTreatmentBase.cs`

```csharp
public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) =>
    pawn.Map.mapPawns.SpawnedHumanlikesWithAnyHediff;
```

**Hard assumption:** The work candidate pool is explicitly restricted to **humanlike** pawns. Non-humanlike pawns (animals, robots, and many modded races that are not flagged `Humanlike`) will **never** be considered as potential patients for any MoreInjuries treatment job assigned by these work givers.

```csharp
protected virtual bool IsValidPatient(Pawn doctor, Thing thing, out Pawn patient)
{
    ...
    return doctor != patient
        && GoodLayingStatusForTend(patient, doctor)
        && !patient.IsForbidden(doctor)
        && (!patient.IsMutant || patient.mutant.Def.entitledToMedicalCare)
        && !patient.InAggroMentalState;
}
```

Additional filter: mutants must have `entitledToMedicalCare` set on their mutant def. This is a Vanilla-Anomaly integration assumption.

```csharp
public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor) =>
    patient != doctor && patient.InBed();
```

**Assumption:** Patients must be in a bed to be treated. This is a general RimWorld convention and is race-agnostic.

#### WorkGiver_RemoveTourniquetFromDead

**File:** `Source/.../AI/WorkGivers/WorkGiver_RemoveTourniquetFromDead.cs`

```csharp
if (t is not Corpse { InnerPawn: Pawn { RaceProps.Humanlike: true } deadPawn } corpse ...)
```

**Hard assumption:** Tourniquets are only recovered from the corpses of **humanlike** pawns. Non-humanlike races that had a tourniquet applied (if they ever could) would have their tourniquets permanently lost on death.

---

### Medical Device Helpers and Job Drivers

#### MedicalDeviceHelper.GetCauseForDisabledProcedure

**File:** `Source/.../Things/MedicalDeviceHelper.cs`

Several checks use null-safe patterns (`?.`):
- `patient.playerSettings?.medCare` — safe for any pawn
- `doctor.health.capacities.CapableOf(...)` — standard pawn capacities, race-agnostic

Non-null-safe assumptions:
- `doctor.WorkTagIsDisabled(WorkTags.Caring)` — standard pawn work tag, race-agnostic
- `doctor.WorkTypeIsDisabled(WorkTypeDefOf.Doctor)` — standard pawn work type, race-agnostic

#### JobDriver_HarvestBlood

**File:** `Source/.../HealthConditions/HeavyBleeding/Transfusions/JobDriver_HarvestBlood.cs`

```csharp
if (doctor.needs?.mood?.thoughts?.memories is not null)
{
    Thought_Memory thought = ...
    doctor.needs.mood.thoughts.memories.TryGainMemory(thought);
}
```

**Graceful:** Null-safe chain. Doctors without a `needs`/`mood`/`thoughts` tracker (e.g., mechanoids) simply don't gain the memory.

```csharp
ThoughtUtility.GiveThoughtsForPawnExecuted(patient, doctor, PawnExecutionKind.OrganHarvesting);
TaleRecorder.RecordTale(TaleDefOf.ExecutedPrisoner, doctor, patient);
```

**Assumption:** These calls assume the patient is a pawn that can be "executed" in the social/tale sense. For non-humanlike races, RimWorld's vanilla utilities handle this gracefully (they check for humanlike internally), but the blood harvesting mechanic semantically assumes a humanlike patient.

```csharp
QuestUtility.SendQuestTargetSignals(patient.questTags, ...);
```

Standard utility — works for any pawn.

#### PawnExtensions.GetMedicalSkillLevelOrDefault

**File:** `Source/.../Extensions/PawnExtensions.cs`

```csharp
if (pawn.skills?.GetSkill(SkillDefOf.Medicine) is SkillRecord skill)
    return skill.Level;
if (!pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
    return 15; // assume they're good at it
return defaultValue;
```

**Graceful:** Null-safe. Pawns without skills (mechanoids) fall through to the work-type check. If they can be doctors, they're assumed to have skill 15. Otherwise, the default value is used.

---

### Extensions and Caching Layers

#### HediffSetExtensions.GetNonMissingPartsOfType

**File:** `Source/.../Extensions/HediffSetExtensions.cs`

```csharp
List<BodyPartRecord> allPartsList = hediffSet.pawn.def.race.body.AllParts;
```

**Assumption:** `pawn.def.race.body` is non-null. This is a standard RimWorld contract (any valid pawn `ThingDef` with `race` must define a `body`). This assumption is documented as out-of-scope ("true-by-contract") per the issue specification.

#### Caching Infrastructure

**Files:** `Source/.../Caching/`

The `WeakTimedDataCache`, `WeakTimedMapThingCache`, and related types use `Pawn` as a key type and operate on `Map`-level data. No race-specific assumptions are made in the caching layer itself. Correctness depends on the data providers (lambdas) supplied to each cache, which may themselves carry race assumptions (documented above under their respective workers/job drivers).

#### WeakTimedMapThingCache (CorpseCache in WorkGiver_RemoveTourniquetFromDead)

```csharp
if (thing is Corpse { InnerPawn: Pawn { RaceProps.Humanlike: true } deadPawn } corpse ...)
```

The corpse cache filters by `RaceProps.Humanlike: true` (see [WorkGiver_RemoveTourniquetFromDead](#workgiver_removetourniquetfromdead)).

---

## Summary Table

| Location | Assumption | Race Impact | Severity |
|---|---|---|---|
| `Patch_Core.xml` | `MoreInjuryComp` only injected into `Human` ThingDef | Non-human races get no injury pipeline | **High** (feature gap) |
| `Patch_Core.xml` | `BetterInjury` / `BetterMissingPart` injected globally | All races use MoreInjuries injury hediff classes | Low (safe) |
| `Patch_BodyParts.xml` | Custom body parts (SpinalCord, FemoralArtery, etc.) only added to `Human` body | Paralysis, artery lacerations, intestinal spill partially unavailable for non-humans | Medium (feature gap) |
| `Patch_Surgeries.xml` | Surgery recipes only added to `Human` ThingDef | No MoreInjuries surgeries for non-human races | **High** (feature gap) |
| `Patch_BloodLoss.xml` | `HediffModifier_DisallowUntracked` guards most secondary blood loss effects | Non-tracked pawns are protected from secondary effects | Low (safe, intentional guard) |
| `Patch_BloodLoss.xml` | Blood loss → hypoxia targets `Brain` by name | Races without `Brain` silently skip hypoxia from blood loss | Low (graceful) |
| `Patch_Hypothermia.xml` | Hypothermia → cardiac arrest targets `Heart` by name | Races without `Heart` silently skip cardiac arrest from hypothermia | Low (graceful) |
| `Patch_Hypothermia.xml` | Coagulopathy severity link has no `DisallowUntracked` guard | All pawns with hypothermia have coagulopathy severity adjusted | Low (unintentional; minor) |
| `Mod_HumanoidAlienRaces.xml` | Only flesh HAR races get `MoreInjuryComp` | Non-flesh HAR races excluded | Low (intentional) |
| `Mod_HumanoidAlienRaces.xml` | Surgery recipes not added to HAR races | No MoreInjuries surgeries for HAR races | **High** (feature gap) |
| `Mod_CombatExtended.xml` | Coverage adjustments only for `Human` body | Non-human races have vanilla CE coverage values | Low (intentional) |
| `MoreInjuryComp.Pawn` | Casts `parent` to `Pawn` | Crash if comp attached to non-Pawn Thing | Low (safe in practice) |
| `AdrenalineWorker.IsEnabled` | Checks `!Pawn.IsShambler` | Shamblers don't get adrenaline | Low (intentional) |
| `FractureWorker` | Hard-coded dict of Human bone defs | Non-human races get no fractures even with equivalent bones | Medium (feature gap) |
| `HeadInjuryWorker` | Uses `BodyPartGroupDefOf.FullHead` | Races without `FullHead` group get no head injuries | Medium (feature gap) |
| `HydrostaticShockWorker` | `GetBrain()` used for stroke target | Races without `Brain` silently skip hydrostatic shock | Low (graceful) |
| `ParalysisWorker` | Checks for Human-only `SpinalCord` body part | Non-human races never get paralysis | Medium (feature gap) |
| `IntestinalSpillWorker` | Hard-coded set including Human-only intestine defs | Non-human races skip intestinal spill for missing organs | Low (graceful, partial feature gap) |
| `SpallingInjuryWorker` | `patient.apparel` null-safe, uses `Torso` | Races without apparel skip spalling; races without `Torso` skip armor check | Low (graceful) |
| `LungCollapse*Worker` | Checks for `BodyPartDefOf.Lung` | Races without lungs get no lung collapse | Low (graceful) |
| `InhalationInjuryWorker` | `GetNonMissingPartsOfType(Lung)` | Races without lungs get no inhalation injury | Low (graceful) |
| `HearingLossWorker` | Ear protection depends on apparel covering ear groups | Non-human races may get hearing damage without protection | Low (minor) |
| `ChokingWorker` | Uses `BreathingSource`/`BreathingPathway` tags | Data-driven, works for any race that properly tags its respiratory parts | Low (safe, data-driven) |
| `HediffComp_PersonalityShift` | `pawn.skills` accessed with null-guard | Pawns without skills skip skill reshuffling | Low (graceful) |
| `BleedRateByLimbEnumerable` | Tourniquets only on `Shoulder`, `Leg`, `Neck` body parts | Races with different limb structure get no tourniquet options | **High** (feature gap) |
| `TourniquetHediffComp` | Excludes `FemoralArtery`, `PoplitealArtery` from gangrene | Human-specific exclusion; no effect on other races | Low (safe) |
| `JobDriver_TourniquetBase.PawnKnowsWhatTheyreDoing` | `pawn.story` and `pawn.skills` accessed without null-checks | **Crash** if called on a pawn without story/skills tracker | **Critical** (latent crash) |
| `WorkGiver_MoreInjuriesTreatmentBase` | `SpawnedHumanlikesWithAnyHediff` as patient pool | Non-humanlike races never receive AI-assigned MoreInjuries treatments | **High** (feature gap) |
| `WorkGiver_RemoveTourniquetFromDead` | Filters corpses by `RaceProps.Humanlike: true` | Tourniquets only recovered from humanlike corpses | Medium (feature gap) |
| `MedicalDeviceHelper` | `patient.playerSettings` null-safe | Graceful for all pawn types | Low (safe) |
| `JobDriver_HarvestBlood` | `doctor.needs.mood.thoughts` null-safe chain | Graceful for pawns without mood needs | Low (graceful) |
| `PawnExtensions.GetMedicalSkillLevelOrDefault` | `pawn.skills` null-safe | Graceful fallback for pawns without skills | Low (graceful) |
