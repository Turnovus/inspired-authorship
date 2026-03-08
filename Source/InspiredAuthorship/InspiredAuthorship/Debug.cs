using System;
using System.Collections.Generic;
using System.Linq;
using InspiredAuthorship.Passages;
using LudeonTK;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship
{
    public static class Debug
    {
        [DebugOutput("Inspired Authorship")]
        public static void TestTitles()
        {
            string s = "Output:";
            for (int i = 0; i < 100; i++)
            {
                s += "\n" + BookGenerator.GenerateBookTitle(null);
            }
            Log.Message(s);
        }

        [DebugOutput("Inspired Authorship")]
        public static void ReportPassageDefs()
        {
            foreach (PassageDef def in DefDatabase<PassageDef>.AllDefsListForReading)
                def.LogReport();
        }

        [DebugAction("Inspired Authorship", actionType = DebugActionType.ToolMapForPawns,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TestDescriptionByAuthor(Pawn pawn)
        {
            string book = BookGenerator.GenerateBookDescription(pawn, out string _);
            Log.Message(book);
            Log.TryOpenLogWindow();
        }

        [DebugAction("Inspired Authorship", actionType = DebugActionType.Action,
            allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ForceTestPassage()
        {
            List<DebugMenuOption> options = new List<DebugMenuOption>();
            foreach (PassageDef passageDef in DefDatabase<PassageDef>.AllDefsListForReading)
            {
                options.Add(new DebugMenuOption(passageDef.defName, DebugMenuOptionMode.Tool, () =>
                {
                    foreach (Pawn pawn in UI.MouseCell().GetThingList(Find.CurrentMap).OfType<Pawn>().ToList())
                    {
                        if (!passageDef.Worker.CanUseFor(pawn))
                            Log.Warning("Forced to generate passage while CanUseFor = false, this may cause errors.");
                        GrammarRequest request = PassageGenerator.GetGrammarFromPassage(pawn, passageDef);
                        request.Includes.Add(MyDefOf.ModTuning.passageMiddleRules);
                        string passage = GrammarResolver.Resolve("passage", request);
                        Log.Message(passage.StripTags());
                        Log.TryOpenLogWindow();
                    }
                }));
            }
            Find.WindowStack.Add((new Dialog_DebugOptionListLister(options)));
        }
    }
}