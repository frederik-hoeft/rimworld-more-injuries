using System.Buffers;

namespace MoreInjuries.Extensions.Bcl;

public static class ArrayPoolExtensions
{
    extension<T>(ArrayPool<T> arrayPool)
    {
        public RentedArray<T> RentDisposable(int minimumLength, bool clearArray = false) => new(arrayPool.Rent(minimumLength), clearArray);
    }
}

public readonly struct RentedArray<T>(T[] array, bool clearArray) : IDisposable
{
    public readonly T[] Array => array;

    public void Dispose() => ArrayPool<T>.Shared.Return(array, clearArray);
}