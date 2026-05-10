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
                self.ThrowMissingRequiredModExtensionException<T>();
            }
            return modExtension;
        }

        [DoesNotReturn]
        private void ThrowMissingRequiredModExtensionException<T>() =>
            throw new InvalidOperationException($"Def {self.defName} is missing required mod extension of type {typeof(T).FullName}");
    }
}
