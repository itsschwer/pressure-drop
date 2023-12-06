using BepInEx.Configuration;

namespace PressureDrop
{
    internal class Config
    {
        /// <summary>
        /// Only marked public to expose the SettingChanged event. Use the accessor instead where possible.
        /// </summary>
        public readonly ConfigEntry<float> _pressurePlateTimer;
        /// <summary>
        /// The length of time (seconds) a pressure plate will remain pressed after being activated.
        /// </summary>
        public float PressurePlateTimer => _pressurePlateTimer.Value;


        // Drop Recyclable
        private readonly ConfigEntry<bool> _dropRecyclableWhite;
        private readonly ConfigEntry<bool> _dropRecyclableGreen;
        private readonly ConfigEntry<bool> _dropRecyclableRed;
        private readonly ConfigEntry<bool> _dropRecyclableYellow;
        private readonly ConfigEntry<bool> _dropRecyclableLunar;
        private readonly ConfigEntry<bool> _dropRecyclableVoid;
        private readonly ConfigEntry<bool> _dropRecyclableEquipment;
        private readonly ConfigEntry<bool> _dropRecyclableEquipmentLunar;
        // Accessors
        public bool DropRecyclableWhite => _dropRecyclableWhite.Value;
        public bool DropRecyclableGreen => _dropRecyclableGreen.Value;
        public bool DropRecyclableRed => _dropRecyclableRed.Value;
        public bool DropRecyclableYellow => _dropRecyclableYellow.Value;
        public bool DropRecyclableLunar => _dropRecyclableLunar.Value;
        public bool DropRecyclableVoid => _dropRecyclableVoid.Value;
        public bool DropRecyclableEquipment => _dropRecyclableEquipment.Value;
        public bool DropRecyclableEquipmentLunar => _dropRecyclableEquipmentLunar.Value;

        public Config(ConfigFile config)
        {
            // Prefix with underscore to sort at top in config editor
            _pressurePlateTimer = config.Bind<float>("_Timed Pressure Plates", "pressurePlateTimer", 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nSpecial cases: negative values are permanent, 0 to disable.");


            // Drop Recyclable
            _dropRecyclableWhite = config.Bind<bool>("Drop Recyclable", "dropRecyclableWhite", true,
                "Whether dropped white tier items should be recyclable.");
            _dropRecyclableGreen = config.Bind<bool>("Drop Recyclable", "dropRecyclableGreen", true,
                "Whether dropped green tier items should be recyclable.");
            _dropRecyclableRed = config.Bind<bool>("Drop Recyclable", "dropRecyclableRed", false,
                "Whether dropped red tier items should be recyclable.");
            _dropRecyclableYellow = config.Bind<bool>("Drop Recyclable", "dropRecyclableYellow", false,
                "Whether dropped yellow tier items should be recyclable.");
            _dropRecyclableLunar = config.Bind<bool>("Drop Recyclable", "dropRecyclableLunar", false,
                "Whether dropped lunar items should be recyclable.");
            _dropRecyclableVoid = config.Bind<bool>("Drop Recyclable", "dropRecyclableVoid", false,
                "Whether dropped void items should be recyclable.");
            _dropRecyclableEquipment = config.Bind<bool>("Drop Recyclable", "dropRecyclableEquipment", true,
                "Whether dropped equipment should be recyclable.");
            _dropRecyclableEquipmentLunar = config.Bind<bool>("Drop Recyclable", "dropRecyclableEquipmentLunar", false,
                "Whether dropped lunar equipment should be recyclable.");
        }
    }
}
