using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Overlord : Gene
    {

        public int nextTick = 180;

        public override void PostAdd()
        {
            base.PostAdd();
        }

        public override void Tick()
        {
            if (!GeneResourceUtility.CanTick(ref nextTick, 180))
            {
                return;
            }
            TryGiveJob();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: ShamblerJob",
                    action = delegate
                    {
                        TryGiveJob();
                    }
                };
            }
        }

        private void TryGiveJob()
        {
            foreach (Pawn shambler in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (shambler.IsShambler && shambler.Faction == pawn.Faction)
                {
                    shambler.TryTakeOrderedJob(TryGiveJob(shambler), JobTag.Misc, true);
                }
            }
        }

        public static Job TryGiveJob(Pawn pawn)
        {
            bool validator(Thing t)
            {
                if (t.IsForbidden(pawn))
                {
                    return false;
                }
                if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced: false))
                {
                    return false;
                }
                if (pawn.carryTracker.MaxStackSpaceEver(t.def) <= 0)
                {
                    return false;
                }
                return StoreUtility.TryFindBestBetterStoreCellFor(t, pawn, pawn.Map, StoreUtility.CurrentStoragePriorityOf(t), pawn.Faction, out IntVec3 foundCell);
            }
            Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerHaulables.ThingsPotentiallyNeedingHauling(), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, validator);
            if (thing != null)
            {
                return HaulAIUtility.HaulToStorageJob(pawn, thing);
            }
            return null;
        }

        public override void PostRemove()
        {
            base.PostRemove();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 180);
        }

    }

}
