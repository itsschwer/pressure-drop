﻿using RoR2;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
                Log.Debug($"itemIndex: {def.itemIndex} | itemTier: {def.itemTier} | equipmentIndex: {def.equipmentIndex} | isLunar: {def.isLunar} | isBoss: {def.isBoss}");
                if (def.itemIndex != ItemIndex.None) {
                    ItemDef itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                    Log.Debug($"    hidden: {itemDef.hidden} | canRemove: {itemDef.canRemove} | {itemDef.nameToken}");
                }
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

        /// <summary>
        /// Searches the inventory for an item with a name that matches the search query. Items are checked in reverse acquisition order (most recent match first).
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="query"></param>
        /// <returns>The ItemIndex of the matched item. If there is no match, returns ItemIndex.None instead.</returns>
        public static ItemIndex FindItemInInventory(this Inventory inventory, string query)
        {
            List<ItemIndex> items = inventory.itemAcquisitionOrder;
            if (items == null || items.Count <= 0) return ItemIndex.None;

            query = FormatStringForQuerying(query);
            // Iterate in reverse to match most recent
            for (int i = (items.Count - 1); i >= 0; i--) {
                ItemDef check = ItemCatalog.GetItemDef(items[i]);
                // Do not match hidden (internal) items
                if (check.hidden) continue;
                // Do not match non-removable (consumed) items
                if (!check.canRemove) continue;

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
    }
}
