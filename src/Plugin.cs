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
        public const string Version = "1.5.1";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public static new Config Config { get; private set; }

        // MonoBehaviour components
        private PressurePlateTimer pressure;

        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Config = new Config(base.Config);

            ChatCommander.Hook();
            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();

            Logger.LogMessage("~awake.");
        }

        private void OnEnable()
        {
            ManageHooks();
            ChatCommander.Register("/reload", ReloadConfig, true);
            Logger.LogMessage("~enabled.");
        }

        private void OnDisable()
        {
            ManageHooks();
            ChatCommander.Unregister("/reload", ReloadConfig);
            Logger.LogMessage("~disabled.");
        }

        /// <summary>
        /// Wrapper for <see cref="SetActive"/>, passing in <see cref="UnityEngine.Networking.NetworkServer.active"/>,
        /// which appears to be used for determining if client is host.
        /// </summary>
        private void SetPluginActiveState(Run _ = null) => SetActive(UnityEngine.Networking.NetworkServer.active);

        /// <summary>
        /// All plugins are attached to the
        /// <see href="https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Bootstrap/Chainloader.cs#L88">same</see>
        /// <see cref="UnityEngine.GameObject"/>, so manually manage components instead of calling <see cref="UnityEngine.GameObject.SetActive"/>.
        /// </summary>
        private void SetActive(bool value) {
            this.enabled = value;
            if (this.pressure) this.pressure.enabled = value;
            Logger.LogMessage($"~{(value ? "active" : "inactive")}.");
        }

        private void ReloadConfig(NetworkUser user, string[] args)
        {
            if (user != LocalUserManager.GetFirstLocalUser().currentNetworkUser) return; // Only allow host to reload

            Config.Reload();
            ManageHooks();
            ChatCommander.Output($"Reloaded configuration for <style=cWorldEvent>{Plugin.GUID}</style>");
        }

        private void ManageHooks()
        {
            ManagePressurePlateTimer();
            ManageDropCommand();
            ManageTweaks();
        }

        private void ManagePressurePlateTimer()
        {
            if (this.enabled && Config.PressurePlateTimer != 0) {
                if (!this.pressure) this.pressure = this.gameObject.AddComponent<PressurePlateTimer>();
            }
            else Destroy(this.pressure);
        }

        private void ManageDropCommand()
        {
            Drop.Unhook();
            if (this.enabled && Config.DropEnabled) Drop.Hook();
        }

        private void ManageTweaks()
        {
            Tweaks.ScoreboardShowChat.Unhook();
            if (this.enabled && Config.ScoreboardShowChat) Tweaks.ScoreboardShowChat.Hook();

            Tweaks.VoidPickupConfirmAll.SetActive(this.enabled && Config.VoidPickupConfirmAll);

            Tweaks.SendItemCostInChat.Unhook();
            if (this.enabled && Config.SendItemCostInChat) Tweaks.SendItemCostInChat.Hook();
        }
    }
}
