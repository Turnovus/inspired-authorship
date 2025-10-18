using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public class InspiredAuthorship_Mod : Mod
    {
        public const float HeaderHeight = 30f;
        public const float TabsHeight = 35f;
        public const float BookEntryHeight = 30f;
        public const float ScrollbarWidth = 10f;

        public const float BookIdWidthRatio = 0.5f;
        public const float BookMajorDetailWidthRatio = 5.5f; // Title, author, date
        public const float BookMinorDetailWidthRatio = 2.5f; // Status, quality, defName

        public const float TotalBookDetailWidth =
            BookIdWidthRatio + BookMajorDetailWidthRatio * 3 + BookMinorDetailWidthRatio * 3;
        
        public SettingsTab activeTab = SettingsTab.General;

        private Vector2 databaseScroll = Vector2.zero;
        
        public InspiredAuthorship_Mod(ModContentPack content) : base(content) {}

        public InspiredAuthorship_ModSettings Settings => GetSettings<InspiredAuthorship_ModSettings>();

        public static InspiredAuthorship_Mod LoadedMod => LoadedModManager.GetMod<InspiredAuthorship_Mod>();

        public BookDatabase Database => Settings.Database;

        public override string SettingsCategory() => "Inspired Authorship";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Rect drawRect = new Rect(inRect);
            drawRect.yMin += HeaderHeight;
            DrawTabs(drawRect);
            drawRect.yMin += TabsHeight;
            
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            switch (activeTab)
            {
                case SettingsTab.General:
                    DrawGeneralSettings(drawRect);
                    break;
                case SettingsTab.Spawning:
                    DrawSpawningSettings(drawRect);
                    break;
                case SettingsTab.Database:
                    DrawDatabase(drawRect);
                    break;
            }

            Text.Anchor = anchor;
        }

        private void DrawTabs(Rect inRect)
        {
            List<TabRecord> tabs = new List<TabRecord>();
            tabs.Add(GetTabRecord(SettingsTab.General));
            tabs.Add(GetTabRecord(SettingsTab.Spawning));
            tabs.Add(GetTabRecord(SettingsTab.Database));
            Rect tabRect = new Rect(inRect);
            tabRect.height = TabsHeight;
            TabDrawer.DrawTabs(tabRect, tabs);
        }

        private TabRecord GetTabRecord(SettingsTab forTab) => new TabRecord(
            ("InspiredAuthorship.Enums.SettingsTab." + forTab.ToString()).Translate(),
            () => activeTab = forTab,
            activeTab == forTab);

        private void DrawGeneralSettings(Rect inRect)
        {
            Widgets.Label(inRect, "TODO - General");
        }

        private void DrawSpawningSettings(Rect inRect)
        {
            Widgets.Label(inRect, "TODO - Spawning");
        }

        private void DrawDatabase(Rect inRect)
        {
            int bookCount = Settings.Database.books.Count;
            if (bookCount <= 0)
            {
                Widgets.Label(inRect, "InspiredAuthorship.Settings.NoBooksInDatabase".Translate());
                return;
            }
            
            Rect scrollRect = new Rect(inRect);
            scrollRect.y = 0f;
            scrollRect.height = bookCount * BookEntryHeight;
            scrollRect.xMax -= ScrollbarWidth;
                
            Widgets.BeginScrollView(inRect, ref databaseScroll, scrollRect);

            bool altRow = false;
            Rect rowRect = new Rect(scrollRect);
            rowRect.height = BookEntryHeight;
            foreach (WrittenBookData data in Settings.Database.books)
            {
                DrawBookRow(rowRect, data, altRow);
                rowRect.y += rowRect.height;
                altRow = !altRow;
            }
            
            Widgets.EndScrollView();
        }

        private void DrawBookRow(Rect rect, WrittenBookData data, bool altRow)
        {
            if (altRow)
                Widgets.DrawHighlight(rect);
            
            Widgets.DrawHighlightIfMouseover(rect);
            TooltipHandler.TipRegion(rect, data.description);

            float bookIdWidth = (BookIdWidthRatio / TotalBookDetailWidth) * rect.width;
            float bookMajorDetailWidth = (BookMajorDetailWidthRatio / TotalBookDetailWidth) * rect.width;
            float bookMinorDetailWidth = (BookMinorDetailWidthRatio / TotalBookDetailWidth) * rect.width;
            
            // ID#
            Rect workRect = new Rect(rect);
            workRect.width = bookIdWidth;
            Widgets.Label(workRect, data.id.ToString());
            DrawDividerRight(workRect);
            
            // ThingDef.label
            workRect.x += workRect.width;
            workRect.width = bookMinorDetailWidth;
            string defLabel = DefDatabase<ThingDef>.GetNamedSilentFail(data.defName ?? "")?.label ?? "ERR: " + data.defName;
            Widgets.Label(workRect, defLabel);
            DrawDividerRight(workRect);
            
            // Title
            workRect.x += workRect.width;
            workRect.width = bookMajorDetailWidth;
            Widgets.Label(workRect, data.title);
            DrawDividerRight(workRect);
            
            // Author
            workRect.x += workRect.width;
            string authorStatus = ("InspiredAuthorship.Enums.AuthorStatus." + data.authorStatus).Translate();
            string authorFull = "{0} ({1})".Formatted(data.authorName, authorStatus);
            Widgets.Label(workRect, authorFull);
            DrawDividerRight(workRect);
            
            // Planet of origin
            /*
             workRect.x += workRect.width;
            Widgets.Label(workRect, data.originPlanetName);
            DrawDividerRight(workRect);
            */
            
            //Date
            workRect.x += workRect.width;
            Widgets.Label(workRect, data.date.ToString());
            DrawDividerRight(workRect);
            
            // Quality
            workRect.x += workRect.width;
            workRect.width = bookMinorDetailWidth;
            Widgets.Label(workRect, data.quality.GetLabel());
            DrawDividerRight(workRect);
            
            //Status
            workRect.x += workRect.width;
            string bookStatus = ("InspiredAuthorship.Enums.BookStatus." + data.bookStatus).Translate();
            Widgets.Label(workRect, bookStatus);
        }

        private void DrawDividerRight(Rect rect)
        {
            Widgets.DrawLineVertical(rect.xMax - 2f, rect.y + 1f, rect.height - 2f);
        }

        public enum SettingsTab
        {
            General,
            Spawning,
            Database,
        }
    }
}