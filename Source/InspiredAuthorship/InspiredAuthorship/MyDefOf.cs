using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    [DefOf]
    public static class MyDefOf
    {
        public static JobDef Turn_Job_WorkOnManuscript;
        
        public static ThingDef Turn_Authorship_Manuscript;
        
        static MyDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(MyDefOf));
    }
}