using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class ModExtension_TraitRules : DefModExtension
    {
        public List<TraitDegreeRules> rulesByDegree = new List<TraitDegreeRules>();
        
        public class TraitDegreeRules
        {
            public int degree = 0;
            public RulePack rules;
        }
    }
}