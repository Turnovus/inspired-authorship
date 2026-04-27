using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public abstract class PassageWorker_BaseSkill : PassageWorker
    {
        public List<SkillDef> contextUsedSkills = new List<SkillDef>();
        
        public abstract float WeightForSkill(SkillRecord skill);

        private bool CanUseSkill(SkillRecord skill) => WeightForSkill(skill) > 0f && !contextUsedSkills.Contains(skill.def);

        protected override bool CanUseForInt(Pawn author)
        {
            if (author.skills == null)
                return false;
            return author.skills.skills.Any(CanUseSkill);
        }

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext=false)
        {
            foreach (Rule rule in base.GetRules(author, request, useContext))
                yield return rule;
            
            Dictionary<SkillDef, float> weights = new Dictionary<SkillDef, float>();
            foreach (SkillRecord skill in author.skills.skills)
            {
                if (CanUseSkill(skill))
                    weights[skill.def] = WeightForSkill(skill);
            }
            
            SkillDef randomSkill = weights.Keys.RandomElementByWeight(k => weights[k]);
            yield return new Rule_String("AUTHOR_skill", randomSkill.skillLabel);
            request.Constants["authorSkillLevel"] = author.skills.GetSkill(randomSkill).Level.ToString();
            foreach (Rule rule in randomSkill.generalRules.Rules)
                yield return rule;
            
            if (useContext)
                contextUsedSkills.Add(randomSkill);
        }

        public override void Notify_GenerationFinished() => contextUsedSkills.Clear();
    }
}