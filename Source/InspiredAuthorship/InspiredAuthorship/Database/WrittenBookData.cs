using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class WrittenBookData : IExposable
    {
        public int id;

        public string defName;

        public string title;
        
        public string description;

        public string authorName;

        public string originPlanetName;

        public Date date;

        public QualityCategory quality;
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref defName, "defName");
            Scribe_Values.Look(ref title, "title");
            Scribe_Values.Look(ref description, "description");
            Scribe_Values.Look(ref authorName, "authorName");
            Scribe_Values.Look(ref originPlanetName, "originPlanetName");
            Scribe_Deep.Look(ref date, "date");
            Scribe_Values.Look(ref quality, "quality");
        }
    }
}