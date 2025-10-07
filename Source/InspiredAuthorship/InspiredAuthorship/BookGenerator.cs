using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public static class BookGenerator
    {
        private static bool generatingBook = false;
        
        public static bool IsGeneratingBookNow => generatingBook;
        
        public static float GetQualityPreProcessedNow(Pawn author, int ticksWorked, out float luck)
        {
            float fromWork = GetOffsetFromWork(ticksWorked);
            luck = Rand.Range(0f, MyDefOf.ModTuning.maxLuckContribution);
            return fromWork + GetOffsetFromSkills(author, fromWork) + luck;
        }
        
        public static float GetOffsetFromWork(int ticksWorked) =>
            Mathf.Min(
                MyDefOf.ModTuning.maxWorkTimeContribution,
                ticksWorked / (MyDefOf.ModTuning.qualityTargetDays * GenDate.TicksPerDay));
        
        public static float GetOffsetFromSkills(Pawn author, float workOffset)
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
        
        public static QualityCategory GetQualityNow(Pawn author, int ticksWorked, out float preprocessed)
        {
            preprocessed = BookGenerator.GetQualityPreProcessedNow(author, ticksWorked, out _);
            float qualityPercent = preprocessed / MyDefOf.ModTuning.MaxQualityOffset;
            return MyDefOf.ModTuning.qualitySelectionCurves.Keys.RandomElementByWeight(k =>
            {
                SimpleCurve weightByQuality = MyDefOf.ModTuning.qualitySelectionCurves[k];
                return weightByQuality.Evaluate(qualityPercent);
            });
        }

        public static Thing GenerateBook(Pawn author, QualityCategory quality)
        {
            generatingBook = true;
            try
            {
                return GenerateBookInternal(author, quality);
            }
            finally
            {
                generatingBook = false;
            }
        }

        private static Thing GenerateBookInternal(Pawn author, QualityCategory quality)
        {
            ThingDef bookDef = MyDefOf.ModTuning.bookDefs.RandomElement();
            Thing book = ThingMaker.MakeThing(bookDef);
            book.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Colony);
            // TODO: Generate book details
            return book;
        }

        public static void LogQuality(Pawn author, int ticksWorked)
        {
            float fromWork = BookGenerator.GetOffsetFromWork(ticksWorked);
            Log.Message("Overall: {0}. From work: {1}. From skill: {2}/{3}. From luck: {4}.".Formatted(
                BookGenerator.GetQualityPreProcessedNow(author, ticksWorked, out float luck).ToStringDecimalIfSmall(),
                fromWork.ToStringDecimalIfSmall(),
                BookGenerator.GetOffsetFromSkills(author, fromWork).ToStringDecimalIfSmall(),
                BookGenerator.GetOffsetFromSkills(author, 1000f).ToStringDecimalIfSmall(),
                luck.ToStringDecimalIfSmall()
            ));
            Log.TryOpenLogWindow();
        }
    }
}