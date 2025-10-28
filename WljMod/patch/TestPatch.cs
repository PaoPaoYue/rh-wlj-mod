using HarmonyLib;
using Game;
using System.Collections.Generic;
using System.Reflection.Emit;
using cfg.element;
using System;
using cfg;

namespace WljMod;

static class TestPatch
{
    [HarmonyPatch(typeof(ModRaceCell), "ScrollCellIndex")]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> ScrollCellIndexTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false,
                new CodeMatch(OpCodes.Stloc_0)
            )
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                Transpilers.EmitDelegate(TTestPatch)
            )
            .InstructionEnumeration();
    }

    static void TTestPatch(ModRaceCell __instance, RaceAttribute raceConf)
    {
        Plugin.Logger.LogInfo($"ModRaceCell.ScrollCellIndex called: {raceConf}");
    }
}