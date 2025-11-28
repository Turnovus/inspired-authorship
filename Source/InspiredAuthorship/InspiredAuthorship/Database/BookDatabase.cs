using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace InspiredAuthorship
{
    public class BookDatabase : IExposable
    {
        public int lastUniqueId = -1;
        
        public List<WrittenBookData> books = new List<WrittenBookData>();

        public int GetUniqueId() => ++lastUniqueId;

        public int RegisterBook(string defName, string title, string description, string authorName, string planetName, Date date, QualityCategory quality)
        {
            int id = GetUniqueId();
            WrittenBookData book = new WrittenBookData()
            {
                defName = defName,
                id = id,
                title = title,
                description = description,
                authorName = authorName,
                originPlanetName = planetName,
                date = date,
                quality = quality,
            };
            books.Add(book);
            Write();
            
            return id;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref lastUniqueId, "lastUniqueId", -1);
            Scribe_Collections.Look(ref books, "books", LookMode.Deep);
            
            if (books.NullOrEmpty())
                books = new List<WrittenBookData>();
        }

        public void Write()
        {
            InspiredAuthorship_Mod.LoadedMod.Settings.Write();
        }
    }
}