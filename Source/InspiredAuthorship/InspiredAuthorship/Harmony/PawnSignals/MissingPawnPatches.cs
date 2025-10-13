using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch]
    public static class MissingPawnPatches
    {
        private static readonly string[] TargetMethodNames =
        {
            nameof(Pawn.Discard),
            nameof(Pawn.PreKidnapped),
            nameof(Pawn.Notify_AbandonedAtTile),
            nameof(Pawn.Notify_MyMapRemoved),
        };
        
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (string methodName in TargetMethodNames)
            {
                yield return AccessTools.Method(typeof(Pawn), methodName);
            }
        }

        [HarmonyPostfix]
        public static void SetPawnMissing(Pawn __instance) =>
            LocalBookTracker.CurrentTracker.Notify_PawnMissing(__instance);
    }
}