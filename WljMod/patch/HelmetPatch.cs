using HarmonyLib;
using Game;

namespace WljMod;

static class HelmetPatch
{
    internal static bool Active = false;

    [HarmonyPatch(typeof(PlayerEntity), "GetHurt")]
    [HarmonyPrefix]
    static void GetHurtPrefix(PlayerEntity __instance, ref DamageCtx rDamageData, ref int __state)
    {
        __state = __instance.GetAttribute(1);
        var helmetAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Invincible);
        if (__instance.GetAttribute(helmetAttrId) > 0)
        {
            __instance.ChangeAttribute(helmetAttrId, -1);
            rDamageData.endDamageValue = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerEntity), "GetHurt")]
    [HarmonyPostfix]
    static void GetHurtPostHarmonyPostfix(PlayerEntity __instance, ref int __state)
    {
        if (Active && __instance.GetAttribute(1) < __state)
        {
            var helmetAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Invincible);
            __instance.ChangeAttribute(helmetAttrId, 1);
            Active = false;
        }
    }
}