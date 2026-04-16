using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_AuthorTrait : PassageWorker
    {
        private static RulePack RulesFor(TraitDef traitDef, int degree)
        {
            ModExtension_TraitRules extension = traitDef.GetModExtension<ModExtension_TraitRules>();
            if (extension == null)
                return null;
            foreach (ModExtension_TraitRules.TraitDegreeRules degreeRules in extension.rulesByDegree)
                if (degreeRules.degree == degree)
                    return degreeRules.rules;
            return null;
        }
        
        protected override bool CanUseForInt(Pawn author)
        {
            if (author.story?.traits?.allTraits.Any() != true)
                return false;

            foreach (Trait trait in author.story.traits.allTraits)
                if (!trait.Suppressed && RulesFor(trait.def, trait.Degree) != null)
                    return true;

            return false;
        }

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request)
        {
            foreach (Rule rule in base.GetRules(author, request))
                yield return rule;

            List<RulePack> rulePacks = new List<RulePack>();
            foreach (Trait trait in author.story.traits.allTraits)
            {
                if (trait.Suppressed)
                    continue;
                RulePack rulePack = RulesFor(trait.def, trait.Degree);
                if (rulePack != null)
                    rulePacks.Add(rulePack);
            }

            RulePack randomPack = rulePacks.RandomElement();
            foreach (Rule rule in randomPack.Rules)
                yield return rule;
        }
    }
}