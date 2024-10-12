using System.Linq;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using MoreInjuries.Initialization;
using System.Collections.Generic;
using MoreInjuries.Extensions;

namespace MoreInjuries;

using static MoreInjuriesSettings;

public class MoreInjuriesMod : Mod
{
    private static bool? s_combatExtendedLoaded = null;

    public static MoreInjuriesSettings Settings { get; private set; } = null!;

    internal static bool CombatExtendedLoaded => 
        s_combatExtendedLoaded ??= LoadedModManager.RunningModsListForReading.Any(mod => mod.PackageIdPlayerFacing?.Equals("CETeam.CombatExtended") is true);

    public MoreInjuriesMod(ModContentPack content) : base(content)
    {
        Settings = GetSettings<MoreInjuriesSettings>();
        Harmony harmony = new("Th3Fr3d.MoreInjuries");
        harmony.PatchAll();
    }

    private Vector2 _scrollPosition;
    private const float MIN_CONTENT_HEIGHT = 512f;
    private float _knownContentHeight = MIN_CONTENT_HEIGHT;
    private bool _requiresScrolling = false;
    private bool _hasMap = false;

    public override void DoSettingsWindowContents(Rect canvas)
    {
        if (Settings is null)
        {
            Logger.Error("Settings was null!");
            throw new ArgumentNullException(nameof(Settings));
        }
        float scrollbarMargin = _requiresScrolling ? 25f : 0f;
        Listing_Standard list = new();
        Rect content = new(canvas.x, canvas.y, canvas.width - scrollbarMargin, _knownContentHeight);
        Rect view = new(canvas.x, canvas.y, canvas.width, canvas.height);
        if (_requiresScrolling)
        {
            Widgets.BeginScrollView(view, ref _scrollPosition, content);
        }
        list.Begin(content);
        Text.Font = GameFont.Medium;
        list.Label("General Settings");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable logging (default: false)", ref Settings.EnableLogging);
        list.CheckboxLabeled("Enable verbose logging (default: false)", ref Settings.EnableVerboseLogging);
        // TODO: not sure if or how this works
        if (Find.CurrentMap is not null)
        {
            if (!_hasMap)
            {
                _hasMap = true;
                _knownContentHeight = MIN_CONTENT_HEIGHT;
            }
            if (list.ButtonText("Fix misplaced bionics"))
            {
                IEnumerable<Pawn> humans = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.def == ThingDefOf.Human);
                foreach (Pawn human in humans)
                {
                    List<Hediff> hediffs = human.health.hediffSet.hediffs.FindAll(x => x.def.addedPartProps is not null && x.def.HasModExtension<FixMisplacedBionicsModExtension>());

                    foreach (Hediff hediff in hediffs)
                    {
                        FixMisplacedBionicsModExtension modExtension = hediff.def.GetModExtension<FixMisplacedBionicsModExtension>();

                        List<BodyPartRecord> bodyParts = [.. human.health.hediffSet.GetNotMissingParts().Where(p => modExtension.BodyParts.Contains(p.def))];

                        if (bodyParts.Any())
                        {
                            hediff.Part = bodyParts.SelectRandom();
                        }
                        else
                        {
                            human.health.RemoveHediff(hediff);
                        }
                    }
                }
            }
        }
        else if (_hasMap)
        {
            _hasMap = false;
            _knownContentHeight = MIN_CONTENT_HEIGHT;
        }
        list.GapLine();
        list.Gap();
        Text.Font = GameFont.Medium;
        list.Label("Feature Flags and Values");
        Text.Font = GameFont.Small;
        // fractures
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Fractures");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable bone fractures (default: true)", ref Settings.EnableFractures,
            """
            If enabled, pawns that take certain types of damage may, as a result, receive bone fractures from the impact. This is especially likely for blunt damage.
            """);
        list.Label($"Minimum damage threshold for fractures: {Settings.FractureDamageTreshold} (default: 10)", -1,
            """
            The minimum amount of damage required to cause a bone fracture.
            
            Being punched by a squirrel is unlikely to cause a fracture, but being hit by a club or a bullet is a different story.
            """);
        Settings.FractureDamageTreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.FractureDamageTreshold, 1f, 20f), 1f, 20f));
        list.Label($"Chance of bone fractures on damage: {Settings.FractureChanceOnDamage} (default: 0.25)", -1,
            """
            The likelihood of a bone fracture actually being applied to an affected body part after all conditions for a fracture have been met. 

            Simulates the complex geometry of the human body and its susceptibility to fractures through random chance.
            """);
        Settings.FractureChanceOnDamage = (float)Math.Round(list.Slider(Settings.FractureChanceOnDamage, 0f, 1f), 2);
        list.CheckboxLabeled("Enable bone fragment lacerations (default: true)", ref Settings.EnableBoneFragmentLacerations,
            """
            If enabled, pawns that receive bone fractures may also receive lacerations from the bone fragments that result from the fracture.
            """);
        list.Label($"Change of bone fragment lacerations: {Settings.SplinteringFractureChance} (default: 0.5)", -1,
            """
            The likelihood that a fracture may cause the bone to splinter, which can cause additional lacerations to nearby body parts.
            """);
        Settings.SplinteringFractureChance = (float)Math.Round(list.Slider(Settings.SplinteringFractureChance, 0f, 1f), 2);
        list.Label($"Chance of bone fragment lacerations per body part: {Settings.BoneFragmentLacerationChancePerBodyPart} (default: 0.5)", -1,
            """
            The likelihood that a nearby body part may receive bone fragment lacerations after a splintering fracture has occurred.
            """);
        Settings.BoneFragmentLacerationChancePerBodyPart = (float)Math.Round(list.Slider(Settings.BoneFragmentLacerationChancePerBodyPart, 0f, 1f), 2);
        // respiratory conditions
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Respiratory Conditions");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable choking on blood mechanics (default: true)", ref Settings.EnableChoking,
            """
            If enabled, pawns that receive severe lacerations to the respiratory system may choke on their own blood, leading to asphyxiation and death if not treated in time.

            A gruesome reminder of the fragility of life.
            """);
        list.CheckboxLabeled("Enable choking sounds (default: true)", ref Settings.EnableChokingSounds,
            """
            Whether to play a sound effect when a pawn starts choking on their own blood.

            Potentially disturbing, but adds a sense of urgency to the situation.
            """);
        list.Label($"Minimum bleed rate for choking: {Settings.ChokingMinimumBleedRate} (default: 0.2)", -1,
            """
            The minimum bleed rate of respiratory injuries required to cause a pawn to choke on their own blood.
            """);
        Settings.ChokingMinimumBleedRate = (float)Math.Round(list.Slider(Settings.ChokingMinimumBleedRate, 0f, 1f), 2);
        list.Label($"Chance of choking on blood after severe damage: {Settings.ChokingChanceOnDamage} (default: 0.75)", -1,
            """
            The likelihood of a pawn choking on their own blood after receiving severe bleeding injuries to the respiratory system.
            """);
        Settings.ChokingChanceOnDamage = (float)Math.Round(list.Slider(Settings.ChokingChanceOnDamage, 0f, 1f), 2);
        list.Label($"Minimum suction device success rate for unskilled users: {Settings.SuctionDeviceMinimumSuccessRate} (default: 0.25)", -1,
            """
            The minimum likelihood of a suction device successfully removing blood from the airways of a choking pawn when used by an unskilled user (Medicine skill < 8). 
            
            The actual success rate scales with the user's medicine skill: max(<this setting>, <medicine skill> / 8).
            """);
        Settings.SuctionDeviceMinimumSuccessRate = (float)Math.Round(list.Slider(Settings.SuctionDeviceMinimumSuccessRate, 0f, 1f), 2);
        list.CheckboxLabeled("Enable inhalation injuries (default: true)", ref Settings.EnableFireInhalation,
            """
            If enabled, pawns that are exposed to fire or other sources of smoke and hot gases may suffer from inhalation injuries, causing severe damage to the respiratory system.

            Smoking is bad for your health, but inhaling smoke from a burning building or residue from thermobaric munitions is even worse.
            """);
        list.CheckboxLabeled("Enable lung collapses (default: true)", ref Settings.EnableLungCollapse,
            """
            If enabled, thermobaric weapons and other high-explosive devices can cause the lungs to rupture and collapse due to the sudden pressure changes.

            Fatal if not treated in time, must be surgically repaired.
            """);
        list.Label($"Chance of lung collapse on thermobaric damage: {Settings.LungCollapseChanceOnDamage} (default: 0.4)", -1,
            """
            The likelihood of a pawn suffering from lung collapse after being hit by a thermobaric weapon or other high-explosive device.
            """);
        Settings.LungCollapseChanceOnDamage = (float)Math.Round(list.Slider(Settings.LungCollapseChanceOnDamage, 0f, 1f), 2);
        list.Label($"Square root of the maximum initial severity of lung collapse: {Settings.LungCollapseMaxSeverityRoot} (default: 0.85)", -1,
            "The maximum initial severity of lung collapse is determined by a random factor between 0 and this value, squared. Stacks with subsequent exposure to thermobaric damage. " +
            "The resulting severity of lung collapse therefore follows a quadratic distribution, with a higher likelihood of low severities and a small chance of very high severities. " +
            "For example, at 0.5, the severity of lung collapse will follow a quadratic distribution between 0 and 0.25.");
        Settings.LungCollapseMaxSeverityRoot = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.LungCollapseMaxSeverityRoot, 0.1f, 1f), 0.1f, 1f), 2);
        // Spalling
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Spalling");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable spalling mechanics (default: true)", ref Settings.EnableSpalling,
            "When high-velocity projectiles are stopped by armor, the large amount of kinetic energy can cause the projectile and top layer of the armor to shatter and send fragments flying in all directions. " +
            "The amount of spalling depends on the angle of impact and the hardness and condition of the armor. " +
            "Spalling can cause additional injuries to the wearer of the armor, even if the projectile itself did not penetrate the armor.");
        list.Label(
            $"""
            Base chance of armor creating spall based on condition: {Settings.ArmorHealthSpallingThreshold} (default: 0.95)
            
            """ +
            "Modern armor is designed to prevent spalling by adding softer layers above the hard armor plates to catch and absorb bullet fragments. " +
            "As armor condition deteriorates after absorbing damage, the chance of spalling naturally increases when these absorbing layers are compromised. " +
            "For example, at 0.8, the chance of spalling remains 0 until the armor is at 80% hp, etc. At 0, spalling is disabled."
            );
        Settings.ArmorHealthSpallingThreshold = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.ArmorHealthSpallingThreshold, 0.1f, 1f), 0.1f, 1f), 2);
        list.Label($"Chance of spalling injuries: {Settings.SpallingChance} (default: 0.75)", -1,
            """
            The likelihood of exposed body parts receiving spalling injuries after spalling has occurred. Evaluated per body part.
            """);
        Settings.SpallingChance = (float)Math.Round(list.Slider(Settings.SpallingChance, 0f, 1f), 2);
        // hypovolemic shock
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Blood Loss (Hypovolemic Shock)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hypovolemic shock mechanics (requires game reload, default: true)", ref Settings.EnableHypovolemicShock,
            "If enabled, pawns that lose a significant amount of blood may suffer from hypovolemic shock, which can be fatal if not treated in time. " +
            "The body needs a certain amount of blood to function properly, and losing too much can lead to organ failure and death. " +
            """
            
            Must be treated with a blood transfusion to stabilize the patient.
            """);
        list.Label($"Chance of hypovolemic shock to cause organ hypoxia (every 300 ticks/5s): {Settings.OrganHypoxiaChance} (default: 0.35)", -1,
            """
            Loosing a lot of blood will lead to organs not receiving enough oxygen to function properly. This can ultimately lead to organ failure and death.

            Treat the patient with a blood transfusion in time to prevent this from happening.
            """);
        Settings.OrganHypoxiaChance = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChance, 0f, 1f), 2);
        list.Label($"Reduction factor for organ hypoxia chance when patient is tended: {Settings.OrganHypoxiaChanceReductionFactor} (default: 0.5)", -1,
            "Tending the hypovolemic shock condition will reduce the chance of organ hypoxia by this factor and will slow down the progression of the shock. " +
            "Note that this will not prevent the shock from progressing, but only slow it down. To fully stabilize the patient, a blood transfusion is required.");
        Settings.OrganHypoxiaChanceReductionFactor = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChanceReductionFactor, 0f, 1f), 2);
        list.CheckboxLabeled("Enable cardiac arrest on high blood loss (default: true)", ref Settings.EnableCardiacArrestOnHighBloodLoss,
            "If enabled, pawns that lose a very large amount of blood may suffer from cardiac arrest, which can be fatal if not treated in time. " +
            "The heart needs a certain amount of blood to function properly, and losing too much can lead to heart failure and death. " +
            """

            A skilled doctor can attempt to resuscitate the patient with CPR.
            """);
        list.Label($"Chance of cardiac arrest on high blood loss: {Settings.CardiacArrestChanceOnHighBloodLoss} (default: 0.05)", -1,
            """
            The likelihood of a pawn suffering from cardiac arrest after losing a very large amount of blood. Evaluated every 300 ticks/5s.
            """);
        Settings.CardiacArrestChanceOnHighBloodLoss = (float)Math.Round(list.Slider(Settings.CardiacArrestChanceOnHighBloodLoss, 0f, 1f), 2);
        list.Label($"Minimum defibrillator success rate for unskilled user: {Settings.DefibrillatorMinimumSuccessRate} (default: 0.25)", -1,
            """
            The minimum likelihood of a defibrillator successfully resuscitating a patient when used by an unskilled user (Medicine skill < 8). 
            
            The actual success rate scales with the user's medicine skill: max(<this setting>, <medicine skill> / 8).
            """);
        Settings.DefibrillatorMinimumSuccessRate = (float)Math.Round(list.Slider(Settings.DefibrillatorMinimumSuccessRate, 0f, 1f), 2);
        // concussion after blunt trauma
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Concussion (Head Injury)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable concussion mechanics (default: true)", ref Settings.EnableConcussion,
            """
            If enabled, pawns that receive head injuries may suffer from concussions, causing temporary loss of consciousness and disorientation.
            """);
        list.Label($"Raw head trauma threshold for guaranteed concussion: {Settings.ConcussionThreshold} (default: 6)", -1,
            "The equivalent amount of damage dealt to the skull required to cause a concussion in 100% of all cases. " +
            "The actual chance is evaluated based on the location on the head and the severity of the damage (e.g. being punched on the nose vs. being clubbed on the brain). " +
            "The final chance is then scaled by the concussion chance.");
        Settings.ConcussionThreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.ConcussionThreshold, 1f, 50f), 1f, 50f));
        list.Label($"Chance of concussion after severe head injuries: {Settings.ConcussionChance} (default: 0.75)", -1,
            "Scaling factor for the calculated chance of a concussion after severe head injuries. Evaluated per damage event. " +
            "If set to 1, the calculated chance is used as-is (every head injury surpassing the configured threshold leads to concussion). " +
            "Lower values allow to reduce the overall chance of concussions, even if the damage threshold is met.");
        Settings.ConcussionChance = (float)Math.Round(list.Slider(Settings.ConcussionChance, 0f, 1f), 2);
        // hemorrhagic stroke after blunt trauma
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Hemorrhagic Stroke (Head Injury)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hemorrhagic stroke mechanics (default: true)", ref Settings.EnableHemorrhagicStroke,
            """
            If enabled, pawns that receive severe head injuries may suffer from a hemorrhagic stroke, which can be fatal if not treated in time.
            """);
        list.Label($"Raw head trauma threshold for guaranteed hemorrhagic stroke: {Settings.HemorrhagicStrokeThreshold} (default: 15)", -1,
            "The equivalent amount of damage dealt to the skull required to cause a hemorrhagic stroke in 100% of all cases. " +
            "The actual chance is evaluated based on the location on the head and the severity of the damage (e.g. being punched on the nose vs. being stabbed in the brain). " +
            "The final chance is then scaled by the hemorrhagic stroke chance.");
        Settings.HemorrhagicStrokeThreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.HemorrhagicStrokeThreshold, 1f, 50f), 1f, 50f));
        list.Label($"Chance of hemorrhagic stroke after severe head injuries: {Settings.HemorrhagicStrokeChance} (default: 0.25)", -1,
            "Scaling factor for the calculated chance of a hemorrhagic stroke after severe head injuries. Evaluated per damage event. " +
            "If set to 1, the calculated chance is used as-is (every head injury surpassing the configured threshold leads to a hemorrhagic stroke). " +
            "Lower values allow to reduce the overall chance of hemorrhagic strokes, even if the damage threshold is met.");
        Settings.HemorrhagicStrokeChance = (float)Math.Round(list.Slider(Settings.HemorrhagicStrokeChance, 0f, 1f), 2);
        // EMP damage to bionics
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("EMP Damage to Bionics");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable EMP damage to bionics (default: true)", ref Settings.EnableEmpDamageToBionics,
            """
            If enabled, electromagnetic pulse (EMP) damage can cause bionic implants to malfunction and shut down temporarily.

            Sophisticated technology can be a double-edged sword.
            """);
        list.Label($"Chance of EMP damage to bionics: {Settings.EmpDamageToBionicsChance} (default: 0.45)", -1,
            """
            The likelihood of EMP damage causing bionic implants to shut down temporarily. Evaluated per bionic implant after EMP damage has been applied.
            """);
        Settings.EmpDamageToBionicsChance = (float)Math.Round(list.Slider(Settings.EmpDamageToBionicsChance, 0f, 1f), 2);
        // adrenaline
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Adrenaline");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable adrenaline mechanics (default: true)", ref Settings.EnableAdrenaline,
            """
            If enabled, pawns that take damage may receive a rush of adrenaline that temporarily boosts moving capabilitites and numbs the perception of pain.
            """);
        list.Label($"Chance of adrenaline rush on damage: {Settings.AdrenalineChanceOnDamage} (default: 0.75)", -1,
            "The likelihood of a pawn receiving an adrenaline rush after taking damage. Evaluated per damage event. " +
            "The intensity of the adrenaline rush depends on the amount of damage taken.");
        Settings.AdrenalineChanceOnDamage = (float)Math.Round(list.Slider(Settings.AdrenalineChanceOnDamage, 0f, 1f), 2);
        list.Label($"Damage threshold for certain adrenaline rush: {Settings.CertainAdrenalineThreshold} (default: 15)", -1,
            "The amount of damage required to always cause an adrenaline rush, which provides a significant boost to moving capabilities and numbs the perception of pain for a short time. " +
            "If the damage taken exceeds this threshold, an adrenaline rush is guaranteed to occur. Otherwise, the chance is determined by the adrenaline chance on damage.");
        Settings.CertainAdrenalineThreshold = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.CertainAdrenalineThreshold, 1f, 50f), 1f, 50f));
        // hydrostatic shock (controversial)
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Hydrostatic Shock (Controversial)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hydrostatic shock mechanics (default: false)", ref Settings.EnableHydrostaticShock,
            "Hydrostatic shock, also known as Hydro-shock, is the controversial concept that a penetrating projectile (such as a bullet) can produce a pressure wave that causes \"remote neural damage\", \"subtle damage in neural tissues\" and \"rapid effects\" in living targets. " +
            "If enabled, pawns that are hit by very-high-energy projectiles may suffer from hydrostatic shock, a type of intracerebral hemorrhage (rupture of blood vessels in the brain), which can be fatal if not treated in time. " +
            "The existence of hydrostatic shock is a topic of debate among medical professionals and firearms experts. Use at your own discretion.");
        list.Label($"Chance of hydrostatic shock on damage: {Settings.HydrostaticShockChanceOnDamage} (default: 0.2)", -1,
            """
            The likelihood of a pawn suffering from hydrostatic shock after being hit by a very-high-energy projectile. Evaluated per damage event.
            """);
        Settings.HydrostaticShockChanceOnDamage = (float)Math.Round(list.Slider(Settings.HydrostaticShockChanceOnDamage, 0f, 1f), 2);
        // tourniquets and gangrene
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Tourniquets Cause Tissue Damage");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled($"Enable tourniquets causing gangrene (default: {TOURNIQUETS_CAN_CAUSE_GANGRENE_DEFAULT})", ref Settings.TourniquetsCanCauseGangrene,
            """
            If enabled, pawns that have a tourniquet applied for an extended period of time may suffer from gangrene, a potentially fatal condition caused by the death of body tissue due to a lack of blood flow.

            If only applied for a few hours, a tourniquet can save a life. If applied for a few days, it can cost a limb.
            """);
        list.Label($"Mean time between gangrene on tourniquet: {Math.Round(Settings.MeanTimeBetweenGangreneOnTourniquet / 2500f, 1)}h (default: {MEAN_TIME_BETWEEN_GANGRENE_ON_TOURNIQUET_DEFAULT / 2500f}h)", -1,
            """
            The mean time between child body parts (e.g. fingers or toes) suffering from gangrene when the severity of adverse conditions due to the applied tourniquet is at its maximum.
            """);
        Settings.MeanTimeBetweenGangreneOnTourniquet = Mathf.Floor(list.Slider((float)Math.Round(Settings.MeanTimeBetweenGangreneOnTourniquet / 2500f, 1), 0.1f, 24f) * 2500f);
        list.Label($"Chance of dry gangrene on tourniquet: {Settings.DryGangreneChance} (default: {DRY_GANGRENE_CHANCE_DEFAULT})", -1,
            """
            If a tourniquet is applied for an extended period of time, the affected body part will starve of oxygen and nutrients, leading to the death of the tissue (gangrene). Gangrene can be either dry or wet. Dry gangrene refers to the death of tissue (loss of the body part) without bacterial infection, while wet gangrene implies an accompanying bacterial infection potentially causing sepsis and death.

            As such, both types of gangrene imply the loss of the affected body part, but wet gangrene can lead to death if not amputated in time. Dry gangrene may develop into wet gangrene over time if bacteria enter the dead tissue.

            If set to 1, the gangrene will always be initially dry. If set to 0.8, there is an 80% chance of the gangrene being initially dry and a 20% chance of it being initially wet.
            """);
        Settings.DryGangreneChance = (float)Math.Round(list.Slider(Settings.DryGangreneChance, 0f, 1f), 2);
        list.Label($"Mean time to infection on dry gangrene: {Math.Round(Settings.DryGangreneMeanTimeToInfection / 60_000f, 1)}d (default: {DRY_GANGRENE_MEAN_TIME_TO_INFECTION_DEFAULT / 60_000f}d)", -1,
            """
            The mean time between dry gangrene becoming infected with bacteria and turning into wet gangrene. Wet gangrene is a life-threatening condition that requires immediate amputation.
            """);
        Settings.DryGangreneMeanTimeToInfection = Mathf.Floor(list.Slider((float)Math.Round(Settings.DryGangreneMeanTimeToInfection / 60_000f, 1), 0.1f, 15f) * 60_000f);
        // miscellaneous
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Hearing Damage");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hearing damage mechanics (default: true)", ref Settings.EnableBasicHearingDamage,
            """
            If enabled, pawns shooting or being exposed to explosions may suffer from hearing loss, especially if indoors or not wearing ear protection. Helmets and other apparel that covers the ears can reduce the risk of hearing damage. Being exposed to an explosion will likely cause hearing damage no matter what.

            Apparently, gunshots are loud. Who knew?
            """);
        bool advancedHearingDamage = Settings.EnableAdvancedHearingDamage;
        list.CheckboxLabeled("Enable advanced hearing damage simulation (default: false)", ref advancedHearingDamage,
            """
            Requires basic hearing damage to be enabled. If enabled, bystanding pawns near a shooter may also suffer from hearing damage, especially if the shooter is using a loud weapon or firing in an enclosed space.

            Disabled by default, may reduce performance during large battles, especially with Combat Extended loaded, due to the increased number of calculations required.
            """);
        Settings.EnableAdvancedHearingDamage = Settings.EnableBasicHearingDamage && advancedHearingDamage;
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Miscellaneous");
        Text.Font = GameFont.Small;
        list.Label($"Multiplier for bleeding from closed internal injuries: {Settings.ClosedInternalWouldBleedingModifier} (default: 0.75)", -1,
            "Assume a pawn is shot in the torso and the bullet penetrates the skin and stomach, causing internal and external bleeding. " +
            "Simply applying a bandage to the skin wound will not stop the internal bleeding, but may still slow it down. " +
            "This modifier determines how much the internal bleeding is reduced by applying a bandage to the skin wound. " +
            "A value of 1 means that the internal bleeding is not affected by the bandage, while a value of 0 means that the internal bleeding is completely stopped.");
        Settings.ClosedInternalWouldBleedingModifier = (float)Math.Round(list.Slider(Settings.ClosedInternalWouldBleedingModifier, 0f, 1f), 2);
        list.Label($"Paralysis damage threshold (50% point): {Settings.ParalysisDamageTreshold50Percent} (default: 6)", -1,
            """
            The amount of spinal cord damage required to cause paralysis in 50% of all cases. The actual chance of paralysis scales linearly with the damage dealt.
            """);
        Settings.ParalysisDamageTreshold50Percent = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.ParalysisDamageTreshold50Percent, 1f, 20f), 1f, 20f));
        list.Label($"Chance of intestinal spilling on damage: {Settings.IntestinalSpillingChanceOnDamage} (default: 0.45)", -1,
            """
            The likelihood of a pawn suffering from intestinal spilling after receiving severe damage to the abdomen. Can lead to severe infections and stomach acid burns.
            """);
        Settings.IntestinalSpillingChanceOnDamage = (float)Math.Round(list.Slider(Settings.IntestinalSpillingChanceOnDamage, 0f, 1f), 2);

        // FIXME: this isn't great, but it works
        // we basically do an initial draw with a rather small height, and see if it's enough
        // - if it is, we remember the exact height of the content and resize the window to fit
        // - otherwise CurHeight will be smaller than a known epsilon, so we start at the epsilon and double it with each draw until it's big enough
        // - once we have enough space, we clamp the height to the known content height
        // if the content is too small to fit everything, CurHeight will be 23.something, (don't ask me why)
        if (list.CurHeight < MIN_CONTENT_HEIGHT)
        {
            // so on each draw, we double the known content height until it's big enough
            // yes, we could also just set it to an unreasonably high value, but that seems a bit wasteful
            // we should arrive at the correct height in O(log n) iterations anyway
            _knownContentHeight = 1000000f;
        }
        else
        {
            // when we have enough space, we remember the height
            _knownContentHeight = list.CurHeight;
        }
        list.End();
        bool requiresScrolling = _knownContentHeight > canvas.height;
        if (_requiresScrolling)
        {
            Widgets.EndScrollView();
        }
        _requiresScrolling = requiresScrolling;
        base.DoSettingsWindowContents(canvas);
    }

    public override string SettingsCategory() => "More Injuries (Continued)";
}