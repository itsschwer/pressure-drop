using RoR2;
using UnityEngine;

namespace PressureDrop.Commands
{
    internal static class Drop
    {
        public static void  Enable() => ChatCommander.OnChatCommand += ParseCommand;
        public static void Disable() => ChatCommander.OnChatCommand -= ParseCommand;

        private static void ParseCommand(NetworkUser user, string[] args)
        {
            if (!Run.instance || args.Length < 1) return;

            switch (args[0].ToLowerInvariant()) {
                default:
                    break;
                case "d":
                case "drop":
                    DropCommand(user, args);
                    break;
            }
        }

        private static void DropCommand(NetworkUser user, string[] args)
        {
            const int expectedArgs = 2;
            if (args.Length < expectedArgs) { ShowHelp(args); return; }

            bool dropAtTeleporter = false;
            if (args.Length > expectedArgs) {
                if (args[2] == "@") { dropAtTeleporter = true; }
                else { ShowHelp(args); return; }
            }

            ItemIndex itemIndex = user.master.inventory.FindItemInInventory(args[1]);
            if (itemIndex == ItemIndex.None) {
                Feedback($"Could not match '<color=#e5eefc>{args[1]}</color>' to an item in {user.masterController.GetDisplayName()}'s inventory.");
                return;
            }

            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
            if (itemDef == RoR2Content.Items.CaptainDefenseMatrix || (!Plugin.Config.DropVoidAllowed && PressureDrop.Drop.IsVoidTier(itemDef.tier))) {
                Feedback($"{ChatCommander.GetColoredPickupLanguageString(itemDef.nameToken, itemDef.itemIndex)} can not be dropped.");
                return;
            }

            DropExecute(user, itemDef, dropAtTeleporter);
        }

        private static void DropExecute(NetworkUser user, ItemDef itemDef, bool dropAtTeleporter)
        {
            int count = user.master.inventory.GetItemCount(itemDef.itemIndex);
            if (count > Plugin.Config.MaxItemsToDropAtATime) count = Plugin.Config.MaxItemsToDropAtATime;

            string displayCount = ((count != 1) ? $"({count})" : "");
            string message = $"{user.masterController.GetDisplayName()} dropped {ChatCommander.GetColoredPickupLanguageString(itemDef.nameToken, itemDef.itemIndex)}{displayCount}";

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
                        message += " at the Teleporter";
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
            PressureDrop.Drop.DropStyleChest(target, PickupCatalog.FindPickupIndex(itemDef.itemIndex), count, 3.4f, 14f, GetAimDirection(user));
            Feedback(message);
        }

        private static Vector3? GetAimDirection(NetworkUser user)
        {
            InputBankTest inputBank = user.GetCurrentBody()?.inputBank;
            if (inputBank) {
                Vector3 aimDirection = inputBank.aimDirection;
                aimDirection.y = 0f;
                return aimDirection.normalized;
            }

            return null;
        }

        /// <summary>
        /// Wrapper for sending a chat message styled after the vanilla "picked up {item}" message.
        /// </summary>
        /// <param name="message"></param>
        private static void Feedback(string message)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</color>"});

        private static void ShowHelp(string[] args) => ChatCommander.OutputFail(args[0], "expects an item name (without spaces).");
    }
}
