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
                    Output($"Commands provided by <style=cSub>{Plugin.Slug}</style>:");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}aq</style>: changes the stage to Abandoned Aqueduct.");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}g</style>: gives the user helpful items for testing pressure plate changes.");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}p</style>: drops an assortment of items for testing recyclability config.");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}ne</style>: disables further enemy spawns.");
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void ForceAqueduct(string[] args)
        {
            if (args.Length > 1) {
                OutputFail(args[0], "expects zero arguments.");
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
                        OutputFail(args[0], invalid);
                        return;
                    case "e":
                        {
                            const string equipmentName = "Scanner";
                            user.master.inventory.GiveEquipmentString(equipmentName);
                            EquipmentDef equipment = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex(equipmentName));
                            Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(equipment.nameToken)}</style>");
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

                Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(item.nameToken)}</style> <style=cStack>x{amount}</style>");
            }
            else OutputFail(args[0], invalid);
        }

        private static void PickupCommand(NetworkUser user, string[] args)
        {
            string invalid = "expects one argument from the following options [<style=cSub>i</style>, <style=cSub>e</style>, <style=cSub>p</style>, <style=cSub>s</style>, <style=cSub>v</style>]";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                Transform target = user.masterController.master.GetBodyObject().transform;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        OutputFail(args[0], invalid);
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
                        break;
                    case "e":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("Recycle")),
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("Tonic")),
                            PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("EliteFireEquipment"))
                        ]);
                        break;
                    case "s":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapWhite")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapGreen")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapRed")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ScrapYellow"))
                        ]);
                        break;
                    case "v":
                        Commands.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("BearVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("MissileVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("ExtraLifeVoid")),
                            PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("VoidMegaCrabItem"))
                        ]);
                        break;
                }
            }
            else OutputFail(args[0], invalid);
        }

        private static void DisableEnemySpawns(string[] args)
        {
            if (args.Length > 1) {
                OutputFail(args[0], "expects zero arguments.");
                return;
            }

            bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";

            CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);

            if (wasDisabled) Output("Enemy spawns enabled.");
            else Output("Enemy spawns disabled.");
        }

        private static void Output(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });
        }

        private static void OutputFail(string cmd, string message)
        {
            Output($"<style=cDeath>Failed:</style> <style=cSub>{ChatCommander.commandPrefix}{cmd}</style> {message}");
        }
    }
}
#endif
