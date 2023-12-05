using BepInEx;

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

        internal static new Config Config { get; private set; }

        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);
            Config._pressurePlateTimer.SettingChanged += SetPressurePlateTimer;
        }

        private void OnEnable()
        {
            SetPressurePlateTimer();
#if DEBUG
            ChatCommander.Subscribe();
            DebugCheats.Enable();
#endif
        }

        private void OnDisable()
        {
#if DEBUG
            ChatCommander.Unsubscribe();
            DebugCheats.Disable();
#endif
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
