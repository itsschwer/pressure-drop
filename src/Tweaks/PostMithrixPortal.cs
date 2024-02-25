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

            On.RoR2.HoldoutZoneController.Start += HoldoutZoneController_Start;
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.RoR2.HoldoutZoneController.Start -= HoldoutZoneController_Start;
        }

        private static void HoldoutZoneController_Start(On.RoR2.HoldoutZoneController.orig_Start orig, HoldoutZoneController self)
        {
            orig(self);

            if (self.inBoundsObjectiveToken == "OBJECTIVE_MOON_CHARGE_DROPSHIP") {
#if DEBUG
                Log.Debug($"Dropship> pos: {self.transform.position} | rot: {self.transform.rotation} | fwd: {self.transform.forward} | up: {self.transform.up} ");
#endif
                InstantiatePortal(new Vector3(306.5f, -172.2f, 391.8f), new Quaternion(0.0f, 1.0f, 0.0f, -0.2f));
            }
        }

        internal static void InstantiatePortal(Vector3 position, Quaternion rotation)
        {
            InteractableSpawnCard isc = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/iscInfiniteTowerPortal.asset").WaitForCompletion();

            // RoR2.InteractableSpawnCard.Spawn() & RoR2.ArtifactTrialMissionController.SpawnExitPortalAndIdle.OnEnter()
            GameObject gameObject = Object.Instantiate(isc.prefab, position, rotation);
            gameObject.GetComponent<SceneExitController>().useRunNextStageScene = true;
#if DEBUG
            Log.Debug($"{nameof(PostMithrixPortal)}> pos: {position} | rot: {rotation}");
#endif
            NetworkServer.Spawn(gameObject);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsVoid>The Void beckons...</style>" });
        }
    }
}
