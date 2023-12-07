using BepInEx.Configuration;

namespace PressureDrop
{
    internal class Config
    {
        private readonly ConfigEntry<float> _pressurePlateTimer;
        /// <summary>
        /// The length of time (seconds) a pressure plate will remain pressed after being activated.
        /// </summary>
        public float PressurePlateTimer => _pressurePlateTimer.Value;


        // Drop Command
        private readonly ConfigEntry<bool> _dropEnabled;
        private readonly ConfigEntry<int> _maxItemsToDropAtATime;
        // Accessors
        public bool DropEnabled => _dropEnabled.Value;
        public int MaxItemsToDropAtATime => _maxItemsToDropAtATime.Value > 0 ? _maxItemsToDropAtATime.Value : 1;


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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE0290")] // Use primary constructor
        public Config(ConfigFile config)
        {
            // Prefix section to sort to top in config editor
            _pressurePlateTimer = config.Bind<float>("> Timed Pressure Plates", "pressurePlateTimer", 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nZero disables time functionality (reverts to vanilla behaviour).\nNegative values prevent pressure plates from releasing once activated.");


            // Drop Command
            _dropEnabled = config.Bind<bool>("Drop Command", "dropEnabled", true,
                "TODO");
            _maxItemsToDropAtATime = config.Bind<int>("Drop Command", "maxItemsToDropAtATime", 10,
                "The maximum amount of items to drop from the player at a time (similar to Scrappers).\nMinimum value is 1.");


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
