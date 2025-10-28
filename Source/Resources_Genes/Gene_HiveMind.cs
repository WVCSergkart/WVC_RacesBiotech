using RimWorld;
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

        public virtual void ResetCollection()
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

        public virtual void Notify_OverriddenBy(Gene overriddenBy)
        {
            ResetCollection();
        }

        public virtual void Notify_Override()
        {
            ResetCollection();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            ResetCollection();
        }

    }

    public class Gene_Hivemind : Gene_Hivemind_Drone
    {

        public int nextTick = 2000;

        public float PsyFactor => pawn.GetStatValue(StatDefOf.PsychicSensitivity);

        public override void PostAdd()
        {
            base.PostAdd();
            if (MiscUtility.GameStarted())
            {
                HivemindUtility.ResetTick(ref nextTick);
            }
        }

        public override void TickInterval(int delta)
        {
            nextTick -= delta;
            if (nextTick > 0)
            {
                return;
            }
            HivemindUtility.ResetTick(ref nextTick);
            if (!HivemindUtility.SuitableForHivemind(pawn))
            {
                return;
            }
            UpdGeneSync();
        }

        public virtual void UpdGeneSync()
        {

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
                        UpdGeneSync();
                    }
                };
            }
        }

        //public override void ResetCollection()
        //{
        //    base.ResetCollection();
        //    if (MiscUtility.GameStarted() && pawn.InHivemind())
        //    {
        //        SyncHive();
        //    }
        //}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextTick, "nextTick", 1200);
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
            if (!HivemindUtility.InHivemind(pawn))
            {
                return;
            }
            GeneInteractionsUtility.TryInteractRandomly(pawn, HivemindUtility.HivemindPawns.Where((hiver) => hiver != pawn && hiver.Spawned).ToList(), psychicInteraction: true, ignoreTalking: true, closeTarget: false, out _);
        }

    }

    public class Gene_Hivemind_DeathChain : Gene_Hivemind_Drone
    {

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            if (!Active || !pawn.InHivemind())
            {
                return;
            }
            ResetCollection();
            foreach (Pawn hiveMember in HivemindUtility.HivemindPawns.ToList())
            {
                if (!hiveMember.Dead)
                {
                    hiveMember.Kill(null);
                }
            }
            ResetCollection();
        }

    }

    public class Gene_Hivemind_Metabolism : Gene_Hivemind_Drone
    {

        private int savedMetCount = 0;
        public int LastMetCount
        {
            get
            {
                if (MiscUtility.GameNotStarted())
                {
                    return savedMetCount;
                }
                if (savedMetCount <= 0)
                {
                    savedMetCount = HivemindUtility.HivemindPawns.Count;
                }
                return savedMetCount;
            }
        }

        public override void PostAdd()
        {
            base.PostAdd();
            if (Active && MiscUtility.GameStarted())
            {
                UpdMet();
            }
        }

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(59889, delta))
            {
                return;
            }
            if (HivemindUtility.InHivemind(pawn))
            {
                UpdMet();
            }
        }

        private void UpdMet()
        {
            def.biostatMet = LastMetCount;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref savedMetCount, "savedMetCount", 0);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                savedMetCount = LastMetCount;
            }
        }

    }

}
