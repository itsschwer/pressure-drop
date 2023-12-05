using BepInEx;
using RoR2;

namespace PressureDrop
{
    // This attribute is required, and lists metadata for this plugin.
    [BepInPlugin(GUID, Name, Version)]

    // This is the main declaration of this plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    public class Plugin : BaseUnityPlugin
    {
        // The GUID should be a unique ID for this plugin,
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
            Config._pressurePlateTimer.SettingChanged += SetPressurePlateTimer;
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
            if (args.Length != 1 || args[0].ToLowerInvariant() != "reload") return; // Only accept command with no additional args
            if (user != LocalUserManager.GetFirstLocalUser().currentNetworkUser) return; // Only allow host to reload

            base.Config.Reload();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"Reloaded configuration for <style=cSub>{Plugin.Slug}</style>" });
        }

        private void SetPressurePlateTimer(object sender = null, System.EventArgs e = null)
        {
            PressurePlateTimer self = this.gameObject.GetComponent<PressurePlateTimer>();
            if (Config.PressurePlateTimer != 0) {
                if (!self) this.gameObject.AddComponent<PressurePlateTimer>();
            }
            else Destroy(self);
#if DEBUG
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>ptt> {Config._pressurePlateTimer.Definition.Key} updated to {Config.PressurePlateTimer}</style>" });
#endif
        }
    }
}
