using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public struct Date : IExposable
    {
        public int day;
        public Quadrum quadrum;
        public int year;

        public Date(int day = 1, Quadrum quadrum = Quadrum.Aprimay, int year = GenDate.DefaultStartingYear)
        {
            this.day = day;
            this.quadrum = quadrum;
            this.year = year;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref day, "day");
            Scribe_Values.Look(ref quadrum, "quadrum");
            Scribe_Values.Look(ref year, "year");
        }

        public static Date GetDateAt(Map map) => GetDateAt(map.Tile);

        public static Date GetDateAt(PlanetTile tile) => tile == PlanetTile.Invalid
            ? GetDateAt(Vector2.zero)
            : GetDateAt(Find.WorldGrid.LongLatOf(tile));

        public static Date GetDateAt(Vector2 coordinates)
        {
            int ticks = Find.TickManager.TicksAbs;
            int day = GenDate.DayOfYear(ticks, coordinates.x);
            Quadrum quadrum = GenDate.Quadrum(ticks, coordinates.x);
            int year = GenDate.Year(ticks, coordinates.x);
            return new Date(day, quadrum, year);
        }
    }
}