using System.Text;

namespace MoreInjuries.Roslyn.SourceGen.Extensions;

internal static class TypeExtensions
{
    private const string GLOBAL = "global::";

    extension(Type self)
    {
        public string ConstructFullyQualifiedGenericName(params ReadOnlySpan<Type> genericArguments)
        {
            if (!self.IsGenericTypeDefinition)
            {
                return $"{GLOBAL}{self.Namespace}.{self.Name}";
            }
            StringBuilder builder = new();
            builder.Append(GLOBAL).Append(self.Namespace).Append('.').Append(self.Name);
            if (genericArguments.Length > 0)
            {
                builder.Append('<');
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(genericArguments[i].ConstructFullyQualifiedGenericName());
                }
                builder.Append('>');
            }
            return builder.ToString();
        }

        public string GetFullyQualifiedBaseTypeName()
        {
            if (self.IsGenericType)
            {
                return $"{GLOBAL}{self.Namespace}.{self.Name[..self.Name.IndexOf('`')]}";
            }
            return $"{GLOBAL}{self.Namespace}.{self.Name}";
        }
    }
}
