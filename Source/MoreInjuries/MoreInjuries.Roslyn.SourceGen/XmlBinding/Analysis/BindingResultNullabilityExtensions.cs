namespace MoreInjuries.Roslyn.SourceGen.XmlBinding.Analysis;

// Essentially another monadic wrapper, but for nullable <see cref="BindingResult{T}"/>s to allow chaining off of optional results without needing to check for null at each step.
// Unit() function is implicit since any struct can be implicitly converted to its nullable version, and the extension methods will only execute if the value is not null.
internal static class BindingResultNullabilityExtensions
{
    extension<T> (BindingResult<T>? self)
    {
        public BindingResult<TResult>? Map<TResult>(Func<T, TResult> map) => self?.Map(map);

        public BindingResult<TResult>? Bind<TResult>(Func<T, BindingResult<TResult>> bind) => self?.Bind(bind);
    }
}
