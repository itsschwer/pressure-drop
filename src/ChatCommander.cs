using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace PressureDrop
{
    internal static class ChatCommander
    {
        private static readonly Dictionary<string, Action<NetworkUser, string[]>> Commands = [];

        private static bool _hooked = false;
        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            On.RoR2.Console.RunCmd += Console_RunCmd;
        }

        private static void Console_RunCmd(On.RoR2.Console.orig_RunCmd orig, RoR2.Console self, RoR2.Console.CmdSender sender, string concommandName, List<string> userArgs)
        {
            string message = userArgs.FirstOrDefault();
            string[] args = message?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Not server, not a chat command, or not a registered command
            if (!NetworkServer.active || !concommandName.Equals("say", StringComparison.InvariantCultureIgnoreCase)
                || string.IsNullOrWhiteSpace(message) || args == null || args.Length <= 0
                || !Commands.TryGetValue(args[0], out Action<NetworkUser, string[]> command) || command == null)
            {
                orig.Invoke(self, sender, concommandName, userArgs);
                return;
            }
            // Otherwise, execute and stop propagation

            // Finish say command (add player chat message) | RoR2.Chat.CCSay()
            Chat.SendBroadcastChat(new Chat.UserChatMessage {
                sender = sender.networkUser.gameObject,
                text = message
            });
            // And execute chat command
            command.Invoke(sender.networkUser, args);
        }

        public static bool Register(string token, Action<NetworkUser, string[]> action)
        {
            if (Commands.ContainsKey(token)) {
                Plugin.Logger.LogWarning($"{nameof(ChatCommander)}> A chat command is already registered under '{token}'");
                return false;
            }

            Commands[token] = action;
            string origin = action.Method.DeclaringType.Assembly.GetName().Name;
            origin = (origin != typeof(ChatCommander).Assembly.GetName().Name) ? $" [from {origin}.dll]" : "";
            Plugin.Logger.LogInfo($"{nameof(ChatCommander)}> Registering a chat command under '{token}'{origin}");
            return true;
        }

        public static bool Unregister(string token, Action<NetworkUser, string[]> action)
        {
            if (Commands.TryGetValue(token, out Action<NetworkUser, string[]> command)) {
                if (command == action) {
                    Plugin.Logger.LogInfo($"{nameof(ChatCommander)}> Unregistered '{token}' chat command.");
                    Commands.Remove(token);
                    return true;
                }
                else Plugin.Logger.LogWarning($"{nameof(ChatCommander)}> Could not unregister chat command '{token}' as the action does not match.");
            }
            else Plugin.Logger.LogInfo($"{nameof(ChatCommander)}> Could not unregister chat command '{token}' (not registered).");

            return false;
        }
    }
}
