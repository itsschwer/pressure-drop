using BepInEx;
using RoR2;

namespace PressureDrop
{
    [BepInPlugin(GUID, Name, Version)]

    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "PressureDrop";
        public const string Version = "1.0.2";

        internal static new Config Config { get; private set; }

        // MonoBehaviour components
        private PressurePlateTimer pressure;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);
            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();
            Log.Message($"{Plugin.GUID}> awake.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            ConfigureModules();
            ChatCommander.Subscribe();
            ChatCommander.OnChatCommand += ParseReload;
#if DEBUG
            Commands.DebugCheats.Enable();
#endif
            Log.Message($"{Plugin.GUID}> enabled.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            ConfigureModules();
            ChatCommander.Unsubscribe();
            ChatCommander.OnChatCommand -= ParseReload;
#if DEBUG
            Commands.DebugCheats.Disable();
#endif
            Log.Message($"{Plugin.GUID}> disabled.");
        }

        /// <summary>
        /// Wrapper for SetActive(bool), passing in NetworkServer.active,
        /// which appears to be used for determining if client is host.
        /// </summary>
        private void SetPluginActiveState(Run run = null) => SetActive(UnityEngine.Networking.NetworkServer.active);

        /// <summary>
        /// All plugins are attached to the same
        /// <see href="https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Bootstrap/Chainloader.cs#L88">GameObject</see>,
        /// so manually manage components instead of calling
        /// this.gameObject.SetActive(bool).
        /// </summary>
        private void SetActive(bool value) {
            this.enabled = value;
            if (this.pressure) this.pressure.enabled = value;
            Log.Message($"{Plugin.GUID}> {(value ? "active" : "inactive")}.");
        }

        private void ParseReload(NetworkUser user, string[] args)
        {
            if (args.Length != 1 || args[0].ToLowerInvariant() != "reload") return; // Only accept command with no additional args
            if (user != LocalUserManager.GetFirstLocalUser().currentNetworkUser) return; // Only allow host to reload

            base.Config.Reload();
            ConfigureModules();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"Reloaded configuration for <style=cSub>{Plugin.GUID}</style>" });
        }

        private void ConfigureModules()
        {
            ConfigurePressurePlateTimerComponent();
            ConfigureDropModule();
        }

        private void ConfigurePressurePlateTimerComponent()
        {
            if (Config.PressurePlateTimer != 0 && this.enabled) {
                if (!pressure) pressure = this.gameObject.AddComponent<PressurePlateTimer>();
            }
            else Destroy(pressure);
        }

        private void ConfigureDropModule()
        {
            Drop.Unhook();
            if (Config.DropEnabled && this.enabled) Drop.Hook();
        }
    }
}
