#if DEBUG
using BepInEx;
using RoR2;

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
                case "give":
                    GiveCommand(user, args);
                    break;
                case "ne":
                    DisableEnemySpawns(args);
                    break;
                case "?":
                case "h":
                case "help":
                    Output($"Commands provided by <style=cSub>{Plugin.Slug}</style>:");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}aq</style>: changes the stage to Abandoned Aqueduct.");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}give</style>: gives the user helpful items for testing pressure plate changes.");
                    Output($"      Aliases: <style=cSub>{ChatCommander.commandPrefix}g</style>");
                    Output($"  <style=cSub>{ChatCommander.commandPrefix}ne</style>: disables further enemy spawns.");
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void ForceAqueduct(string[] args)
        {
            if (args.Length > 1) {
                Output($"<style=cDeath>Failed:</style> <style=cSub>{ChatCommander.commandPrefix}{args[0]}</style> expects zero arguments.");
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
            string invalid = $"<style=cDeath>Failed:</style> <style=cSub>{ChatCommander.commandPrefix}{args[0]}</style> expects one argument from the following options [<style=cSub>t</style>, <style=cSub>s</style>, <style=cSub>g</style>, <style=cSub>e</style>]";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                string e = args[1].ToLowerInvariant();
                string equipmentName = (e == "e") ? "Scanner" : (e == "e2") ? "Recycle" : "";

                if (!equipmentName.IsNullOrWhiteSpace()) {
                    user.master.inventory.GiveEquipmentString(equipmentName);
                    EquipmentDef equipment = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex(equipmentName));
                    Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(equipment.nameToken)}</style>");
                    return;
                }

                string itemName = "";
                int amount = 0;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        Output(invalid);
                        return;
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

                ItemDef item = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemName));
                user.master.inventory.GiveItem(item, amount);

                Output($"Gave {user.userName} <style=cIsDamage>{Language.GetString(item.nameToken)}</style> <style=cStack>x{amount}</style>");
            }
            else Output(invalid);
        }

        private static void DisableEnemySpawns(string[] args)
        {
            if (args.Length > 1) {
                Output($"<style=cDeath>Failed:</style> <style=cSub>{ChatCommander.commandPrefix}{args[0]}</style> expects zero arguments.");
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
    }
}
#endif
