using RoR2;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PressureDrop
{
    internal static class DropCommand
    {
        internal static void Enable()
        {
            ChatCommander.Register("/d", Parse);
            ChatCommander.Register("/drop", Parse);
        }
        internal static void Disable()
        {
            ChatCommander.Unregister("/d", Parse);
            ChatCommander.Unregister("/drop", Parse);
        }

        private static void Parse(NetworkUser user, string[] args)
        {
            if (Run.instance == null) return;

            const int expectedArgs = 2;
            if (args.Length < expectedArgs) { ShowHelp(args); return; }

            bool dropAtTeleporter = false;
            if (args.Length > expectedArgs) {
                if (args[2] == "@") { dropAtTeleporter = true; }
                else { ShowHelp(args); return; }
            }

            ItemIndex itemIndex = user.master.inventory.FindItemInInventory(args[1]);
            if (itemIndex == ItemIndex.None) {
                Feedback($"Could not match '<color=#e5eefc>{args[1]}</color>' to an item in {user.userName}'s inventory.");
                return;
            }

            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
            if (itemDef == RoR2Content.Items.CaptainDefenseMatrix || (!Plugin.Config.DropVoidAllowed && itemDef.tier.IsVoidTier())) {
                Feedback($"{Utils.GetColoredPickupLanguageString(itemDef.itemIndex)} can not be dropped.");
                return;
            }

            Execute(user, itemDef, dropAtTeleporter);
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
                // Do not match hidden (internal) items
                if (check.hidden) continue;
                // Do not match non-removable (consumed) items
                if (!check.canRemove && !OverrideCanRemoveFlag(check)) continue;

                if (FormatStringForQuerying(Language.GetString(check.nameToken)).Contains(query)) return check.itemIndex;
            }

            return ItemIndex.None;
        }

        /// <summary>
        /// Formats the input string to be ready for comparison (removes some common punctuation marks and converts to lowercase).
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The formatted string.</returns>
        internal static string FormatStringForQuerying(string input) => Regex.Replace(input, "[ '_.,-]", string.Empty).ToLowerInvariant();

        private static bool OverrideCanRemoveFlag(ItemDef def)
        {
            const string LongstandingSolitude = "ITEM_ONLEVELUPFREEUNLOCK_NAME";

            switch (def.nameToken) {
                default: return false;
                case LongstandingSolitude:
                    if (def.canRemove) {
                        Plugin.Logger.LogWarning($"Item does not need to be manually whitelisted: {LongstandingSolitude} | {Language.GetString(LongstandingSolitude)}");
                    }
                    return true;
            }
        }

        private static void Execute(NetworkUser user, ItemDef itemDef, bool dropAtTeleporter)
        {
            int count = user.master.inventory.GetItemCount(itemDef.itemIndex);
            if (count > Plugin.Config.MaxItemsToDropAtATime) count = Plugin.Config.MaxItemsToDropAtATime;

            Transform target = user.GetCurrentBody()?.gameObject.transform;
            // Assume dead if no body
            if (target == null) {
                if (Plugin.Config.DropDeadEnabled && Plugin.Config.DropTeleporterEnabled) {
                    dropAtTeleporter = true;
                }
                else {
                    Feedback("Dead players can't drop items <sprite name=\"Skull\" tint=1>");
                    return;
                }
            }

            if (dropAtTeleporter) {
                if (Plugin.Config.DropTeleporterEnabled) {
                    Transform tp = TeleporterInteraction.instance?.transform;

                    if (tp != null) {
                        target = tp;
                    }
                    else {
                        Feedback("There is no Teleporter to drop at <sprite name=\"Skull\" tint=1>");
                        return;
                    }
                }
                else {
                    Feedback("Dropping at the Teleporter is disabled <sprite name=\"Skull\" tint=1>");
                    return;
                }
            }

            if (target == null) {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cUtility>No drop target <sprite name=\"Skull\" tint=1></style>" });
                return;
            }

            user.master.inventory.RemoveItem(itemDef.itemIndex, count);
            Vector3? direction = Drop.GetAimDirection(user?.GetCurrentBody());
            if (Plugin.Config.DropInvertDirection && direction.HasValue) direction = -direction.Value;
            Drop.DropStyleChest(target, PickupCatalog.FindPickupIndex(itemDef.itemIndex), count, 3.4f, 14f, direction);
            Feedback(user, itemDef, count, dropAtTeleporter);
        }

        /// <summary>
        /// Wrapper for sending a chat message styled after the vanilla "{<paramref name="user"/>} picked up {<paramref name="item"/>}[&lt;<paramref name="count"/>&gt;]" message.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <param name="dropAtTeleporter"></param>
        private static void Feedback(NetworkUser user, ItemDef item, int count, bool dropAtTeleporter)
        {
            string itemDisplay = Utils.GetColoredPickupLanguageString(item.itemIndex);
            string countDisplay = (count != 1) ? $"({count})" : "";
            string location = dropAtTeleporter ? " at the Teleporter" : "";
            Feedback($"{user.userName} dropped {itemDisplay}{countDisplay}{location}");
        }

        private static void Feedback(string message)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</style>"});
        private static void ShowHelp(string[] args)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility><style=cDeath>Failed:</style> <color=#ffffff>{args[0]}</color> expects an item name (without spaces).</style>" });
    }
}
