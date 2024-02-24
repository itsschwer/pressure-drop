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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
        private static void ChatBox_UpdateFade(On.RoR2.UI.ChatBox.orig_UpdateFade orig, RoR2.UI.ChatBox self, float deltaTime)
        {
            // Scoreboard visibility logic from RoR2.UI.HUD.Update()
            Rewired.Player inputPlayer = LocalUserManager.GetFirstLocalUser()?.inputPlayer;
            if (inputPlayer != null && inputPlayer.GetButton("info")) {
                self.ResetFadeTimer();
            }
            orig(self, deltaTime);
        }
    }
}
