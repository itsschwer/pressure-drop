using BepInEx;
using RoR2;
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

        // The Awake() method is run at the very start when the game is initialized.
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "Publicizer001")]
        private static void PermaPressPressurePlate(On.RoR2.PressurePlateController.orig_SetSwitch orig, PressurePlateController self, bool switchIsDown)
        {
            // Only call original function when pressure plate is being pressed -- pressure plate will never release
            if (switchIsDown) {
                if (switchIsDown != self.switchDown) {
                    string message = (Random.value <= 0.2) ? "Press your plate!" : "A pressure plate is pressed..";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cUserSetting>" + message + "</style>" });
                }

                orig(self, switchIsDown);
            }

            // Would ideally replace with a configurable timer for how long a pressure plate remains depressed
        }
    }
}
