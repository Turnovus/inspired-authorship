using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(ArchonexusCountdown))]
    [HarmonyPatch("EndGame")]
    public static class ArchonexusVictoryPatch
    {
        private static MethodInfo Getter_StoryWatcher =
            AccessTools.PropertyGetter(typeof(Find), nameof(Find.StoryWatcher));

        private static MethodInfo Method_LaunchPawn =
            AccessTools.Method(typeof(ArchonexusVictoryPatch), nameof(LaunchPawn));

        public static void LaunchPawn(Pawn pawn)
        {
            LocalBookTracker.CurrentTracker.Notify_PawnEscaped(pawn);
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendLaunchedPawnNotifications(
            IEnumerable<CodeInstruction> instructions)
        {
            bool flag = false;
            
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Call && instruction.operand as MethodInfo == Getter_StoryWatcher && !flag)
                {
                    flag = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_3);
                    yield return new CodeInstruction(OpCodes.Call, Method_LaunchPawn);
                }
                
                yield return instruction;
            }
            
            if (!flag)
                Log.Error("Patch {0} failed".Formatted(nameof(SendLaunchedPawnNotifications)));
        }
    }
}