using HarmonyLib;
using Verse;

namespace InspiredAuthorship
{
    [StaticConstructorOnStartup]
    public class PatchRunner
    {
        static PatchRunner()
        {
            Harmony harmony = new Harmony("turnovus.rimworld.inspiredauthorship");
            harmony.PatchAll();
        }
    }
}