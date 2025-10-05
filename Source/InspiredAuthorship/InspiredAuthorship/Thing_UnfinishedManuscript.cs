using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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
            // TODO: Generate finished book
            Destroy();
        }

        public float GetQualityPreProcessedNow(out float luck)
        {
            float fromWork = GetOffsetFromWork();
            luck = Rand.Range(0f, MyDefOf.ModTuning.maxLuckContribution);
            return fromWork + GetOffsetFromSkills(fromWork) + luck;
        }
        
        public float GetOffsetFromWork() =>
            Mathf.Min(
                MyDefOf.ModTuning.maxWorkTimeContribution,
                ticksWorked / (MyDefOf.ModTuning.qualityTargetDays * GenDate.TicksPerDay));

        public float GetOffsetFromSkills(float workOffset)
        {
            List<SkillRecord> bestSkills = new List<SkillRecord>();
            for (int i = 0; i < 3; i++)
            {
                SkillRecord highestSkill = null;
                foreach (SkillRecord skill in author.skills.skills)
                {
                    if (!bestSkills.Contains(skill) && !skill.TotallyDisabled && (highestSkill == null || highestSkill.levelInt < skill.levelInt))
                        highestSkill = skill;
                }

                if (highestSkill != null)
                    bestSkills.Add(highestSkill);
                else
                    break;
            }

            int levels = 0;
            foreach (SkillRecord skill in bestSkills)
                levels += skill.levelInt;

            float preWorkCap = Mathf.Min(MyDefOf.ModTuning.maxSkillContribution, MyDefOf.ModTuning.offsetPerSkillLevel * levels);
            return Mathf.Min(preWorkCap, workOffset * MyDefOf.ModTuning.skillCapFromWorkFactor);
        }

        public QualityCategory GetQualityNow(out float preprocessed)
        {
            preprocessed = GetQualityPreProcessedNow(out _);
            float qualityPercent = preprocessed / MyDefOf.ModTuning.MaxQualityOffset;
            return MyDefOf.ModTuning.qualitySelectionCurves.Keys.RandomElementByWeight(k =>
            {
                SimpleCurve weightByQuality = MyDefOf.ModTuning.qualitySelectionCurves[k];
                return weightByQuality.Evaluate(qualityPercent);
            });
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
                    float fromWork = GetOffsetFromWork();
                    Log.Message("Overall: {0}. From work: {1}. From skill: {2}/{3}. From luck: {4}.".Formatted(
                        GetQualityPreProcessedNow(out float luck).ToStringDecimalIfSmall(),
                        fromWork.ToStringDecimalIfSmall(),
                        GetOffsetFromSkills(fromWork).ToStringDecimalIfSmall(),
                        GetOffsetFromSkills(1000f).ToStringDecimalIfSmall(),
                        luck.ToStringDecimalIfSmall()
                        ));
                    Log.TryOpenLogWindow();
                },
            };

            yield return new Command_Action()
            {
                defaultLabel = "DEV: Test quality outcome",
                action = delegate
                {
                    float qualityPreprocessed;
                    QualityCategory quality = GetQualityNow(out qualityPreprocessed);
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
        }
    }
}