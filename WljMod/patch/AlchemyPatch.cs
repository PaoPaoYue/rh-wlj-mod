using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg.element;
using cfg;

namespace WljMod;

static class AlchemyPatch
{
    static bool CanAlchemyCombine(ElementEntity e1, ElementEntity e2)
    {
        int elementMaxLevel = Singleton<Model>.Instance.Element.GetElementMaxLevel(EEntityType.Player);
        if (e1 == null || e2 == null || !e1.Fill || !e2.Fill || e1.Level >= elementMaxLevel || e2.Level >= elementMaxLevel)
            return false;
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
    
    static bool CanAlchemyCombineChooseItem(ElementEntity e, Element ec)
    {
        if (e == null || !e.Fill)
            return false;
        var ec1 = Singleton<Model>.Instance.Element.GetElementConf(e.ID);
        var ec2 = ec;
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
    static void CombinePostfix(ElementEntity __instance, ElementEntity rElement, ref int __state)
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
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel"))
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
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel"))
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
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel"))
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
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ElementModel), "GetElementMaxLevel"))
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

    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropChooseItemToPrepare")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropChooseItemToPrepareTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Element), "Id")),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Entity), "ID"))
            )
            .Advance(4)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e, Element ec) =>
                {
                    return !CanAlchemyCombineChooseItem(e, ec);
                }),

                new CodeInstruction(OpCodes.And)
            )
            .SetOpcodeAndAdvance(OpCodes.Brtrue)
            .InstructionEnumeration();
    }

    
    [HarmonyPatch(typeof(ElementModel.ElementInteractive), "DropChooseItemToLoop")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> DropChooseItemToLoopTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Element), "Id")),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Entity), "ID"))
            )
            .Advance(4)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate((ElementEntity e, Element ec) =>
                {
                    return !CanAlchemyCombineChooseItem(e, ec);
                }),

                new CodeInstruction(OpCodes.And)
            )
            .SetOpcodeAndAdvance(OpCodes.Brtrue)
            .InstructionEnumeration();
    }

}