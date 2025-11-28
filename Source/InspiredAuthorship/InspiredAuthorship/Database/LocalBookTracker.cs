using System.Collections.Generic;
using System.Text;
using LudeonTK;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class LocalBookTracker : GameComponent
    {
        public List<TrackedBook> trackedBooks = new List<TrackedBook>();
        
        public static LocalBookTracker CurrentTracker => Current.Game.GetComponent<LocalBookTracker>();
        
        public static string PlanetName => Find.World.info.name;

        public LocalBookTracker(Game game)
        {
        }
        
        public void RegisterBook(CustomBook book, Pawn author)
        {
            QualityCategory quality = book.compQuality?.Quality ?? QualityCategory.Normal;
            int id = InspiredAuthorship_Mod.LoadedMod.Database.RegisterBook(
                book.def.defName,
                book.Title,
                book.innerDescription,
                author.Name.ToStringFull,
                PlanetName,
                Date.GetDateAt(book.MapHeld),
                quality
                );
            TrackedBook tracked = new TrackedBook()
            {
                id = id,
                author = author,
                book = book,
                
            };
            trackedBooks.Add(tracked);
        }

        public override void AppendDebugString(StringBuilder sb)
        {
            base.AppendDebugString(sb);
            sb.AppendLine("Tracking {0} books.".Formatted(trackedBooks.Count));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref trackedBooks, "trackedBooks", LookMode.Deep);

            if (trackedBooks.NullOrEmpty())
                trackedBooks = new List<TrackedBook>();
            
            if ((Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.PostLoadInit) && trackedBooks.Count > 0)
                WriteData();
        }

        public void WriteData()
        {
            InspiredAuthorship_Mod.LoadedMod.Settings.Write();
        }

        [DebugOutput("Inspired Authorship", true)]
        public static void LogLocallyTrackedBooks()
        {
            StringBuilder sb = new StringBuilder("Books currently tracked this game:");
            foreach (TrackedBook book in CurrentTracker.trackedBooks)
                sb.AppendLine(book.DebugString);
            Log.Message(sb.ToString());
        }

        public class TrackedBook : IExposable
        {
            public int id;
            public Pawn author;
            public Thing book;

            public string DebugString
            {
                get => "{0} - {1} by {3}".Formatted(
                    id.ToString(),
                    book?.Label ?? "Unknown",
                    author?.Label ?? "Unknown");
            }
            
            public void ExposeData()
            {
                Scribe_Values.Look(ref id, "id");
                Scribe_References.Look(ref author, "author");
                Scribe_References.Look(ref book, "book");
            }
        }
    }
}