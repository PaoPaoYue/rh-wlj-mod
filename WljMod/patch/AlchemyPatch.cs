using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg.element;

namespace WljMod;

static class AlchemyPatch
{
    static bool CanAlchemyCombine(ElementEntity e1, ElementEntity e2)
    {
        var ec1 = Singleton<Model>.Instance.Element.GetElementConf(e1.ID);
        var ec2 = Singleton<Model>.Instance.Element.GetElementConf(e2.ID);
        if (ec1.RaceType == ERaceType.Mod2)
        {
            var relicAttrId = Plugin.Register.GetRelicGlobalValueId((int)Plugin.RelicGlobalValue.Alchemy);
            if (Singleton<Model>.Instance.Relic.GetRelicGlobalValue(relicAttrId, EEntityType.Player) > 0)
            {
                if (ec1.Rare <= ec2.Rare)
                {
                    return true;
                }
            }
        }
        return false;
    }

    [HarmonyPatch(typeof(ElementEntity), "Combine")]
    [HarmonyPrefix]
    static void CombinePrefix(ElementEntity __instance, ElementEntity rElement, ref int __state)
    {
        __state = rElement.ID;
        if (CanAlchemyCombine(__instance, rElement))
        {
            rElement.ID = __instance.ID;
        }
    }

    [HarmonyPatch(typeof(ElementEntity), "Combine")]
    [HarmonyPostfix]
    static void CombinePosHarmonyPostfix(ElementEntity __instance, ElementEntity rElement, ref int __state)
    {
        rElement.ID = __state;
    }

    static int tmpElementId;

    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropLoopItemToPrepare")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropLoopItemToPrepareTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel")),
                new CodeMatch(OpCodes.Add)
            )
            .Advance(2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e1, ElementEntity e2) =>
                {
                    tmpElementId = e2.ID;
                    if (CanAlchemyCombine(e1, e2))
                    {
                        e2.ID = e1.ID;
                    }
                })
            )
            .Advance(13)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e2) =>
                {
                    e2.ID = tmpElementId;
                })
            )
            .InstructionEnumeration();
    }

    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropLoopItemToLoop")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropLoopItemToLoopTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel")),
                new CodeMatch(OpCodes.Add)
            )
            .Advance(2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e1, ElementEntity e2) =>
                {
                    tmpElementId = e2.ID;
                    if (CanAlchemyCombine(e1, e2))
                    {
                        e2.ID = e1.ID;
                    }
                })
            )
            .Advance(13)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e2) =>
                {
                    e2.ID = tmpElementId;
                })
            )
            .InstructionEnumeration();
    }

    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropPrepareItemToLoop")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropPrepareItemToLoopTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel")),
                new CodeMatch(OpCodes.Add)
            )
            .Advance(2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e1, ElementEntity e2) =>
                {
                    tmpElementId = e2.ID;
                    if (CanAlchemyCombine(e1, e2))
                    {
                        e2.ID = e1.ID;
                    }
                })
            )
            .Advance(13)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e2) =>
                {
                    e2.ID = tmpElementId;
                })
            )
            .InstructionEnumeration();
    }

    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropPrepareItemToPrepare")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropPrepareItemToPrepareTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel")),
                new CodeMatch(OpCodes.Add)
            )
            .Advance(2)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e1, ElementEntity e2) =>
                {
                    tmpElementId = e2.ID;
                    if (CanAlchemyCombine(e1, e2))
                    {
                        e2.ID = e1.ID;
                    }
                })
            )
            .Advance(13)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e2) =>
                {
                    e2.ID = tmpElementId;
                })
            )
            .InstructionEnumeration();
    }

}