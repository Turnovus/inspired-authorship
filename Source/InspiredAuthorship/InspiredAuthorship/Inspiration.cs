using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class Inspiration_Authorship : Inspiration
    {
        public Thing_UnfinishedManuscript manuscript;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref manuscript, "manuscript");
        }
    }

    public class InspirationWorker_Authorship : InspirationWorker
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            //TODO: Prevent duplication, implement cooldown.
            return base.InspirationCanOccur(pawn);
        }
    }
}