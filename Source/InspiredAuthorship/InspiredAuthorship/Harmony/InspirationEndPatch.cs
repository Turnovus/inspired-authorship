using HarmonyLib;
using RimWorld;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(InspirationHandler))]
    [HarmonyPatch(nameof(InspirationHandler.Reset))]
    public class InspirationEndPatch
    {
        [HarmonyPrefix]
        public static void ProperlyEndAuthorship(ref InspirationHandler __instance)
        {
            __instance.EndInspiration(MyDefOf.Turn_Inspired_Authorship);
        }
    }
}