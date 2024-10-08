﻿using System.Collections.Generic;
using Verse;
using MoreInjuries.Debug;
using MoreInjuries.HealthConditions.Choking;
using MoreInjuries.HealthConditions.Fractures;
using MoreInjuries.HealthConditions.Paralysis;
using MoreInjuries.HealthConditions.IntestinalSpill;
using MoreInjuries.HealthConditions.AdrenalineRush;
using MoreInjuries.HealthConditions.HydrostaticShock;
using MoreInjuries.HealthConditions.LungCollapse;
using MoreInjuries.HealthConditions.InhalationInjury;
using MoreInjuries.HealthConditions.SpallingInjury;
using MoreInjuries.HealthConditions.EmpShutdown;
using MoreInjuries.HealthConditions.HeadInjury;
using MoreInjuries.HealthConditions.HearingLoss;
using MoreInjuries.HealthConditions.HeadInjury.Concussions;
using MoreInjuries.HealthConditions.CardiacArrest;
using MoreInjuries.HealthConditions.HeavyBleeding;
using System.Linq;

namespace MoreInjuries.HealthConditions;

public class MoreInjuryComp : ThingComp
{
    private static readonly UIBuilder<FloatMenuOption> s_floatMenuOptionsBuilder = new([], []);
    private static readonly UIBuilder<Gizmo> s_gizmosBuilder = new([], []);

    private DamageInfo _damageInfo;
    private readonly InjuryWorker[] _pipeline;
    private readonly ICompGetGizmosExtraHandler[] _compGetGizmosExtraHandlers;
    private readonly ICompFloatMenuOptionsHandler[] _compFloatMenuOptionsHandlers;
    private readonly IPostPostApplyDamageHandler[] _postPostApplyDamageHandlers;
    private readonly IPostTakeDamageHandler[] _postTakeDamageHandlers;

    public bool CallbackActive { get; private set; } = false;

    public MoreInjuryComp()
    {
        _pipeline =
        [
            new ParalysisWorker(this),
            new IntestinalSpillWorker(this),
            new CardiacArrestWorker(this),
            new HeadInjuryWorker(this),
            new AdrenalineWorker(this),
            new HydrostaticShockWorker(this),
            new FractureWorker(this),
            new LungCollapseWorker(this),
            new InhalationInjuryWorker(this),
            new SpallingWorker(this),
            new ChokingWorker(this),
            new EmpBionicsWorker(this),
            new HearingLossExplosionsWorker(this),
            new ConcussionExplosionsWorker(this),
            new HeavyBleedingWorker(this),
            new ProvideFirstAidWorker(this)
        ];
        // cache handlers for performance
        _compGetGizmosExtraHandlers = _pipeline.OfType<ICompGetGizmosExtraHandler>().ToArray();
        _compFloatMenuOptionsHandlers = _pipeline.OfType<ICompFloatMenuOptionsHandler>().ToArray();
        _postPostApplyDamageHandlers = _pipeline.OfType<IPostPostApplyDamageHandler>().ToArray();
        _postTakeDamageHandlers = _pipeline.OfType<IPostTakeDamageHandler>().ToArray();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        UIBuilder<Gizmo> builder = s_gizmosBuilder;
        builder.Clear();
        foreach (ICompGetGizmosExtraHandler handler in _compGetGizmosExtraHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.AddGizmosExtra(builder, (Pawn)parent);
            }
        }
        return builder.Options;
    }

    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selectedPawn)
    {
        UIBuilder<FloatMenuOption> builder = s_floatMenuOptionsBuilder;
        builder.Clear();
        foreach (ICompFloatMenuOptionsHandler handler in _compFloatMenuOptionsHandlers)
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
        if (parent.Map is not null)
        {
            foreach (IPostPostApplyDamageHandler handler in _postPostApplyDamageHandlers)
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

        foreach (IPostTakeDamageHandler handler in _postTakeDamageHandlers)
        {
            if (handler.IsEnabled)
            {
                handler.PostTakeDamage(damage, in _damageInfo);
            }
        }
        CallbackActive = false;
        _damageInfo = default;
    }
}