using RoR2;
using UnityEngine;

namespace PressureDrop
{
    public static class Drop
    {
        /// <summary>
        /// An invalid <see cref="Quaternion"/> to check for as a hacky workaround
        /// for finding pickups the plugin aims to manipulate.
        /// <para>(A valid <see cref="Quaternion"/>'s xyzw values range from -1 to 1).</para>
        /// </summary>
        private static readonly Quaternion identifier = new Quaternion(-2, -4, -8, -16);

        private static bool _hooked = false;

        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            On.RoR2.GenericPickupController.CreatePickup += GenericPickupController_CreatePickup;
            DropCommand.Enable();
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;
            On.RoR2.GenericPickupController.CreatePickup -= GenericPickupController_CreatePickup;
            DropCommand.Disable();
        }

        private static GenericPickupController GenericPickupController_CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            if (createPickupInfo.rotation == identifier) {
                createPickupInfo.rotation = Quaternion.identity;
                GenericPickupController drop = orig(ref createPickupInfo);

                PickupDef def = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
                if (!IsRecyclable(def)) drop.Recycled = true;

                return drop;
            }
            else return orig(ref createPickupInfo);
        }

        private static bool IsRecyclable(PickupDef def)
        {
            bool result = true;

            if (def.itemIndex != ItemIndex.None) {
                if ((def.itemTier == ItemTier.Tier1 && !Plugin.Config.DropRecyclableWhite) ||
                    (def.itemTier == ItemTier.Tier2 && !Plugin.Config.DropRecyclableGreen) ||
                    (def.itemTier == ItemTier.Tier3 && !Plugin.Config.DropRecyclableRed)   ||
                    (def.itemTier == ItemTier.Boss && !Plugin.Config.DropRecyclableYellow) ||
                    (def.itemTier == ItemTier.Lunar && !Plugin.Config.DropRecyclableLunar) ||
                    (!Plugin.Config.DropRecyclableVoid && def.itemTier.IsVoidTier()))
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


        // Drop ============================================

        public static Vector3? GetAimDirection(CharacterBody body)
        {
            InputBankTest inputBank = body?.inputBank;
            if (inputBank == null) return null;

            Vector3 aimDirection = inputBank.aimDirection;
            aimDirection.y = 0f;
            return aimDirection.normalized;
        }

        public static void DropStyleChest(Transform target, PickupIndex dropPickup, int dropCount, float forwardVelocity = 2f, float upVelocity = 20f, Vector3? forwardOverride = null)
        {
            if (dropCount > 0) {
                float angle = 360f / (float)dropCount;
                Vector3 forward = forwardOverride ?? target.forward;
                Vector3 position = target.position + Vector3.up * 1.5f;
                Vector3 velocity = Vector3.up * upVelocity + forward * forwardVelocity;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < dropCount; i++) {
                    GenericPickupController.CreatePickupInfo info = default;
                    info.position = position;
                    info.rotation = identifier;
                    info.pickupIndex = dropPickup;
                    CreatePickupDroplet(info, velocity);
                    velocity = rotation * velocity;
                }
            }
        }

        // Near-duplicate of above, modified to support dropping different item types (probably should refactor). Only used by debug command.
        public static void DropStyleChest(Transform target, PickupIndex[] drops, float forwardVelocity = 2f, float upVelocity = 20f, Vector3? forwardOverride = null)
        {
            if (drops.Length > 0) {
                float angle = 360f / (float)drops.Length;
                Vector3 forward = forwardOverride ?? target.forward;
                Vector3 position = target.position + Vector3.up * 1.5f;
                Vector3 velocity = Vector3.up * upVelocity + forward * forwardVelocity;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < drops.Length; i++) {
                    GenericPickupController.CreatePickupInfo info = default;
                    info.position = position;
                    info.rotation = identifier;
                    info.pickupIndex = drops[i];
                    CreatePickupDroplet(info, velocity);
                    velocity = rotation * velocity;
                }
            }
        }

        private static void CreatePickupDroplet(GenericPickupController.CreatePickupInfo pickupInfo, Vector3 velocity)
        {
            // Use try-catch block to attempt backwards-compatibility
            try {
                CreatePickupDroplet_Wrapper(pickupInfo, velocity);
            }
            catch (System.MissingMethodException) {
                CreatePickupDroplet_Devotion(pickupInfo, velocity);
            }
        }

        // https://stackoverflow.com/questions/3546580/why-is-it-not-possible-to-catch-missingmethodexception/3546611#3546611
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void CreatePickupDroplet_Wrapper(GenericPickupController.CreatePickupInfo pickupInfo, Vector3 velocity)
        {
            PickupDropletController.CreatePickupDroplet(pickupInfo, pickupInfo.position, velocity);
        }

        private static System.Action<GenericPickupController.CreatePickupInfo, Vector3> _CreatePickupDroplet_Devotion;
        private static System.Action<GenericPickupController.CreatePickupInfo, Vector3> CreatePickupDroplet_Devotion {
            get {
                if (_CreatePickupDroplet_Devotion == null) {
                    Plugin.Logger.LogWarning($"{nameof(System.MissingMethodException)}: Using Devotion Update version of {nameof(PickupDropletController)}.{nameof(PickupDropletController.CreatePickupDroplet)}");
                    System.Reflection.MethodInfo methodInfo = typeof(PickupDropletController).GetMethod(nameof(PickupDropletController.CreatePickupDroplet), [typeof(GenericPickupController.CreatePickupInfo), typeof(Vector3)]);
                    _CreatePickupDroplet_Devotion = (System.Action<GenericPickupController.CreatePickupInfo, Vector3>)methodInfo.CreateDelegate(typeof(System.Action<GenericPickupController.CreatePickupInfo, Vector3>));
                }
                return _CreatePickupDroplet_Devotion;
            }
        }
    }
}
