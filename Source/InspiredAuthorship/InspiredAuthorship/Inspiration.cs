using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class Inspiration_Authorship : Inspiration
    {
        public Thing_UnfinishedManuscript manuscript;

        public void Notify_ManuscriptDestroyed(bool completed)
        {
            manuscript = null;
            // TODO: Notify player
            End();
        }

        public override void PostEnd()
        {
            manuscript?.Notify_InspirationEnded();
            base.PostEnd();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref manuscript, "manuscript");
        }

        public Thing CreateManuscript()
        {
            Thing thing = ThingMaker.MakeThing(MyDefOf.Turn_Authorship_Manuscript);
            manuscript = thing as Thing_UnfinishedManuscript;
            manuscript.author = pawn;
            return thing;
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