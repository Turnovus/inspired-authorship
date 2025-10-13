using HarmonyLib;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.Kill))]
    public static class DeadPawnPatch
    {
        [HarmonyPostfix]
        public static void SetPawnDead(Pawn __instance) => LocalBookTracker.CurrentTracker.Notify_PawnDied(__instance);
    }
}