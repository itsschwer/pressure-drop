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
        public const string Version = "1.1.0";

        public static new Config Config { get; private set; }

        // MonoBehaviour components
        private PressurePlateTimer pressure;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);
            ChatCommander.Hook();
            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();
            Log.Message("~awake.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            ConfigureModules();
            ChatCommander.Register("/reload", ReloadConfig, true);
#if DEBUG
            Commands.DebugCheats.Enable();
#endif
            Log.Message("~enabled.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            ConfigureModules();
            ChatCommander.Unregister("/reload", ReloadConfig);
#if DEBUG
            Commands.DebugCheats.Disable();
#endif
            Log.Message("~ disabled.");
        }

        /// <summary>
        /// Wrapper for <see cref="SetActive"/>, passing in <see cref="UnityEngine.Networking.NetworkServer.active"/>,
        /// which appears to be used for determining if client is host.
        /// </summary>
        private void SetPluginActiveState(Run run = null) => SetActive(UnityEngine.Networking.NetworkServer.active);

        /// <summary>
        /// All plugins are attached to the
        /// <see href="https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Bootstrap/Chainloader.cs#L88">same</see>
        /// <see cref="UnityEngine.GameObject"/>, so manually manage components instead of calling <see cref="UnityEngine.GameObject.SetActive"/>.
        /// </summary>
        private void SetActive(bool value) {
            this.enabled = value;
            if (this.pressure) this.pressure.enabled = value;
            Log.Message($"~{(value ? "active" : "inactive")}.");
        }

        private void ReloadConfig(NetworkUser user, string[] args)
        {
            if (user != LocalUserManager.GetFirstLocalUser().currentNetworkUser) return; // Only allow host to reload

            base.Config.Reload();
            ConfigureModules();
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"Reloaded configuration for <style=cWorldEvent>{Plugin.GUID}</style>" });
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
