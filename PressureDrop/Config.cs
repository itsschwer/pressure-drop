using BepInEx.Configuration;

namespace PressureDrop
{
    internal class Config
    {
        private readonly ConfigEntry<float> _pressurePlateTimer;
        public float PressurePlateTimer => _pressurePlateTimer.Value;

        public Config(ConfigFile config)
        {
            _pressurePlateTimer = config.Bind<float>("Timed Pressure Plates", "pressurePlateTimer", 30f,
                "The amount of time a pressure plate will remain pressed after being activated.\nSpecial cases: negative values are permanent, 0 to disable");
        }
    }
}
