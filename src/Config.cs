using BepInEx.Configuration;

namespace PressureDrop
{
    public sealed class Config
    {
        private readonly ConfigEntry<float> _pressurePlateTimer;
        public float PressurePlateTimer => _pressurePlateTimer.Value;


        // Drop Command
        private readonly ConfigEntry<bool> _dropEnabled;
        private readonly ConfigEntry<bool> _dropTeleporterEnabled;
        private readonly ConfigEntry<bool> _dropDeadEnabled;
        private readonly ConfigEntry<bool> _dropInvertDirection;
        private readonly ConfigEntry<bool> _dropVoidAllowed;
        private readonly ConfigEntry<int> _maxItemsToDropAtATime;
        // Accessors
        public bool DropEnabled => _dropEnabled.Value;
        public bool DropTeleporterEnabled => _dropTeleporterEnabled.Value;
        public bool DropDeadEnabled => _dropDeadEnabled.Value;
        public bool DropInvertDirection => _dropInvertDirection.Value;
        public bool DropVoidAllowed => _dropVoidAllowed.Value;
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


        // Tweaks
        private readonly ConfigEntry<bool> _voidPickupConfirmAll;
        private readonly ConfigEntry<bool> _voidFieldFogAltStart;
        // Accessors
        public bool VoidPickupConfirmAll => _voidPickupConfirmAll.Value;
        public bool VoidFieldFogAltStart => _voidFieldFogAltStart.Value;


        public Config(ConfigFile config)
        {
            // Prefix section to sort to top in config editor
            _pressurePlateTimer = config.Bind<float>("> Timed Pressure Plates", "pressurePlateTimer", 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nZero disables time functionality (reverts to vanilla behaviour). Negative values prevent pressure plates from releasing once activated.");


            const string DropCommand = "Drop Command";
            _dropEnabled = config.Bind<bool>(DropCommand, "dropEnabled", true,
                "Whether the /drop command should be enabled or not.");
            _dropTeleporterEnabled = config.Bind<bool>(DropCommand, "dropTeleporterEnabled", true,
                "Whether players should be able to send their drops to the Teleporter or not.");
            _dropDeadEnabled = config.Bind<bool>(DropCommand, "dropDeadEnabled", true,
                "Whether dead players should be able to drop items or not.\nRequires dropTeleporterEnabled to be true.");
            _dropInvertDirection = config.Bind<bool>(DropCommand, "dropInvertDirection", false,
                "Whether items should be dropped opposite the aim direction or not.");
            _dropVoidAllowed = config.Bind<bool>(DropCommand, "dropVoidAllowed", false,
                "Whether void items are allowed to be dropped or not.");
            _maxItemsToDropAtATime = config.Bind<int>(DropCommand, "maxItemsToDropAtATime", 10,
                "The maximum amount of items to drop from the player at a time (similar to Scrappers).\nMinimum value is 1.");


            const string DropRecyclable = "Drop Recyclable";
            _dropRecyclableWhite = config.Bind<bool>(DropRecyclable, "dropRecyclableWhite", true,
                "Whether dropped white tier items should be recyclable.");
            _dropRecyclableGreen = config.Bind<bool>(DropRecyclable, "dropRecyclableGreen", true,
                "Whether dropped green tier items should be recyclable.");
            _dropRecyclableRed = config.Bind<bool>(DropRecyclable, "dropRecyclableRed", false,
                "Whether dropped red tier items should be recyclable.");
            _dropRecyclableYellow = config.Bind<bool>(DropRecyclable, "dropRecyclableYellow", false,
                "Whether dropped yellow tier items should be recyclable.");
            _dropRecyclableLunar = config.Bind<bool>(DropRecyclable, "dropRecyclableLunar", false,
                "Whether dropped lunar items should be recyclable.");
            _dropRecyclableVoid = config.Bind<bool>(DropRecyclable, "dropRecyclableVoid", false,
                "Whether dropped void items should be recyclable.");
            _dropRecyclableEquipment = config.Bind<bool>(DropRecyclable, "dropRecyclableEquipment", true,
                "Whether dropped equipment should be recyclable.");
            _dropRecyclableEquipmentLunar = config.Bind<bool>(DropRecyclable, "dropRecyclableEquipmentLunar", false,
                "Whether dropped lunar equipment should be recyclable.");


            const string Tweaks = "Tweaks";
            _voidPickupConfirmAll = config.Bind<bool>(Tweaks, "voidPickupConfirmAll", false,
                "Whether void items always need to picked up intentionally via interact prompt.");
            _voidFieldFogAltStart = config.Bind<bool>(Tweaks, "voidFieldFogAltStart", false,
                "Whether the Void Fog should only become active once a Cell Vent has been activated.");
        }
    }
}
