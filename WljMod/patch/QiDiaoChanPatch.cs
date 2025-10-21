using HarmonyLib;
using Game;
using System.Collections.Generic;

namespace WljMod;

class QiDiaoChanPatch
{
    static int QiDiaoChanId = 100_018;

    static Dictionary<int, int> attrAddDict = new Dictionary<int, int>()
    {
        {3, 0},
        {8, 0},
        {9, 0},
        {11,0},
        {14,0},
        {18,0},
    };

    [HarmonyPatch(typeof(ElementEntity), "InitData")]
    [HarmonyPostfix]
    static void InitDataPostfix(ElementEntity __instance, int nLevel)
    {
        if (__instance.ID != QiDiaoChanId)
            return;
        foreach (var (attrId, addition) in attrAddDict)
        {
            var value = __instance.AttributeDict[attrId];
            value += addition * nLevel;
            __instance.SetAttribute(attrId, value, false);
        }
    }

    [HarmonyPatch(typeof(ElementEntity), "Upgrade")]
    [HarmonyPostfix]
    static void UpgradePostfix(ElementEntity __instance)
    {
        if (__instance.ID != QiDiaoChanId)
            return;
        foreach (var (attrId, addition) in attrAddDict)
        {
            var value = __instance.AttributeDict[attrId];
            value += addition;
            __instance.SetAttribute(attrId, value, false);
        }
    }

    [HarmonyPatch(typeof(ChooseElementCell), "UpdateAttribute")]
    [HarmonyPostfix]
    static void UpdateAttributePostfix(ChooseElementCell __instance, int ___mIndex, LotteCellAttr ___attrCell)
    {
        var element = Singleton<Model>.Instance.Element.CacheElement[___mIndex];
        if (element.Id != QiDiaoChanId)
            return;
        foreach (var (attrId, addition) in attrAddDict)
        {
            if (element.Attribute[0].ID == attrId)
            {
                var value = element.Attribute[0].Value;
                value += addition;
                ___attrCell.UpdateData(Singleton<Model>.Instance.Buff.GetAttributeConf(attrId), value);
                break;
            }
        }
    }

    [HarmonyPatch(typeof(ElementModel), "SellLoopItem")]
    [HarmonyPrefix]
    static bool SellLoopItemPrefix(ElementModel __instance, int nLoopIndex, out bool __state)
    {
        __state = false;
        if (nLoopIndex == -1)
        {
            return false;
        }
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(nLoopIndex, EEntityType.Player);
        if (!elementData.Fill || !elementData.Enable || elementData.Wait)
        {
            return false;
        }
        if (elementData.ID != QiDiaoChanId || elementData.Level < 3)
            return true;
        __state = true;
        // record the attribute values before selling
        foreach (var (attrId, _) in attrAddDict)
        {
            attrAddDict[attrId] = elementData.AttributeDict[attrId];
        }
        return true;
    }
    
    [HarmonyPatch(typeof(ElementModel), "SellLoopItem")]
    [HarmonyPostfix]
    static void SellLoopItemPostfix(ElementModel __instance, int nLoopIndex, bool __state)
    {
        if (__state)
            Singleton<GameEventManager>.Instance.Dispatch(10015, []);
    }


}