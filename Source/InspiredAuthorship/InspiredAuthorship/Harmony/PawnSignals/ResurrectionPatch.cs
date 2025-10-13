using HarmonyLib;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(ResurrectionUtility))]
    [HarmonyPatch(nameof(ResurrectionUtility.TryResurrect))]
    public class ResurrectionPatch
    {
        [HarmonyPostfix]
        public static void SendResurrectionSignal(Pawn pawn) =>
            LocalBookTracker.CurrentTracker.Notify_PawnResurrected(pawn);
    }
}