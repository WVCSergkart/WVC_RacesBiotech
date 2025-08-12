using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{
    public class Hediff_Duplicator : Hediff
    {

        public override bool ShouldRemove => false;
        public override bool Visible => false;
        public virtual int TictRate => 76663;

        public override void TickInterval(int delta)
        {
            if (!pawn.IsHashIntervalTick(TictRate, delta))
            {
                return;
            }
            TryRemoveOrUpdate();
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (pawn.IsDuplicate)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        public virtual void TryRemoveOrUpdate()
        {

        }

    }

    public class Hediff_DuplicatorBandwidth : Hediff_Duplicator
    {

        [Unsaved(false)]
        private Gene_Duplicator_Bandwidth cachedDuplicatorGene;
        public Gene_Duplicator_Bandwidth DuplicatorBandwidth
        {
            get
            {
                if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
                {
                    cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator_Bandwidth>();
                }
                return cachedDuplicatorGene;
            }
        }
        public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;
        public override int TictRate => 60613;

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
                        StatModifier statMod = new();
                        statMod.stat = StatDefOf.MechBandwidth;
                        statMod.value = DuplicatorBandwidth.TotalBandwidth;
                        curStage.statOffsets = new();
                        curStage.statOffsets.Add(statMod);
                    }
                }
                return curStage;
            }
        }

        public override void TryRemoveOrUpdate()
        {
            //Log.Error("0");
            if (DuplicatorBandwidth == null)
            {
                pawn.health.RemoveHediff(this);
            }
            else
            {
                Recache();
            }
        }

        public void Recache()
        {
            curStage = null;
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (DuplicatorBandwidth != null)
            {
                if (HediffUtility.TryAddHediff(def, pawn, null, null))
                {
                    if (DebugSettings.ShowDevGizmos)
                    {
                        Log.Warning("Trying to remove " + def.label + " hediff, but " + pawn.Name.ToString() + " has the required gene. Hediff is added back.");
                    }
                }
            }
        }

    }

}
