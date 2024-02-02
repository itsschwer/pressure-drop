using RoR2;
using System.Collections.Generic;

namespace PressureDrop
{
    internal static class VoidPickupTweak
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
                if (Drop.IsVoidTier(def.tier)) {
                    originalRules[def.tier] = def.pickupRules;
                    def.pickupRules = ItemTierDef.PickupRules.ConfirmAll;
                }
            }
            Log.Message($"Void Pickup Rule> applied.");
        }

        private static void Revert()
        {
            foreach (ItemTierDef def in ItemTierCatalog.allItemTierDefs) {
                if (originalRules.TryGetValue(def.tier, out var original)) {
                    def.pickupRules = original;
                }
            }
            Log.Message($"Void Pickup Rule> reverted.");
        }
    }
}
