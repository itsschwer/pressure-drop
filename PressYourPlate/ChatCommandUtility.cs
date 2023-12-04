using RoR2;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PressYourPlate
{
    public static class ChatCommandUtility
    {
        private const string commandPrefix = "/";

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
            Log.Debug(chatLogMessage);
            const string pattern = @"<color=#e5eefc><noparse>(.+?)<\/noparse>: <noparse>(.+?)<\/noparse><\/color>";
            Match match = Regex.Match(chatLogMessage, pattern, RegexOptions.Compiled);

            if (!match.Success || match.Groups.Count < 3) return null;

            foreach(NetworkUser user in NetworkUser.readOnlyInstancesList) {
                if (user.userName != match.Groups[1].Value.Trim()) continue;

                return (user, match.Groups[2].Value.Trim());
            }

            return null;
        }
    }
}
