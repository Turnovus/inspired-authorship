using System.Reflection;
using HarmonyLib;
using Verse;

namespace InspiredAuthorship
{
    public class CustomBook : Book
    {
        private static readonly FieldInfo Field_Title = AccessTools.Field(typeof(Book), "title");

        public void ForceSetTitle(string newTitle)
        {
            Field_Title.SetValue(this, newTitle);
        }
        
        public override void GenerateBook(Pawn author = null, long? fixedDate = null)
        {
            if (!BookGenerator.IsGeneratingBookNow)
                base.GenerateBook(author, fixedDate); // TODO: Load a saved book
        }
    }
}