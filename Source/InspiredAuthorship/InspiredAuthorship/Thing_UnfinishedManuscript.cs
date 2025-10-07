using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public class Thing_UnfinishedManuscript : ThingWithComps
    {
        public Pawn author;
        public int ticksWorked = 0;
        private bool completed = false;

        public void DoWork(int ticks) => ticksWorked += ticks;

        public void Notify_InspirationEnded() => CompleteBook();

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Inspiration_Authorship inspiration = author?.Inspiration as Inspiration_Authorship;
            inspiration?.Notify_ManuscriptDestroyed();
            
            if (!completed)
                SendDestroyedMessage();
            
            base.Destroy(mode);
        }

        public void CompleteBook()
        {
            completed = true;
            // TODO: Generate book
            Destroy();
        }

        public void SendDestroyedMessage()
        {
            string label;
            string content;
            if (author != null)
            {
                label = "InspiredAuthorship.Letters.ManuscriptDestroyed.Label".Translate(author.Named("PAWN"));
                content = "InspiredAuthorship.Letters.ManuscriptDestroyed.Content".Translate(author.Named("PAWN"));
            }
            else
            {
                label = "InspiredAuthorship.Letters.ManuscriptDestroyed.NoAuthor.Label".Translate();
                content = "InspiredAuthorship.Letters.ManuscriptDestroyed.NoAuthor.Content".Translate();
            }

            Letter letter = LetterMaker.MakeLetter(label, content, LetterDefOf.NegativeEvent);
            
            letter.lookTargets = new LookTargets();
            if (MapHeld != null)
                letter.lookTargets.targets.Add(new GlobalTargetInfo(PositionHeld, MapHeld));
            if (author != null)
                letter.lookTargets.targets.Add(author);
            
            Find.LetterStack.ReceiveLetter(letter);
        }

        public override void Notify_LeftBehind()
        {
            base.Notify_LeftBehind();
            if (!Destroyed)
                Destroy();
        }

        public override void Notify_AbandonedAtTile(PlanetTile tile)
        {
            base.Notify_AbandonedAtTile(tile);
            if (!Destroyed)
                Destroy();
        }

        public override void Notify_MyMapRemoved()
        {
            base.Notify_MyMapRemoved();
            if (!Destroyed)
                Destroy();
        }

        public override string GetInspectString()
        {
            string s = base.GetInspectString();
            if (!s.NullOrEmpty())
                s += "\n";
            
            s += "InspiredAuthorship.Inspect.Manuscript.Author".Translate(author.Named("PAWN"));
            s += "\n";
            s += "InspiredAuthorship.Inspect.Manuscript.Work".Translate(ticksWorked.ToStringTicksToPeriod(false));
            return s;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            IEnumerable<Gizmo> baseGizmos = base.GetGizmos();
            if (!baseGizmos.EnumerableNullOrEmpty())
                foreach (Gizmo baseGizmo in baseGizmos)
                    yield return baseGizmo;

            if (!DebugSettings.ShowDevGizmos)
                yield break;

            yield return new Command_Action()
            {
                defaultLabel = "DEV: +1 hour",
                action = delegate
                {
                    ticksWorked += GenDate.TicksPerHour;
                },
            };

            yield return new Command_Action()
            {
                defaultLabel = "DEV: Log quality",
                action = delegate
                {
                    BookGenerator.LogQuality(author, ticksWorked);
                },
            };

            yield return new Command_Action()
            {
                defaultLabel = "DEV: Test quality outcome",
                action = delegate
                {
                    float qualityPreprocessed;
                    QualityCategory quality = BookGenerator.GetQualityNow(author, ticksWorked, out qualityPreprocessed);
                    Log.Message("Got quality {0} from factor {1}.".Formatted(
                        quality.ToString(),
                        qualityPreprocessed.ToStringDecimalIfSmall()
                        ));
                    Log.TryOpenLogWindow();
                },
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref author, "author");
            Scribe_Values.Look(ref ticksWorked, "ticksWorked");
            Scribe_Values.Look(ref completed, "completed");
        }
    }
}