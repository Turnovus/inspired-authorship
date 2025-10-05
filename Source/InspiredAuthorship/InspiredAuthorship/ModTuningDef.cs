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

        public float MaxQualityOffset => maxWorkTimeContribution + maxSkillContribution + maxLuckContribution;
    }
}