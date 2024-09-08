using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace PressureDrop.Tweaks
{
    internal static class SendItemCostInChat
    {
        private static bool _hooked = false;

        internal static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.RoR2.CostTypeDef.PayCost += CostTypeDef_PayCost;
        }

        internal static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.RoR2.CostTypeDef.PayCost -= CostTypeDef_PayCost;
        }

        private static CostTypeDef.PayCostResults CostTypeDef_PayCost(On.RoR2.CostTypeDef.orig_PayCost orig, CostTypeDef self, int cost, Interactor activator, UnityEngine.GameObject purchasedObject, Xoroshiro128Plus rng, ItemIndex avoidedItemIndex)
        {
            CostTypeDef.PayCostResults __result = orig(self, cost, activator, purchasedObject, rng, avoidedItemIndex);

            try {
                CostTypeDef_PayCost(__result, activator, purchasedObject);
            }
            catch (System.Exception e) {
                Plugin.Logger.LogError(e);
            }

            return __result;
        }

        private static void CostTypeDef_PayCost(CostTypeDef.PayCostResults __result, Interactor activator, UnityEngine.GameObject purchasedObject)
        {
            PurchaseInteraction purchase = purchasedObject?.GetComponent<PurchaseInteraction>();
            if (purchase != null) return;
            string action = GetPurchaseInteractionVerb(purchase);
            if (string.IsNullOrEmpty(action)) return;

            NetworkUser user = activator?.GetComponent<CharacterBody>()?.master?.playerCharacterMasterController?.networkUser;
            if (user == null) return;

            Dictionary<PickupDef, int> exchanged = [];
            // RoR2.PurchaseInteraction.OnInteractionBegin()
            foreach (ItemIndex item in __result.itemsTaken)
            {
                if (IsScrap(item)) continue;

                PickupDef def = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(item));
                if (!exchanged.ContainsKey(def)) exchanged[def] = 0;
                exchanged[def]++;
            }
            foreach (EquipmentIndex item in __result.equipmentTaken)
            {
                PickupDef def = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(item));
                if (!exchanged.ContainsKey(def)) exchanged[def] = 0;
                exchanged[def]++;
            }

            AnnounceExchangedItems(exchanged, user, action);
        }

        private static void AnnounceExchangedItems(Dictionary<PickupDef, int> exchanged, NetworkUser user, string action)
        {
            if (exchanged.Count <= 0) return;

            System.Text.StringBuilder items = new();
            List<PickupDef> keys = exchanged.Keys.ToList();
            for (int i = 0; i < keys.Count; i++) {
                PickupDef item = keys[i];
                items.Append(ChatCommander.GetColoredPickupLanguageString(item));
                if (exchanged[item] != 1) items.Append($"({exchanged[item]})");

                int remaining = keys.Count - i;
                if (remaining > 2) items.Append(", ");
                else if (remaining > 1) items.Append((keys.Count > 2) ? ", and " : " and ");
            }

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cEvent>{user.userName} {action} {items}</color>" });
        }

        private static bool IsScrap(ItemIndex itemIndex)
        {
            return itemIndex == RoR2Content.Items.ScrapWhite.itemIndex
                || itemIndex == RoR2Content.Items.ScrapGreen.itemIndex
                || itemIndex == RoR2Content.Items.ScrapRed.itemIndex
                || itemIndex == RoR2Content.Items.ScrapYellow.itemIndex
                || itemIndex == DLC1Content.Items.RegeneratingScrap.itemIndex;
        }

        private static string GetPurchaseInteractionVerb(PurchaseInteraction purchaseInteraction)
        {
            switch (purchaseInteraction.displayNameToken) {
                default: return "";
                case "BAZAAR_CAULDRON_NAME":
                    return "reforged";
                case "SCRAPPER_NAME":
                    return "scrapped";
                case "SHRINE_CLEANSE_NAME":
                    return "cleansed themself of"; // based on SHRINE_CLEANSE_USE_MESSAGE
                case "DUPLICATOR_NAME":
                case "DUPLICATOR_MILITARY_NAME":
                case "DUPLICATOR_WILD_NAME":
                    return "gave up";
            }
        }
    }
}
