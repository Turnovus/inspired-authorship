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

        protected override Job TryGiveJob(Pawn pawn) => JobMaker.MakeJob(writeJob, 1500, true);
    }
}