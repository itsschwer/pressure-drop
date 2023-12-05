using BepInEx.Configuration;

namespace PressureDrop
{
    internal class Config
    {
        /// <summary>
        /// Only public to expose the SettingChanged event. Use the accessor instead where possible.
        /// </summary>
        public readonly ConfigEntry<float> _pressurePlateTimer;
        /// <summary>
        /// The length of time (seconds) a pressure plate will remain pressed after being activated.
        /// </summary>
        public float PressurePlateTimer => _pressurePlateTimer.Value;

        public Config(ConfigFile config)
        {
            _pressurePlateTimer = config.Bind<float>("Timed Pressure Plates", "pressurePlateTimer", 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nSpecial cases: negative values are permanent, 0 to disable.");
        }
    }
}
