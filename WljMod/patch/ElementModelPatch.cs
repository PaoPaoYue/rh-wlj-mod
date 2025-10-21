using HarmonyLib;
using Game;

namespace WljMod;

static class ElementModelPatch
{
    static int DongShiZhangId = 100_019;

    [HarmonyPatch(typeof(ElementModel), "AddAllPassiveAttribute")]
    [HarmonyPostfix]
    static void AddAllPassiveAttributePostfix(ElementModel __instance, EEntityType rOwner)
    {
        for (int i = 0; i < __instance.LoopItemCount; i++)
        {
            ElementEntity elementData = __instance.GetElementData(i, rOwner);
            if (elementData.Fill && elementData.Enable && !elementData.Wait)
            {
                if (elementData.ID == DongShiZhangId)
                {
                    var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BatleCryRepeat);
                    Singleton<Model>.Instance.Buff.GetPlayerEntity(rOwner).ChangeAttribute(attrId, 1);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ElementModel), "ChangePassiveAttribute")]
    [HarmonyPostfix]
    static void ChangePassiveAttributePostfix(ElementModel __instance, int nIndex, bool bAdd, EEntityType rOwner,  bool bExchange = false)
    {
        ElementEntity elementData = __instance.GetElementData(nIndex, rOwner);
        if (!bExchange && elementData != null && elementData.Fill && elementData.Enable && !elementData.Wait)
        {
            if (elementData.ID == DongShiZhangId)
            {
                var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BatleCryRepeat);
                Singleton<Model>.Instance.Buff.GetPlayerEntity(rOwner).ChangeAttribute(attrId, bAdd ? 1 : -1);
            }
        }
    }
}