using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MoreInjuries.Caching.Threading;

/// <summary>
/// A variant of <see cref="ReaderWriterLockSlim"/> that supports multiple readers and multiple writers concurrently,
/// but does not allow simultaneous access by members of different groups, and does not allow upgrading from one group
/// to another. The semantics of the groups are application-defined, but typically a group represents a common type of
/// operation that is compatible with other operations in the same group but not with operations in other groups.
/// </summary>
/// <remarks>
/// Alpha locks take precedence over beta locks. If a thread holds an alpha lock, it will block any threads attempting
/// to acquire a beta lock. Similarly, newly requested beta locks will block if there are alphas waiting to acquire the
/// lock, even if the lock is currently held by the beta group.
/// </remarks>
[DebuggerDisplay("Lock ID: {_lockId}, Owner: {OwnerGroup}")]
public sealed class AlphaBetaLockSlim : IDisposable
{
    // Lock specification for _spinLock:  This lock protects exactly the local fields associated with this
    // instance of AlphaBetaLockSlim.  It does NOT protect the memory associated with
    // the events that are raised by this lock (eg _alphaEvent, _betaEvent).
    private SpinLock _spinLock;

    // These variables allow use to avoid Setting events (which is expensive) if we don't have to.
    private uint _numAlphaWaiters;      // maximum number of threads that can be doing a WaitOne on the alphaEvent
    private uint _numBetaWaiters;       // maximum number of threads that can be doing a WaitOne on the betaEvent

    // conditions we wait on.
    private EventWaitHandle? _alphaEvent;       // threads waiting to acquire an alpha lock go here (will be released in bulk)
    private EventWaitHandle? _betaEvent;        // threads waiting to acquire a beta lock go here (will be released in bulk)

    // Every lock instance has a unique ID, which is used by AlphaBetaCount to associate itself with the lock
    // without holding a reference to it.
    private static long s_globalNextLockId;
    private readonly long _lockId;

    [ThreadStatic]
    [SuppressMessage(CODE_STYLE, STYLE_IDE1006_NAMING_STYLES, Justification = "thread-local variable")]
    private static AlphaBetaCount? __counts;

    private const int MAX_SPIN_COUNT = 20;

    // The ulong, that contains info about the lock, is divided as follows:
    //
    // Waiting-Alphas Num-Alphas    RESERVED     Num-Betas
    //    63            62.....32      31        30.......0
    //
    // Dividing the ulong, allows to vastly simplify logic for checking if an
    // alpha/beta should go in etc. Setting any alpha bit will automatically
    // make the value of the ulong much larger than the max num of betas
    // allowed, thus causing the check for max_betas to fail, and causing
    // the beta to wait.
    private ulong _owners;

    private const ulong WAITING_ALPHAS = 0x8000000000000000;
    private const ulong ALPHA_COUNT_MASK = 0x7FFFFFFF00000000;
    private const ulong BETA_COUNT_MASK = 0x000000007FFFFFFF;

    // The max numbers in each group are actually one less then their theoretical max.
    // This is done in order to prevent count overflows. If the count reaches max,
    // other members will wait.
    private const ulong MAX_BETAS = BETA_COUNT_MASK - 1;
    private const ulong MAX_ALPHAS = MAX_BETAS;

    private bool _disposedValue;

    /// <summary>
    /// Constructs a new instance of the <see cref="AlphaBetaLockSlim"/> class.
    /// </summary>
    public AlphaBetaLockSlim()
    {
        _lockId = Interlocked.Increment(ref s_globalNextLockId);
    }

