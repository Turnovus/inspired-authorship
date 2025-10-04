using RimWorld;
using Verse;
using Verse.AI;

namespace InspiredAuthorship
{
    public class JobGiver_Author : ThinkNode_JobGiver
    {
        public override float GetPriority(Pawn pawn) =>
            !(pawn.Inspiration is Inspiration_Authorship) ? 0.0f : 6.5f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            Inspiration_Authorship inspiration = pawn.Inspiration as Inspiration_Authorship;
            if (inspiration == null)
                return null;

            Thing manuscript = inspiration.manuscript;
            if (manuscript != null)
                return pawn.CanReserve(manuscript) ? JobMaker.MakeJob(MyDefOf.Turn_Job_WorkOnManuscript, manuscript, 1500, true) : null ;
            
            return JobMaker.MakeJob(MyDefOf.Turn_Job_WorkOnManuscript, 1500, true);
        }
    }
}