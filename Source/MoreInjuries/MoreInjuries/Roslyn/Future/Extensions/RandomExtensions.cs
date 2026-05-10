namespace MoreInjuries.Roslyn.Future.Extensions;

internal static class RandomExtensions
{
    private static readonly Random s_sharedInstance = new();

    extension(Random)
    {
        public static Random Shared => s_sharedInstance;
    }
}
