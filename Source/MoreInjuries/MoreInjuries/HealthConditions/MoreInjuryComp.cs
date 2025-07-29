using System.Collections.Generic;
using Verse;
using MoreInjuries.Debug;
using System.Linq;

namespace MoreInjuries.HealthConditions;

public class MoreInjuryComp : ThingComp
{
    private static readonly UIBuilder<FloatMenuOption> s_floatMenuOptionsBuilder = new(Keys: [], Options: []);
    private static readonly UIBuilder<Gizmo> s_gizmosBuilder = new(Keys: [], Options: []);

    private DamageInfo _damageInfo;
    private InjuryWorker[]? _pipeline;
    private ICompGetGizmosExtraHandler[]? _compGetGizmosExtraHandlers;
    private ICompFloatMenuOptionsHandler[]? _compFloatMenuOptionsHandlers;
    private IPostPostApplyDamageHandler[]? _postPostApplyDamageHandlers;
    private IPostTakeDamageHandler[]? _postTakeDamageHandlers;
    private ICompTickHandler[]? _compTickHandlers;

    // we need an XML node to store our job parameters, so we do that on the pawn doing the job
    // because we can't be sure that the we are notified when the job is done, we need to store weak references
    // to allow the GC to do its job
    private readonly List<Std::WeakReference<IExposable>> _weakJobParameters = [];

    public bool CallbackActive { get; private set; } = false;

    internal bool FailedLoading { get; set; } = false;

    public MoreInjuryCompProperties Properties => (MoreInjuryCompProperties)props;

    public Pawn Pawn => (Pawn)parent;

    private ICompGetGizmosExtraHandler[] CompGetGizmosExtraHandlers => _compGetGizmosExtraHandlers!;

    private ICompFloatMenuOptionsHandler[] CompFloatMenuOptionsHandlers => _compFloatMenuOptionsHandlers!;

    private IPostPostApplyDamageHandler[] PostPostApplyDamageHandlers => _postPostApplyDamageHandlers!;

    private IPostTakeDamageHandler[] PostTakeDamageHandlers => _postTakeDamageHandlers!;

    private ICompTickHandler[] CompTickHandlers => _compTickHandlers!;

    [MemberNotNull(
        nameof(_pipeline), 
        nameof(_compGetGizmosExtraHandlers), 
        nameof(_compFloatMenuOptionsHandlers), 
        nameof(_postPostApplyDamageHandlers),
        nameof(_postTakeDamageHandlers), 
        nameof(_compTickHandlers))]
    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);

        _pipeline = [.. Properties.WorkerFactories.Select(factory => factory.Create(this))];
        _compGetGizmosExtraHandlers = [.. _pipeline.OfType<ICompGetGizmosExtraHandler>()];
        _compFloatMenuOptionsHandlers = [.. _pipeline.OfType<ICompFloatMenuOptionsHandler>()];
        _postPostApplyDamageHandlers = [.. _pipeline.OfType<IPostPostApplyDamageHandler>()];
        _postTakeDamageHandlers = [.. _pipeline.OfType<IPostTakeDamageHandler>()];
        _compTickHandlers = [.. _pipeline.OfType<ICompTickHandler>()];
    }

    public void PersistJobParameters(IExposable jobParameter)
    {
        // remove dead references if we are at capacity
        if (_weakJobParameters.Count + 1 > _weakJobParameters.Capacity)
        {
            _weakJobParameters.RemoveAll(static wr => !wr.TryGetTarget(out _));
        }
        _weakJobParameters.Add(new Std::WeakReference<IExposable>(jobParameter));
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        List<IExposable>? jobParameters = null;
        if (Scribe.mode is LoadSaveMode.Saving)
        {
            // remove dead references
            _weakJobParameters.RemoveAll(static wr => !wr.TryGetTarget(out _));
            // box everything to a list of strong references for serialization
            jobParameters = _weakJobParameters.Select(wr =>
            {
                if (wr.TryGetTarget(out IExposable target))
                {
                    return target;
                }
                return null;
            }).Where(static x => x is not null).ToList()!;
        }
        Scribe_Collections.Look(ref jobParameters, "jobParameters", LookMode.Deep);
        if (Scribe.mode is LoadSaveMode.LoadingVars)
        {
            // update our weak references
            _weakJobParameters.Clear();
            if (jobParameters is not null)
            {
                foreach (IExposable jobParameter in jobParameters)
                {
                    _weakJobParameters.Add(new Std::WeakReference<IExposable>(jobParameter));
                }
            }
            else
            {
                Logger.Log($"Failed to load jobParameters cache from Pawn {parent?.ToStringSafe()}! Either MoreInjuries was newly added or the save file is corrupt :O");
                FailedLoading = true;
            }
        }
    }

    public override void CompTick()
    {
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && Pawn.IsShambler)
        {
            return;
        }
        foreach (ICompTickHandler handler in CompTickHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.CompTick();
            }
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        Pawn pawn = Pawn;
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && pawn.IsShambler)
        {
            return [];
        }
        UIBuilder<Gizmo> builder = s_gizmosBuilder;
        builder.Clear();
        foreach (ICompGetGizmosExtraHandler handler in CompGetGizmosExtraHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.AddGizmosExtra(builder, pawn);
            }
        }
        return builder.Options;
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && (Pawn.IsShambler || selectedPawn.IsShambler))
        {
            return [];
        }
        UIBuilder<FloatMenuOption> builder = s_floatMenuOptionsBuilder;
        builder.Clear();
        foreach (ICompFloatMenuOptionsHandler handler in CompFloatMenuOptionsHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.AddFloatMenuOptions(builder, selectedPawn);
            }
        }
        return builder.Options;
    }

    public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && parent is Pawn { IsShambler: true })
        {
            return;
        }
        if (parent.Map is not null)
        {
            foreach (IPostPostApplyDamageHandler handler in PostPostApplyDamageHandlers)
            {
                if (handler.IsEnabled)
                {
                    handler.PostPostApplyDamage(in dinfo);
                }
            }
        }

        base.PostPostApplyDamage(dinfo, totalDamageDealt);
    }

    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        CallbackActive = true;
        _damageInfo = dinfo;

        base.PostPreApplyDamage(ref dinfo, out absorbed);
    }

    public void PostDamageFull(DamageWorker.DamageResult damage)
    {
        DebugAssert.IsTrue(CallbackActive, "CallbackActive is false in PostDamageFull");
        DebugAssert.NotNull(damage, "damage is null in PostDamageFull");

        if (MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers || parent is Pawn { IsShambler: false })
        {
            foreach (IPostTakeDamageHandler handler in PostTakeDamageHandlers)
            {
                if (handler.IsEnabled)
                {
                    handler.PostTakeDamage(damage, in _damageInfo);
                }
            }
        }
        CallbackActive = false;
        _damageInfo = default;
    }
}