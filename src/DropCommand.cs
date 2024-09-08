using RoR2;
using UnityEngine;

namespace PressureDrop
{
    internal static class DropCommand
    {
        public static void  Enable()
        {
            ChatCommander.Register("/d", Parse);
            ChatCommander.Register("/drop", Parse);
        }
        public static void Disable()
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
            if (itemDef == RoR2Content.Items.CaptainDefenseMatrix || (!Plugin.Config.DropVoidAllowed && Drop.IsVoidTier(itemDef.tier))) {
                Feedback($"{ChatCommander.GetColoredPickupLanguageString(itemDef.itemIndex)} can not be dropped.");
                return;
            }

            Execute(user, itemDef, dropAtTeleporter);
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
            string itemDisplay = ChatCommander.GetColoredPickupLanguageString(item.itemIndex);
            string countDisplay = (count != 1) ? $"({count})" : "";
            string location = dropAtTeleporter ? " at the Teleporter" : "";
            Feedback($"{user.userName} dropped {itemDisplay}{countDisplay}{location}");
        }

        /// <summary>
        /// Wrapper for sending a styled chat message.
        /// </summary>
        /// <remarks>
        /// The missing closing style tag and the extraneous closing color tag mimics vanilla.
        /// </remarks>
        /// <param name="message"></param>
        private static void Feedback(string message)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</color>"});

        private static void ShowHelp(string[] args) => ChatCommander.OutputFail(args[0], "expects an item name (without spaces).");
    }
}
