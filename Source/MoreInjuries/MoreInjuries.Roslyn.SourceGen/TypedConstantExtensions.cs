using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MoreInjuries.Roslyn.SourceGen;

internal static class TypedConstantExtensions
{
    public static string ToCSharpStringWithPostfix(this TypedConstant constant)
    {
        string postfix = constant.Type?.SpecialType switch
        {
            SpecialType.System_Int64 => "L",
            SpecialType.System_UInt32 => "u",
            SpecialType.System_UInt64 => "uL",
            SpecialType.System_Single => "f",
            SpecialType.System_Double => "d",
            SpecialType.System_Decimal => "m",
            _ => string.Empty
        };
        return $"{constant.ToCSharpString()}{postfix}";
    }
}