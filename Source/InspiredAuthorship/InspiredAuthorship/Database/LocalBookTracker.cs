using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class LocalBookTracker : GameComponent
    {
        public List<TrackedBook> trackedBooks = new List<TrackedBook>();
        
        public static LocalBookTracker CurrentTracker => Current.Game.GetComponent<LocalBookTracker>();

        public LocalBookTracker(Game game)
        {
        }
        
        public void RegisterBook(CustomBook book, Pawn author)
        {
            int id = InspiredAuthorship_Mod.LoadedMod.Database.RegisterBook(book.Title, book.innerDescription);
            TrackedBook tracked = new TrackedBook()
            {
                id = id,
                author = author,
                book = book,
                
            };
            trackedBooks.Add(tracked);
            
            if (author == null || author.IsKidnapped() || (!author.Spawned && !author.Faction.IsPlayer))
                InspiredAuthorship_Mod.LoadedMod.Database.SetAuthorStatus(id, AuthorStatus.Missing);
            else if (author.Dead)
                InspiredAuthorship_Mod.LoadedMod.Database.SetAuthorStatus(id, AuthorStatus.Dead);
        }

        public void UpdatePawn(Pawn pawn, AuthorStatus newStatus)
        {
            foreach (TrackedBook trackedBook in trackedBooks)
            {
                if (trackedBook.author == pawn)
                {
                    trackedBook.authorStatus = newStatus;
                    if (newStatus == AuthorStatus.Escaped)
                        trackedBook.author = null;
                }
            }
        }

        public void UpdateBook(CustomBook book, BookStatus newStatus)
        {
            foreach (TrackedBook trackedBook in trackedBooks)
            {
                if (trackedBook.book == book)
                {
                    // Don't allow exported books to be lost/destroyed
                    if (newStatus == BookStatus.None || trackedBook.bookStatus != BookStatus.Exported)
                        trackedBook.bookStatus = newStatus;
                }
            }
        }

        #region Signals

        public void Notify_PawnDied(Pawn pawn) => UpdatePawn(pawn, AuthorStatus.Dead);

        public void Notify_PawnResurrected(Pawn pawn)
        {
            AuthorStatus newStatus = pawn.Spawned || pawn.Faction.IsPlayer
                ? AuthorStatus.None
                : AuthorStatus.Missing;
            UpdatePawn(pawn, newStatus);
        }

        public void Notify_PawnMissing(Pawn pawn) => UpdatePawn(pawn, AuthorStatus.Missing);

        public void Notify_PawnJoinedPlayerFaction(Pawn pawn)
        {
            if (!pawn.Dead)
                UpdatePawn(pawn, AuthorStatus.None);
        }

        public void Notify_PawnEscaped(Pawn pawn) => UpdatePawn(pawn, AuthorStatus.Escaped);

        public void Notify_BookDestroyed(CustomBook book) => UpdateBook(book, BookStatus.Destroyed);

        public void Notify_BookExported(CustomBook book) => UpdateBook(book, BookStatus.Exported);

        public void Notify_BookImported(CustomBook book) => UpdateBook(book, BookStatus.None);

        public void Notify_BookLost(CustomBook book) => UpdateBook(book, BookStatus.Missing);

        #endregion

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
            for (int i = trackedBooks.Count - 1; i >= 0; i--)
            {
                TrackedBook trackedBook = trackedBooks[i];
                
                InspiredAuthorship_Mod.LoadedMod.Database.SetBookStatus(trackedBook.id, trackedBook.bookStatus);
                if (trackedBook.author != null)
                    InspiredAuthorship_Mod.LoadedMod.Database.SetAuthorStatus(trackedBook.id, trackedBook.authorStatus);
                
                if (trackedBook.author == null && (trackedBook.book == null || trackedBook.bookStatus == BookStatus.Destroyed))
                    trackedBooks.RemoveAt(i);
            }
            
            InspiredAuthorship_Mod.LoadedMod.Settings.Write();
        }

        public class TrackedBook : IExposable
        {
            public int id;
            public Pawn author;
            public Thing book;
            public AuthorStatus authorStatus = AuthorStatus.None;
            public BookStatus bookStatus = BookStatus.None;
            
            public void ExposeData()
            {
                Scribe_Values.Look(ref id, "id");
                Scribe_References.Look(ref author, "author");
                Scribe_References.Look(ref book, "book");
                Scribe_Values.Look(ref authorStatus, "authorStatus");
                Scribe_Values.Look(ref bookStatus, "bookStatus");
            }
        }
    }
}