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
            config.Bind<float>("> Timed Pressure Plates", ref pressurePlateTimer, 30f,
                "The length of time (seconds) a pressure plate will remain pressed after being activated.\nZero disables time functionality (reverts to vanilla behaviour). Negative values prevent pressure plates from releasing once activated.");


            const string DropCommand = "Drop Command";
            config.Bind<bool>(DropCommand, ref dropEnabled, true,
                "Whether the /drop command should be enabled or not.");
            config.Bind<bool>(DropCommand, ref dropTeleporterEnabled, true,
                "Whether players should be able to send their drops to the Teleporter or not.");
            config.Bind<bool>(DropCommand, ref dropDeadEnabled, true,
                "Whether dead players should be able to drop items or not.\nRequires dropTeleporterEnabled to be true.");
            config.Bind<bool>(DropCommand, ref dropInvertDirection, false,
                "Whether items should be dropped opposite the aim direction or not.");
            config.Bind<bool>(DropCommand, ref dropVoidAllowed, false,
                "Whether void items are allowed to be dropped or not.");
            config.Bind<int>(DropCommand, ref maxItemsToDropAtATime, 10,
                "The maximum amount of items to drop from the player at a time (similar to Scrappers).\nMinimum value is 1.");


            const string DropRecyclable = "Drop Recyclable";
            config.Bind<bool>(DropRecyclable, ref dropRecyclableWhite, true,
                "Whether dropped white tier items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableGreen, true,
                "Whether dropped green tier items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableRed, false,
                "Whether dropped red tier items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableYellow, false,
                "Whether dropped yellow tier items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableLunar, false,
                "Whether dropped lunar items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableVoid, false,
                "Whether dropped void items should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableEquipment, true,
                "Whether dropped equipment should be recyclable.");
            config.Bind<bool>(DropRecyclable, ref dropRecyclableEquipmentLunar, false,
                "Whether dropped lunar equipment should be recyclable.");


            const string Tweaks = "Tweaks";
            config.Bind<bool>(Tweaks, ref voidPickupConfirmAll, false,
                "Whether void items always need to picked up intentionally via interact prompt.");
            config.Bind<bool>(Tweaks, ref voidFieldFogAltStart, false,
                "Whether the Void Fog should only become active once a Cell Vent has been activated.");
        }
    }

    internal static class ConfigUtility
    {
        public static void Bind<T>(this ConfigFile config, string section, ref ConfigEntry<T> key, T defaultValue, string description) {
            key = config.Bind(section, nameof(key), defaultValue, description);
        }
    }
}
