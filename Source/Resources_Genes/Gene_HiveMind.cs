using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Hivemind_Drone : Gene, IGeneOverridden, IGeneHivemind
    {

        public override void PostAdd()
        {
            base.PostAdd();
            ResetCollection();
        }

        public void ResetCollection()
        {
            if (!pawn.IsColonist)
            {
                return;
            }
            Gene_Hivemind.ResetCollection();
        }

        public override void TickInterval(int delta)
        {

        }

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            ResetCollection();
        }

        public void Notify_Override()
        {
            ResetCollection();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            ResetCollection();
        }

    }

    public class Gene_Hivemind : Gene, IGeneOverridden, IGeneHivemind
    {

        private static List<Pawn> cachedPawns;
        public static List<Pawn> HivemindPawns
        {
            get
            {
                if (cachedPawns == null)
                {
                    cachedPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Where((target) => target.IsPsychicSensitive() && target.genes != null && target.genes.GenesListForReading.Any((gene) => gene is IGeneHivemind)).ToList();
                }
                return cachedPawns;
            }
        }

        public static void ResetCollection()
        {
            cachedPawns = null;
            cachedRefreshRate = null;
            Gene_Chimera_HiveGeneline.cachedGenelineGenes = null;
            // HediffWithComps_ChimeraLimitFromHiveMind.curStage = null; // Reset by chimera gene
        }

        private static int? cachedRefreshRate;
        public static int TickRefresh
        {
            get
            {
                if (!cachedRefreshRate.HasValue)
                {
                    cachedRefreshRate = (int)(11992 * ((HivemindPawns.Count > 1 ? HivemindPawns.Count : 5) * 0.4f));
                }
                return cachedRefreshRate.Value;
            }
        }

        public override void PostAdd()
        {
            base.PostAdd();
            UpdHive(true);
        }

        public override void Notify_NewColony()
        {
            base.Notify_NewColony();
            UpdHive(false);
        }

        public int nextTick = 2000;
        public override void TickInterval(int delta)
        {
            //if (!pawn.IsHashIntervalTick(TickRefresh, delta))
            //{
            //    return;
            //}
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            nextTick = new IntRange((int)(TickRefresh * 0.9f), (int)(TickRefresh * 1.2f)).RandomInRange;
            if (pawn.Faction != Faction.OfPlayer || !pawn.IsColonist)
            {
                return;
            }
            SyncHive();
        }

        public virtual void SyncHive()
        {
            //if (pawn.SpawnedOrAnyParentSpawned)
            //{
            //    FleckMaker.AttachedOverlay(pawn, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
            //}
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: SyncHive",
                    action = delegate
                    {
                        SyncHive();
                    }
                };
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            UpdHive(true);
        }

        private void UpdHive(bool syncHive)
        {
            if (!pawn.IsColonist)
            {
                return;
            }
            ResetCollection();
            if (syncHive && MiscUtility.GameStarted())
            {
                SyncHive();
            }
        }

        //public void Notify_GenesChanged(Gene changedGene)
        //{
        //    UpdHive(false);
        //}

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            UpdHive(false);
        }

        public void Notify_Override()
        {
            UpdHive(false);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 1200);
        }

    }

    public class Gene_HiveMind_Opinion : Gene_Hivemind
    {

        private GeneExtension_Opinion cachedExtension;
        public GeneExtension_Opinion Opinion
        {
            get
            {
                if (cachedExtension == null)
                {
                    cachedExtension = def?.GetModExtension<GeneExtension_Opinion>();
                }
                return cachedExtension;
            }
        }

        public override void SyncHive()
        {
            base.SyncHive();
            List<Pawn> bondedPawns = HivemindPawns;
            //string phase = "start";
            try
            {
                foreach (Pawn otherPawn in bondedPawns)
                {
                    if (otherPawn == pawn)
                    {
                        continue;
                    }
                    //phase = otherPawn.NameShortColored.ToString();
                    otherPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, pawn);
                    //phase = pawn.NameShortColored.ToString();
                    pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, otherPawn);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed sync hive opinions. Reason: " + arg);
            }
        }

    }

    public class Gene_Hivemind_Mood : Gene_HiveMind_Opinion
    {

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(59998, delta))
            {
                return;
            }
            SyncHive();
        }

        public override void SyncHive()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.thoughtDef);
        }

    }

    public class Gene_Hivemind_SyncNode : Gene_Hivemind_Drone
    {

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(59997, delta))
            {
                return;
            }
            ResetCollection();
        }

    }

}
