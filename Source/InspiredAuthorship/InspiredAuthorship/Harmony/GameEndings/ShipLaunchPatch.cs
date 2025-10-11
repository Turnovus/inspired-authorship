using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(ShipCountdown))]
    [HarmonyPatch("CountdownEnded")]
    public static class ShipLaunchPatch
    {
        private static readonly FieldInfo Field_ColonistsLaunched =
            AccessTools.Field(typeof(StatsRecord), nameof(StatsRecord.colonistsLaunched));

        private static readonly MethodInfo Method_LaunchCasket =
            AccessTools.Method(typeof(ShipLaunchPatch), nameof(LaunchCasket));

        public static void LaunchCasket(Building_CryptosleepCasket casket)
        {
            if (casket.ContainedThing is Pawn pawn)
                LocalBookTracker.CurrentTracker.Notify_PawnEscaped(pawn);
        }
        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SendLaunchedPawnNotifications(
            IEnumerable<CodeInstruction> instructions)
        {
            // 0 - Looking for isinst RimWorld.Building_CryptosleepCasket
            // 1 - Grabbing argument from stloc.s
            // 2 - Looking for stfld RimWorld.StatsRecord::colonistsLaunched to inject
            // 3 - Patch done
            int flag = 0;
            LocalBuilder cryptosleepCasketField = null;
            
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                switch (flag)
                {
                    case 0:
                        if (instruction.opcode == OpCodes.Isinst &&
                            instruction.operand as Type == typeof(Building_CryptosleepCasket))
                            flag++;
                        break;
                    case 1:
                        if (instruction.opcode == OpCodes.Stloc_S)
                        {
                            cryptosleepCasketField = instruction.operand as LocalBuilder;
                            flag++;
                        }

                        break;
                    case 2:
                        if (instruction.operand as FieldInfo == Field_ColonistsLaunched)
                        {
                            // LaunchCasket(cryptosleepCasket)
                            yield return new CodeInstruction(OpCodes.Ldloc_S, cryptosleepCasketField);
                            yield return new CodeInstruction(OpCodes.Call, Method_LaunchCasket);
                            
                            flag++;
                        }

                        break;
                }
            }
            
            if (flag != 3)
                Log.Error("Transpiler {0} failed with flag {1}".Formatted(nameof(SendLaunchedPawnNotifications), flag.ToString()));
        }
    }
}