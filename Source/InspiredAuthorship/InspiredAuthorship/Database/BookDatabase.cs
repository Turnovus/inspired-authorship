using System;
using System.Collections.Generic;
using Verse;

namespace InspiredAuthorship
{
    public class BookDatabase : IExposable
    {
        public int lastUniqueId = -1;
        
        public List<WrittenBookData> books;

        public int GetUniqueId() => ++lastUniqueId;

        public int RegisterBook(string title, string description)
        {
            int id = GetUniqueId();
            WrittenBookData book = new WrittenBookData()
            {
                id = id,
                title = title,
                description = description,
            };
            books.Add(book);
            
            return id;
        }

        public void SetAuthorStatus(int bookId, AuthorStatus status)
        {
            foreach (WrittenBookData book in books)
            {
                if (book.id == bookId)
                {
                    book.authorStatus = status;
                    return;
                }
            }
        }

        public void SetBookStatus(int bookId, BookStatus status)
        {
            foreach (WrittenBookData book in books)
            {
                if (book.id == bookId)
                {
                    book.bookStatus = status;
                    return;
                }
            }
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref lastUniqueId, "lastUniqueId", -1);
            Scribe_Collections.Look(ref books, "books", LookMode.Deep);
        }
    }
}