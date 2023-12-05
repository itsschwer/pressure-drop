using BepInEx;
using RoR2;

namespace PressureDrop
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
        public const string Name = "PressureDrop";
        public const string Version = "0.0.0";
        public const string Slug = "pressure-drop";

        internal static new Config Config { get; private set; }

        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);
        }

        private void OnEnable()
        {
            ChatCommander.Subscribe();
            ChatCommander.OnChatCommand += ParseReload;

            SetPressurePlateTimer();
#if DEBUG
            DebugCheats.Enable();
#endif
        }

        private void OnDisable()
        {
            ChatCommander.Unsubscribe();
            ChatCommander.OnChatCommand -= ParseReload;
#if DEBUG
            DebugCheats.Disable();
#endif
        }

        private void ParseReload(NetworkUser user, string[] args)
        {
            if (args.Length != 1 && args[0].ToLowerInvariant() != "reload") return;
            base.Config.Reload();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"Reloaded configuration for <style=cSub>{Plugin.Slug}</style>" });
        }

        private void SetPressurePlateTimer(object sender = null, System.EventArgs e = null)
        {
            if (Config.PressurePlateTimer != 0) this.gameObject.AddComponent<PressurePlateTimer>();
            else Destroy(this.gameObject.GetComponent<PressurePlateTimer>());
#if DEBUG
            Log.Info($"{Config._pressurePlateTimer.Definition.Key} updated to {Config.PressurePlateTimer}");
#endif
        }
    }
}
