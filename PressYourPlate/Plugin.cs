using BepInEx;
using RoR2;
using UnityEngine;

namespace PressYourPlate
{
    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    public class Plugin : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "itsschwer";
        public const string PluginName = "PressYourPlate";
        public const string PluginVersion = "0.0.0";

        // The Awake() method is run at the very start when the game is initialized.
        private void Awake()
        {
            // Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
        }

        private void OnEnable() => On.RoR2.PressurePlateController.SetSwitch += PermaPressPressurePlate;
        private void OnDisable() => On.RoR2.PressurePlateController.SetSwitch -= PermaPressPressurePlate;

        private static void PermaPressPressurePlate(On.RoR2.PressurePlateController.orig_SetSwitch orig, PressurePlateController self, bool switchIsDown)
        {
            // Only allow pressure plate to be pressed -- never release
            if (switchIsDown) {
                if (switchIsDown != self.switchDown) {
                    string message = (Random.value <= 0.2) ? "Press your plate!" : "A pressure plate was pressed...";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cUserSetting>" + message + "</style>" });
                }

                orig(self, switchIsDown);
            }

            // Would ideally replace with a configurable timer for how long a pressure plate remains depressed
        }

#if DEBUG
        // The Update() method is run on every frame of the game.
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2)) ForceAqueduct();
            else PickupMaker.Poll();
        }

        private void ForceAqueduct()
        {
            if (!Run.instance) return;

            var scene = SceneCatalog.FindSceneDef("goolake");
            // Run.instance.AdvanceStage(scene);
            Run.instance.GenerateStageRNG();
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(scene.cachedName);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cWorldEvent>F2> Sending you to the Origin of Tar <sprite name=\"Skull\" tint=1></style>" });
        }
#endif
    }
}
