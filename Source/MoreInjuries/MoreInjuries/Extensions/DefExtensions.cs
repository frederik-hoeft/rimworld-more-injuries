using Verse;

namespace MoreInjuries.Extensions;

public static class DefExtensions
{
    extension(Def self)
    {
        public T GetRequiredModExtension<T>() where T : DefModExtension
        {
            T? modExtension = self.GetModExtension<T>();
            if (modExtension is null)
            {
                ThrowMissingRequiredModExtensionException<T>(self);
            }
            return modExtension;
        }

        [DoesNotReturn]
        private static void ThrowMissingRequiredModExtensionException<T>(Def def) =>
            throw new InvalidOperationException($"Def {def.defName} is missing required mod extension of type {typeof(T).FullName}");
    }
}
