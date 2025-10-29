using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg.element;

namespace WljMod;

static class UIGameInfoPatch
{
    [HarmonyPatch(typeof(UIGameInfo), "OnBtnRefresh")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> OnBtnRefreshTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(ElementModel), "GenerateChooseElement"))
            )
            .Advance(1)
            .InsertAndAdvance(
                Transpilers.EmitDelegate(() =>
                {
                    var eventId = Plugin.Register.GetEventId((int)Plugin.Event.AfterNormalRefresh);
                    Singleton<GameEventManager>.Instance.Dispatch(eventId, []);
                })
            )
            .InstructionEnumeration();
    }
}