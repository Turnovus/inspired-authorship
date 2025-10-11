using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(QuestPart_EndGame))]
    [HarmonyPatch(nameof(QuestPart_EndGame.Notify_QuestSignalReceived))]
    public static class QuestVictoryPatch
    {
        private static readonly MethodInfo Method_InitiateCountdown =
            AccessTools.Method(typeof(ShipCountdown), nameof(ShipCountdown.InitiateCountdown), new []{typeof(string)});
        
        private static readonly MethodInfo Method_LaunchPawns =
            AccessTools.Method(typeof(QuestVictoryPatch), nameof(LaunchPawns));

        public static void LaunchPawns(List<Pawn> pawns)
        {
            if (pawns.NullOrEmpty())
                return;
            
            foreach (Pawn pawn in pawns)
                LocalBookTracker.CurrentTracker.Notify_PawnEscaped(pawn);
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendLaunchedPawnNotifications(
            IEnumerable<CodeInstruction> instructions)
        {
            bool flag = false;
            
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == Method_InitiateCountdown && !flag)
                {
                    flag = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, Method_LaunchPawns);
                }
            }
            
            if (!flag)
                Log.Error("Patch {0} failed".Formatted(nameof(SendLaunchedPawnNotifications)));
        }
    }
}