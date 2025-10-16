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
            if (!HivemindUtility.SuitableForHivemind(pawn))
            {
                return;
            }
            HivemindUtility.ResetCollection();
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
            nextTick = new IntRange((int)(HivemindUtility.TickRefresh * 0.9f), (int)(HivemindUtility.TickRefresh * 1.2f)).RandomInRange;
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
            if (!HivemindUtility.SuitableForHivemind(pawn))
            {
                return;
            }
            HivemindUtility.ResetCollection();
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
            List<Pawn> bondedPawns = HivemindUtility.HivemindPawns;
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

    public class Gene_Hivemind_Telepathy : Gene_Hivemind_Drone
    {

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(60001, delta))
            {
                return;
            }
            RandomInteraction();
        }

        public void RandomInteraction()
        {
            GeneInteractionsUtility.TryInteractRandomly(pawn, HivemindUtility.HivemindPawns, psychicInteraction: true, ignoreTalking: true, closeTarget: false, out _);
        }

    }

    public class Gene_Hivemind_Regeneration : Gene_Hivemind_Drone
    {

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(3335, delta))
            {
                return;
            }
            Regen();
        }

        public void Regen()
        {
            HealingUtility.Regeneration(pawn, HivemindUtility.HivemindPawns.Count * 4, 3335);
        }

    }

}
