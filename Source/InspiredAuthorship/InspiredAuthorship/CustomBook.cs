using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace InspiredAuthorship
{
    public class CustomBook : Book
    {
        private static readonly FieldInfo Field_Title = AccessTools.Field(typeof(Book), "title");

        private static readonly FieldInfo
            Field_DescriptionFlavor = AccessTools.Field(typeof(Book), "descriptionFlavor");

        private static readonly FieldInfo Field_Description = AccessTools.Field(typeof(Book), "description");

        private static readonly MethodInfo Method_GenerateFullDescription =
            AccessTools.Method(typeof(Book), "GenerateFullDescription");
        
        public void ForceSetTitle(string newTitle)
        {
            Field_Title.SetValue(this, newTitle);
        }

        public void ForceSetDescription(string newDescription)
        {
            Field_DescriptionFlavor.SetValue(this, newDescription);
            string description = (string)Method_GenerateFullDescription.Invoke(this, Array.Empty<object>());
            Field_Description.SetValue(this, description);
        }
        
        public override void GenerateBook(Pawn author = null, long? fixedDate = null)
        {
            if (!BookGenerator.IsGeneratingBookNow)
                base.GenerateBook(author, fixedDate); // TODO: Load a saved book
        }
    }
}