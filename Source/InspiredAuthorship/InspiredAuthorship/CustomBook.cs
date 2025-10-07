using Verse;

namespace InspiredAuthorship
{
    public class CustomBook : Book
    {
        public override void GenerateBook(Pawn author = null, long? fixedDate = null)
        {
            if (!BookGenerator.IsGeneratingBookNow)
                base.GenerateBook(author, fixedDate); // TODO: Load a saved book
        }
    }
}