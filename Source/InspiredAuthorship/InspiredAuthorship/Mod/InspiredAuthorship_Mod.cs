using Verse;

namespace InspiredAuthorship
{
    public class InspiredAuthorship_Mod : Mod
    {
        public InspiredAuthorship_Mod(ModContentPack content) : base(content) {}

        public InspiredAuthorship_ModSettings Settings => GetSettings<InspiredAuthorship_ModSettings>();

        public static InspiredAuthorship_Mod LoadedMod => LoadedModManager.GetMod<InspiredAuthorship_Mod>();

        public BookDatabase Database => Settings.Database;
    }
}