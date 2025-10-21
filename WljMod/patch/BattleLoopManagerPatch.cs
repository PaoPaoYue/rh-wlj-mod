using HarmonyLib;
using Game;
using System.Collections.Generic;

namespace WljMod;

static class BattleLoopManagerPatch
{
    [HarmonyPatch(typeof(BattleLoopManager), "CreateRandomTarget")]
    [HarmonyPrefix]
    static void CreateRandomTargetPrefix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Remove(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateRandomTarget")]
    [HarmonyPostfix]
    static void CreateRandomTargetPostHarmonyPostfix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Add(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateSmallLoopTarget")]
    [HarmonyPrefix]
    static void CreateSmallLoopTargetPrefix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Remove(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateSmallLoopTarget")]
    [HarmonyPostfix]
    static void CreateSmallLoopTargetPostHarmonyPostfix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Add(i);
            }
        }
    }

}