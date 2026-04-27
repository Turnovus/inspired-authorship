using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_AuthorTale : PassageWorker
    {
        private List<TaleDef> contextUsedTaleDefs = new List<TaleDef>();
        
        private bool CanUseTale(Tale tale, Pawn author)
        {
            if (!tale.Concerns(author))
                return false;
            return tale.def?.HasModExtension<ModExtension_TaleRules>() == true && !contextUsedTaleDefs.Contains(tale.def);
        }

        protected override bool CanUseForInt(Pawn author) =>
            Find.TaleManager.AllTalesListForReading.Any(t => CanUseTale(t, author));

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext=false)
        {
            foreach (Rule rule in base.GetRules(author, request, useContext))
                yield return rule;

            Tale tale = Find.TaleManager.AllTalesListForReading.Where(t => CanUseTale(t, author)).RandomElement();

            foreach (Rule rule in tale.GetTextGenerationRules(request.Constants))
                yield return rule;
            foreach (Rule rule in tale.def.rulePack.Rules)
                yield return rule;

            foreach (Rule rule in tale.def.GetModExtension<ModExtension_TaleRules>().rules.Rules)
                yield return rule;
            
            if (useContext)
                contextUsedTaleDefs.Add(tale.def);
        }

        public override void Notify_GenerationFinished() => contextUsedTaleDefs.Clear();
    }
}