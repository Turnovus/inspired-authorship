using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public static class PassageGenerator
    {
        public static int MaxPassagesPossibleFor(Pawn author)
        {
            int count = 0;
            foreach (PassageDef def in DefDatabase<PassageDef>.AllDefsListForReading)
            {
                if (def.Worker.CanUseFor(author))
                    count++;
            }

            return count;
        }
        
        public static GrammarRequest GetRandomGrammarFor(Pawn author, IEnumerable<PassageDef> usedDefs, out PassageDef usedDef, bool useContext = false)
        {
            usedDef = DefDatabase<PassageDef>.AllDefsListForReading.RandomElementByWeight(
                d => usedDefs.Count(ud => ud == d) < d.maxUses && d.Worker.CanUseFor(author)
                    ? d.Worker.CommonalityFor(author)
                    : 0.0f);
            
            return GetGrammarFromPassage(author, usedDef, useContext);
        }

        public static GrammarRequest GetGrammarFromPassage(Pawn author, PassageDef passageDef, bool useContext = false)
        {
            GrammarRequest request = new GrammarRequest();
            
            foreach(Rule rule in passageDef.Worker.GetRules(author, request, useContext))
                request.Rules.Add(rule);
            
            foreach (Rule rule in passageDef.rules.Rules)
                request.Rules.Add(rule);

            return request;
        }
    }
}