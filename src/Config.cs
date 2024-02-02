using BepInEx.Configuration;

namespace PressureDrop
{
    public sealed class Config
    {
        private readonly ConfigEntry<float> pressurePlateTimer;
        public float PressurePlateTimer => pressurePlateTimer.Value;


        // Drop Command
        private readonly ConfigEntry<bool> dropEnabled;
        private readonly ConfigEntry<bool> dropTeleporterEnabled;
        private readonly ConfigEntry<bool> dropDeadEnabled;
        private readonly ConfigEntry<bool> dropInvertDirection;
        private readonly ConfigEntry<bool> dropVoidAllowed;
        private readonly ConfigEntry<int> maxItemsToDropAtATime;
        // Accessors
        public bool DropEnabled => dropEnabled.Value;
        public bool DropTeleporterEnabled => dropTeleporterEnabled.Value;
        public bool DropDeadEnabled => dropDeadEnabled.Value;
        public bool DropInvertDirection => dropInvertDirection.Value;
        public bool DropVoidAllowed => dropVoidAllowed.Value;
        public int MaxItemsToDropAtATime => maxItemsToDropAtATime.Value > 0 ? maxItemsToDropAtATime.Value : 1;


        // Drop Recyclable
        private readonly ConfigEntry<bool> dropRecyclableWhite;
        private readonly ConfigEntry<bool> dropRecyclableGreen;
        private readonly ConfigEntry<bool> dropRecyclableRed;
        private readonly ConfigEntry<bool> dropRecyclableYellow;
        private readonly ConfigEntry<bool> dropRecyclableLunar;
        private readonly ConfigEntry<bool> dropRecyclableVoid;
        private readonly ConfigEntry<bool> dropRecyclableEquipment;
        private readonly ConfigEntry<bool> dropRecyclableEquipmentLunar;
        // Accessors
        public bool DropRecyclableWhite => dropRecyclableWhite.Value;
        public bool DropRecyclableGreen => dropRecyclableGreen.Value;
        public bool DropRecyclableRed => dropRecyclableRed.Value;
        public bool DropRecyclableYellow => dropRecyclableYellow.Value;
        public bool DropRecyclableLunar => dropRecyclableLunar.Value;
        public bool DropRecyclableVoid => dropRecyclableVoid.Value;
        public bool DropRecyclableEquipment => dropRecyclableEquipment.Value;
        public bool DropRecyclableEquipmentLunar => dropRecyclableEquipmentLunar.Value;


        // Tweaks
        private readonly ConfigEntry<bool> voidPickupConfirmAll;
        private readonly ConfigEntry<bool> voidFieldFogAltStart;
        // Accessors
        public bool VoidPickupConfirmAll => voidPickupConfirmAll.Value;
        public bool VoidFieldFogAltStart => voidFieldFogAltStart.Value;


        public Config(ConfigFile config)
        {
            // Prefix section to sort to top in config editor
            pressurePlateTimer = config.Bind<float>("> Timed Pressure Plates", nameof(pressurePlateTimer), 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nZero disables time functionality (reverts to vanilla behaviour). Negative values prevent pressure plates from releasing once activated.");


            const string DropCommand = "Drop Command";
            dropEnabled = config.Bind<bool>(DropCommand, nameof(dropEnabled), true,
                "Whether the /drop command should be enabled or not.");
            dropTeleporterEnabled = config.Bind<bool>(DropCommand, nameof(dropTeleporterEnabled), true,
                "Whether players should be able to send their drops to the Teleporter or not.");
            dropDeadEnabled = config.Bind<bool>(DropCommand, nameof(dropDeadEnabled), true,
                "Whether dead players should be able to drop items or not.\nRequires dropTeleporterEnabled to be true.");
            dropInvertDirection = config.Bind<bool>(DropCommand, nameof(dropInvertDirection), false,
                "Whether items should be dropped opposite the aim direction or not.");
            dropVoidAllowed = config.Bind<bool>(DropCommand, nameof(dropVoidAllowed), false,
                "Whether void items are allowed to be dropped or not.");
            maxItemsToDropAtATime = config.Bind<int>(DropCommand, nameof(maxItemsToDropAtATime), 10,
                "The maximum amount of items to drop from the player at a time (similar to Scrappers).\nMinimum value is 1.");


            const string DropRecyclable = "Drop Recyclable";
            dropRecyclableWhite = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableWhite), true,
                "Whether dropped white tier items should be recyclable.");
            dropRecyclableGreen = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableGreen), true,
                "Whether dropped green tier items should be recyclable.");
            dropRecyclableRed = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableRed), false,
                "Whether dropped red tier items should be recyclable.");
            dropRecyclableYellow = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableYellow), false,
                "Whether dropped yellow tier items should be recyclable.");
            dropRecyclableLunar = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableLunar), false,
                "Whether dropped lunar items should be recyclable.");
            dropRecyclableVoid = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableVoid), false,
                "Whether dropped void items should be recyclable.");
            dropRecyclableEquipment = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableEquipment), true,
                "Whether dropped equipment should be recyclable.");
            dropRecyclableEquipmentLunar = config.Bind<bool>(DropRecyclable, nameof(dropRecyclableEquipmentLunar), false,
                "Whether dropped lunar equipment should be recyclable.");


            const string Tweaks = "Tweaks";
            voidPickupConfirmAll = config.Bind<bool>(Tweaks, nameof(voidPickupConfirmAll), false,
                "Whether void items always require confirmation before being picked up.");
            voidFieldFogAltStart = config.Bind<bool>(Tweaks, nameof(voidFieldFogAltStart), false,
                "Whether the fog in the Void Fields should only become active once a Cell Vent has been activated.");
        }
    }
}
