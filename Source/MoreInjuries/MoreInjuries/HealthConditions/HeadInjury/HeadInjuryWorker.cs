using MoreInjuries.HealthConditions.HeadInjury.Concussions;
using MoreInjuries.HealthConditions.HeadInjury.HemorrhagicStroke;
using MoreInjuries.KnownDefs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MoreInjuries.HealthConditions.HeadInjury;

public class HeadInjuryWorker(MoreInjuryComp parent) : InjuryWorker(parent), IPostTakeDamageHandler
{
    private readonly HeadInjuryGiver[] _headInjuryGivers =
    [
        new ConcussionGiver(),
        new HemorrhagicStrokeGiver(),
    ];

    public static HashSet<string> DamageDefNameWhitelist { get; }

    public override bool IsEnabled => _headInjuryGivers.Any(static giver => giver.IsEnabled);

    static HeadInjuryWorker()
    {
        // allowed damage types that may cause a head injury when applied to the head
        DamageDefNameWhitelist =
        [
            "Arrow",
            "ArrowHighVelocity",
            "Beanbag",
            "Bite",
            "BiteToxic",
            DamageDefOf.Blunt.defName,
            DamageDefOf.Bomb.defName,
            "BombSuper",
            DamageDefOf.Bullet.defName,
            "BulletToxic",
            DamageDefOf.Crush.defName,
            "EnergyBolt",
            "Nerve",
            DamageDefOf.Stab.defName,
            "Thermobaric", // from Combat Extended
        ];
    }

    private float CalculateSeverityFactor(BodyPartDef bodyPart) => __ switch
    {
        _ when bodyPart == KnownBodyPartDefOf.Brain => 3.0f,
        _ when bodyPart == KnownBodyPartDefOf.Skull => 1.5f,
        _ when bodyPart == KnownBodyPartDefOf.Ear => 1f,
        _ when bodyPart == BodyPartDefOf.Eye => 0.7f,
        _ when bodyPart == KnownBodyPartDefOf.Nose => 0.5f,
        _ => 0.75f
    };

    public void PostTakeDamage(DamageWorker.DamageResult damage, ref readonly DamageInfo dinfo)
    {
        if (damage.parts is not List<BodyPartRecord> { Count: > 0 } bodyParts)
        {
            return;
        }
        if (!DamageDefNameWhitelist.Contains(dinfo.Def.defName))
        {
            return;
        }
        Pawn patient = Target;
        // assuming an even distribution of the damage across all affected body parts, we can calculate the weighted damage to the head
        float weightedHeadTrauma = 0;
        float aggregatedBodyTrauma = bodyParts.Sum(static bodyPart => bodyPart.coverage);
        foreach (BodyPartRecord bodyPart in bodyParts)
        {
            // check if the body part of the head (body part group FullHead)
            if (bodyPart.groups.Contains(BodyPartGroupDefOf.FullHead))
            {
                // determine severity factor based on the specific body part
                float severityFactor = CalculateSeverityFactor(bodyPart.def);
                // calculate the damage to the head based on the severity factor and the weighted damage absorbed by the body part
                weightedHeadTrauma += severityFactor * damage.totalDamageDealt * bodyPart.coverage / aggregatedBodyTrauma;
            }
        }
        if (weightedHeadTrauma == 0f)
        {
            return;
        }
        // equivalentHeadTrauma is the equivalent damage directly applied to the skull to determine the maximum likeliness of the head injury
        float equivalentHeadTrauma = weightedHeadTrauma / 1.5f;
        foreach (HeadInjuryGiver giver in _headInjuryGivers)
        {
            if (giver.IsEnabled)
            {
                giver.TryGiveInjury(patient, equivalentHeadTrauma);
            }
        }
    }
}
