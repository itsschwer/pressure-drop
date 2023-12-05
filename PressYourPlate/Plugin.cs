using BepInEx;

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

        private void Awake() => Log.Init(Logger);

        private void OnEnable()
        {
            // todo: check config before adding
            this.gameObject.AddComponent<TimedPressurePlate>();
#if DEBUG
            ChatCommandUtility.Subscribe();
            ChatCommands.Enable();
#endif
        }

        private void OnDisable()
        {
#if DEBUG
            ChatCommandUtility.Unsubscribe();
            ChatCommands.Disable();
#endif
        }
    }
}
