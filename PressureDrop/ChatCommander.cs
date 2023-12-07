using RoR2;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PressureDrop
{
    public static class ChatCommander
    {
        public const string commandPrefix = "/";

        public static event Action<NetworkUser, string[]> OnChatCommand;

        public static void   Subscribe() => Chat.onChatChanged += OnChatChanged;
        public static void Unsubscribe() => Chat.onChatChanged -= OnChatChanged;

        private static void OnChatChanged()
        {
            if (Chat.readOnlyLog.Count <= 0) return;

            var userMessage = Construct(Chat.readOnlyLog.Last());
            if (userMessage == null) return;

            string message = userMessage.Value.text;
            if (!message.StartsWith(commandPrefix)) return;

            message = message.Substring(commandPrefix.Length);
            
            OnChatCommand.Invoke(userMessage.Value.user, message.Split(' '));
        }

        private static (NetworkUser user, string text)? Construct(string chatLogMessage)
        {
            const string pattern = @"<color=#e5eefc><noparse>(.+?)<\/noparse>: <noparse>(.+?)<\/noparse><\/color>";
            Match match = Regex.Match(chatLogMessage, pattern, RegexOptions.Compiled);

            if (!match.Success || match.Groups.Count < 3) return null;

            foreach(NetworkUser user in NetworkUser.readOnlyInstancesList) {
                if (user.userName != match.Groups[1].Value.Trim()) continue;

                return (user, match.Groups[2].Value.Trim());
            }

            return null;
        }

        public static string GetColoredPickupLanguageString(string token, ItemIndex itemIndex)
            => GetColoredPickupLanguageString(token, PickupCatalog.FindPickupIndex(itemIndex));
        public static string GetColoredPickupLanguageString(string token, EquipmentIndex equipmentIndex)
            => GetColoredPickupLanguageString(token, PickupCatalog.FindPickupIndex(equipmentIndex));
        public static string GetColoredPickupLanguageString(string token, PickupIndex pickupIndex)
        {
            PickupDef def = PickupCatalog.GetPickupDef(pickupIndex);
            return Util.GenerateColoredString(Language.GetString(token), def.baseColor);
        }

        /// <summary>
        /// Wrapper for sending a chat message styled as an output of a command.
        /// </summary>
        /// <param name="message"></param>
        public static void Output(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });
        }

        /// <summary>
        /// Wrapper for sending a chat message styled as a response to a failed command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="message"></param>
        public static void OutputFail(string cmd, string message)
        {
            Output($"<style=cDeath>Failed:</style> <color=#ffffff>{ChatCommander.commandPrefix}{cmd}</color> {message}");
        }
    }
}
