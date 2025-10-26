using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg;

namespace WljMod;

static class BattleManagerPatch
{
    [HarmonyPatch(typeof(BattleManager), "StartPlayerTurn")]
    [HarmonyPostfix]
    static void StartPlayerTurnPostfix(BattleManager __instance)
    {
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(i, EEntityType.Player);
            if (elementData.Fill && elementData.Enable && !elementData.Wait)
            {
                var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
                if (elementData.GetAttribute(attrId) > 0)
                    elementData.ChangeAttribute(attrId, -1);
            }
        }
    }

    [HarmonyPatch(typeof(BattleManager), "OnEnemyDie")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> OnEnemyDieTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Enemy), "Gold")),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Enemy), "GoldMax"))
            )
            .Advance(5)
            .InsertAndAdvance(
                Transpilers.EmitDelegate(() =>
                {
                    var relicAttrId = Plugin.Register.GetRelicGlobalValueId((int)Plugin.RelicGlobalValue.Encourage);
                    return Singleton<Model>.Instance.Relic.GetRelicGlobalValue(relicAttrId, EEntityType.Player);
                }
                 ),
                new CodeInstruction(OpCodes.Add)
            )
            .InstructionEnumeration();
    }
}