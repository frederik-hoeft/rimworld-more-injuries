namespace MoreInjuries.Roslyn.Future.ThrowHelpers;

public static partial class Throw
{
    public static class ObjectDisposedException
    {
        [DoesNotReturn]
        private static void ThrowObjectDisposed(object instance) =>
            throw new Std::ObjectDisposedException(instance.GetType().Name);

        /// <summary>Throws an <see cref="Std::ObjectDisposedException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.</summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="instance">The object whose type's full name should be included in any resulting <see cref="Std::ObjectDisposedException"/>.</param>
        /// <exception cref="Std::ObjectDisposedException">The <paramref name="condition"/> is <see langword="true"/>.</exception>
        public static void If([DoesNotReturnIf(true)] bool condition, object instance)
        {
            if (condition)
            {
                ThrowObjectDisposed(instance);
            }
        }
    }
}
