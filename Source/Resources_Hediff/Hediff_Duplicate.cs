using RimWorld;
using System;
using Verse;


namespace WVC_XenotypesAndGenes
{

    public class Hediff_Duplicate : Hediff
    {

        public override bool ShouldRemove => false;
        //public override bool Visible => base.Visible;

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(88881, delta))
            {
                return;
            }
            TryRemoveOrUpdate();
        }

        public virtual void TryRemoveOrUpdate()
        {
            if (Source == null || Source.Dead)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        private Pawn cachedSource;
        public Pawn Source
        {
            get
            {
                if (cachedSource == null)
                {
                    cachedSource = pawn.GetSourceCyclic();
                }
                return cachedSource;
            }
        }

        public void Notify_SourceGeneRemoved()
        {
            pawn.health.RemoveHediff(this);
        }

        //public override void PostRemoved()
        //{
        //    base.PostRemoved();
        //    if (Source?.Dead == false)
        //    {
        //        if (HediffUtility.TryAddHediff(def, pawn, null, null))
        //        {
        //            if (DebugSettings.ShowDevGizmos)
        //            {
        //                Log.Warning("Trying to remove " + def.label + " hediff from duplicate, but " + Source.Name.ToString() + " has the required gene. Hediff is added back.");
        //            }
        //        }
        //    }
        //}

    }

    public class Hediff_DuplicateBandwidth : Hediff_Duplicate
    {

        [Unsaved(false)]
        private Gene_Duplicator_Bandwidth cachedDuplicatorGene;
        public Gene_Duplicator_Bandwidth DuplicatorBandwidth
        {
            get
            {
                if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
                {
                    cachedDuplicatorGene = Source?.genes?.GetFirstGeneOfType<Gene_Duplicator_Bandwidth>();
                }
                return cachedDuplicatorGene;
            }
        }

        public override bool Visible => false;

        private HediffStage curStage;
        public override HediffStage CurStage
        {
            get
            {
                if (curStage == null)
                {
                    curStage = new();
                    if (DuplicatorBandwidth != null)
                    {
                        DuplicatorBandwidth.BandwidthPerDuplicate(pawn);
                        curStage.statFactors = new();
                        StatModifier statMod = new();
                        statMod.stat = StatDefOf.MechBandwidth;
                        statMod.value = 0f;
                        curStage.statFactors.Add(statMod);
                    }
                }
                return curStage;
            }
        }

        public override void TryRemoveOrUpdate()
        {
            if (Source == null || DuplicatorBandwidth == null || Source.Dead)
            {
                pawn.health.RemoveHediff(this);
            }
            else
            {
                curStage = null;
                DuplicatorBandwidth.Recache();
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (Source?.Dead == false && DuplicatorBandwidth != null)
            {
                if (HediffUtility.TryAddHediff(def, pawn, null, null))
                {
                    if (DebugSettings.ShowDevGizmos)
                    {
                        Log.Warning("Trying to remove " + def.label + " hediff from duplicate, but " + Source.Name.ToString() + " has the required gene. Hediff is added back.");
                    }
                }
            }
        }

    }

}
