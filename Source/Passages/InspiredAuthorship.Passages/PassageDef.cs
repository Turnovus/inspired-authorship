using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageDef : Def
    {
        public float baseCommonality = 1.0f;
        public int maxUses = 1;

        public Type workerClass = typeof(PassageWorker);

        public RulePack rules;
        
        [XmlInheritanceAllowDuplicateNodes]
        public List<TraitRequirement> requiresAnyTrait;

        private PassageWorker workerInt;

        public PassageWorker Worker
        {
            get
            {
                if (workerInt == null)
                {
                    workerInt = (PassageWorker)Activator.CreateInstance(this.workerClass);
                    workerInt.def = this;
                }

                return workerInt;
            }
        }

        public bool SatisfiesRequiredTraits(Pawn author)
        {
            if (requiresAnyTrait.EnumerableNullOrEmpty())
                return true;
            
            foreach (TraitRequirement requirement in requiresAnyTrait)
                if (requirement.HasTrait(author))
                    return true;
            
            return false;
        }

        public void LogReport()
        {
            string report = defName;

            if (workerClass != null)
                report += "\n- Worker: " + workerClass.Name;

            if (!requiresAnyTrait.EnumerableNullOrEmpty())
            {
                report += "\n- requiresAnyTrait:";
                foreach (TraitRequirement requirement in requiresAnyTrait)
                    report += "\n  - {0} ({1})".Formatted(requirement.def.defName, requirement.degree?.ToString() ?? "any");
            }

            Log.Message(report);
        }
    }
}