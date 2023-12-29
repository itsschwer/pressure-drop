using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PressureDrop
{
    internal class PressurePlateTimer : MonoBehaviour
    {
        private static readonly Dictionary<PressurePlateController, float> timers = new(2);

        private void  OnEnable() => On.RoR2.PressurePlateController.SetSwitch += PressurePlateController_SetSwitch;
        private void OnDisable() => On.RoR2.PressurePlateController.SetSwitch -= PressurePlateController_SetSwitch;

        private void Update()
        {
            foreach (PressurePlateController key in timers.Keys.ToList()) {
                timers[key] -= Time.deltaTime;
                if (timers[key] <= 0) {
                    timers.Remove(key);
#if DEBUG
                    // Pressure plates may no longer exist if unloaded (e.g. stage transition)
                    string identifier = $"[no longer exists.]";
                    if (key) identifier = $"{key.name} @ {key.transform.position}";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{identifier} inactive ({timers.Count} active)</style>" });
#endif
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void PressurePlateController_SetSwitch(On.RoR2.PressurePlateController.orig_SetSwitch orig, PressurePlateController self, bool switchIsDown)
        {
            float time = Plugin.Config.PressurePlateTimer;

            if (switchIsDown) {
                if (switchIsDown != self.switchDown) {
                    string message = (Random.value <= 0.2) ? "Press your plate!" : "A pressure plate is pressed..";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</style>" });
                }

                orig(self, switchIsDown);
                timers[self] = time;
#if DEBUG
                string identifier = $"[no longer exists?]";
                if (self) identifier = $"{self.name} @ {self.transform.position}";
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{identifier} active {time}s ({timers.Count} active)</style>" });
#endif
            }
            else if (time > 0 && !timers.ContainsKey(self)) {
                if (switchIsDown != self.switchDown) {
                    const string message = "A pressure plate releases...";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</style>" });
                }

                orig(self, switchIsDown);
            }
        }
    }
}
