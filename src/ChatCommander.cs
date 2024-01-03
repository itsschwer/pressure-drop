using BepInEx.Logging;
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
            string[] args = message.Split(' ');
            // Not server, not a chat command, or not a registered command
            if (!NetworkServer.active || !concommandName.Equals("say", StringComparison.InvariantCultureIgnoreCase)
                || string.IsNullOrWhiteSpace(message)
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
        /// Returns <see cref="false"/> if the chat command could not be registered.
        /// </returns>
        public static bool Register(string token, Action<NetworkUser, string[]> action, bool replace = false)
        {
            if (Commands.ContainsKey(token)) {
                if (replace) {
                    Log.Warning($"{nameof(ChatCommander)}> '{token}' is replacing a previously registered chat command.");
                }
                else {
                    Log.Warning($"{nameof(ChatCommander)}> A chat command is already registered under '{token}'.");
                    return false;
                }
            }

            Commands[token] = action;
            Log.Info($"{nameof(ChatCommander)}> Registering a chat command under '{token}'.");
            return true;
        }

        /// <summary>
        /// Unregisters a chat command.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <returns>Returns <see cref="true"/> if the chat command was successfully unregistered.</returns>
        public static bool Unregister(string token, Action<NetworkUser, string[]> action)
        {
            if (Commands.TryGetValue(token, out Action<NetworkUser, string[]> command)) {
                if (command == action) {
                    Log.Info($"{nameof(ChatCommander)}> Unregistered '{token}' chat command.");
                    Commands.Remove(token);
                    return true;
                }
                else Log.Warning($"{nameof(ChatCommander)}> Could not unregister chat command '{token}' as the action does not match.");
            }
            else Log.Info($"{nameof(ChatCommander)}> Could not unregister chat command '{token}' (not registered).");

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
            => GetColoredPickupLanguageString(PickupCatalog.FindPickupIndex(itemIndex));
        public static string GetColoredPickupLanguageString(EquipmentIndex equipmentIndex)
            => GetColoredPickupLanguageString(PickupCatalog.FindPickupIndex(equipmentIndex));
        public static string GetColoredPickupLanguageString(PickupIndex pickupIndex)
        {
            PickupDef def = PickupCatalog.GetPickupDef(pickupIndex);
            return Util.GenerateColoredString(Language.GetString(def.nameToken), def.baseColor);
        }
    }
}
