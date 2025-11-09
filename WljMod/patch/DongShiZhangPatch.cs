using HarmonyLib;
using Game;

namespace WljMod;

static class DongShiZhangPatch
{

    public static bool CanBattleCryRepeat
    {
        get
        {
            //iterate all loop items to see if Dong Shi Zhang is present
            for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
            {
                ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(i, EEntityType.Player);
                if (elementData.Fill && elementData.Enable && elementData.ID == DongShiZhangId)
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal static int DongShiZhangId = 10009;

}