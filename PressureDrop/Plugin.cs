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

        // MonoBehaviour components
        private PressurePlateTimer pressure;

        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);

            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();
        }

        private void OnEnable()
        {
            ChatCommander.Subscribe();
            ChatCommander.OnChatCommand += ParseReload;
#if DEBUG
            DebugCheats.Enable();
#endif
            Config._pressurePlateTimer.SettingChanged += ConfigurePressurePlateTimerComponent;

            ConfigurePressurePlateTimerComponent();

            ConfigureDrop(true);

            Log.Message($"{Plugin.Slug}> enabled.");
        }

        private void OnDisable()
        {
            ChatCommander.Unsubscribe();
            ChatCommander.OnChatCommand -= ParseReload;
#if DEBUG
            DebugCheats.Disable();
#endif
            Config._pressurePlateTimer.SettingChanged -= ConfigurePressurePlateTimerComponent;

            ConfigureDrop(false);

            Log.Message($"{Plugin.Slug}> disabled.");
        }

        private void SetPluginActiveState(Run run = null) => SetActive(UnityEngine.Networking.NetworkServer.active);

        /// <summary>
        /// All plugins are attached to the same
        /// <see href="https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Bootstrap/Chainloader.cs#L88">GameObject</see>,
        /// so manually manage components instead of calling
        /// <c>this.gameObject.SetActive(value)</c>.
        /// </summary>
        private void SetActive(bool value) {
            this.enabled = value;
            if (this.pressure) this.pressure.enabled = value;
            Log.Message($"{Plugin.Slug}> {(value ? "active" : "inactive")}.");
        }

        private void ParseReload(NetworkUser user, string[] args)
        {
            if (args.Length != 1 || args[0].ToLowerInvariant() != "reload") return; // Only accept command with no additional args
            if (user != LocalUserManager.GetFirstLocalUser().currentNetworkUser) return; // Only allow host to reload

            base.Config.Reload();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"Reloaded configuration for <style=cSub>{Plugin.Slug}</style>" });
        }

        private void ConfigurePressurePlateTimerComponent(object sender = null, System.EventArgs e = null)
        {
            if (Config.PressurePlateTimer != 0) {
                if (!pressure) pressure = this.gameObject.AddComponent<PressurePlateTimer>();
            }
            else Destroy(pressure);
#if DEBUG
            if (sender != null) Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>ptt> {Config._pressurePlateTimer.Definition.Key} updated to {Config.PressurePlateTimer}</style>" });
#endif
        }

        private void ConfigureDrop(bool shouldHook)
        {
            // todo: configs
            if (shouldHook) Drop.Hook();
            else Drop.Unhook();
        }
    }
}
