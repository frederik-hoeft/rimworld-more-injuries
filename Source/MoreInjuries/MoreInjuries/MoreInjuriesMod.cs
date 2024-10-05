using System.Linq;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using MoreInjuries.Initialization;
using System.Collections.Generic;
using MoreInjuries.Extensions;

namespace MoreInjuries;

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
    private const float MIN_CONTENT_HEIGHT = 256f;
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
        list.CheckboxLabeled("Enable logging", ref Settings.EnableLogging);
        list.CheckboxLabeled("Enable verbose logging", ref Settings.EnableVerboseLogging);
        list.GapLine();
        list.Gap();
        Text.Font = GameFont.Medium;
        list.Label("Gameplay Settings");
        Text.Font = GameFont.Small;
        list.GapLine();
        list.CheckboxLabeled("Show individual options for hemostat usage alongside 'Provide first aid option'", ref Settings.UseIndividualFloatMenus);
        list.CheckboxLabeled("Hide undiagnosed internal injuries", ref Settings.HideUndiagnosedInternalInjuries,
            """
            If enabled, internal injuries that have not been diagnosed by a doctor will not be shown in the health tab.
            This can make it harder to determine the exact state of a pawn's health, but can also add a sense of realism to the game.
            Only enable this if you want to make the game more challenging.
            """);
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
        list.CheckboxLabeled("Enable bone fractures", ref Settings.EnableFractures,
            """
            If enabled, pawns that take certain types of damage may, as a result, receive bone fractures from the impact. This is especially likely for blunt damage.
            """);
        list.Label($"Minimum damage threshold for fractures: {Settings.FractureDamageTreshold}", -1,
            """
            The minimum amount of damage required to cause a bone fracture.
            Being punched by a squirrel is unlikely to cause a fracture, but being hit by a club or a bullet is a different story.
            """);
        Settings.FractureDamageTreshold = (float)Math.Ceiling(list.Slider(Mathf.Clamp(Settings.FractureDamageTreshold, 1f, 20f), 1f, 20f));
        list.Label($"Chance of bone fractures on damage: {Settings.FractureChanceOnDamage}", -1,
            """
            The likelihood of a bone fracture actually being applied to an affected body part after all conditions for a fracture have been met. 
            Simulates the complex geometry of the human body and its susceptibility to fractures through random chance.
            """);
        Settings.FractureChanceOnDamage = (float)Math.Round(list.Slider(Settings.FractureChanceOnDamage, 0f, 1f), 2);
        list.CheckboxLabeled("Enable bone fragment lacerations", ref Settings.EnableBoneFragmentLacerations,
            """
            If enabled, pawns that receive bone fractures may also receive lacerations from the bone fragments that result from the fracture.
            """);
        list.Label($"Change of bone fragment lacerations: {Settings.SplinteringFractureChance}", -1,
            """
            The likelihood that a fracture may cause the bone to splinter, which can cause additional lacerations to nearby body parts.
            """);
        Settings.SplinteringFractureChance = (float)Math.Round(list.Slider(Settings.SplinteringFractureChance, 0f, 1f), 2);
        list.Label($"Chance of bone fragment lacerations per body part: {Settings.BoneFragmentLacerationChancePerBodyPart}", -1,
            """
            The likelihood that a nearby body part may receive bone fragment lacerations after a splintering fracture has occurred.
            """);
        Settings.BoneFragmentLacerationChancePerBodyPart = (float)Math.Round(list.Slider(Settings.BoneFragmentLacerationChancePerBodyPart, 0f, 1f), 2);
        // respiratory conditions
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Respiratory Conditions");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable choking on blood mechanics", ref Settings.EnableChoking,
            """
            If enabled, pawns that receive severe lacerations to the respiratory system may choke on their own blood, leading to asphyxiation and death if not treated in time.
            A gruesome reminder of the fragility of life.
            """);
        list.CheckboxLabeled("Enable choking sounds", ref Settings.EnableChokingSounds,
            """
            Whether to play a sound effect when a pawn starts choking on their own blood.
            Potentially disturbing, but adds a sense of urgency to the situation.
            """);
        list.Label($"Chance of choking on blood after severe damage: {Settings.ChokingChanceOnDamage}", -1,
            """
            The likelihood of a pawn choking on their own blood after receiving severe, bleeding injuries to the respiratory system.
            """);
        Settings.ChokingChanceOnDamage = (float)Math.Round(list.Slider(Settings.ChokingChanceOnDamage, 0f, 1f), 2);
        list.CheckboxLabeled("Enable inhalation injuries", ref Settings.EnableFireInhalation,
            """
            If enabled, pawns that are exposed to fire or other sources of smoke and hot gases may suffer from inhalation injuries, causing severe damage to the respiratory system.
            Smoking is bad for your health, but inhaling smoke from a burning building or residue from thermobaric munitions is even worse.
            """);
        list.CheckboxLabeled("Enable lung collapses", ref Settings.EnableLungCollapse,
            """
            If enabled, thermobaric weapons and other high-explosive devices can cause the lungs to rupture and collapse due to the sudden pressure changes. Must be surgically repaired.
            May be fatal if not treated in time.
            """);
        list.Label($"Chance of lung collapse on thermobaric damage: {Settings.LungCollapseChanceOnDamage}", -1,
            """
            The likelihood of a pawn suffering from lung collapse after being hit by a thermobaric weapon or other high-explosive device.
            """);
        Settings.LungCollapseChanceOnDamage = (float)Math.Round(list.Slider(Settings.LungCollapseChanceOnDamage, 0f, 1f), 2);
        list.Label($"Square root of the maximum initial severity of lung collapse: {Settings.LungCollapseMaxSeverityRoot}", -1,
            """
            The maximum initial severity of lung collapse is determined by a random factor between 0 and this value, squared. Stacks with subsequent exposure to thermobaric damage.
            The resulting severity of lung collapse therefore follows a quadratic distribution, with a higher likelihood of low severities and a small chance of very high severities.
            For example, at 0.5, the severity of lung collapse will follow a quadratic distribution between 0 and 0.25.
            """);
        Settings.LungCollapseMaxSeverityRoot = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.LungCollapseMaxSeverityRoot, 0.1f, 1f), 0.1f, 1f), 2);
        // Spalling
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Spalling");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable spalling mechanics", ref Settings.EnableSpalling,
            """
            When high-velocity projectiles are stopped by armor, the large amount of kinetic energy can cause the projectile and top layer of the armor to shatter and send fragments flying in all directions.
            The amount of spalling depends on the angle of impact and the hardness and condition of the armor.
            Spalling can cause additional injuries to the wearer of the armor, even if the projectile itself did not penetrate the armor.
            """);
        list.Label(
            $"""
            Base chance of armor creating spall based on condition (at 1, the chance of creating spall is 0 with armor having 100% hp, 0.01 with armor 99% hp etc.): {Settings.ArmorHealthSpallingThreshold}
            Modern armor is designed to prevent spalling by adding softer layers above the hard armor plates to catch and absorb bullet fragments.
            As armor condition deteriorates after absorbing damage, the chance of spalling naturally increases when these absorbing layers are compromised.
            At 0.8, the chance of spalling remains 0 until the armor is at 80% hp, etc. At 0, spalling is disabled.
            """);
        Settings.ArmorHealthSpallingThreshold = list.Slider(Mathf.Clamp(Settings.ArmorHealthSpallingThreshold, 0.1f, 1f), 0.1f, 1f);
        list.Label($"Chance of spalling injuries: {Settings.SpallingChance}", -1,
            """
            The likelihood of exposed body parts receiving spalling injuries after spalling has occurred. Evaluated per body part.
            """);
        Settings.SpallingChance = (float)Math.Round(list.Slider(Settings.SpallingChance, 0f, 1f), 2);
        // hypovolemic shock
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Blood Loss (Hypovolemic Shock)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hypovolemic shock mechanics (requires game reload)", ref Settings.EnableHypovolemicShock,
            """
            If enabled, pawns that lose a significant amount of blood may suffer from hypovolemic shock, which can be fatal if not treated in time with a blood transfusion.
            The body needs a certain amount of blood to function properly, and losing too much can lead to organ failure and death.
            """);
        list.Label($"Chance of hypovolemic shock to cause organ hypoxia (every 300 ticks/5s): {Settings.OrganHypoxiaChance}", -1,
            """
            Loosing a lot of blood will lead to organs not receiving enough oxygen to function properly. This can ultimately lead to organ failure and death.
            Treat the patient with a blood transfusion in time to prevent this from happening.
            """);
        Settings.OrganHypoxiaChance = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChance, 0f, 1f), 2);
        list.Label($"Reduction factor for organ hypoxia chance when patient is tended: {Settings.OrganHypoxiaChanceReductionFactor}", -1,
            """
            Tending the hypovolemic shock condition will reduce the chance of organ hypoxia by this factor and will slow down the progression of the shock.
            Note that this will not prevent the shock from progressing, but only slow it down. To fully stabilize the patient, a blood transfusion is required.
            """);
        Settings.OrganHypoxiaChanceReductionFactor = (float)Math.Round(list.Slider(Settings.OrganHypoxiaChanceReductionFactor, 0f, 1f), 2);
        // hemorrhagic stroke after blunt trauma
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Traumatic Head Injuries (Hemorrhagic Stroke)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hemorrhagic stroke mechanics", ref Settings.EnableHemorrhagicStroke,
            """
            If enabled, pawns that receive major blunt trauma may suffer from a hemorrhagic stroke, which can be fatal if not treated in time.
            Beating up your prisoners may have more severe consequences than you think.
            """);
        list.Label($"Chance of hemorrhagic stroke on blunt trauma: {Settings.HemorrhagicStrokeChance}", -1,
            """
            The likelihood of a hemorrhagic stroke being applied to a pawn after receiving a massive amount of blunt trauma.
            """);
        Settings.HemorrhagicStrokeChance = (float)Math.Round(list.Slider(Settings.HemorrhagicStrokeChance, 0f, 1f), 2);
        // EMP damage to bionics
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("EMP Damage to Bionics");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable EMP damage to bionics", ref Settings.EnableEmpDamageToBionics,
            """
            If enabled, electromagnetic pulse (EMP) damage can cause bionic implants to malfunction and shut down temporarily.
            Sophisticated technology can be a double-edged sword.
            """);
        list.Label($"Chance of EMP damage to bionics: {Settings.EmpDamageToBionicsChance}", -1,
            """
            The likelihood of EMP damage causing bionic implants to shut down temporarily. Evaluated per bionic implant after EMP damage has been applied.
            """);
        Settings.EmpDamageToBionicsChance = (float)Math.Round(list.Slider(Settings.EmpDamageToBionicsChance, 0f, 1f), 2);
        // adrenaline
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Adrenaline");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable adrenaline mechanics", ref Settings.EnableAdrenaline,
            """
            If enabled, pawns that take damage may receive a rush of adrenaline that temporarily boosts moving capabilitites and numbs the perception of pain.
            """);
        list.Label($"Chance of adrenaline rush on damage: {Settings.AdrenalineChanceOnDamage}", -1,
            """
            The likelihood of a pawn receiving an adrenaline rush after taking damage. Evaluated per damage event.
            The intensity of the rush depends on the amount of damage taken.
            """);
        Settings.AdrenalineChanceOnDamage = (float)Math.Round(list.Slider(Settings.AdrenalineChanceOnDamage, 0f, 1f), 2);
        list.Label($"Damage threshold for certain adrenaline rush: {Settings.CertainAdrenalineThreshold}", -1,
            """
            The amount of damage required to always cause an adrenaline rush, which provides a significant boost to moving capabilities and numbs the perception of pain for a short time.
            If the damage taken exceeds this threshold, an adrenaline rush is guaranteed to occur. Otherwise, the chance is determined by the adrenaline chance on damage.
            """);
        Settings.CertainAdrenalineThreshold = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.CertainAdrenalineThreshold, 1f, 50f), 1f, 50f));
        // hydrostatic shock (controversial)
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Hydrostatic Shock (Controversial)");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hydrostatic shock mechanics", ref Settings.EnableHydrostaticShock,
            """
            Hydrostatic shock, also known as Hydro-shock, is the controversial concept that a penetrating projectile (such as a bullet) can produce a pressure wave that causes "remote neural damage", "subtle damage in neural tissues" and "rapid effects" in living targets.
            If enabled, pawns that are hit by very-high-energy projectiles may suffer from hydrostatic shock, a type of intracerebral hemorrhage (rupture of blood vessels in the brain), which can be fatal if not treated in time.
            The existence of hydrostatic shock is a topic of debate among medical professionals and firearms experts. Use at your own discretion.
            """);
        list.Label($"Chance of hydrostatic shock on damage: {Settings.HydrostaticShockChanceOnDamage}", -1,
            """
            The likelihood of a pawn suffering from hydrostatic shock after being hit by a very-high-energy projectile. Evaluated per damage event.
            """);
        Settings.HydrostaticShockChanceOnDamage = (float)Math.Round(list.Slider(Settings.HydrostaticShockChanceOnDamage, 0f, 1f), 2);
        // miscellaneous
        list.GapLine();
        Text.Font = GameFont.Medium;
        list.Label("Miscellaneous");
        Text.Font = GameFont.Small;
        list.CheckboxLabeled("Enable hearing damage mechanics (requires game reload)", ref Settings.EnableHearingDamage,
            """
            If enabled, pawns shooting or being close to loud weapons may suffer from hearing loss, especially if indoors or not wearing ear protection.
            Helmets and other apparel that covers the ears can reduce the risk of hearing damage.
            Apparently, gunshots are loud. Who knew?
            """);
        list.Label($"Multiplier for bleeding from closed internal injuries: {Settings.ClosedInternalWouldBleedingModifier}", -1,
            """
            Assume a pawn is shot in the torso and the bullet penetrates the skin and stomach, causing internal and external bleeding.
            Simply applying a bandage to the skin wound will not stop the internal bleeding, but may still slow it down.
            This modifier determines how much the internal bleeding is reduced by applying a bandage to the skin wound.
            A value of 1 means that the internal bleeding is not affected by the bandage, while a value of 0 means that the internal bleeding is completely stopped.
            """);
        Settings.ClosedInternalWouldBleedingModifier = (float)Math.Round(list.Slider(Settings.ClosedInternalWouldBleedingModifier, 0f, 1f), 2);
        list.Label($"Paralysis damage threshold (50% point): {Settings.ParalysisDamageTreshold50Percent}", -1,
            """
            The amount of spinal cord damage required to cause paralysis in 50% of all cases. The actual chance of paralysis scales linearly with the damage dealt.
            """);
        Settings.ParalysisDamageTreshold50Percent = (float)Math.Round(list.Slider(Mathf.Clamp(Settings.ParalysisDamageTreshold50Percent, 1f, 20f), 1f, 20f));
        list.Label($"Chance of intestinal spilling on damage: {Settings.IntestinalSpillingChanceOnDamage}", -1,
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