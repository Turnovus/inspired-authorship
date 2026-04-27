using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_AuthorTrait : PassageWorker
    {
        private List<TraitDef> contextUsedTraits = new List<TraitDef>();
        
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

        private bool CanUseTrait(Trait trait) =>
            !trait.Suppressed && !contextUsedTraits.Contains(trait.def) && RulesFor(trait.def, trait.Degree) != null;
        
        protected override bool CanUseForInt(Pawn author)
        {
            if (author.story?.traits?.allTraits.Any() != true)
                return false;

            foreach (Trait trait in author.story.traits.allTraits)
                if (CanUseTrait(trait))
                    return true;

            return false;
        }

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext=false)
        {
            foreach (Rule rule in base.GetRules(author, request, useContext))
                yield return rule;

            List<Trait> usableTraits = new List<Trait>();
            foreach (Trait trait in author.story.traits.allTraits)
                if (CanUseTrait(trait))
                    usableTraits.Add(trait);

            Trait randomTrait = usableTraits.RandomElement();
            RulePack randomPack = RulesFor(randomTrait.def, randomTrait.Degree);
            foreach (Rule rule in randomPack.Rules)
                yield return rule;
            
            if (useContext)
                contextUsedTraits.Add(randomTrait.def);
        }

        public override void Notify_GenerationFinished() => contextUsedTraits.Clear();
    }
}