using MoreInjuries.Caching;
using MoreInjuries.Defs.WellKnown;
using MoreInjuries.HealthConditions.Choking.Simulation;
using MoreInjuries.HealthConditions.Secondary;
using MoreInjuries.HealthConditions.Secondary.Handlers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MoreInjuries.HealthConditions.Choking;

public sealed class HediffComp_Choking : HediffComp, IHediffCompHandler
{
    private readonly ChokingSimulation _simulation;
    private Std::WeakReference<Hediff_Injury>? _source;
    private float _fluidBurden;

    public HediffComp_Choking()
    {
        _simulation = new ChokingSimulation(context: this, ChokingSimulationParameters.Default);
    }

    private TimedDataField<HediffComp_Choking, bool, Pawn, TimedDataEntry<bool>> PawnCoughingCache => field ??= new
    (
        owner: this,
        minRefreshIntervalTicks: GenTicks.TickRareInterval,
        dataProvider: static (self, pawn) =>
            pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) > 0.3f
            || ModLister.BiotechInstalled && pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest)
    );

    private bool IsCoughing => PawnCoughingCache.GetData(parent.pawn);

    public HediffCompProperties_Choking Properties => (HediffCompProperties_Choking)props;

    public override string CompLabelInBracketsExtra => IsCoughing ? "MI_Coughing".Translate() : string.Empty;

    public override void CompPostMake()
    {
        if (MoreInjuriesMod.Settings.EnableChokingSounds)
        {
            KnownSoundDefOf.Choking.PlayOneShot(SoundInfo.InMap(parent.pawn, MaintenanceType.None));
        }
    }

    public Hediff_Injury? TryGetSource()
    {
        // attempt to materialize the reference to our source
        if (_source is null || !_source.TryGetTarget(out Hediff_Injury? source))
        {
            return null;
        }
        if (!parent.pawn.health.hediffSet.hediffs.Contains(source))
        {
            // got a dead reference
            Logger.LogDebug($"Invalidating dead reference to source hediff {source.def.label}");
            source = null;
            _source = null;
        }
        return source;
    }

    public void SetSource(Hediff_Injury? value)
    {
        if (value is null)
        {
            _source = null;
        }
        else
        {
            _source = new Std::WeakReference<Hediff_Injury>(value);
        }
    }

    public override void CompExposeData()
    {
        base.CompExposeData();
        Hediff_Injury? source = TryGetSource();
        Hediff_Injury? oldSource = source;
        Scribe_References.Look(ref source, "chokingSource");
        Scribe_Values.Look(ref _fluidBurden, "fluidBurden", 0f);
        if (!ReferenceEquals(source, oldSource))
        {
            SetSource(source);
        }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        const float EPSILON = 0.001f;

        if (!parent.pawn.IsHashIntervalTick(Properties.ChokingIntervalTicks))
        {
            return;
        }
        Pawn patient = parent.pawn;
        float currentSeverity = parent.Severity;
        CurrentChokingSimulationState currentState = new(currentSeverity, _fluidBurden, TryGetSource()?.BleedRate ?? 0f, patient.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness));
        NextChokingSimulationState nextState = _simulation.MoveNext(in currentState);
        float severityChange = nextState.SeverityChange;
        float nextFluidBurden = nextState.FluidBurden;
        // allow genes, traits, and other hediffs to modify the severity change calculated by the simulation
        if (parent.def.GetModExtension<HediffModifier_SeverityModifiers_ModExtension>() is { } downstream)
        {
            severityChange = downstream.ApplyTo(severityChange, parent, this);
        }
        float nextSeverity = Mathf.Clamp01(currentSeverity + severityChange);
        bool isResolved = nextSeverity < EPSILON && nextFluidBurden < EPSILON;

        if (isResolved)
        {
            patient.health.RemoveHediff(parent);
            return;
        }

        parent.Severity = nextSeverity;
        _fluidBurden = nextFluidBurden;
        // TODO: ensure that sounds don't overlap too much
        if (MoreInjuriesMod.Settings.EnableChokingSounds)
        {
            SoundDef soundDef = (IsCoughing, patient.gender) switch
            {
                (true, Gender.Female) => KnownSoundDefOf.ChokingCoughFemale,
                (true, _) => KnownSoundDefOf.ChokingCoughMale,
                _ => KnownSoundDefOf.Choking,
            };
            soundDef.PlayOneShot(SoundInfo.InMap(patient, MaintenanceType.None));
        }
    }
}
