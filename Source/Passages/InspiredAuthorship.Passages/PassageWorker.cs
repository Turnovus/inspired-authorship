using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker
    {
        public PassageDef def;

        public float CommonalityFor(Pawn author) => CommonalityForInt(author);

        public float CommonalityForInt(Pawn author) => def.baseCommonality;

        public bool CanUseFor(Pawn author)
        {
            if (!def.SatisfiesRequiredTraits(author))
                return false;
            
            return CanUseForInt(author);
        }

        protected virtual bool CanUseForInt(Pawn author) => true;

        public virtual IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext = false)
        {
            foreach (Rule rule in TaleData_Pawn.GenerateFrom(author).GetRules("AUTHOR", request.Constants))
                yield return rule;
        }
        
        public virtual void Notify_GenerationFinished() {}
    }
}