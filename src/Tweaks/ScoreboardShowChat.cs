using RoR2;

namespace PressureDrop
{
    internal static class ScoreboardShowChat
    {
        private static bool _hooked = false;

        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.RoR2.UI.ChatBox.UpdateFade += ChatBox_UpdateFade;
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.RoR2.UI.ChatBox.UpdateFade -= ChatBox_UpdateFade;
        }

        private static void ChatBox_UpdateFade(On.RoR2.UI.ChatBox.orig_UpdateFade orig, RoR2.UI.ChatBox self, float deltaTime)
        {
            // Let game calculate fade normally
            orig(self, deltaTime);
            // But override fade if scoreboard is open
            if (self.fadeGroup != null) {
                Rewired.Player inputPlayer = LocalUserManager.GetFirstLocalUser()?.inputPlayer;
                // Scoreboard visibility logic from RoR2.UI.HUD.Update()
                if (inputPlayer != null && inputPlayer.GetButton("info")) {
                    self.fadeGroup.alpha = 1;
                }
            }
        }
    }
}
