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
            }
        }

        private static void DropCommand(NetworkUser user, string[] args)
        {
            const int expectedArgs = 2;
            if (args.Length < expectedArgs) { ShowHelp(); return; }

            bool dropAtTeleporter = false;

            if (args.Length > expectedArgs) {
                if (args[2] == "@") dropAtTeleporter = true;
                else { ShowHelp(); return; }
            }

            string name = user.masterController.GetDisplayName();
            Inventory inventory = user.master.inventory;
            Transform target = user.GetCurrentBody().gameObject.transform;
            ItemIndex itemIndex = inventory.FindItemInInventory(args[1]);

            if (itemIndex == ItemIndex.None) {
                ChatCommander.Output($"'{args[1]}' did not match any items in {name}'s inventory.");
            }
            else {
                ItemDef def = ItemCatalog.GetItemDef(itemIndex);
                if (def == RoR2Content.Items.CaptainDefenseMatrix) {
                    ChatCommander.Output($"{ChatCommander.GetColoredPickupLanguageString(def.nameToken, def.itemIndex)} can not be dropped.");
                }
                else {
                    int count = inventory.GetItemCount(def.itemIndex);
                    if (count > Plugin.Config.MaxItemsToDropAtATime) count = Plugin.Config.MaxItemsToDropAtATime;

                    if (count > 0) {
                        inventory.RemoveItem(def.itemIndex, count);
                        DropStyleChest(target, PickupCatalog.FindPickupIndex(def.itemIndex), count);
                    }
                    ChatCommander.Output($"{name} dropped {ChatCommander.GetColoredPickupLanguageString(def.nameToken, def.itemIndex)} <style=cStack>x{count}</style>");
                }
            }
        }

        private static void ShowHelp()
        {
            // todo
            ChatCommander.Output("/syntax error");
        }

        public static void DropStyleChest(Transform target, PickupIndex dropPickup, int dropCount, float forwardVelocity = 2f, float upVelocity = 20f)
        {
            if (dropPickup != PickupIndex.none && dropCount >= 1) {
                float angle = 360f / (float)dropCount;
                Vector3 vector = Vector3.up * upVelocity + target.forward * forwardVelocity;
                Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);

                for (int i = 0; i < dropCount; i++) {
                    GenericPickupController.CreatePickupInfo info = default;
                    info.rotation = PressureDrop.Drop.identifier;
                    info.pickupIndex = dropPickup;
                    PickupDropletController.CreatePickupDroplet(info, target.position + Vector3.up * 1.5f, vector);
                    vector = quaternion * vector;
                }
            }
        }

        public static void DropStyleChest(Transform target, PickupIndex[] drops, float forwardVelocity = 2f, float upVelocity = 20f)
        {
            if (drops.Length >= 1) {
                float angle = 360f / (float)drops.Length;
                Vector3 vector = Vector3.up * upVelocity + target.forward * forwardVelocity;
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
        }
    }
}
