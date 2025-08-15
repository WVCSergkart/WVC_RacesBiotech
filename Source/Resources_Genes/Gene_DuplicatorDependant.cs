using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DuplicatorDependant : Gene, IGeneOverridden
    {

        //public virtual bool IfDuplicate => false;
        public override bool Active
        {
            get
            {
                if (IsDuplicate)
                {
                    return false;
                }
                return base.Active;
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
            //if (IsDuplicate)
            //{
            //    OverrideBy(this);
            //}
        }

        public virtual void Notify_DuplicateCreated(Pawn newDupe)
        {

        }

    }

    //public class Gene_DuplicatorDependant_MeDuplicate : Gene_DuplicatorDependant
    //{

    //    public override bool IfDuplicate
    //    {
    //        get
    //        {
    //            return base.Active;
    //        }
    //    }

    //}

    public class Gene_Duplicator_DeathChain : Gene_DuplicatorDependant
    {

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            if (!Active || Duplicator == null)
            {
                return;
            }
            Duplicator.Notify_GenesChanged(null);
            foreach (Pawn dupe in Duplicator.PawnDuplicates.ToList())
            {
                if (!dupe.Dead)
                {
                    dupe.Kill(null);
                }
            }
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

        //private Hediff_DuplicatorBandwidth cachedDuplicatorHediff;
        //public Hediff_DuplicatorBandwidth Hediff
        //{
        //    get
        //    {
        //        if (cachedDuplicatorHediff == null)
        //        {
        //            cachedDuplicatorHediff = pawn.health?.hediffSet?.GetFirstHediff<Hediff_DuplicatorBandwidth>();
        //        }
        //        return cachedDuplicatorHediff;
        //    }
        //}

        //public override void Notify_DuplicateCreated(Pawn newDupe)
        //{
        //    Hediff?.Recache();
        //}

    }

    public class Gene_Duplicator_PsychicSensitivity : Gene_Duplicator_HediffGiver
    {

        private Hediff_Duplicator_PsychicSensitivity cachedDuplicatorHediff;
        public Hediff_Duplicator_PsychicSensitivity Hediff
        {
            get
            {
                if (cachedDuplicatorHediff == null)
                {
                    cachedDuplicatorHediff = pawn.health?.hediffSet?.GetFirstHediff<Hediff_Duplicator_PsychicSensitivity>();
                }
                return cachedDuplicatorHediff;
            }
        }

        public override void Notify_DuplicateCreated(Pawn newDupe)
        {
            Hediff?.Recache();
        }

    }

}
