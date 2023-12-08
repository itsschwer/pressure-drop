#if DEBUG
using RoR2;
using UnityEngine;

namespace PressureDrop.Commands
{
    internal static class DebugCheats
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
                ItemDef item;
                int count;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "e":
                        {
                            EquipmentDef e = RoR2Content.Equipment.Scanner;
                            user.master.inventory.GiveEquipmentString(e.name);
                            ChatCommander.Output($"Gave {user.userName} {ChatCommander.GetColoredPickupLanguageString(e.nameToken, e.equipmentIndex)}");
                            return;
                        } 
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

                if (args.Length > expectedArgs && int.TryParse(args[2], out int num)) count = num;
                if (count > 1000) count = 1000; // Let's not get too excessive

                user.master.inventory.GiveItem(item, count);
                ChatCommander.Output($"Gave {user.userName} {ChatCommander.GetColoredPickupLanguageString(item.nameToken, item.itemIndex)} <style=cStack>x{count}</style>");
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }

        private static void PickupCommand(NetworkUser user, string[] args)
        {
            string invalid = "expects one argument from the following options [<style=cSub>i</style>, <style=cSub>e</style>, <style=cSub>s</style>, <style=cSub>v</style>, <style=cSub>h</style>]";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                Transform target = user.GetCurrentBody().gameObject.transform;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "i":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintBonus.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.EquipmentMagazine.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLife.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintWisp.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped item pickups @ {user.userName}.");
                        break;
                    case "e":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Recycle.equipmentIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Tonic.equipmentIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixRed.equipmentIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped equipment pickups @ {user.userName}.");
                        break;
                    case "s":
                        PressureDrop.Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapGreen.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapRed.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapYellow.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped scrap pickups @ {user.userName}.");
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
                    case "h":
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
