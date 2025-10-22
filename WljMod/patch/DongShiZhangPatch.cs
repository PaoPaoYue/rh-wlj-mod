using HarmonyLib;
using Game;

namespace WljMod;

static class DongShiZhangPatch
{

    static int BattleCryRepeat = 0;

    public static bool CanBattleCryRepeat
    {
        get
        {
            return BattleCryRepeat > 0;
        }
    }

    static int DongShiZhangId = 100_009;

    [HarmonyPatch(typeof(ElementModel), "AddAllPassiveAttribute")]
    [HarmonyPostfix]
    static void AddAllPassiveAttributePostfix(ElementModel __instance, EEntityType rOwner)
    {
        for (int i = 0; i < __instance.LoopItemCount; i++)
        {
            ElementEntity elementData = __instance.GetElementData(i, rOwner);
            if (elementData != null && elementData.Fill && elementData.Enable && !elementData.Wait)
            {
                if (elementData.ID == DongShiZhangId)
                {
                    BattleCryRepeat++;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ElementModel), "ChangePassiveAttribute")]
    [HarmonyPostfix]
    static void ChangePassiveAttributePostfix(ElementModel __instance, int nIndex, bool bAdd, EEntityType rOwner, bool bExchange = false)
    {
        ElementEntity elementData = __instance.GetElementData(nIndex, rOwner);
        if (!bExchange && elementData != null && elementData.Fill && elementData.Enable && !elementData.Wait)
        {
            if (elementData.ID == DongShiZhangId)
            {
                if (bAdd)
                    BattleCryRepeat++;
                else
                    BattleCryRepeat--;
            }
        }
    }
}