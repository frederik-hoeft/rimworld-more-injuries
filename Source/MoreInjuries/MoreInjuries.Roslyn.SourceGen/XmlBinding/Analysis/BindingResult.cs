using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

/// <summary>
/// Small monadic result type used by the XML binding generator's functional core.
/// It keeps diagnostics as data instead of mutating a shared collector through the analysis pipeline.
/// </summary>
internal readonly record struct BindingResult<T>(T? Value, ImmutableArray<Diagnostic> Diagnostics)
{
    public bool IsSuccess => Diagnostics.IsDefaultOrEmpty;

    public static BindingResult<T> Success(T value) => new(value, []);

    public static BindingResult<T> Failure(Diagnostic diagnostic) => Failure([diagnostic]);

    public static BindingResult<T> Failure(ImmutableArray<Diagnostic> diagnostics)
    {
        if (diagnostics is not [_, ..])
        {
            throw new ArgumentException("Diagnostics array must not be empty.", nameof(diagnostics));
        }
        return new BindingResult<T>(default, diagnostics);
    }

    public BindingResult<TResult> Map<TResult>(Func<T, TResult> map) => IsSuccess
        ? BindingResult<TResult>.Success(map(Value!))
        : BindingResult<TResult>.Failure(Diagnostics);

    public BindingResult<TResult> Bind<TResult>(Func<T, BindingResult<TResult>> bind) => IsSuccess
        ? bind(Value!)
        : BindingResult<TResult>.Failure(Diagnostics);
}
