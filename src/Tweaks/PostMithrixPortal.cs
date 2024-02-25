using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace PressureDrop
{
    internal static class PostMithrixPortal
    {
        private static bool _hooked = false;

        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += EncounterFinished_OnEnter;
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= EncounterFinished_OnEnter;
        }

        private static void EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            InstantiatePortal(new Vector3(-87.5f, 492f, 5.2f), Quaternion.identity);
        }

        internal static void InstantiatePortal(Vector3 position, Quaternion rotation)
        {
            InteractableSpawnCard isc = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/VoidOutroPortal/iscVoidOutroPortal.asset").WaitForCompletion();

            // RoR2.InteractableSpawnCard.Spawn()
            GameObject gameObject = Object.Instantiate(isc.prefab, position, rotation);
            NetworkServer.Spawn(gameObject);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cWorldEvent>The void beckons..</style>" });
        }
    }
}
