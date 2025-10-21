using HarmonyLib;
using Game;

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
}