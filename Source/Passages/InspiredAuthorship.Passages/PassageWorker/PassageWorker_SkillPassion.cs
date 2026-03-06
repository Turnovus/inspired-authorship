using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_SkillPassion : PassageWorker
    {
        protected override bool CanUseForInt(Pawn author)
        {
            foreach (SkillRecord record in author.skills.skills)
                if (record.passion != Passion.None)
                    return true;
            return false;
        }

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request)
        {
            foreach (Rule rule in base.GetRules(author, request))
                yield return rule;

            Dictionary<SkillDef, float> weights = new Dictionary<SkillDef, float>();
            foreach (SkillRecord skill in author.skills.skills)
            {
                if (skill.TotallyDisabled || skill.passion == Passion.None)
                    continue;
                weights[skill.def] = skill.Level * (skill.passion == Passion.Major ? 2f : 1f);
            }

            SkillDef randomSkill = weights.Keys.RandomElementByWeight(k => weights[k]);

            yield return new Rule_String("passionSkill", randomSkill.skillLabel);
            foreach (Rule rule in randomSkill.generalRules.Rules)
                yield return rule;
        }
    }
}