using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public abstract class PassageWorker_OtherFaction : PassageWorker
    {
        public abstract bool IsAcceptableFaction(Faction faction);
        
        public override bool CanUseFor(Pawn author) => AllFactions.Any(IsAcceptableFaction);

        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request)
        {
            foreach (Rule rule in base.GetRules(author, request)) yield return rule;
            
            Faction faction = AllFactions.Where(IsAcceptableFaction).RandomElementWithFallback();
            if (faction == null)
            {
                Log.Error("Failed to find valid faction while generating {0}".Formatted(GetType().ToString()));
                yield break;
            }
            
            yield return new Rule_String("otherFaction_name", faction.Name);
            yield return new Rule_String("otherFaction_defLabel", faction.def.LabelCap);
            yield return new Rule_String("otherFaction_pawnSingular", faction.def.pawnSingular);
            yield return new Rule_String("otherFaction_pawnsPlural", faction.def.pawnsPlural);
            yield return new Rule_String("otherFaction_leader", faction.leader.LabelCap);
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