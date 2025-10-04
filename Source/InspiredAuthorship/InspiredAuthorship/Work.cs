using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace InspiredAuthorship
{
    public class JobDriver_Write : JobDriver
    {
        private Inspiration_Authorship Inspiration => pawn.Inspiration as Inspiration_Authorship;

        public override string GetReport()
        {
            // TODO: Localization
            if (TargetThingA == null)
                return "Starting manuscript.";
            return base.GetReport();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return TargetThingA == null || pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            job.count = 1;
            this.FailOnBurningImmobile(TargetIndex.A);
            
            if (TargetThingA != null)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
                yield return Toils_Haul.StartCarryThing(TargetIndex.A);
            }
            
            yield return SitToil();
            yield return CreateManuscriptToil();
            yield return WorkToil();
        }

        private Toil SitToil()
        {
            float maxDistance = 64f;
            
            Toil toil = ToilMaker.MakeToil(nameof(SitToil));
            toil.initAction = () =>
            {
                Thing chair = GenClosest.ClosestThingReachable(pawn.Position,
                    pawn.Map,
                    ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                    PathEndMode.OnCell,
                    TraverseParms.For(pawn),
                    maxDistance,
                    t => IsValidChair(t, pawn) && t.Position.GetDangerFor(pawn, t.Map) == Danger.None);

                IntVec3 cell = IntVec3.Invalid;
                if (chair == null || !Toils_Ingest.TryFindFreeSittingSpotOnThing(chair, pawn, out cell))
                {
                    cell = pawn.Position;
                    pawn.CurJob.SetTarget(TargetIndex.C, cell);
                }
                else
                {
                    pawn.CurJob.SetTarget(TargetIndex.B, chair);
                    TryFindAdjacentWorkSpot(chair, out IntVec3 Workcell);
                    pawn.CurJob.SetTarget(TargetIndex.C, Workcell);
                }
                
                pawn.ReserveSittableOrSpot(cell, pawn.CurJob);
                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, pawn.CurJob, cell);
                pawn.pather.StartPath(cell, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    
        // Copied from Toils_Ingest.TryFindChairOrSpot.BaseChairValidator
        private bool IsValidChair(Thing thing, Pawn pawn)
        {
            IntVec3 cell;
            if (!PawnCanSitOn(thing, pawn) || !Toils_Ingest.TryFindFreeSittingSpotOnThing(thing, pawn, out cell))
                return false;
            
            return TryFindAdjacentWorkSpot(thing, out _);
        }

        public static bool TryFindAdjacentWorkSpot(Thing thing, out IntVec3 spot)
        {
            for (int i = 0; i < 4; ++i)
            {
                IntVec3 cardinal = thing.Position + GenAdj.CardinalDirections[i];
                Building edifice = cardinal.GetEdifice(thing.Map);
                if (edifice != null && edifice.def.surfaceType == SurfaceType.Eat)
                {
                    spot = cardinal;  
                    return true;
                }
            }

            spot = thing.Position;
            return false;
        }

        private bool PawnCanSitOn(Thing thing, Pawn pawn)
        {
            if (thing.def.building == null)
                return false;
            if (!thing.def.building.isSittable)
                return false;
            if (thing.Faction != pawn.Faction && pawn.Faction != null && pawn.Faction.IsPlayer)
                return false;
            if (pawn.IsColonist && thing.Position.Fogged(thing.Map))
                return false;
            if (!(pawn.CanReserve(thing) && thing.IsSociallyProper(pawn)))
                return false;
            if (thing.IsBurning())
                return false;
            
            return !thing.IsForbidden(pawn);
        }
        
        private Toil CreateManuscriptToil()
        {
            Toil toil = ToilMaker.MakeToil(nameof(CreateManuscriptToil));

            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            toil.initAction = () => {
                Inspiration_Authorship inspiration = pawn.Inspiration as Inspiration_Authorship;
                if (inspiration == null)
                {
                    pawn.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                }
                else if (inspiration.manuscript == null)
                {
                    Thing thing = inspiration.CreateManuscript();
                    toil.actor.carryTracker.TryStartCarry(thing);
                    toil.actor.CurJob.SetTarget(TargetIndex.A, thing);
                }
            };
            
            return toil;
        }

        private Toil WorkToil()
        {
            Toil toil = ToilMaker.MakeToil(nameof(WorkToil));
            Thing_UnfinishedManuscript manuscript = TargetThingA as Thing_UnfinishedManuscript;

            toil.initAction = () =>
            {
                if (manuscript == null)
                    pawn.jobs.curDriver.EndJobWith(JobCondition.Incompletable);
                pawn.rotationTracker.FaceCell(TargetC.Cell);
            };
            toil.tickAction = () =>
            {
                manuscript.DoWork(1);
                toil.actor.skills.Learn(SkillDefOf.Intellectual, 0.005f);
                toil.actor.GainComfortFromCellIfPossible(1, true);
            };
            
            toil.AddFailCondition(() => toil.actor.carryTracker.CarriedThing != manuscript);
            toil.AddFailCondition(() => manuscript.Destroyed || manuscript.IsForbidden(toil.actor));
            toil.FailOnCannotTouch(TargetIndex.C, PathEndMode.Touch);
            

            toil.defaultCompleteMode = ToilCompleteMode.Never;
            // TODO:
            toil.PlaySustainerOrSound(SoundDefOf.PageChange);
            return toil;
        }

        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool flip)
        {
            IntVec3 cell = job.GetTarget(TargetIndex.C).Cell;
            if (pawn.pather.Moving)
                return false;
            
            Thing carried = pawn.carryTracker.CarriedThing;
            if (carried == null)
                return false;

            if (cell.IsValid && cell.AdjacentToCardinal(pawn.Position) && cell.HasEatSurface(pawn.Map))
            {
                drawPos = new Vector3(cell.x + 0.5f, drawPos.y, cell.z + 0.5f);
                return true;
            }
            return false;
        }
    }
}