using Verse;

namespace InspiredAuthorship
{
    public class WrittenBookData : IExposable
    {
        public int id;

        public string title;
        
        public string description;

        public string authorName;

        public string originPlanetName;

        public Date date;

        public AuthorStatus authorStatus = AuthorStatus.None;
        
        public BookStatus bookStatus = BookStatus.None;
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref title, "title");
            Scribe_Values.Look(ref description, "description");
            Scribe_Values.Look(ref authorName, "authorName");
            Scribe_Values.Look(ref originPlanetName, "originPlanetName");
            Scribe_Deep.Look(ref date, "date");
            Scribe_Values.Look(ref authorStatus, "authorStatus");
            Scribe_Values.Look(ref bookStatus, "bookStatus");
        }
    }

    public enum AuthorStatus
    {
        None,
        Dead,
        Missing,
        Escaped,
    }

    public enum BookStatus
    {
        None,
        Destroyed,
        Missing,
        Exported,
    }
}