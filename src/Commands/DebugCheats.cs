#if DEBUG
using RoR2;
using UnityEngine;

namespace PressureDrop.Commands
{
    internal static class DebugCheats
    {
        public static void  Enable()
        {
            ChatCommander.Register("/?", Help);
            ChatCommander.Register("/p", PickupCommand);
            ChatCommander.Register("/dstress", StressTest);
        }
        public static void Disable()
        {
            ChatCommander.Unregister("/?", Help);
            ChatCommander.Unregister("/p", PickupCommand);
            ChatCommander.Unregister("/dstress", StressTest);
        }

        private static void Help(NetworkUser user, string[] args)
        {
            ChatCommander.Output($"<style=cWorldEvent>{Plugin.GUID}</style> debug cheats:");
            ChatCommander.Output($"  <style=cSub>/p</style>: drops items for testing recyclability config.");
        }

        private static void StressTest(NetworkUser user, string[] args)
        {
            Transform target = user.GetCurrentBody().gameObject.transform;
            // Will freeze the game when the droplets are created
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Drop.DropStyleChest(target, PickupIndex.none, 10000);
            sw.Stop();
            Plugin.Logger.LogInfo(sw.Elapsed);
        }

        private static void PickupCommand(NetworkUser user, string[] args)
        {
            const string invalid = "{ <style=cSub>t</style> | <style=cSub>v</style> | <style=cSub>s</style> }";

            const int expectedArgs = 2;
            if (args.Length == expectedArgs) {
                Transform target = user.GetCurrentBody().gameObject.transform;

                switch (args[1].ToLowerInvariant()) {
                    default:
                        ChatCommander.OutputFail(args[0], invalid);
                        return;
                    case "t":
                        Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintBonus.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.EquipmentMagazine.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLife.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.SprintWisp.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Recycle.equipmentIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Equipment.Tonic.equipmentIndex),
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped test pickups @ {user.userName}.");
                        break;
                    case "v":
                        Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.BearVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.MissileVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.ExtraLifeVoid.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.VoidMegaCrabItem.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped void pickups @ {user.userName}.");
                        break;
                    case "s":
                        Drop.DropStyleChest(target, [
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLifeConsumed.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.TonicAffliction.itemIndex),
                            PickupCatalog.FindPickupIndex(DLC1Content.Items.HealingPotionConsumed.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.CaptainDefenseMatrix.itemIndex),
                            PickupCatalog.FindPickupIndex(RoR2Content.Items.DrizzlePlayerHelper.itemIndex)
                        ], 3.4f, 14f);
                        ChatCommander.Output($"Dropped special pickups @ {user.userName}.");
                        break;
                }
            }
            else ChatCommander.OutputFail(args[0], invalid);
        }
    }
}
#endif
