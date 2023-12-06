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
                if (def.itemTier == ItemTier.Lunar) {
                    drop.Recycled = true;
                }
                return drop;
            }
            else return orig(ref createPickupInfo);
        }
    }
}