    private AlphaBetaOwner OwnerGroup
    {
        get
        {
            // atomic 64-bit read (no tearing)
            ulong owners = Volatile.Read(ref _owners);
            uint alphas = (uint)((owners & ALPHA_COUNT_MASK) >>> 32);
            uint betas = (uint)(owners & BETA_COUNT_MASK);
            Std::Diagnostics.Debug.Assert(alphas == 0 || betas == 0, "alphas and betas cannot be non-zero at the same time");
            if (alphas > 0)
            {
                return AlphaBetaOwner.Alpha;
            }
            if (betas > 0)
            {
                return AlphaBetaOwner.Beta;
            }
            return AlphaBetaOwner.None;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsABCEmpty(AlphaBetaCount abc) =>
        abc.LockId == 0 || abc.Ownership == AlphaBetaOwner.None;

    private bool IsABCHashEntryChanged(AlphaBetaCount abc) => abc.LockId != _lockId;

    /// <summary>
    /// This routine retrieves/sets the per-thread counts needed to enforce the
    /// various rules related to acquiring the lock.
    ///
    /// DontAllocate is set to true if the caller just wants to get an existing
    /// entry for this thread, but doesn't want to add one if an existing one
    /// could not be found.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AlphaBetaCount? GetThreadABCount(bool dontAllocate)
    {
        AlphaBetaCount? empty = null;
        for (AlphaBetaCount? abc = __counts; abc != null; abc = abc.Next)
        {
            // If we find an entry for this thread, return it.
            if (abc.LockId == _lockId)
            {
                return abc;
            }
            // re-use an empty ABC if we find one.
            if (!dontAllocate && empty == null && IsABCEmpty(abc))
            {
                empty = abc;
            }
        }

        if (dontAllocate)
        {
            return null;
        }

        if (empty == null)
        {
            empty = new AlphaBetaCount
            {
                Next = __counts
            };
            __counts = empty;
        }

        empty.LockId = _lockId;
        return empty;
    }

    /// <summary>
    /// Acquires an <see cref="ILockOwnership"/> for the beta group lock, blocking the current thread until the lock is acquired.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold an alpha lock, or if there are threads waiting
    /// to acquire an alpha lock, this method will only succeed once all alpha locks are released and no new alpha lock requests are pending.
    /// As a result, alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <returns>An <see cref="ILockOwnership"/> that represents the acquired lock.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the beta lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the alpha lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public ILockOwnership AcquireBetaLock() => new BetaLockOwnership(this);

    /// <summary>
    /// Enters the lock for the beta group, blocking the current thread until the lock is acquired.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold an alpha lock, or if there are threads waiting
    /// to acquire an alpha lock, this method will only succeed once all alpha locks are released and no new alpha lock requests are pending.
    /// As a result, alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <exception cref="LockRecursionException">The current thread already holds the beta lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the alpha lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public void EnterBetaLock() => TryEnterBetaLock(Timeout.Infinite);

    /// <summary>
    /// Attempts to enter the lock for the beta group. If the lock is not available, this method will return immediately.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold an alpha lock, or if there are threads waiting
    /// to acquire an alpha lock, this method will only succeed once all alpha locks are released and no new alpha lock requests are pending.
    /// As a result, alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the beta lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the alpha lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterBetaLock() => TryEnterBetaLockCore(new TimeoutTracker(0));

    /// <summary>
    /// Attempts to enter the lock for the beta group. If the lock is not available, the thread will wait for the specified amount of time.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold an alpha lock, or if there are threads waiting
    /// to acquire an alpha lock, this method will only succeed once all alpha locks are released and no new alpha lock requests are pending.
    /// As a result, alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <param name="timeout">The maximum time to wait for the lock.</param>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/> if the lock was not acquired before the specified timeout.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the beta lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the alpha lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterBetaLock(TimeSpan timeout) => TryEnterBetaLockCore(new TimeoutTracker(timeout));

    /// <summary>
    /// Attempts to enter the lock for the beta group. If the lock is not available, the thread will wait for the specified amount of time.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold an alpha lock, or if there are threads waiting
    /// to acquire an alpha lock, this method will only succeed once all alpha locks are released and no new alpha lock requests are pending.
    /// As a result, alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <param name="millisecondsTimeout">The maximum time to wait for the lock, in milliseconds, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/> if the lock was not acquired before the specified timeout.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the beta lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the alpha lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterBetaLock(int millisecondsTimeout) => TryEnterBetaLockCore(new TimeoutTracker(millisecondsTimeout));

    private bool TryEnterBetaLockCore(TimeoutTracker timeout)
    {
        AlphaBetaCount abc = GetThreadABCount(dontAllocate: false)!;
        // Can't acquire beta lock with alpha lock held.
        _ = abc.Ownership switch
        {
            AlphaBetaOwner.Alpha => throw new InvalidOperationException("Beta lock cannot be acquired while holding an alpha lock."),
            AlphaBetaOwner.Beta => throw new LockRecursionException($"Lock recursion detected while trying to enter the beta lock. {nameof(AlphaBetaLockSlim)} does not support lock recursion."),
            _ => __
        };
        bool retVal = true;
        int spinCount = 0;
        _spinLock.Enter(EnterSpinLockReason.EnterBeta);
        while (true)
        {
            // We can enter a beta lock if there have only beta-locks been given out
            // and an alpha is not trying to get in.
            if (_owners < MAX_BETAS)
            {
                // Good case, there is no contention, we are basically done
                SignalBetaLockAcquisition();
                abc.Ownership = AlphaBetaOwner.Beta;
                break;
            }
            if (timeout.IsExpired)
            {
                // We timed out.  Fail.
                _spinLock.Exit();
                return false;
            }

            if (spinCount < MAX_SPIN_COUNT && ShouldSpinForEnterBeta())
            {
                _spinLock.Exit();
                spinCount++;
                SpinWait(spinCount);
                _spinLock.Enter(EnterSpinLockReason.EnterBeta);
                // The per-thread structure may have been recycled as the lock is acquired (due to message pumping), load again.
                if (IsABCHashEntryChanged(abc))
                {
                    abc = GetThreadABCount(dontAllocate: false)!;
                }
                continue;
            }
            // Drat, we need to wait. Mark that we have waiters and wait.
            if (_betaEvent == null)
            {
                // Create the needed event (temporarily leaving the lock during wait handle creation)
                LazyCreateEvent(ref _betaEvent, isAcquiringBetaLock: true);
                if (IsABCHashEntryChanged(abc))
                {
                    abc = GetThreadABCount(dontAllocate: false)!;
                }
                // since we left the lock, start over.
                continue;
            }

            retVal = WaitOnEvent(_betaEvent, ref _numBetaWaiters, timeout, isAcquiringBetaLock: true);
            if (!retVal)
            {
                return false;
            }
            if (IsABCHashEntryChanged(abc))
            {
                abc = GetThreadABCount(dontAllocate: false)!;
            }
        }

        _spinLock.Exit();
        return retVal;
    }

    /// <summary>
    /// Acquires an <see cref="ILockOwnership"/> for the alpha group lock, blocking the current thread until the lock is acquired.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold a beta lock, this method will wait until all beta locks are released.
    /// New attempts to acquire a beta lock will be delayed until all alpha lock requests are satisfied and the alpha lock is released. As a result,
    /// alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <returns>An <see cref="ILockOwnership"/> that represents the acquired lock.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the alpha lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the beta lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public ILockOwnership AcquireAlphaLock() => new AlphaLockOwnership(this);

    /// <summary>
    /// Enters the lock for the alpha group, blocking the current thread until the lock is acquired.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold a beta lock, this method will wait until all beta locks are released.
    /// New attempts to acquire a beta lock will be delayed until all alpha lock requests are satisfied and the alpha lock is released. As a result,
    /// alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <exception cref="LockRecursionException">The current thread already holds the alpha lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the beta lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public void EnterAlphaLock() => TryEnterAlphaLock(Timeout.Infinite);

    /// <summary>
    /// Attempts to enter the lock for the alpha group. If the lock is not available, this method will return immediately.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks.
    /// New attempts to acquire a beta lock will be delayed until all alpha lock requests are satisfied and the alpha lock is released. As a result,
    /// alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the alpha lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the beta lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterAlphaLock() => TryEnterAlphaLockCore(new TimeoutTracker(0));

    /// <summary>
    /// Attempts to enter the lock for the alpha group. If the lock is not available, the thread will wait for the specified amount of time.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold a beta lock, this method will wait until all beta locks are released.
    /// New attempts to acquire a beta lock will be delayed until all alpha lock requests are satisfied and the alpha lock is released. As a result,
    /// alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <param name="timeout">The maximum time to wait for the lock.</param>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/> if the lock was not acquired before the specified timeout.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the alpha lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the beta lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterAlphaLock(TimeSpan timeout) => TryEnterAlphaLockCore(new TimeoutTracker(timeout));

    /// <summary>
    /// Attempts to enter the lock for the alpha group. If the lock is not available, the thread will wait for the specified amount of time.
    /// </summary>
    /// <remarks>
    /// Alpha locks take precedence over beta locks. If other threads hold a beta lock, this method will wait until all beta locks are released.
    /// New attempts to acquire a beta lock will be delayed until all alpha lock requests are satisfied and the alpha lock is released. As a result,
    /// alpha lock requests may starve beta lock requests.
    /// </remarks>
    /// <param name="millisecondsTimeout">The maximum time to wait for the lock, in milliseconds, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
    /// <returns><see langword="true"/> if the lock was acquired; otherwise, <see langword="false"/> if the lock was not acquired before the specified timeout.</returns>
    /// <exception cref="LockRecursionException">The current thread already holds the alpha lock.</exception>
    /// <exception cref="InvalidOperationException">The current thread holds the beta lock.</exception>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    public bool TryEnterAlphaLock(int millisecondsTimeout) => TryEnterAlphaLockCore(new TimeoutTracker(millisecondsTimeout));

    private bool TryEnterAlphaLockCore(TimeoutTracker timeout)
    {
        AlphaBetaCount abc = GetThreadABCount(dontAllocate: false)!;
        // Can't acquire alpha lock with beta lock held.
        _ = abc.Ownership switch
        {
            AlphaBetaOwner.Alpha => throw new LockRecursionException($"Lock recursion detected while trying to enter the alpha lock. {nameof(AlphaBetaLockSlim)} does not support lock recursion."),
            AlphaBetaOwner.Beta => throw new InvalidOperationException("Alpha lock cannot be acquired while holding a beta lock."),
            _ => __
        };

        _spinLock.Enter(EnterSpinLockReason.EnterAlpha);
        bool retVal;
        int spinCount = 0;

        while (true)
        {
            // check if the lock is available and if we fit in the max number of alphas.
            if (GetNumBetas() == 0 && GetNumAlphas() < MAX_ALPHAS)
            {
                // Good case, there is no contention, we are basically done
                break;
            }
            // there are beta locks out there.  We need to contend with them.
            if (timeout.IsExpired)
            {
                // well, unless we timed out.
                _spinLock.Exit();
                return false;
            }
            if (spinCount < MAX_SPIN_COUNT)
            {
                _spinLock.Exit();
                spinCount++;
                SpinWait(spinCount);
                _spinLock.Enter(EnterSpinLockReason.EnterAlpha);
                continue;
            }
            // Drat, we need to wait. Mark that we have waiters and wait.
            if (_alphaEvent == null)
            {
                // Create the needed event (temporarily leaving the lock during wait handle creation)
                LazyCreateEvent(ref _alphaEvent, isAcquiringBetaLock: false);
                continue;   // since we left the lock, start over.
            }
            // wait for the event to be set.
            retVal = WaitOnEvent(_alphaEvent, ref _numAlphaWaiters, timeout, isAcquiringBetaLock: false);
            // The lock is not held in case of failure.
            if (!retVal)
            {
                return false;
            }
        }
        // need to increment the number of alphas out there. (before releasing the lock)
        abc.Ownership = AlphaBetaOwner.Alpha;
        SignalAlphaLockAcquisition();
        _spinLock.Exit();

        return true;
    }

    /// <summary>
    /// Releases the currently held beta lock.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    /// <exception cref="SynchronizationLockException">The current thread does not hold the beta lock.</exception>
    public void ExitBetaLock() => ExitLockCore(AlphaBetaOwner.Beta);

    /// <summary>
    /// Releases the currently held alpha lock.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The <see cref="AlphaBetaLockSlim"/> has been disposed.</exception>
    /// <exception cref="SynchronizationLockException">The current thread does not hold the alpha lock.</exception>
    public void ExitAlphaLock() => ExitLockCore(AlphaBetaOwner.Alpha);

    private void ExitLockCore(AlphaBetaOwner lockType)
    {
        AlphaBetaCount? abc = GetThreadABCount(dontAllocate: true);
        if (abc == null || abc.Ownership != lockType || OwnerGroup != lockType)
        {
            Std::Diagnostics.Debug.Assert(abc == null || OwnerGroup == abc.Ownership, $"Internal inconsistency, exiting {lockType} lock when not holding it");
            // You have to be holding the alpha lock to make this call.
            throw new SynchronizationLockException($"{lockType} lock cannot be released while not holding it.");
        }
        // this should get constant-folded away.
        if (lockType == AlphaBetaOwner.Alpha)
        {
            Std::Diagnostics.Debug.Assert(GetNumAlphas() > 0, "Calling ReleaseAlphaLock when no alpha lock is held");
            _spinLock.Enter(EnterSpinLockReason.ExitAlpha);
            SignalAlphaLockRelease();
        }
        else
        {
            Std::Diagnostics.Debug.Assert(GetNumBetas() > 0, "Calling ReleasingBetaLock when no beta lock is held");
            _spinLock.Enter(EnterSpinLockReason.ExitBeta);
            SignalBetaLockRelease();
        }
        // we need to check if we need to wake up any waiters.
        // we need to check all waiter types, since we could simply have reached the max number of
        // concurrent access (MAX_ALPHAS or MAX_BETAS), so we might end up releasing a waiter of
        // the same type as the lock we are releasing.
        ExitAndWakeUpAppropriateWaiters();
        // can set the thread local variable outside the lock.
        abc.Ownership = AlphaBetaOwner.None;
    }

    /// <summary>
    /// Determines the appropriate events to set, leaves the locks, and sets the events.
    /// </summary>
    private void ExitAndWakeUpAppropriateWaiters()
    {
#if DEBUG
        Std::Diagnostics.Debug.Assert(_spinLock.IsHeld);
#endif
        if (_numAlphaWaiters == 0 && _numBetaWaiters == 0)
        {
            _spinLock.Exit();
            return;
        }

        ExitAndWakeUpAppropriateWaitersPreferringAlphas();
    }

    private void ExitAndWakeUpAppropriateWaitersPreferringAlphas()
    {
        uint betaCount = GetNumBetas();

        if (betaCount == 0 && _numAlphaWaiters > 0)
        {
            _spinLock.Exit();
            // release the alphas.  Known non-null because _numAlphaWaiters > 0.
            _alphaEvent!.Set();
        }
        else
        {
            ExitAndWakeUpAppropriateBetaWaiters();
        }
    }

    /// <summary>
    /// A routine for lazily creating a event outside the lock (so if errors
    /// happen they are outside the lock and that we don't do much work
    /// while holding a spin lock).  If all goes well, reenter the lock and
    /// set 'waitEvent'
    /// </summary>
    private void LazyCreateEvent([NotNull] ref EventWaitHandle? waitEvent, bool isAcquiringBetaLock)
    {
#if DEBUG
        Std::Diagnostics.Debug.Assert(_spinLock.IsHeld);
        Std::Diagnostics.Debug.Assert(waitEvent == null);
#endif
        _spinLock.Exit();

        EventWaitHandle newEvent = new(initialState: false, EventResetMode.ManualReset);

        EnterSpinLockReason enterMyLockReason = isAcquiringBetaLock
            ? EnterSpinLockReason.EnterBeta | EnterSpinLockReason.Wait
            : EnterSpinLockReason.EnterAlpha | EnterSpinLockReason.Wait;

        _spinLock.Enter(enterMyLockReason);
        // maybe someone snuck in.
        if (waitEvent == null)
        {
            waitEvent = newEvent;
        }
        else
        {
            newEvent.Dispose();
        }
    }

    /// <summary>
    /// Waits on 'waitEvent' with a timeout
    /// Before the wait 'numWaiters' is incremented and is restored before leaving this routine.
    /// </summary>
    private bool WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, TimeoutTracker timeout, bool isAcquiringBetaLock)
    {
#if DEBUG
        Std::Diagnostics.Debug.Assert(_spinLock.IsHeld);
#endif
        EnterSpinLockReason enterMyLockReason = isAcquiringBetaLock
            ? EnterSpinLockReason.EnterBeta
            : EnterSpinLockReason.EnterAlpha;

        waitEvent.Reset();
        numWaiters++;

        // Setting this bit will prevent new betas from getting in.
        if (_numAlphaWaiters == 1)
        {
            SetAlphasWaiting();
        }
        bool waitSuccessful = false;
        // Do the wait outside of any lock
        _spinLock.Exit();

        try
        {
            waitSuccessful = waitEvent.WaitOne(timeout.RemainingMilliseconds);
        }
        finally
        {
            _spinLock.Enter(enterMyLockReason);
            numWaiters--;

            if (_numAlphaWaiters == 0)
            {
                ClearAlphasWaiting();
            }
            // We may also be about to throw for some reason.  Exit myLock.
            if (!waitSuccessful)
            {
                // it is possible that another alpha has acquired the lock for our group while we were waiting.
                // in that case, we should not wake up any betas.
                if (!isAcquiringBetaLock && OwnerGroup != AlphaBetaOwner.Alpha)
                {
                    // Alpha waiters block beta waiters from acquiring the lock. Since no other alpha is currently holding
                    // the lock, this thread can safely attempt to wake up betas. ExitAndWakeUpAppropriateBetaWaiters will
                    // check if there are any betas waiting, and if so, will wake them up.
                    ExitAndWakeUpAppropriateBetaWaiters();
                }
                else
                {
                    _spinLock.Exit();
                }
            }
        }
        return waitSuccessful;
    }

    private void ExitAndWakeUpAppropriateBetaWaiters()
    {
#if DEBUG
        Std::Diagnostics.Debug.Assert(_spinLock.IsHeld);
#endif
        // if there are other alphas waiting or there are no betas waiting, we are done.
        // there can be alphas waiting without blocking on the event because they just need
        // to acquire the spinlock.
        if (_numAlphaWaiters != 0 || _numBetaWaiters == 0)
        {
            _spinLock.Exit();
            return;
        }
        Std::Diagnostics.Debug.Assert(_numBetaWaiters != 0);

        bool setBetaEvent = _numBetaWaiters != 0;
        // Exit before signaling to improve efficiency (wakee will need the lock)
        _spinLock.Exit();

        if (setBetaEvent)
        {
            // release all betas. Known non-null because _numBetaWaiters != 0.
            _betaEvent!.Set();
        }
    }

    private void SetAlphasWaiting() => _owners |= WAITING_ALPHAS;

    private void ClearAlphasWaiting() => _owners &= ~WAITING_ALPHAS;

    private uint GetNumBetas() => (uint)(_owners & BETA_COUNT_MASK);

    private void SignalBetaLockAcquisition() => _owners++;

    private void SignalBetaLockRelease() => _owners--;

    private uint GetNumAlphas() => (uint)((_owners & ALPHA_COUNT_MASK) >>> 32);

    private void SignalAlphaLockAcquisition() => _owners += 0x1_0000_0000;

    private void SignalAlphaLockRelease() => _owners -= 0x1_0000_0000;

    private bool ShouldSpinForEnterBeta() =>
        // If there are more than 2 alphas, the beta is not likely to make progress by spinning.
        // Although other threads holding an alpha lock would prevent this thread from acquiring a beta lock, it is by
        // itself not a good enough reason to skip spinning.
        GetNumAlphas() <= 2;

    private static void SpinWait(int spinCount)
    {
        const int LOCK_SPIN_CYCLES = 20;

        // Exponential back-off
        if (spinCount < 5 && Environment.ProcessorCount > 1)
        {
            Thread.SpinWait(LOCK_SPIN_CYCLES * spinCount);
        }
        else
        {
            Thread.Sleep(0);
        }

        // Don't want to Sleep(1) in this spin wait:
        //   - Don't want to spin for that long, since a proper wait will follow when the spin wait fails. The artificial
        //     delay introduced by Sleep(1) will in some cases be much longer than desired.
        //   - Sleep(1) would put the thread into a wait state, and a proper wait will follow when the spin wait fails
        //     anyway, so it's preferable to put the thread into the proper wait state
    }

    /// <summary>
    /// Gets the number of threads waiting to acquire the alpha lock.
    /// </summary>
    public int WaitingAlphaCount => (int)_numAlphaWaiters;

    /// <summary>
    /// Gets the number of threads waiting to acquire the beta lock.
    /// </summary>
    public int WaitingBetaCount => (int)_numBetaWaiters;

    /// <summary>
    /// Indicates whether the current thread holds the alpha lock.
    /// </summary>
    public bool IsAlphaLockHeld
    {
        get
        {
            AlphaBetaCount? abc = GetThreadABCount(dontAllocate: true);
            return abc != null && abc.Ownership == AlphaBetaOwner.Alpha;
        }
    }

    /// <summary>
    /// Indicates whether the current thread holds the beta lock.
    /// </summary>
    public bool IsBetaLockHeld
    {
        get
        {
            AlphaBetaCount? abc = GetThreadABCount(dontAllocate: true);
            return abc != null && abc.Ownership == AlphaBetaOwner.Beta;
        }
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !_disposedValue)
        {
            if (WaitingAlphaCount > 0 || WaitingBetaCount > 0)
            {
                throw new SynchronizationLockException("Cannot dispose of a lock that has waiters.");
            }
            if (IsAlphaLockHeld || IsBetaLockHeld)
            {
                throw new SynchronizationLockException("Cannot dispose of a lock that is held.");
            }

            _alphaEvent?.Dispose();
            _betaEvent?.Dispose();
            _alphaEvent = null;
            _betaEvent = null;

            _disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private struct SpinLock
    {
        private const int LOCKED = 1;
        private const int UNLOCKED = 0;

        private int _lockState;

        /// <summary>
        /// Used to deprioritize threads attempting to enter the lock when they would not make progress after doing so.
        /// <see cref="EnterSpin(EnterSpinLockReason)"/> avoids acquiring the lock as long as the operation for which it
        /// was called is deprioritized.
        ///
        /// Layout:
        /// - Low 16 bits: Number of threads that have deprioritized an enter-alpha operation
        /// - High 16 bits: Number of threads that have deprioritized an enter-beta operation
        /// </summary>
        private int _enterDeprioritizationState;

        // Layout-specific constants for _enterDeprioritizationState
        private const int DEPRIORITIZE_ENTER_BETA_INCREMENT = 1 << 16;
        private const int DEPRIORITIZE_ENTER_ALPHA_INCREMENT = 1;

        // The variables controlling spinning behavior of this spin lock
        private const int LOCK_SPIN_CYCLES = 20;
        private const int LOCK_SLEEP0_SPIN_THRESHOLD = 10;
        private const int REPRIORITIZE_LOCK_SPIN_THRESHOLD = LOCK_SLEEP0_SPIN_THRESHOLD + 60;

        private static int GetEnterDeprioritizationStateChange(EnterSpinLockReason reason)
        {
            EnterSpinLockReason operation = reason & EnterSpinLockReason.OperationMask;
            switch (operation)
            {
                case EnterSpinLockReason.EnterBeta:
                    return 0;

                case EnterSpinLockReason.ExitBeta:
                    // A beta lock is held until this thread is able to exit it, so deprioritize enter-alpha threads as they
                    // will not be able to make progress
                    return DEPRIORITIZE_ENTER_ALPHA_INCREMENT;

                case EnterSpinLockReason.EnterAlpha:
                    // Waiting alphas take precedence over new beta access attempts in order to let current betas release
                    // their lock and allow the alphas to obtain the lock. Before an alpha can register as a waiter though,
                    // the presence of just relatively few enter-beta spins can easily starve the enter-alpha from even
                    // entering this lock, delaying its spin loop for an unreasonable duration.
                    //
                    // Deprioritize enter-beta to preference enter-alpha. This makes it easier for enter-alpha threads to
                    // starve enter-beta threads. However, writers can already by design starve readers. A waiting writer
                    // blocks enter-beta threads and a new enter-alpha that needs to wait will be given precedence over
                    // previously waiting enter-beta threads.
                    return DEPRIORITIZE_ENTER_BETA_INCREMENT;

                default:
                    Std::Diagnostics.Debug.Assert(operation == EnterSpinLockReason.ExitAlpha);

                    // ExitAlpha:
                    // - an alpha lock is held until this thread is able to exit it, so deprioritize
                    //   enter-beta threads as they will not be able to make progress
                    return DEPRIORITIZE_ENTER_BETA_INCREMENT;
            }
        }

        private readonly ushort EnterForEnterBetaDeprioritizedCount
        {
            get
            {
                Std::Diagnostics.Debug.Assert(DEPRIORITIZE_ENTER_BETA_INCREMENT == 1 << 16);
                return (ushort)(_enterDeprioritizationState >>> 16);
            }
        }

        private readonly ushort EnterForEnterAlphaDeprioritizedCount
        {
            get
            {
                Std::Diagnostics.Debug.Assert(DEPRIORITIZE_ENTER_ALPHA_INCREMENT == 1);
                return (ushort)_enterDeprioritizationState;
            }
        }

        private readonly bool IsEnterDeprioritized(EnterSpinLockReason reason)
        {
            Std::Diagnostics.Debug.Assert((reason & EnterSpinLockReason.Wait) != 0 || reason == (reason & EnterSpinLockReason.OperationMask));
            Std::Diagnostics.Debug.Assert(
                (reason & EnterSpinLockReason.Wait) == 0 ||
                (reason & EnterSpinLockReason.OperationMask) == EnterSpinLockReason.EnterBeta ||
                (reason & EnterSpinLockReason.OperationMask) == EnterSpinLockReason.EnterAlpha);

            switch (reason)
            {
                default:
                    Std::Diagnostics.Debug.Assert(
                        (reason & EnterSpinLockReason.Wait) != 0 ||
                        reason == EnterSpinLockReason.ExitBeta ||
                        reason == EnterSpinLockReason.ExitAlpha);
                    return false;

                case EnterSpinLockReason.EnterBeta:
                    return EnterForEnterBetaDeprioritizedCount != 0;

                case EnterSpinLockReason.EnterAlpha:
                    Std::Diagnostics.Debug.Assert((GetEnterDeprioritizationStateChange(reason) & DEPRIORITIZE_ENTER_ALPHA_INCREMENT) == 0);
                    return EnterForEnterAlphaDeprioritizedCount != 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryEnter() => Interlocked.CompareExchange(ref _lockState, LOCKED, UNLOCKED) == UNLOCKED;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enter(EnterSpinLockReason reason)
        {
            if (!TryEnter())
            {
                EnterSpin(reason);
            }
        }

        private void EnterSpin(EnterSpinLockReason reason)
        {
            int deprioritizationStateChange = GetEnterDeprioritizationStateChange(reason);
            if (deprioritizationStateChange != 0)
            {
                Interlocked.Add(ref _enterDeprioritizationState, deprioritizationStateChange);
            }

            for (uint spinIndex = 0; ; spinIndex++)
            {
                // - Spin-wait until the Sleep(0) threshold
                // - Beyond the Sleep(0) threshold, alternate between sleeping and spinning. Avoid using only Sleep(0), as
                //   it may be ineffective when there are no other threads waiting to run. Thread.SpinWait() is not used
                //   where there is a single processor since it would be unlikely for a meaningful change in state to occur
                //   during that.
                // - Don't Sleep(1) here, as it can lead to long latencies in the alpha/beta lock operations
                if ((spinIndex < LOCK_SLEEP0_SPIN_THRESHOLD || (spinIndex - LOCK_SLEEP0_SPIN_THRESHOLD) % 2 != 0) &&
                    Environment.ProcessorCount != 1)
                {
                    Thread.SpinWait(
                        LOCK_SPIN_CYCLES *
                        (spinIndex < LOCK_SLEEP0_SPIN_THRESHOLD ? (int)spinIndex + 1 : LOCK_SLEEP0_SPIN_THRESHOLD));
                }
                else
                {
                    Thread.Sleep(0);
                }

                if (!IsEnterDeprioritized(reason))
                {
                    if (_lockState == UNLOCKED && TryEnter())
                    {
                        if (deprioritizationStateChange != 0)
                        {
                            Interlocked.Add(ref _enterDeprioritizationState, -deprioritizationStateChange);
                        }
                        return;
                    }
                    continue;
                }

                // It's possible for an Enter thread to be deprioritized for an extended duration. It's undesirable for a
                // deprioritized thread to keep spin-waiting for the lock when a large number of such threads are involved.
                // After a threshold, ignore the deprioritization and enter this lock to allow this thread to stop spinning
                // and hopefully enter a proper wait state.
                Std::Diagnostics.Debug.Assert(reason is EnterSpinLockReason.EnterBeta or EnterSpinLockReason.EnterAlpha);
                if (spinIndex >= REPRIORITIZE_LOCK_SPIN_THRESHOLD)
                {
                    reason |= EnterSpinLockReason.Wait;
                    spinIndex = uint.MaxValue;
                }
            }
        }

        public void Exit()
        {
            Std::Diagnostics.Debug.Assert(_lockState != UNLOCKED, "Exiting spin lock that is not held");
            Volatile.Write(ref _lockState, UNLOCKED);
        }

#if DEBUG
        public readonly bool IsHeld => _lockState != UNLOCKED;
#endif
    }

    private enum EnterSpinLockReason
    {
        EnterBeta = 0,
        ExitBeta = 1,
        EnterAlpha = 2,
        ExitAlpha = 5,

        OperationMask = 0x7,

        Wait = 0x8
    }
}

internal enum AlphaBetaOwner
{
    None,
    Alpha,
    Beta
}

//
// AlphaBetaCount tracks how many of each kind of lock is held by each thread.
// We keep a linked list for each thread, attached to a ThreadStatic field.
// These are reused wherever possible, so that a given thread will only
// allocate N of these, where N is the maximum number of locks held simultaneously
// by that thread.
//
[DebuggerDisplay("Ownership: {ownership}")]
internal sealed class AlphaBetaCount
{
    // Which lock does this object belong to?  This is a numeric ID for two reasons:
    // 1) We don't want this field to keep the lock object alive, and a WeakReference would
    //    be too expensive.
    // 2) Setting the value of a long is faster than setting the value of a reference.
    //    The "hot" paths in GroupLockSlim are short enough that this actually
    //    matters.
    public long LockId;

    // Does this thread own any locks?
    public AlphaBetaOwner Ownership;

    // Next ABC in this thread's list.
    public AlphaBetaCount? Next;
}

file readonly struct AlphaLockOwnership : ILockOwnership
{
    private readonly AlphaBetaLockSlim _abls;

    public AlphaLockOwnership(AlphaBetaLockSlim abls)
    {
        _abls = abls;
        abls.EnterAlphaLock();
    }

    public void Dispose() => _abls.ExitAlphaLock();
}

file readonly struct BetaLockOwnership : ILockOwnership
{
    private readonly AlphaBetaLockSlim _abls;

    public BetaLockOwnership(AlphaBetaLockSlim abls)
    {
        _abls = abls;
        abls.EnterBetaLock();
    }

    public void Dispose() => _abls.ExitBetaLock();
}