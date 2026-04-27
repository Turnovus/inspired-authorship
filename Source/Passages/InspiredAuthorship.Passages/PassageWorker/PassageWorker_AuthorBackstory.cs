using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageWorker_AuthorBackstory : PassageWorker
    {
        public override IEnumerable<Rule> GetRules(Pawn author, GrammarRequest request, bool useContext=false)
        {
            foreach (Rule rule in base.GetRules(author, request, useContext))
                yield return rule;

            yield return new Rule_String("AUTHOR_childhood", author.story.Childhood.title);
            if (author.story.Adulthood != null)
                yield return new Rule_String("AUTHOR_adulthood", author.story.Adulthood.title);
        }
    }
}