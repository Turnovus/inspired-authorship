using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_SkillPassion : PassageWorker_BaseSkill
    {
        public override float WeightForSkill(SkillRecord skill)
        {
            if (skill.TotallyDisabled || skill.passion == Passion.None)
                return 0f;

            return skill.passion == Passion.Major ? 2.0f : 1.0f;
        }
    }
}