using System.Text;

namespace MoreInjuries.Extensions.Bcl;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendEnumerationItem(this StringBuilder builder, string value, ref bool hasPreviousElements)
    {
        if (hasPreviousElements)
        {
            builder.Append(", ");
        }
        builder.Append(value);
        hasPreviousElements = true;
        return builder;
    }
}
