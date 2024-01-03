#if DEBUG
using RoR2;
using UnityEngine;

namespace PressureDrop.Commands
{
    internal static class DebugCheats
    {
        public static void  Enable()
        {
            ChatCommander.Register(".aq", ForceAqueduct);
            ChatCommander.Register(".g", GiveCommand);
            ChatCommander.Register(".p", PickupCommand);
            ChatCommander.Register(".ne", DisableEnemySpawns);

            ChatCommander.Register("/?", Help);
            ChatCommander.Register("/h", Help);
            ChatCommander.Register("/help", Help);
        }
        public static void Disable()
        {
            ChatCommander.Unregister(".aq", ForceAqueduct);
            ChatCommander.Unregister(".g", GiveCommand);
            ChatCommander.Unregister(".p", PickupCommand);
            ChatCommander.Unregister(".ne", DisableEnemySpawns);

            ChatCommander.Unregister("/?", Help);
            ChatCommander.Unregister("/h", Help);
            ChatCommander.Unregister("/help", Help);
        }

        private static void Help(NetworkUser user, string[] args)
        {
            ChatCommander.Output($"<style=cWorldEvent>{Plugin.GUID}</style> debug cheats:");
            ChatCommander.Output($"  <style=cSub>.aq</style>: changes the stage to Abandoned Aqueduct.");
            ChatCommander.Output($"  <style=cSub>.g</style>: gives the user items for testing pressure plate changes.");
            ChatCommander.Output($"  <style=cSub>.p</style>: drops items for testing recyclability config.");
            ChatCommander.Output($"  <style=cSub>.ne</style>: toggles enemy spawns.");
        }

        private static void ForceAqueduct(NetworkUser user, string[] args)
        {
            if (args.Length > 1) {
                ChatCommander.OutputFail(args[0], "expects zero arguments.");
                return;
            }

            // Run.instance.AdvanceStage(scene);
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            Run.instance.GenerateStageRNG();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene("goolake");

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>Sending you to the Origin of Tar <sprite name=\"Skull\" tint=1></style>" });
        }

        private static void GiveCommand(NetworkUser user, string[] args)
        {
            string invalid = "{ <style=cSub>e</style> | <style=cSub>t</style> | <style=cSub>s</style> | <style=cSub>g</style> } [<style=cSub><count></style>]";

            const int expectedArgs = 2;
            if (args.Length >= expectedArgs) {
                ParseGiveCommandItem(args[1].ToLowerInvariant(), out ItemDef item, out int count, out EquipmentDef equipment);
                if (item == null) item = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(args[1]));

                if (equipment != null) {
                    user.master.inventory.GiveEquipmentString(equipment.name);
                    ChatCommander.Output($"Gave {user.userName} {ChatCommander.GetColoredPickupLanguageString(equipment.equipmentIndex)}");
                    return;
                }
                else if (item == null) { ChatCommander.OutputFail(args[0], invalid); return; }

                if (args.Length > expectedArgs && int.TryParse(args[2], out int num)) count = num;
                if (count > 1000) count = 1000; // Let's not get too excessive

                user.master.inventory.GiveItem(item, count);
                ChatCommander.Output($"Gave {user.userName} {ChatCommander.GetColoredPickupLanguageString(item.itemIndex)} <style=cStack>x{count}</style>");
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }

        private static void ParseGiveCommandItem(string arg, out ItemDef item, out int count, out EquipmentDef equipment)
        {
            item = null;
            equipment = null;
            count = 1;

            switch (arg) {
                default: break;
                case "e":
                    equipment = RoR2Content.Equipment.Scanner;
                    break;
                case "t":
                    item = RoR2Content.Items.Bear;
                    count = 70;
                    break;
                case "s":
                    item = RoR2Content.Items.SprintBonus;
                    count = 5;
                    break;
                case "g":
                    item = RoR2Content.Items.AutoCastEquipment;
                    count = 7;
                    break;
            }
        }


        private static void PickupCommand(NetworkUser user, string[] args)
        {
            string invalid = "{ <style=cSub>t</style> | <style=cSub>v</style> | <style=cSub>s</style> }";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                Transform target = user.GetCurrentBody().gameObject.transform;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "t":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintBonus.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.EquipmentMagazine.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLife.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintWisp.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Recycle.equipmentIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Tonic.equipmentIndex),
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped test pickups @ {user.userName}.");
                        break;
                    case "v":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.BearVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.MissileVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.ExtraLifeVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.VoidMegaCrabItem.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped void pickups @ {user.userName}.");
                        break;
                    case "s":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLifeConsumed.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.TonicAffliction.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.HealingPotionConsumed.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.CaptainDefenseMatrix.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.DrizzlePlayerHelper.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped special pickups @ {user.userName}.");
                        break;
                }
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }


        private static void DisableEnemySpawns(NetworkUser user, string[] args)
        {
            if (args.Length > 1) {
                ChatCommander.OutputFail(args[0], "expects zero arguments.");
                return;
            }

            bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";
            CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);
            if (wasDisabled) ChatCommander.Output("Enemy spawns enabled.");
            else ChatCommander.Output("Enemy spawns disabled.");
        }
    }
}
#endif
