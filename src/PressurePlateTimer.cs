using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PressureDrop
{
    internal sealed class PressurePlateTimer : MonoBehaviour
    {
        private static readonly Dictionary<PressurePlateController, float> timers = new(2);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void  OnEnable() => On.RoR2.PressurePlateController.SetSwitch += PressurePlateController_SetSwitch;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable() => On.RoR2.PressurePlateController.SetSwitch -= PressurePlateController_SetSwitch;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Update()
        {
            foreach (PressurePlateController self in timers.Keys.ToList()) {
                timers[self] -= Time.deltaTime;
                if (timers[self] <= 0) {
                    timers.Remove(self);
#if DEBUG
                    // Pressure plates may no longer exist if unloaded (e.g. stage transition)
                    string identifier = (self != null) ? $"[{self.name}]" : "[no longer exists.]";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{identifier} inactive ({timers.Count} active)</style>" });
#endif
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
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
                string identifier = (self != null) ? $"[{self.name}]" : "[no longer exists?]";
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
