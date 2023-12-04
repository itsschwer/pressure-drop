using RoR2;
using UnityEngine;

namespace PressYourPlate
{
    public static class PickupMaker
    {
        public static void Poll()
        {
            if (!Run.instance) return;

            if (Input.GetKeyDown(KeyCode.F3)) {
                Create(PickupCatalog.FindPickupIndex(EquipmentCatalog.FindEquipmentIndex("Scanner")));
                Create(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("AutoCastEquipment")), 7);
            }

            if (Input.GetKeyDown(KeyCode.F4)) Create(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("SprintBonus")), 5);
            if (Input.GetKeyDown(KeyCode.F5)) CreateAt(PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex("Bear")), 70);
        }

        private static void Create(PickupIndex pickupIndex, Vector3 position, Vector3 velocity, int amount = 1)
        {
            for (int i = 0; i < amount; i++) {
                PickupDropletController.CreatePickupDroplet(pickupIndex, position, velocity);
            }

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cUserSetting>> Spawning item(s) at {position}</style>" });
        }

        private static void Create(PickupIndex pickupIndex, int amount = 1)
        {
            // Get the player body to use a position:
            var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

            var position = transform.position + Vector3.up * 1.5f;
            var velocity = Vector3.up * 10f + transform.forward * 2f;

            Create(pickupIndex, position, velocity, amount);
        }

        private static void CreateAt(PickupIndex pickupIndex, int amount = 1)
        {
            var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
            var position = transform.position + Vector3.up * 1.5f;
            Create(pickupIndex, position, Vector3.zero, amount);
        }
    }
}
