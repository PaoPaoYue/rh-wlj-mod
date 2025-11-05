using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WljMod;

static class BattleLoopManagerPatch
{
    [HarmonyPatch(typeof(BattleLoopManager), "CreateRandomTarget")]
    [HarmonyPrefix]
    static void CreateRandomTargetPrefix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        if (AllowTired())
            return;
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.Enable && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Remove(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateRandomTarget")]
    [HarmonyPostfix]
    static void CreateRandomTargetPostHarmonyPostfix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        if (AllowTired())
            return;
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.Enable && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Add(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateSmallLoopTarget")]
    [HarmonyPrefix]
    static void CreateSmallLoopTargetPrefix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        if (AllowTired())
            return;
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.Enable && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Remove(i);
            }
        }
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateSmallLoopTarget")]
    [HarmonyPostfix]
    static void CreateSmallLoopTargetPostHarmonyPostfix(BattleLoopManager __instance, List<int> ___LoopIndexList)
    {
        if (AllowTired())
            return;
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, __instance.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.Enable && tempElement.GetAttribute(tiredAttrId) > 0)
            {
                ___LoopIndexList.Add(i);
            }
        }
    }

    static bool AllowTired()
    {
        var relicAttrId = Plugin.Register.GetRelicGlobalValueId((int)Plugin.RelicGlobalValue.Fire);
        return Singleton<Model>.Instance.Relic.GetRelicGlobalValue(relicAttrId, EEntityType.Player) > 0;
    }

    [HarmonyPatch(typeof(BattleLoopManager), "CreateRandomTarget")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> CreateRandomTargetTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(BattleLoopManager), "get_AddTargetCount")),
                new CodeMatch(OpCodes.Add)
            )
            .Advance(2)
            .InsertAndAdvance(
                Transpilers.EmitDelegate(() =>
                {
                    int nValue = 0;
                    var relicAttrId = Plugin.Register.GetRelicGlobalValueId((int)Plugin.RelicGlobalValue.Fire);
                    if (Singleton<Model>.Instance.Relic.GetRelicGlobalValue(relicAttrId, EEntityType.Player) > 0)
                    {
                        int attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
                        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
                        {
                            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, EEntityType.Player);
                            if (tempElement == null || !tempElement.Fill)
                                continue;
                            nValue += tempElement.GetAttribute(attrId) > 0 ? 1 : 0;
                        }
                    }
                    return nValue;
                }),
                new CodeInstruction(OpCodes.Add)
            )
            .InstructionEnumeration();
    }

}