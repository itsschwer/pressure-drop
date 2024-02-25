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
                Log.Debug($"Dropship> pos: {self.transform.position} | rot: {self.transform.rotation} | euler: {self.transform.rotation.eulerAngles} | up: {self.transform.up} | forward: {self.transform.forward} | right: {self.transform.right}");
                hold = self;
#endif
                Vector3 position = self.transform.position;
                position += (self.transform.up * 10f);
                position += (self.transform.forward * Plugin.Config.PortalOffsetForward);
                position += (self.transform.right * Plugin.Config.PortalOffsetRight);
                Quaternion rotation = Quaternion.Inverse(self.transform.rotation) * Quaternion.Euler(self.transform.up * 90f);
                InstantiatePortal(position, rotation);
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
            portal = gameObject;
#endif
            NetworkServer.Spawn(gameObject);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsVoid>The Void beckons...</style>" });
        }

#if DEBUG
        private static HoldoutZoneController hold;
        private static GameObject portal;
        internal static void Reposition()
        {
            if (portal == null) return;
            if (hold == null) return;

            Object.Destroy(portal);

            Vector3 pos = hold.transform.position;
            pos += (hold.transform.up * 10f);
            pos += (hold.transform.forward * Plugin.Config.PortalOffsetForward);
            pos += (hold.transform.right * Plugin.Config.PortalOffsetRight);
            InstantiatePortal(pos, Quaternion.Inverse(hold.transform.rotation) * Quaternion.Euler(hold.transform.up * 90f));
        }
#endif
    }
}
