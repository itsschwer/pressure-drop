using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace PressureDrop
{
    public static class ChatCommander
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

        /// <summary>
        /// Registers a chat command.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <returns>
        /// Returns <see langword="false"/> if the chat command could not be registered.
        /// </returns>
        public static bool Register(string token, Action<NetworkUser, string[]> action, bool replace = false)
        {
            if (Commands.ContainsKey(token)) {
                if (replace) {
                    Plugin.Logger.LogWarning($"{nameof(ChatCommander)}> '{token}' is replacing a previously registered chat command.");
                }
                else {
                    Plugin.Logger.LogWarning($"{nameof(ChatCommander)}> A chat command is already registered under '{token}'");
                    return false;
                }
            }

            Commands[token] = action;
            string origin = action.Method.DeclaringType.Assembly.GetName().Name;
            origin = (origin != typeof(ChatCommander).Assembly.GetName().Name) ? $" [from {origin}.dll]" : "";
            Plugin.Logger.LogInfo($"{nameof(ChatCommander)}> Registering a chat command under '{token}'{origin}");
            return true;
        }

        /// <summary>
        /// Unregisters a chat command.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <returns>Returns <see langword="true"/> if the chat command was successfully unregistered.</returns>
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

        /// <summary>
        /// Wrapper for sending a chat message styled as an output of a command.
        /// </summary>
        /// <param name="message"></param>
        public static void Output(string message)
            => Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{message}</style>" });

        /// <summary>
        /// Wrapper for sending a chat message styled as a response to a failed command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="message"></param>
        public static void OutputFail(string cmd, string message) 
            => Output($"<style=cDeath>Failed:</style> <color=#ffffff>{cmd}</color> {message}");


        public static string GetColoredPickupLanguageString(ItemIndex itemIndex)
            => GetColoredPickupLanguageString(PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemIndex)));
        public static string GetColoredPickupLanguageString(PickupDef pickupDef)
            => Util.GenerateColoredString(Language.GetString(pickupDef.nameToken), pickupDef.baseColor);
    }
}
