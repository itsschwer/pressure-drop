using RoR2;
using UnityEngine;

namespace PressureDrop
{
    internal static class Drop
    {
        /// <summary>
        /// An invalid Quaternion to check for as a hacky workaround
        /// for finding pickups the plugin aims to manipulate.
        /// <para>(Valid Quaternion's xyzw values range from -1 to 1).</para>
        /// </summary>
        public static readonly Quaternion identifier = new Quaternion(-2, -4, -8, -16);

        public static void Hook()
        {
            On.RoR2.GenericPickupController.CreatePickup += GenericPickupController_CreatePickup;
            Commands.Drop.Enable();
        }
        public static void Unhook()
        {
            On.RoR2.GenericPickupController.CreatePickup -= GenericPickupController_CreatePickup;
            Commands.Drop.Disable();
        }

        private static GenericPickupController GenericPickupController_CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            if (createPickupInfo.rotation == identifier) {
                createPickupInfo.rotation = Quaternion.identity;
                GenericPickupController drop = orig(ref createPickupInfo);

                PickupDef def = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
#if DEBUG
                Log.Message($"itemIndex: {def.itemIndex} | itemTier: {def.itemTier} | equipmentIndex: {def.equipmentIndex} | isLunar: {def.isLunar} | isBoss: {def.isBoss}");
#endif
                if (!GetDropRecyclable(def)) drop.Recycled = true;

                return drop;
            }
            else return orig(ref createPickupInfo);
        }

        /// <summary>
        /// Checks a PickupDef against the config settings to determine whether the item or equipment should be recyclable.
        /// </summary>
        /// <param name="def"></param>
        /// <returns>Whether the item or equipment should be recyclable.</returns>
        public static bool GetDropRecyclable(PickupDef def)
        {
            bool result = true;

            if (def.itemIndex != ItemIndex.None) {
                if ((def.itemTier == ItemTier.Tier1 && !Plugin.Config.DropRecyclableWhite) ||
                    (def.itemTier == ItemTier.Tier2 && !Plugin.Config.DropRecyclableGreen) ||
                    (def.itemTier == ItemTier.Tier3 && !Plugin.Config.DropRecyclableRed)   ||
                    (def.itemTier == ItemTier.Boss && !Plugin.Config.DropRecyclableYellow) ||
                    (def.itemTier == ItemTier.Lunar && !Plugin.Config.DropRecyclableLunar) ||
                    (!Plugin.Config.DropRecyclableVoid &&
                        ((def.itemTier == ItemTier.VoidTier1) ||
                         (def.itemTier == ItemTier.VoidTier2) ||
                         (def.itemTier == ItemTier.VoidTier3) ||
                         (def.itemTier == ItemTier.VoidBoss))))
                    result = false;
            }
            else if (def.equipmentIndex != EquipmentIndex.None) {
                if (def.isLunar) {
                    if (!Plugin.Config.DropRecyclableEquipmentLunar) result = false;
                }
                else if (!Plugin.Config.DropRecyclableEquipment) result = false;
            }

            return result;
        }
    }
}
