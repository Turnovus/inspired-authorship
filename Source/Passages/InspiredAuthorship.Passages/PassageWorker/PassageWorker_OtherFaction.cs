using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public abstract class PassageWorker_OtherFaction : PassageWorker
    {
        public List<FactionDef> contextUsedFactions = new List<FactionDef>();
        
        public abstract bool IsAcceptableFaction(Faction faction);

        private bool CanUseFaction(Faction faction) =>
            faction.def != null && IsAcceptableFaction(faction) && !contextUsedFactions.Contains(faction.def);
        
        protected override bool CanUseForInt(Pawn author) => AllFactions.Any(CanUseFaction);

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext=false)
        {
            foreach (Rule rule in base.GetRules(author, request, useContext)) yield return rule;
            
            Faction faction = AllFactions.Where(CanUseFaction).RandomElementWithFallback();
            if (faction == null)
            {
                Log.Error("Failed to find valid faction while generating {0}".Formatted(GetType().ToString()));
                yield break;
            }
            
            yield return new Rule_String("otherFaction_name", faction.Name);
            yield return new Rule_String("otherFaction_defLabel", faction.def.LabelCap);
            yield return new Rule_String("otherFaction_pawnSingular", faction.def.pawnSingular);
            yield return new Rule_String("otherFaction_pawnsPlural", faction.def.pawnsPlural);
            if(faction.leader != null)
                yield return new Rule_String("otherFaction_leader", faction.leader.Name.ToStringFull);
            
            if (useContext)
                contextUsedFactions.Add(faction.def);
        }

        private IEnumerable<Faction> AllFactions => Find.FactionManager.GetFactions(allowNonHumanlike: false);
    }

    public class PassageWorker_HostileFaction : PassageWorker_OtherFaction
    {
        public override bool IsAcceptableFaction(Faction faction) => faction.HostileTo(Faction.OfPlayer);
    }

    public class PassageWorker_AlliedFaction : PassageWorker_OtherFaction
    {
        public override bool IsAcceptableFaction(Faction faction) =>
            faction.RelationWith(Faction.OfPlayer).kind == FactionRelationKind.Ally;
    }
}