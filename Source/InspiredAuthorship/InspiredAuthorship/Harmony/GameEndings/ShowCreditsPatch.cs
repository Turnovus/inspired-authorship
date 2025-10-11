using HarmonyLib;
using RimWorld;

namespace InspiredAuthorship
{
    [HarmonyPatch(typeof(GameVictoryUtility))]
    [HarmonyPatch(nameof(GameVictoryUtility.ShowCredits))]
    public class ShowCreditsPatch
    {
        [HarmonyPrefix]
        public static void SaveDataIfExiting(bool exitToMainMenu)
        {
            if (exitToMainMenu)
                LocalBookTracker.CurrentTracker.WriteData();
        }
    }
}