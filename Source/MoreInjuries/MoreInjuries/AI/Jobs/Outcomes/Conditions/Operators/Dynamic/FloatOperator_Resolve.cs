using MoreInjuries.Roslyn.Future.ThrowHelpers;
using System.Collections.Generic;
using Verse;

namespace MoreInjuries.AI.Jobs.Outcomes.Conditions.Operators.Dynamic;

// memebers initialized via XML defs
[SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = JUSTIFY_IDE1006_XML_NAMING_CONVENTION)]
public sealed class FloatOperator_Resolve : FloatOperator
{
    // don't rename this field. XML defs depend on this name
    private readonly string? symbol = default;

    public override float Evaluate(Pawn doctor, Pawn patient, Thing? device, IRuntimeState? runtimeState)
    {
        Throw.ArgumentNullException.IfNull(runtimeState);
        _ = symbol ?? throw new InvalidOperationException($"{nameof(FloatOperator_Resolve)}: cannot resolve null symbol");
        if (runtimeState.TryResolve(symbol, out float value))
        {
            return value;
        }
        throw new KeyNotFoundException($"{nameof(FloatOperator_Resolve)}: Symbol '{symbol}' not found in runtime state");
    }
}