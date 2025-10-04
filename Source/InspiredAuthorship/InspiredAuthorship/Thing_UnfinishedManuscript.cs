using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public class Thing_UnfinishedManuscript : ThingWithComps
    {
        public const float MaxQualityForScrapPossible = 0.6f;
        public const float MaxWorkTimeContribution = 0.6f;
        public const float OffsetPerSkillLevel = 0.015f;
        public const float MaxSkillContribution = 0.6f;
        public const float SkillCapFromWorkFactor = 2.5f;
        public const float MaxLuckContribution = 0.5f;
        public const float MaxQualityOffset = MaxWorkTimeContribution + MaxSkillContribution + MaxLuckContribution;
        
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
            luck = Rand.Range(0f, MaxLuckContribution);
            return fromWork + GetOffsetFromSkills(fromWork) + luck;
        }
        
        public float GetOffsetFromWork() => Mathf.Min(MaxWorkTimeContribution, ticksWorked / (10f * GenDate.TicksPerDay));

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

            float preWorkCap = Mathf.Min(MaxSkillContribution, OffsetPerSkillLevel * levels);
            return Mathf.Min(preWorkCap, workOffset * SkillCapFromWorkFactor);
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