#if DEBUG
using RoR2;
using UnityEngine;

namespace PressureDrop
{
    public static class DebugCheats
    {
        public static void  Enable() => ChatCommander.OnChatCommand += ParseCommand;
        public static void Disable() => ChatCommander.OnChatCommand -= ParseCommand;

        private static void ParseCommand(NetworkUser user, string[] args)
        {
            if (!Run.instance || args.Length < 1) return;

            switch (args[0].ToLowerInvariant()) {
                default:
                    break;
                case "aq":
                    ForceAqueduct(args);
                    break;
                case "g":
                    GiveCommand(user, args);
                    break;
                case "p":
                    PickupCommand(user, args);
                    break;
                case "ne":
                    DisableEnemySpawns(args);
                    break;
                case "?":
                case "h":
                case "help":
                    ChatCommander.Output($"Commands provided by <style=cSub>{Plugin.Slug}</style>:");
                    ChatCommander.Output($"  <style=cSub>{ChatCommander.commandPrefix}aq</style>: changes the stage to Abandoned Aqueduct.");
                    ChatCommander.Output($"  <style=cSub>{ChatCommander.commandPrefix}g</style>: gives the user items for testing pressure plate changes.");
                    ChatCommander.Output($"  <style=cSub>{ChatCommander.commandPrefix}p</style>: drops items for testing recyclability config.");
                    ChatCommander.Output($"  <style=cSub>{ChatCommander.commandPrefix}ne</style>: disables further enemy spawns.");
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void ForceAqueduct(string[] args)
        {
            if (args.Length > 1) {
                ChatCommander.OutputFail(args[0], "expects zero arguments.");
                return;
            }

            SceneDef scene = SceneCatalog.FindSceneDef("goolake");
            // Run.instance.AdvanceStage(scene);
            Run.instance.GenerateStageRNG();
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(scene.cachedName);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>Sending you to the Origin of Tar <sprite name=\"Skull\" tint=1></style>" });
        }

        private static void GiveCommand(NetworkUser user, string[] args)
        {
            string invalid = "expects one argument from the following options [<style=cSub>t</style>, <style=cSub>s</style>, <style=cSub>g</style>, <style=cSub>e</style>]";

            const int expectedArgs = 2;
            if (args.Length >= expectedArgs) {
                string itemName;
                int amount;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "e":
                        {
                            const string equipmentName = "Scanner";
                            user.master.inventory.GiveEquipmentString(equipmentName);
                            EquipmentDef equipment = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex(equipmentName));
                            ChatCommander.Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(equipment.nameToken)}</style>");
                            return;
                        } 
                    case "t":
                        itemName = "Bear";
                        amount = 70;
                        break;
                    case "s":
                        itemName = "SprintBonus";
                        amount = 5;
                        break;
                    case "g":
                        itemName = "AutoCastEquipment";
                        amount = 7;
                        break;
                }

                if (args.Length > expectedArgs && int.TryParse(args[2], out int count)) amount = count;

                ItemDef item = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemName));
                user.master.inventory.GiveItem(item, amount);

                ChatCommander.Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(item.nameToken)}</style> <style=cStack>x{amount}</style>");
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }

        private static void PickupCommand(NetworkUser user, string[] args)
        {
            string invalid = "expects one argument from the following options [<style=cSub>i</style>, <style=cSub>e</style>, <style=cSub>s</style>, <style=cSub>v</style>]";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                Transform target = user.masterController.master.GetBodyObject().transform;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "i":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("SprintBonus")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("EquipmentMagazine")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ExtraLife")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("SprintWisp")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("AutoCastEquipment")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("Pearl"))
                        ]);
                        ChatCommander.Output($"Dropped item pickups @ {user.userName}.");
                        break;
                    case "e":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("Recycle")),
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("Tonic")),
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("EliteFireEquipment"))
                        ]);
                        ChatCommander.Output($"Dropped equipment pickups @ {user.userName}.");
                        break;
                    case "s":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapWhite")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapGreen")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapRed")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapYellow"))
                        ]);
                        ChatCommander.Output($"Dropped scrap pickups @ {user.userName}.");
                        break;
                    case "v":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("BearVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("MissileVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ExtraLifeVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("VoidMegaCrabItem"))
                        ]);
                        ChatCommander.Output($"Dropped void pickups @ {user.userName}.");
                        break;
                }
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }

        private static void DisableEnemySpawns(string[] args)
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
