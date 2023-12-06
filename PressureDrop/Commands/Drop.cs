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
                case "d":
                case "drop":
                    DropCommand(user, args);
                    break;
                case "s":
                case "steal":
                    StealCommand(user, args);
                    break;
            }
        }

        private static void DropCommand(NetworkUser user, string[] args)
        {
            // todo
        }

        private static void StealCommand(NetworkUser user, string[] args)
        {
            // todo
        }

        public static void DropStyleChest(Transform target, PickupIndex dropPickup, int dropCount)
        {
            float upStrength = 20f;
            float forwardStrength = 2f;

            if (dropPickup != PickupIndex.none && dropCount >= 1) {
                float angle = 360f / (float)dropCount;
                Vector3 vector = Vector3.up * upStrength + target.forward * forwardStrength;
                Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < dropCount; i++) {
                    GenericPickupController.CreatePickupInfo info = default;
                    info.rotation = PressureDrop.Drop.identifier;
                    info.pickupIndex = dropPickup;
                    PickupDropletController.CreatePickupDroplet(info, target.position + Vector3.up * 1.5f, vector);
                    vector = quaternion * vector;
                }
            }
            ChatCommander.Output($"dropping @ {target.position}");
        }

        public static void DropStyleChest(Transform target, PickupIndex[] drops)
        {
            float upStrength = 20f;
            float forwardStrength = 2f;

            if (drops.Length >= 1) {
                float angle = 360f / (float)drops.Length;
                Vector3 vector = Vector3.up * upStrength + target.forward * forwardStrength;
                Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < drops.Length; i++) {
                    if (drops[i] == PickupIndex.none) continue;

                    GenericPickupController.CreatePickupInfo info = default;
                    info.rotation = PressureDrop.Drop.identifier;
                    info.pickupIndex = drops[i];
                    PickupDropletController.CreatePickupDroplet(info, target.position + Vector3.up * 1.5f, vector);
                    vector = quaternion * vector;
                }
            }
            ChatCommander.Output($"dropping @ {target.position}");
        }
    }
}
