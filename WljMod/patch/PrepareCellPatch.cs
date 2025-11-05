using HarmonyLib;
using Game;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection.Emit;
using System;
using BaseMod;

namespace WljMod;

static class PrepareCellPatch
{

    static List<GameObject> batteCrayImageObjs = new(4);

    [HarmonyPatch(typeof(PrepareCell), "Initialize")]
    [HarmonyPrefix]
    static void InitializePrefix(int nIndex, Image ___imgIcon)
    {
        GameObject imageObj = new("BattleCryImageObj");
        Image battleCryImage = imageObj.AddComponent<Image>();

        battleCryImage.sprite = Singleton<Model>.Instance.Mod.GetModSprite("wlj_vfx_battlecry");
        imageObj.transform.SetParent(___imgIcon.transform.parent, false); 

        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(32, 32);
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);
        rectTransform.pivot = new Vector2(1, 0);

        batteCrayImageObjs.Add(imageObj);
    }

    [HarmonyPatch(typeof(PrepareCell), "UpdateData")]
    [HarmonyPostfix]
    static void UpdatePostfix(PrepareCell __instance)
    {
        int index = __instance.Index;
        ReflectionUtil.TryInvokePrivateMethod(__instance, "GetElementData", out ElementEntity elementData);
        if (elementData == null)
        {
            Plugin.Logger.LogError("Failed to patch PrepareCell.UpdateData, GetElementData returned null!");
            return;
        }
        if (elementData.Fill)
        {
            var elementConf = Singleton<Model>.Instance.Element.GetElementConf(elementData.ID);
            var hasDoneBattleCryAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BattleCry);
            if (elementConf.Desctip != null && elementConf.Desctip.Contains((cfg.element.Etip)hasDoneBattleCryAttrId) && elementData.GetAttribute(hasDoneBattleCryAttrId) == 0)
            {
                batteCrayImageObjs[index].SetActive(true);
                return;
            }
        }
        batteCrayImageObjs[index].SetActive(false);
    }
    
    [HarmonyPatch(typeof(PrepareCell), "UpdateData")]
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