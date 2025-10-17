using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace InspiredAuthorship
{
    public class InspiredAuthorship_Mod : Mod
    {
        public const float HeaderHeight = 30f;
        public const float TabsHeight = 35f;
        
        public SettingsTab activeTab = SettingsTab.General;
        
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

        public enum SettingsTab
        {
            General,
            Spawning,
            Database,
        }
    }
}