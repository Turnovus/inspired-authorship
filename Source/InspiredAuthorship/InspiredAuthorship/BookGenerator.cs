using System;
using System.Collections.Generic;
using InspiredAuthorship.Passages;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

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

        public static CustomBook GenerateBook(Pawn author, QualityCategory quality)
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

        private static CustomBook GenerateBookInternal(Pawn author, QualityCategory quality)
        {
            ThingDef bookDef = MyDefOf.ModTuning.bookDefs.RandomElement();

            if (!(ThingMaker.MakeThing(bookDef) is CustomBook book))
            {
                Log.Error("Failed to generate book.");
                return null;
            }
            
            book.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Colony);
            book.ForceSetTitle(GenerateBookTitle(author));
            
            book.ForceSetDescription(GenerateBookDescription(author, out string passages));
            book.innerDescription = passages;
            
            author.records.Increment(MyDefOf.ModTuning.writtenBooksRecord);
            
            return book;
        }

        public static string GenerateBookTitle(Pawn author)
        {
            GrammarRequest request = GetRequestFor(author);
            request.Includes.Add(MyDefOf.ModTuning.writtenBookNamer);
            
            return GenText.CapitalizeAsTitle(GrammarResolver.Resolve("title", request)).StripTags();
        }

        public static string GenerateBookDescription(Pawn author, out string passages)
        {
            GrammarRequest request = GetRequestFor(author);
            
            request.Includes.Add(MyDefOf.ModTuning.writtenBookDescriberNew);
            passages = GenerateBookPassages(author);
            request.Rules.Add(new Rule_String("passages", passages));
            
            return GrammarResolver.Resolve("desc", request).StripTags();
        }

        public static GrammarRequest GetRequestFor(Pawn author)
        {
            GrammarRequest request = new GrammarRequest();
            
            if (author != null)
            {
                foreach (Rule rule in TaleData_Pawn.GenerateFrom(author).GetRules("AUTHOR", request.Constants))
                    request.Rules.Add(rule);
                foreach (Rule rule in RulesForLocation("LOCATION", author))
                    request.Rules.Add(rule);
                foreach (Rule rule in RulesForFaction("FACTION", author))
                    request.Rules.Add(rule);
            }
            request.Rules.Add(GetDateRule(author));

            return request;
        }

        public static Rule GetDateRule(Pawn author)
        {
            Vector2 longLat = author?.Tile == null
                ? Vector2.zero
                : Find.WorldGrid.LongLatOf(author.Tile);
            return new Rule_String("creationDate", GenDate.DateFullStringAt(GenTicks.TicksAbs, longLat));
        }

        public static string GenerateBookPassages(Pawn author)
        {
            string description = "";
            int maxPassages = PassageGenerator.MaxPassagesPossibleFor(author);

            if (maxPassages == 0)
            {
                description = "ERR";
            }
            else
            {
                maxPassages = Math.Min(maxPassages, MyDefOf.ModTuning.maxPassageCount);
                int minPassages = Math.Min(maxPassages, MyDefOf.ModTuning.idealMinPassageCount);
                List<PassageDef> usedPassages = new List<PassageDef>();
                List<GrammarRequest> requests = new List<GrammarRequest>();

                for (int i = 0; i < maxPassages; i++)
                {
                    GrammarRequest request =
                        PassageGenerator.GetRandomGrammarFor(author, usedPassages, out PassageDef passage);
                    usedPassages.Add(passage);
                    requests.Add(request);
                }

                int numRequests = requests.Count;
                for(int i = 0; i < numRequests; i++)
                {
                    int index = Rand.Range(0, requests.Count - 1);
                    GrammarRequest request = requests[index];
                    
                    RulePackDef rulePackDef;
                    if (i == 0)
                        rulePackDef = MyDefOf.ModTuning.passageStartRules;
                    else if (i == maxPassages - 1)
                        rulePackDef = MyDefOf.ModTuning.passageEndRules;
                    else
                        rulePackDef = MyDefOf.ModTuning.passageMiddleRules;
                    request.Includes.Add(rulePackDef);
                    
                    description += GrammarResolver.Resolve("passage", request);

                    if (i < maxPassages - 1)
                        description += "\n\n";

                    requests.RemoveAt(index);
                }
            }
            
            return description.StripTags();
        }

        public static IEnumerable<Rule> RulesForLocation(string prefix, Pawn pawn)
        {
            prefix += "_";
            if (Find.World?.info.name != null)
                yield return new Rule_String(prefix + "planetName", Find.World.info.name);
            INameableWorldObject nameable = pawn?.MapHeld?.Parent as INameableWorldObject;
            if (nameable != null)
                yield return new Rule_String(prefix + "mapName", nameable.Name);
        }

        public static IEnumerable<Rule> RulesForFaction(string prefix, Pawn pawn)
        {
            if (pawn?.Faction == null)
                yield break;
            foreach (Rule rule in RulesForFaction(prefix, pawn.Faction))
                yield return rule;
        }

        public static IEnumerable<Rule> RulesForFaction(string prefix, Faction faction)
        {
            prefix += "_";
            if (faction.HasName)
                yield return new Rule_String(prefix + "name", faction.Name);
            yield return new Rule_String(prefix + "pawnSingular", faction.def.pawnSingular);
            yield return new Rule_String(prefix + "pawnsPlural", faction.def.pawnsPlural);
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