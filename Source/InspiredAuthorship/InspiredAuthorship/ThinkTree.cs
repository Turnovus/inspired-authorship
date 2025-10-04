using RimWorld;
using Verse;
using Verse.AI;

namespace InspiredAuthorship
{
    public class ThinkNode_ConditionalInspiration : ThinkNode_Conditional
    {
        public InspirationDef inspiration;
        
        protected override bool Satisfied(Pawn pawn) => pawn.InspirationDef == inspiration;
    }

    public class JobGiver_Author : ThinkNode_JobGiver
    {
        public JobDef writeJob;

        protected override Job TryGiveJob(Pawn pawn)
        {
            Inspiration_Authorship inspiration = pawn.Inspiration as Inspiration_Authorship;
            if (inspiration == null)
                return null;

            Thing manuscript = inspiration.manuscript;
            if (manuscript != null && !pawn.CanReserve(manuscript))
                return null;
            
            return JobMaker.MakeJob(writeJob, manuscript, 1500, true);
        }
    }
}