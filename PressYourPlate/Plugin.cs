using BepInEx;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PressYourPlate
{
    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(GUID, Name, Version)]

    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    public class Plugin : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "PressYourPlate";
        public const string Version = "0.0.0";

        private static Dictionary<PressurePlateController, float> timers = new(2);

        private void Awake() => Log.Init(Logger);

        private void OnEnable()
        {
            On.RoR2.PressurePlateController.SetSwitch += PermaPressPressurePlate;

#if DEBUG
            ChatCommandUtility.Subscribe();
            ChatCommands.Enable();
#endif
        }

        private void OnDisable()
        {
            On.RoR2.PressurePlateController.SetSwitch -= PermaPressPressurePlate;

#if DEBUG
            ChatCommandUtility.Unsubscribe();
            ChatCommands.Disable();
#endif
        }

        private void Update()
        {
            foreach (PressurePlateController key in timers.Keys.ToList()) {
                timers[key] -= Time.deltaTime;
                if (timers[key] <= 0) {
                    timers.Remove(key);

#if DEBUG
                    string identifier = $"[no longer exists.]";
                    if (key) identifier = $"{key.name} @ {key.transform.position}";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>> {identifier} inactive ({timers.Count} active)</style>" });
#endif
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void PermaPressPressurePlate(On.RoR2.PressurePlateController.orig_SetSwitch orig, PressurePlateController self, bool switchIsDown)
        {
            // todo: make configurable -- <0 = perma, 0 = off, >0 = timed
            const float time = 30f;

            if (switchIsDown) {
                if (switchIsDown != self.switchDown) {
                    string message = (Random.value <= 0.2) ? "Press your plate!" : "A pressure plate is pressed..";
                    Output(message);
                }

                timers[self] = time;
                orig(self, switchIsDown);

#if DEBUG
                string identifier = $"[no longer exists?]";
                if (self) identifier = $"{self.name} @ {self.transform.position}";
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>> {identifier} active {time}s ({timers.Count} active)</style>" });
#endif
            }
            else if (time > 0 && !timers.ContainsKey(self)) {
                if (switchIsDown != self.switchDown) {
                    Output("A pressure plate releases...");
                }

                orig(self, switchIsDown);
            }
        }

        private static void Output(string message) {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cStack>" + message + "</style>" });
        }
    }
}
