using RoR2;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

#if DEBUG
        public static string DumpPickupItemInfo(PickupDef def)
        {
            System.Text.StringBuilder sb = new($"itemIndex: {def.itemIndex} | itemTier: {def.itemTier} | equipmentIndex: {def.equipmentIndex} | isLunar: {def.isLunar} | isBoss: {def.isBoss}\n");
            if (def.itemIndex != ItemIndex.None)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                sb.Append($"\t\thidden: {itemDef.hidden} | canRemove: {itemDef.canRemove} | {itemDef.nameToken} | {Language.GetString(itemDef.nameToken)}");
            }
            return sb.ToString();
        }
#endif

        private static GenericPickupController GenericPickupController_CreatePickup(On.RoR2.GenericPickupController.orig_CreatePickup orig, ref GenericPickupController.CreatePickupInfo createPickupInfo)
        {
            if (createPickupInfo.rotation == identifier) {
                createPickupInfo.rotation = Quaternion.identity;
                GenericPickupController drop = orig(ref createPickupInfo);

                PickupDef def = PickupCatalog.GetPickupDef(createPickupInfo.pickupIndex);
                if (!GetDropRecyclable(def)) drop.Recycled = true;

                return drop;
            }
            else return orig(ref createPickupInfo);
        }

        /// <summary>
        /// Checks if an <see cref="ItemTier"/> belongs to a void tier.
        /// </summary>
        /// <param name="tier"></param>
        /// <returns>Whether the <paramref name="tier"/> is a void tier or not.</returns>
        public static bool IsVoidTier(ItemTier tier)
        {
            return tier == ItemTier.VoidTier1 ||
                   tier == ItemTier.VoidTier2 ||
                   tier == ItemTier.VoidTier3 ||
                   tier == ItemTier.VoidBoss;
        }

        /// <summary>
        /// Checks a <see cref="PickupDef"/> against the config settings to determine whether the item or equipment should be recyclable.
        /// </summary>
        /// <param name="def"></param>
        /// <returns>Whether the item or equipment should be recyclable or not.</returns>
        public static bool GetDropRecyclable(PickupDef def)
        {
            bool result = true;

            if (def.itemIndex != ItemIndex.None) {
                if ((def.itemTier == ItemTier.Tier1 && !Plugin.Config.DropRecyclableWhite) ||
                    (def.itemTier == ItemTier.Tier2 && !Plugin.Config.DropRecyclableGreen) ||
                    (def.itemTier == ItemTier.Tier3 && !Plugin.Config.DropRecyclableRed)   ||
                    (def.itemTier == ItemTier.Boss && !Plugin.Config.DropRecyclableYellow) ||
                    (def.itemTier == ItemTier.Lunar && !Plugin.Config.DropRecyclableLunar) ||
                    (!Plugin.Config.DropRecyclableVoid && IsVoidTier(def.itemTier)))
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

        /// <summary>
        /// Searches the <paramref name="inventory"/> for an item with a name that matches the search <paramref name="query"/>.
        /// <br/>Items are checked in reverse acquisition order (most recent match first).
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="query"></param>
        /// <returns>The <see cref="ItemIndex"/> of the matched item. If there is no match, returns <see cref="ItemIndex.None"/> instead.</returns>
        public static ItemIndex FindItemInInventory(this Inventory inventory, string query)
        {
            List<ItemIndex> items = inventory.itemAcquisitionOrder;
            if (items == null || items.Count <= 0) return ItemIndex.None;

            query = FormatStringForQuerying(query);
            // Iterate in reverse to match most recent
            for (int i = (items.Count - 1); i >= 0; i--) {
                ItemDef check = ItemCatalog.GetItemDef(items[i]);
                if (!InManualWhitelist(check)) {
                    // Do not match hidden (internal) items
                    if (check.hidden) continue;
                    // Do not match non-removable (consumed) items
                    if (!check.canRemove) continue;
                }

                if (FormatStringForQuerying(Language.GetString(check.nameToken)).Contains(query)) return check.itemIndex;
            }

            return ItemIndex.None;
        }

        /// <summary>
        /// Formats the input string to be ready for comparison (removes some common punctuation marks and converts to lowercase).
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The formatted string.</returns>
        public static string FormatStringForQuerying(string input) => Regex.Replace(input, "[ '_.,-]", string.Empty).ToLowerInvariant();

        private static bool InManualWhitelist(ItemDef def)
        {
            const string LongstandingSolitude = "ITEM_ONLEVELUPFREEUNLOCK_NAME";

            switch (def.nameToken) {
                default: return false;
                case LongstandingSolitude:
                    if (!def.hidden && !def.canRemove) {
                        Plugin.Logger.LogWarning($"Item does not need to be manually whitelisted: {LongstandingSolitude} | {Language.GetString(LongstandingSolitude)}");
                    }
                    return true;
            }
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
