using RoR2;
using System.Collections.Generic;

namespace PressureDrop.Tweaks
{
    internal static class VoidPickupConfirmAll
    {
        private static readonly Dictionary<ItemTier, ItemTierDef.PickupRules> originalRules = [];

        public static void SetActive(bool value)
        {
            if (originalRules.Count > 0) {
                Revert();
                originalRules.Clear();
            }
            if (value) Apply();
        }

        private static void Apply()
        {
            foreach (ItemTierDef def in ItemTierCatalog.allItemTierDefs) {
                if (def.tier.IsVoidTier()) {
                    originalRules[def.tier] = def.pickupRules;
                    def.pickupRules = ItemTierDef.PickupRules.ConfirmAll;
                }
            }
            Plugin.Logger.LogMessage($"{nameof(VoidPickupConfirmAll)}> pickup rule applied.");
        }

        private static void Revert()
        {
            foreach (ItemTierDef def in ItemTierCatalog.allItemTierDefs) {
                if (originalRules.TryGetValue(def.tier, out var original)) {
                    def.pickupRules = original;
                }
            }
            Plugin.Logger.LogMessage($"{nameof(VoidPickupConfirmAll)}> pickup rule reverted.");
        }
    }
}
