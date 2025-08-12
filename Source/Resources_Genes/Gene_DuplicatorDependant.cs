using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DuplicatorDependant : Gene, IGeneOverridden
	{

        public override bool Active
        {
            get
            {
                if (base.Active)
                {
					return !IsDuplicate;
                }
				return false;
            }
        }

		private bool? cachedIsDuplicate;
        public bool IsDuplicate
        {
            get
            {
				if (!cachedIsDuplicate.HasValue)
                {
					cachedIsDuplicate = pawn.IsDuplicate;
				}
                return cachedIsDuplicate.Value;
            }
        }

        private GeneExtension_Giver cachedGeneExtension;
		public GeneExtension_Giver Giver
		{
			get
			{
				if (cachedGeneExtension == null)
				{
					cachedGeneExtension = def.GetModExtension<GeneExtension_Giver>();
				}
				return cachedGeneExtension;
			}
		}

		[Unsaved(false)]
		private Gene_Duplicator cachedDuplicatorGene;
		public Gene_Duplicator Duplicator
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator>();
				}
				return cachedDuplicatorGene;
			}
		}

        public bool SourceIsAlive
        {
            get
            {
				if (Duplicator == null)
                {
					return false;
                }
                return Duplicator.SourceIsAlive;
            }
        }

        public override void TickInterval(int delta)
        {

		}

        public virtual void Notify_OverriddenBy(Gene overriddenBy)
        {

        }

        public virtual void Notify_Override()
        {
			if (IsDuplicate)
            {
				OverrideBy(this);
            }
        }

        public virtual void Notify_DuplicateCreated(Pawn newDupe)
        {

        }

    }

	public class Gene_Duplicator_Skills : Gene_DuplicatorDependant
    {

        public override void TickInterval(int delta)
        {
			if (!pawn.IsHashIntervalTick(60912, delta))
            {
				return;
            }
			DoSync();
		}

		public void DoSync()
        {
			if (Duplicator == null)
            {
				return;
            }
			Gene_HiveMind_Skills.SyncSkills(Duplicator.PawnDuplicates_WithSource);
        }

	}

    public class Gene_Duplicator_HediffGiver : Gene_DuplicatorDependant, IGeneOverridden
    {

        public override void PostAdd()
        {
            base.PostAdd();
            AddOrRemoveHediff();
            //NotifyDuplicates_AddHediff();
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            NotifyDuplicates_RemoveHediff();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            HediffUtility.TryRemoveHediff(Giver?.hediffDef, pawn);
            NotifyDuplicates_RemoveHediff();
        }

        public override void Notify_OverriddenBy(Gene overriddenBy)
        {
            base.Notify_OverriddenBy(overriddenBy);
            AddOrRemoveHediff();
            NotifyDuplicates_RemoveHediff();
        }

        public override void Notify_Override()
        {
            base.Notify_Override();
            AddOrRemoveHediff();
            //NotifyDuplicates_AddHediff();
        }

        public void AddOrRemoveHediff()
        {
            if (IsDuplicate)
            {
                return;
            }
            try
            {
                HediffUtility.TryAddOrRemoveHediff(Giver?.hediffDef, pawn, this);
            }
            catch (Exception arg)
            {
                Log.Error("Error in Gene_Duplicator_HediffGiver in def: " + def.defName + ". Pawn: " + pawn.Name + ". Reason: " + arg);
            }
        }

        //public virtual void NotifyDuplicates_AddHediff()
        //{
        //    if (Giver?.dupeHediffDef == null)
        //    {
        //        return;
        //    }
        //    if (IsDuplicate)
        //    {
        //        return;
        //    }
        //    if (Duplicator == null)
        //    {
        //        return;
        //    }
        //    if (Duplicator.PawnDuplicates.NullOrEmpty())
        //    {
        //        return;
        //    }
        //    foreach (Pawn item in Duplicator.PawnDuplicates)
        //    {
        //        HediffUtility.TryAddHediff(Giver.dupeHediffDef, item, def);
        //    }
        //}

        public virtual void NotifyDuplicates_RemoveHediff()
        {
            if (Giver?.dupeHediffDef == null)
            {
                return;
            }
            if (IsDuplicate)
            {
                return;
            }
            if (Duplicator == null)
            {
                return;
            }
            if (Duplicator.PawnDuplicates.NullOrEmpty())
            {
                return;
            }
            foreach (Pawn item in Duplicator.PawnDuplicates)
            {
                HediffUtility.TryRemoveHediff(Giver.dupeHediffDef, item);
            }
        }

    }

    public class Gene_Duplicator_Bandwidth : Gene_Duplicator_HediffGiver
    {

        public float TotalBandwidth
        {
            get
            {
                if (Duplicator == null)
                {
                    return 0f;
                }
                float bandwidth = 0f;
                foreach (Pawn item in Duplicator.PawnDuplicates)
                {
                    HediffUtility.TryRemoveHediff(Giver.dupeHediffDef, item);
                    bandwidth += item.GetStatValue(StatDefOf.MechBandwidth);
                    HediffUtility.TryAddHediff(Giver.dupeHediffDef, item, def);
                }
                return bandwidth;
            }
        }
        //private Dictionary<Pawn, float> bandwidthPerPawn;

        //private Hediff_DuplicatorBandwidth cachedHediff_DuplicatorBandwidth;
        //public Hediff_DuplicatorBandwidth Hediff_DuplicatorBandwidth
        //{
        //    get
        //    {
        //        if (cachedHediff_DuplicatorBandwidth == null)
        //        {
        //            cachedHediff_DuplicatorBandwidth = pawn.health?.hediffSet?.GetFirstHediff<Hediff_DuplicatorBandwidth>();
        //        }
        //        return cachedHediff_DuplicatorBandwidth;
        //    }
        //}

        //public float GetTotalBandwidth()
        //{
        //    if (pawn.Dead)
        //    {
        //        return 0;
        //    }
        //    if (bandwidthPerPawn == null)
        //    {
        //        NotifyDuplicates_AddHediff();
        //    }
        //    float bandwidth = 0;
        //    foreach (var item in bandwidthPerPawn)
        //    {
        //        bandwidth += item.Value;
        //    }
        //    return bandwidth;
        //}

        //public void BandwidthPerDuplicate(Pawn dupe)
        //{
        //    if (bandwidthPerPawn == null)
        //    {
        //        bandwidthPerPawn = new();
        //    }
        //    if (bandwidthPerPawn.ContainsKey(dupe))
        //    {
        //        bandwidthPerPawn.Remove(dupe);
        //    }
        //    bandwidthPerPawn[dupe] = dupe.GetStatValue(StatDefOf.MechBandwidth);
        //    //Hediff_DuplicatorBandwidth?.Recache();
        //}

        //public void Recache()
        //{
        //    Hediff_DuplicatorBandwidth?.Recache();
        //}

    }

}
