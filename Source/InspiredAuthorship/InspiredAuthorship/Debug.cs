using System;
using LudeonTK;
using Verse;

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
    }
}