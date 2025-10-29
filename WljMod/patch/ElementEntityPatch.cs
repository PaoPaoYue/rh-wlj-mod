using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WljMod;

static class ElementEntityPatch
{
    [HarmonyPatch(typeof(ElementEntity), "Combine")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> CombineTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;
        foreach (var instruction in instructions)
        {
            if (instruction.StoresField(AccessTools.Field(typeof(ElementEntity), "Level")))
            {
                yield return instruction;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return Transpilers.EmitDelegate(CombinePatch);
                found = true;
                continue;
            }
            yield return instruction;
        }
        if (found is false)
            Plugin.Logger.LogError("Failed to patch ElementEntity.Combine, field Level not found!");
    }

    static void CombinePatch(ElementEntity __instance, ElementEntity rElement)
    {
        var battleCryAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BattleCry);
        rElement.AttributeDict[battleCryAttrId] = 0;
        var subIconAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.SubIcon);
        rElement.AttributeDict[subIconAttrId] = 0;
    }

}