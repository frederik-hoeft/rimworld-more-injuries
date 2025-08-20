using MoreInjuries.Debug;
using System.Runtime.CompilerServices;

namespace MoreInjuries.Roslyn.Future.ThrowHelpers;

public static partial class Throw
{
    public static class InvalidOperationException
    {
        [DoesNotReturn]
        private static void ThrowNull(string instanceName, string? memberName) =>
            throw new Std::InvalidOperationException($"Member '{memberName}' of type {instanceName} must not be null. The requested operation was invalid in the current state of the object.");

        [DoesNotReturn]
        private static void ThrowAssertionFailed(string? conditionName) => 
            throw new Std::InvalidOperationException($"The requested operation was invalid in the current state of the object. Assertion '!({conditionName})' failed.");

        [DoesNotReturn]
        private static void ThrowNullOrEmpty(string instanceName, string? memberName) =>
            throw new Std::InvalidOperationException($"Member '{memberName}' of type {instanceName} must not be null or empty. The requested operation was invalid in the current state of the object.");

        /// <summary>
        /// Throws an <see cref="Std::InvalidOperationException"/> if <paramref name="member"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="instance">The instance whose member is being validated.</param>
        /// <param name="member">The member to validate as non-zero.</param>
        /// <param name="memberName">The name of the member with which <paramref name="member"/> corresponds.</param>
        [return: NotNull]
        public static T IfNull<T>([DisallowNull] object instance, [NotNull] T? member, [CallerArgumentExpression(nameof(member))] string? memberName = null)
        {
            if (member is null)
            {
                ThrowNull(instance.GetType().Name, memberName);
            }
            return member;
        }

        /// <summary>
        /// Throws an <see cref="Std::InvalidOperationException"/> if <paramref name="member"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="instance">The instance whose member is being validated.</param>
        /// <param name="member">The member to validate as non-zero.</param>
        /// <param name="memberName">The name of the member with which <paramref name="member"/> corresponds.</param>
        public static void IfNullFast<TInstance, TMember>(TInstance _, [NotNull] TMember? member, [CallerArgumentExpression(nameof(member))] string? memberName = null)
            where TInstance : struct
        {
            if (member is null)
            {
                ThrowNull(typeof(TInstance).Name, memberName);
            }
        }

        /// <summary>
        /// Throws an <see cref="Std::InvalidOperationException"/> if <paramref name="member"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="instance">The instance whose member is being validated.</param>
        /// <param name="member">The value to validate as non-null and non-empty.</param>
        /// <param name="memberName">The name of the member with which <paramref name="member"/> corresponds.</param>
        public static void IfNullOrEmpty([DisallowNull] object instance, [NotNull] string? member, [CallerArgumentExpression(nameof(member))] string? memberName = null)
        {
            if (string.IsNullOrEmpty(member))
            {
                ThrowNullOrEmpty(instance.GetType().Name, memberName);
            }
            // string.IsNullOrEmpty doesn't have nullability annotations, se we do this noop check to ensure the value is not null
            DebugAssert.IsNotNull(member);
        }

        /// <summary>
        /// Throws an <see cref="Std::InvalidOperationException"/> if <paramref name="member"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="instance">The instance whose member is being validated.</param>
        /// <param name="member">The value to validate as non-null and non-empty.</param>
        /// <param name="memberName">The name of the member with which <paramref name="member"/> corresponds.</param>
        public static void IfNullOrEmptyFast<TInstance>(TInstance _, [NotNull] string? member, [CallerArgumentExpression(nameof(member))] string? memberName = null)
            where TInstance : struct
        {
            if (string.IsNullOrEmpty(member))
            {
                ThrowNullOrEmpty(typeof(TInstance).Name, memberName);
            }
            // string.IsNullOrEmpty doesn't have nullability annotations, se we do this noop check to ensure the value is not null
            DebugAssert.IsNotNull(member);
        }

        /// <summary>
        /// Throws an <see cref="Std::InvalidOperationException"/> if the specified <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="conditionName">The name of the condition with which <paramref name="condition"/> corresponds.</param>
        public static void If([DoesNotReturnIf(true)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionName = null)
        {
            if (condition)
            {
                ThrowAssertionFailed(conditionName);
            }
        }
    }
}
