using Verse;

namespace InspiredAuthorship
{
    public class InspiredAuthorship_ModSettings : ModSettings
    {
        private BookDatabase database = null;

        public BookDatabase Database
        {
            get
            {
                if (database == null)
                    database = new BookDatabase();
                return database;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Database.ExposeData();
        }
    }
}