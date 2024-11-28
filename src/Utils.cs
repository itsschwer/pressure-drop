using RoR2;

namespace PressureDrop
{
    public static class Utils
    {
        /// <summary>
        /// Checks if an <see cref="ItemTier"/> belongs to a void tier.
        /// </summary>
        /// <param name="tier"></param>
        /// <returns><see langword="true"/> if the <paramref name="tier"/> is a void tier, <see langword="false"/> otherwise.</returns>
        public static bool IsVoidTier(this ItemTier tier)
        {
            return tier == ItemTier.VoidTier1 ||
                   tier == ItemTier.VoidTier2 ||
                   tier == ItemTier.VoidTier3 ||
                   tier == ItemTier.VoidBoss;
        }

        public static string GetColoredPickupLanguageString(ItemIndex itemIndex)
            => GetColoredPickupLanguageString(PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemIndex)));
        public static string GetColoredPickupLanguageString(PickupDef pickupDef)
            => Util.GenerateColoredString(Language.GetString(pickupDef.nameToken), pickupDef.baseColor);
    }
}
