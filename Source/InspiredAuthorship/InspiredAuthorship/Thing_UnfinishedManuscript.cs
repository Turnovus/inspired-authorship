using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class Thing_UnfinishedManuscript : ThingWithComps
    {
        public Pawn author;
        public int ticksWorked = 0;

        public void DoWork(int ticks) => ticksWorked += ticks;

        public void Notify_InspirationEnded()
        {
            
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Inspiration_Authorship inspiration = author?.Inspiration as Inspiration_Authorship;
            inspiration?.Notify_ManuscriptDestroyed();
            
            base.Destroy(mode);
        }

        public override string GetInspectString()
        {
            string s = base.GetInspectString();
            if (!s.NullOrEmpty())
                s += "\n";
            
            // TODO: Localization
            s += "Author: {0}".Formatted(author.LabelShort);
            s += "\nTime spent writing: {0}".Formatted(ticksWorked.ToStringTicksToPeriod(false));
            return s;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref author, "author");
            Scribe_Values.Look(ref ticksWorked, "ticksWorked");
        }
    }
}