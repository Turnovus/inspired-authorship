using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_SkillMastery : PassageWorker_BaseSkill
    {
        public const int RequiredLevel = 14;

        public override float WeightForSkill(SkillRecord skill)
        {
            if (skill.Level < RequiredLevel)
                return 0f;

            int overflow = skill.Level - RequiredLevel;
            return 1f + 0.25f * overflow;
        }
    }
}