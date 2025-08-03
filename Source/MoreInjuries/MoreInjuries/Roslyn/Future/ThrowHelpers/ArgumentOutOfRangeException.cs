using MoreInjuries.Roslyn.Future.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.Future.ThrowHelpers;

public static partial class Throw
{
    public static class ArgumentOutOfRangeException
    {
        [DoesNotReturn]
        private static void ThrowZero<T>(T value, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-zero value.");

        [DoesNotReturn]
        private static void ThrowNegative<T>(T value, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative value.");

        [DoesNotReturn]
        private static void ThrowNegativeOrZero<T>(T value, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a value greater than zero.");

        [DoesNotReturn]
        private static void ThrowGreater<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a value less than or equal to {other}.");

        [DoesNotReturn]
        private static void ThrowGreaterEqual<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a value less than {other}.");

        [DoesNotReturn]
        private static void ThrowLess<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a value greater than or equal to {other}.");

        [DoesNotReturn]
        private static void ThrowLessEqual<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a value greater than {other}.");

        [DoesNotReturn]
        private static void ThrowEqual<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must not be equal to {other}.");

        [DoesNotReturn]
        private static void ThrowNotEqual<T>(T value, T other, string? paramName) =>
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be equal to {other}.");

        [DoesNotReturn]
        private static void ThrowNotOneOf<T>(T value, ReadOnlySpan<T> allowedValues, string? paramName) where T : IEquatable<T>
        {
            // yes, we allocate stuff here, but we're about to throw an exception anyway, so it doesn't matter
            string allowedValuesString = string.Join(", ", allowedValues.ToArray());
            throw new Std::ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be one of the following values: {allowedValuesString}.");
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is zero.</summary>
        /// <param name="value">The argument to validate as non-zero.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfZero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) == 0)
            {
                ThrowZero(value, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative.</summary>
        /// <param name="value">The argument to validate as non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfNegative<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) < 0)
            {
                ThrowNegative(value, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is negative or zero.</summary>
        /// <param name="value">The argument to validate as non-zero or non-negative.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfNegativeOrZero<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : struct, IComparable<T>
        {
            if (value.CompareTo(default) <= 0)
            {
                ThrowNegativeOrZero(value, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is equal to <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as not equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
        {
            if (EqualityComparer<T>.Default.Equals(value, other))
            {
                ThrowEqual(value, other, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is not equal to <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as equal to <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfNotEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
        {
            if (!EqualityComparer<T>.Default.Equals(value, other))
            {
                ThrowNotEqual(value, other, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as less or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfGreaterThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(other) > 0)
            {
                ThrowGreater(value, other, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is greater than or equal <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as less than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfGreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(other) >= 0)
            {
                ThrowGreaterEqual(value, other, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as greater than or equal than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfLessThan<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(other) < 0)
            {
                ThrowLess(value, other, paramName);
            }
        }

        /// <summary>Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is less than or equal <paramref name="other"/>.</summary>
        /// <param name="value">The argument to validate as greater than than <paramref name="other"/>.</param>
        /// <param name="other">The value to compare with <paramref name="value"/>.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        public static void IfLessThanOrEqual<T>(T value, T other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
            where T : IComparable<T>
        {
            if (value.CompareTo(other) <= 0)
            {
                ThrowLessEqual(value, other, paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="Std::ArgumentOutOfRangeException"/> if <paramref name="value"/> is not one of the <paramref name="allowedValues"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value to validate.</typeparam>
        /// <param name="value">The argument to validate as one of the allowed values.</param>
        /// <param name="allowedValues">The set of allowed values.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="value"/> corresponds.</param>
        // can't use params spans here since we also use CallerArgumentExpression, and both want to be the last parameter
        // so it's either adding nameof() everywhere or just using collection expressions, which is the lesser evil
        public static void IfNotOneOf<T>(T value, ReadOnlySpan<T> allowedValues, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>
        {
            if (!allowedValues.Contains(value))
            {
                ThrowNotOneOf(value, allowedValues, paramName);
            }
        }
    }
}
