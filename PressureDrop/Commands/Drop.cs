using RoR2;
using UnityEngine;

namespace PressureDrop.Commands
{
    public static class Drop
    {
        public static void  Enable() => ChatCommander.OnChatCommand += ParseCommand;
        public static void Disable() => ChatCommander.OnChatCommand -= ParseCommand;

        private static void ParseCommand(NetworkUser user, string[] args)
        {
            if (!Run.instance || args.Length < 1) return;

            switch (args[0].ToLowerInvariant()) {
                default:
                    break;
                case "s":
                case "steal":
                    StealCommand(user, args);
                    break;
            }
        }

        private static void StealCommand(NetworkUser user, string[] args)
        {
            const int expectedArgs = 2;
            if (args.Length < expectedArgs) {
                Output("not enough args");
                return;
            }

            int count = 1;
            if (args.Length > expectedArgs && !int.TryParse(args[2], out count)) count = 1;

            switch (args[1].ToLowerInvariant()) {
                default:
                    PickupIndex drop;
                    Output("args[1] invalid");
                    break;
                case "g":
                    drop = PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("AutoCastEquipment"));
                    DropStyleChest(user.masterController.master.GetBodyObject().transform, drop, count);
                    break;
                case "d":
                    drop = PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("SprintWisp"));
                    DropStyleChest(user.masterController.master.GetBodyObject().transform, drop, count);
                    break;
            }
        }

        private static void DropStyleChest(Transform transform, PickupIndex dropPickup, int dropCount)
        {
            float upStrength = 20f;
            float forwardStrength = 2f;

            if (dropPickup != PickupIndex.none && dropCount >= 1) {
                float angle = 360f / (float)dropCount;
                Vector3 vector = Vector3.up * upStrength + transform.forward * forwardStrength;
                Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < dropCount; i++) {
                    GenericPickupController.CreatePickupInfo info = default;
                    info.rotation = PressureDrop.Drop.identifier;
                    info.pickupIndex = dropPickup;
                    PickupDropletController.CreatePickupDroplet(info, transform.position + Vector3.up * 1.5f, vector);
                    vector = quaternion * vector;
                }
            }
            Output($"dropping @ {transform.position}");
        }

        private static void Output(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });
        }
    }
}
