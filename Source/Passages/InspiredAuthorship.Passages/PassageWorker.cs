using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker
    {
        public PassageDef def;

        public virtual float CommonalityFor(Pawn author) => def.baseCommonality;

        public virtual bool CanUseFor(Pawn author) => true;

        public virtual IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request)
        {
            foreach (Rule rule in TaleData_Pawn.GenerateFrom(author).GetRules("AUTHOR", request.Constants))
                yield return rule;
        }
        
        
    }
}