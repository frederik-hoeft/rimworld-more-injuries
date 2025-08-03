using MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Nullary;
using MoreInjuries.Defs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

public static class DynamicProcedure
{
    private static readonly ConcurrentDictionary<int, FloatOperator> s_cachedProcedures = [];

    public static FloatOperator PrepareCached(ReferenceableDef procedureDef, params ReadOnlySpan<ProcedureParameter> parameters)
    {
        int signatureHash = GetSignatureHash(procedureDef, parameters);
        if (!s_cachedProcedures.TryGetValue(signatureHash, out FloatOperator? procedureCall))
        {
            procedureCall = Prepare(procedureDef, parameters);
            s_cachedProcedures.TryAdd(signatureHash, procedureCall);
        }
        return procedureCall;
    }

    public static FloatOperator Prepare(ReferenceableDef procedureDef, params ReadOnlySpan<ProcedureParameter> parameters)
    {
        List<FloatOperator_Assign> parameterList = new(parameters.Length);
        foreach (ProcedureParameter parameter in parameters)
        {
            parameterList.Add(new FloatOperator_Assign(parameter.Name, new FloatOperator_Constant(parameter.Value)));
        }
        return new FloatOperator_DynamicRuntime_ProcedureCall(procedureDef, parameterList);
    }

    public static ProcedureParameter Parameter(string name, float value) => new(name, value);

    private static int GetSignatureHash(ReferenceableDef procedureDef, ReadOnlySpan<ProcedureParameter> parameters)
    {
        int hash = procedureDef.defNameHash;
        foreach (ProcedureParameter parameter in parameters)
        {
            hash = HashCode.Combine(hash, parameter.Name.GetHashCode(), parameter.Value.GetHashCode());
        }
        return hash;
    }

    public readonly record struct ProcedureParameter(string Name, float Value);

    public static float Evaluate(this FloatOperator procedure, Pawn doctor, Pawn patient, Thing? device) =>
        procedure.Evaluate(doctor, patient, device, runtimeState: null);
}
