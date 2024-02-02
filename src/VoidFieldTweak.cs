using RoR2;

namespace PressureDrop
{
    internal static class VoidFieldTweak
    {
        private static bool _hooked = false;

        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            On.RoR2.ArenaMissionController.OnStartServer += ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound += ArenaMissionController_BeginRound;
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            On.RoR2.ArenaMissionController.OnStartServer -= ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound -= ArenaMissionController_BeginRound;
        }

        private static void ArenaMissionController_OnStartServer(On.RoR2.ArenaMissionController.orig_OnStartServer orig, ArenaMissionController self)
        {
            orig(self);
            self.SetFogActive(false);
        }

        private static void ArenaMissionController_BeginRound(On.RoR2.ArenaMissionController.orig_BeginRound orig, ArenaMissionController self)
        {
            orig(self);
            self.SetFogActive(true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
        private static void SetFogActive(this ArenaMissionController controller, bool value)
        {
#if DEBUG
            Log.Message($"Arena Fog Active>: {value}");
#endif
            controller.fogDamageInstance?.SetActive(value);
            controller.clearedEffect.SetActive(!value);
        }
    }
}
