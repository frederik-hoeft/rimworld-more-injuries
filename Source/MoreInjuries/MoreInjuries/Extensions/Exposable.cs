using System.Runtime.CompilerServices;

namespace MoreInjuries.Extensions;

internal static class Exposable
{
    public static T RequiredMember<T>(T? value, string containingTypeName, [CallerMemberName] string memberName = null!)
    {
        if (value is null)
        {
            throw new InvalidOperationException($"Member '{memberName}' of type '{containingTypeName}' is required but was not set.");
        }
        return value;
    }
}
