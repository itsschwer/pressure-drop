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
                if (args[2] == "@") dropAtTeleporter = true;
                else { ShowHelp(args); return; }
            }

            string name = user.masterController.GetDisplayName();
            Inventory inventory = user.master.inventory;
            Transform target = user.GetCurrentBody()?.gameObject.transform;
            ItemIndex itemIndex = inventory.FindItemInInventory(args[1]);

            if (itemIndex == ItemIndex.None) {
                Feedback($"Could not match '<color=#e5eefc>{args[1]}</color>' to an item in {name}'s inventory.");
            }
            else {
                ItemDef def = ItemCatalog.GetItemDef(itemIndex);
                if (def == RoR2Content.Items.CaptainDefenseMatrix || (!Plugin.Config.DropVoidAllowed && PressureDrop.Drop.IsVoidTier(def.tier))) {
                    Feedback($"{ChatCommander.GetColoredPickupLanguageString(def.nameToken, def.itemIndex)} can not be dropped.");
                }
                else {
                    int count = inventory.GetItemCount(def.itemIndex);
                    if (count > Plugin.Config.MaxItemsToDropAtATime) count = Plugin.Config.MaxItemsToDropAtATime;

                    string displayCount = ((count != 1) ? $"({count})" : "");
                    string message = $"{name} dropped {ChatCommander.GetColoredPickupLanguageString(def.nameToken, def.itemIndex)}{displayCount}";

                    if (count > 0) {
                        // No body, assume dead
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

                                // Chat message to check for if implementing ping requirement [language specific!]
                                // <style=cIsDamage>USER has found: Teleporter.</style>"

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
                        }
                        else {
                            Vector3? forwardOverride = user.GetCurrentBody()?.characterDirection.forward;

                            inventory.RemoveItem(def.itemIndex, count);
                            PressureDrop.Drop.DropStyleChest(target, PickupCatalog.FindPickupIndex(def.itemIndex), count, 3.4f, 14f, forwardOverride);
                        }
                    }
                    Feedback(message);
                }
            }
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
