namespace MoreInjuries.Caching.Threading;

/// <summary>
/// Represents the temporary ownership of a lock. Allows the lock to be used in a <see langword="using"/> statement.
/// </summary>
public interface ILockOwnership : IDisposable;