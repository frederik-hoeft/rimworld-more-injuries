using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.Future.ThrowHelpers;

public static partial class Throw
{
    public static class ArgumentNullException
    {
        [DoesNotReturn]
        private static void ThrowNull(string? paramName) =>
            throw new Std::ArgumentNullException(paramName, $"{paramName} must not be null.");

        /// <summary>Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is <see langword="null"/>.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value is null)
            {
                ThrowNull(paramName);
            }
        }
    }
}
