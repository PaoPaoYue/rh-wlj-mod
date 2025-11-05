using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg.element;
using System;
using BaseMod;

namespace WljMod;

static class LotteCellPatch
{
    [HarmonyPatch(typeof(LotteCell), "UpdateData")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> UpdateDataTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Stloc_2)
            )
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                Transpilers.EmitDelegate(SubIconPatch),
                new CodeInstruction(OpCodes.Stloc_2)
            )
            .InstructionEnumeration();
    }

    static string SubIconPatch(ElementEntity elementData, string originIcon)
    {
        var modModel = Singleton<Model>.Instance.Mod;
        if (modModel == null || modModel.mModData == null || modModel.mModData.ModName.RStrip("(debug)") != Plugin.ModName)
            return originIcon;
        var subIconAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.SubIcon);
        int subIconIndex = elementData.GetAttribute(subIconAttrId);
        if (subIconIndex > 0)
        {
            return $"wlj_element_20_{subIconIndex}";
        }
        if ((elementData.ID == 10010 || elementData.ID == 10011) && elementData.Level > 1)
        {
            int level = Math.Min(elementData.Level, 3);
            return $"wlj_element_{elementData.ID - 10000}_{level - 1}";
        }
        return originIcon;
    }
}