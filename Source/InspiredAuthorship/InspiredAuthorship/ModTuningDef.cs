using System.Collections.Generic;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class ModTuningDef : Def
    {
        // Highest quality factor (% of max) for a book to be scrapped. Actual chance is linearly interpolated from 0 to
        // this value.
        public float maxQualityForScrapPossible;
        // How many days' worth of work is needed to receive a work quality offset of 1.0.
        public float qualityTargetDays;
        // The maximum quality offset that can be gained from work.
        public float maxWorkTimeContribution;
        // The quality offset per each skill level, taken from the 3 highest skills.
        public float offsetPerSkillLevel;
        // The maximum quality offset that can be gained from skill.
        public float maxSkillContribution;
        // A second cap is applied to skill contribution, equal to the product of this value and current work
        // contribution. This prevents highly-skilled pawns from producing high-quality books without actually doing any
        // work. A higher value makes the second cap easier to raise.
        public float skillCapFromWorkFactor;
        // The upper limit of the random quality offset.
        public float maxLuckContribution;
        
        // The random selection weight of each quality. The x value represents the quality factor from work, skill, and
        // luck (% of maximum), and the y value represents the weight of that quality category for that quality factor.
        public Dictionary<QualityCategory, SimpleCurve> qualitySelectionCurves;

        public List<ThingDef> bookDefs = new List<ThingDef>();

        public RulePackDef writtenBookNamer;

        public RulePackDef passageStartRules;
        public RulePackDef passageMiddleRules;
        public RulePackDef passageEndRules;

        public int maxPassageCount;
        public int idealMinPassageCount;

        public float MaxQualityOffset => maxWorkTimeContribution + maxSkillContribution + maxLuckContribution;
    }
}