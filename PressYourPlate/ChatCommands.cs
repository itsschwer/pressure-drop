using RoR2;

namespace PressYourPlate
{
    public static class ChatCommands
    {
        public static void  Enable() => ChatCommandUtility.OnChatCommand += ParseCommand;
        public static void Disable() => ChatCommandUtility.OnChatCommand -= ParseCommand;

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
                case "?":
                case "h":
                case "help":
                    Output("Commands provided by <style=cSub>press-your-plate</style>:");
                    Output("  <style=cSub>/aq</style>: changes the stage to Abandoned Aqueduct");
                    Output("  <style=cSub>/give</style>: gives the user helpful items for testing this mod");
                    Output("  <style=cSub>/g</style>: alias for <style=cSub>/give</style>");
                    break;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void ForceAqueduct(string[] args)
        {
            if (!Run.instance) return;

            if (args.Length > 1)
            {
                Output($"<style=cDeath>Failed:</style> <style=cSub>{args[0]}</style> expects zero arguments");
                return;
            }

            SceneDef scene = SceneCatalog.FindSceneDef("goolake");
            // Run.instance.AdvanceStage(scene);
            Run.instance.GenerateStageRNG();
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(scene.cachedName);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>> Sending you to the Origin of Tar <sprite name=\"Skull\" tint=1></style>" });
        }

        private static void GiveCommand(NetworkUser user, string[] args)
        {
            string invalid = $"<style=cDeath>Failed:</style> <style=cSub>{args[0]}</style> expects one argument from the following options [<style=cSub>t</style>, <style=cSub>s</style>, <style=cSub>g</style>, <style=cSub>e</style>]";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs)
            {
                if (args[1].ToLowerInvariant() == "e") {
                    user.master.inventory.GiveEquipmentString("Scanner");
                    Output($"Gave {user.userName} <style=cIsDamage>Radar Scanner</style>");
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

                ItemIndex item = ItemCatalog.FindItemIndex(itemName);
                user.master.inventory.GiveItem(item, amount);

                Output($"Gave {user.userName} <style=cIsDamage>{itemName}</style> <style=cStack>x{amount}</style>");
            }
            else Output(invalid);
        }

        private static void Output(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });
        }
    }
}
