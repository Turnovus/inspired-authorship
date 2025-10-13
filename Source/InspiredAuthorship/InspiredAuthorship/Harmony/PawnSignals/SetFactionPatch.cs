using HarmonyLib;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.SetFaction))]
    public static class SetFactionPatch
    {
        [HarmonyPostfix]
        public static void OnFactionSet(Pawn __instance, Faction newFaction)
        {
            if (newFaction.IsPlayer)
                LocalBookTracker.CurrentTracker.Notify_PawnJoinedPlayerFaction(__instance);
        }
    }
}