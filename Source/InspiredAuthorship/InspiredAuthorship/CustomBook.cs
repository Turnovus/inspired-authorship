using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace InspiredAuthorship
{
    public class CustomBook : Book
    {
        private static readonly FieldInfo Field_Title = AccessTools.Field(typeof(Book), "title");

        private static readonly FieldInfo
            Field_DescriptionFlavor = AccessTools.Field(typeof(Book), "descriptionFlavor");

        private static readonly FieldInfo Field_Description = AccessTools.Field(typeof(Book), "description");

        private static readonly MethodInfo Method_GenerateFullDescription =
            AccessTools.Method(typeof(Book), "GenerateFullDescription");

        public string innerDescription;
        
        public void ForceSetTitle(string newTitle)
        {
            Field_Title.SetValue(this, newTitle);
        }

        public void ForceSetDescription(string newDescription)
        {
            Field_DescriptionFlavor.SetValue(this, newDescription);
            string description = (string)Method_GenerateFullDescription.Invoke(this, Array.Empty<object>());
            Field_Description.SetValue(this, description);
        }
        
        public override void GenerateBook(Pawn author = null, long? fixedDate = null)
        {
            if (!BookGenerator.IsGeneratingBookNow)
                base.GenerateBook(author, fixedDate); // TODO: Load a saved book
        }

        #region Signals

        public override void Notify_LeftBehind() => Lost();

        public override void Notify_AbandonedAtTile(PlanetTile tile) => Lost();

        public override void Notify_MyMapRemoved() => Lost();

        public override void PreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            base.PreTraded(action, playerNegotiator, trader);
            if (action == TradeAction.PlayerSells)
                LocalBookTracker.CurrentTracker.Notify_BookExported(this);
            else if (action == TradeAction.PlayerBuys)
                LocalBookTracker.CurrentTracker.Notify_BookImported(this);
        }

        #endregion

        private void Lost() => LocalBookTracker.CurrentTracker.Notify_BookLost(this);

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode == DestroyMode.Vanish)
                Lost();
            else
                LocalBookTracker.CurrentTracker.Notify_BookDestroyed(this);
            
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref innerDescription, "innerDescription");
        }
    }
}