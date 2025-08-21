using System.Collections.Generic;
using Verse;
using MoreInjuries.Debug;
using System.Linq;
using MoreInjuries.Roslyn.Future.ThrowHelpers;
using MoreInjuries.Extensions;

namespace MoreInjuries.HealthConditions;

public class MoreInjuryComp : ThingComp
{
    private static readonly UIBuilder<FloatMenuOption> s_floatMenuOptionsBuilder = new(Keys: [], Options: []);
    private static readonly UIBuilder<Gizmo> s_gizmosBuilder = new(Keys: [], Options: []);

    private DamageInfo _damageInfo;
    private InjuryWorker[]? _pipeline;
    private readonly HandlerChain<ICompGetGizmosExtraHandler> _compGetGizmosExtraHandlers = new();
    private readonly HandlerChain<ICompFloatMenuOptionsHandler> _compFloatMenuOptionsHandlers = new();
    private readonly HandlerChain<IPostPostApplyDamageHandler> _postPostApplyDamageHandlers = new();
    private readonly HandlerChain<IPostTakeDamageHandler> _postTakeDamageHandlers = new();
    private readonly HandlerChain<ICompTickHandler> _compTickHandlers = new();
    private readonly HandlerChain<INotify_UsedVerbHandler> _usedVerbHandlers = new();
    private readonly HandlerChain<IPostApplyDamageToPartHandler> _postApplyDamageToPartHandlers = new();

    // we need an XML node to store our job parameters, so we do that on the pawn doing the job
    // because we can't be sure that the we are notified when the job is done, we need to store weak references
    // to allow the GC to do its job
    private readonly List<Std::WeakReference<IExposable>> _weakJobParameters = [];

    public bool CallbackActive { get; private set; } = false;

    internal bool FailedLoading { get; set; } = false;

    public MoreInjuryCompProperties Properties => (MoreInjuryCompProperties)props;

    public Pawn Pawn => (Pawn)parent;

    [MemberNotNull(nameof(_pipeline))]
    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);

        _pipeline = [.. Properties.WorkerFactories.Select(factory => factory.Create(this))];
        _compGetGizmosExtraHandlers.Initialize(_pipeline.OfType<ICompGetGizmosExtraHandler>());
        _compFloatMenuOptionsHandlers.Initialize(_pipeline.OfType<ICompFloatMenuOptionsHandler>());
        _postPostApplyDamageHandlers.Initialize(_pipeline.OfType<IPostPostApplyDamageHandler>());
        _postTakeDamageHandlers.Initialize(_pipeline.OfType<IPostTakeDamageHandler>());
        _compTickHandlers.Initialize(_pipeline.OfType<ICompTickHandler>());
        _usedVerbHandlers.Initialize(_pipeline.OfType<INotify_UsedVerbHandler>());
        _postApplyDamageToPartHandlers.Initialize(_pipeline.OfType<IPostApplyDamageToPartHandler>());
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
            jobParameters = 
            [
                .. _weakJobParameters.Transform(static (Std::WeakReference<IExposable> wr, out IExposable target) => wr.TryGetTarget(out target))
            ];
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
        foreach (ICompTickHandler handler in _compTickHandlers.GetActive())
        {
            handler.CompTick();
        }
    }

    public override void Notify_UsedVerb(Pawn pawn, Verb verb)
    {
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && Pawn.IsShambler)
        {
            return;
        }
        foreach (INotify_UsedVerbHandler handler in _usedVerbHandlers.GetActive())
        {
            handler.Notify_UsedVerb(pawn, verb);
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
        foreach (ICompGetGizmosExtraHandler handler in _compGetGizmosExtraHandlers.GetActive())
        {
            handler.AddGizmosExtra(builder);
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
        foreach (ICompFloatMenuOptionsHandler handler in _compFloatMenuOptionsHandlers.GetActive())
        {
            handler.AddFloatMenuOptions(builder, selectedPawn);
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
            foreach (IPostPostApplyDamageHandler handler in _postPostApplyDamageHandlers.GetActive())
            {
                handler.PostPostApplyDamage(in dinfo);
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
        DebugAssert.IsNotNull(damage, "damage is null in PostDamageFull");

        if (MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers || parent is Pawn { IsShambler: false })
        {
            foreach (IPostTakeDamageHandler handler in _postTakeDamageHandlers.GetActive())
            {
                handler.PostTakeDamage(damage, in _damageInfo);
            }
        }
        CallbackActive = false;
        _damageInfo = default;
    }

    public void ApplyDamageToPart(ref readonly DamageInfo dinfo, Pawn pawn, DamageWorker.DamageResult result)
    {
        if (!MoreInjuriesMod.Settings.AnomalyEnableConditionsForShamblers && Pawn.IsShambler)
        {
            return;
        }
        foreach (IPostApplyDamageToPartHandler handler in _postApplyDamageToPartHandlers.GetActive())
        {
            handler.ApplyDamageToPart(in dinfo, pawn, result);
        }
    }

    private sealed class HandlerChain<THandler> where THandler : IInjuryHandler
    {
        private THandler[]? _handlers;

        [MemberNotNull(nameof(_handlers))]
        public void Initialize(IEnumerable<THandler> handlers)
        {
            Throw.InvalidOperationException.If(_handlers is not null, "handler chain was already initialized");
            _handlers = [.. handlers];
        }

        public IEnumerable<THandler> GetActive()
        {
            THandler[]? handlers = _handlers;
            DebugAssert.IsNotNull(handlers);
            for (int i = 0; i < handlers.Length; ++i)
            {
                THandler handler = handlers[i];
                if (handler.IsEnabled)
                {
                    yield return handler;
                }
            }
        }
    }
}