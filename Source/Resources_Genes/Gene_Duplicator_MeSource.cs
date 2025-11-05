using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Duplicator_MeSource : Gene_DuplicatorSubGene, IGeneOverridden
	{

		public override bool Active
		{
			get
			{
				if (pawn == null)
				{
					return false;
				}
				if (IsDuplicate)
				{
					return false;
				}
				return base.Active;
			}
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

		//public virtual void Notify_DuplicateDied(Pawn newDupe)
		//{

		//}

	}

	[Obsolete]
	public class Gene_DuplicatorDependant : Gene_Duplicator_MeSource
	{

	}

	// =====================================

	public class Gene_Duplicator_Skills : Gene_Duplicator_MeSource
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

	public class Gene_Duplicator_HediffGiver : Gene_Duplicator_MeSource, IGeneOverridden
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

	public class Gene_Duplicator_Psylink : Gene_Duplicator_MeSource
	{

		public override void PostAdd()
		{
			if (IsDuplicate)
			{
				return;
			}
			base.PostAdd();
			GeneResourceUtility.AddPsylink(pawn);
		}

		public override void TickInterval(int delta)
		{
			GeneResourceUtility.TryAddPsylinkRandomly(pawn, delta);
			PsyfocusOffset(delta);
		}

		private float? cachedRecoveryRate;
		public float RecoveryRate
		{
			get
			{
				if (!cachedRecoveryRate.HasValue)
				{
					GetRecoveryRate();
				}
				return 0.01f;
			}
		}

		public void PsyfocusOffset(int delta)
		{
			if (!pawn.IsHashIntervalTick(750, delta))
			{
				return;
			}
			pawn?.psychicEntropy?.OffsetPsyfocusDirectly(RecoveryRate);
			if (pawn.IsNestedHashIntervalTick(750, 2500))
			{
				cachedRecoveryRate = null;
			}
		}

		public override void Notify_DuplicateCreated(Pawn newDupe)
		{
			cachedRecoveryRate = null;
		}

		private void GetRecoveryRate()
		{
			if (Duplicator == null)
			{
				cachedRecoveryRate = 0;
				return;
			}
			if (Duplicator.PawnDuplicates.NullOrEmpty())
			{
				cachedRecoveryRate = 0;
				return;
			}
			float rate = 0f;
			foreach (Pawn dupe in Duplicator.PawnDuplicates)
			{
				if (dupe.psychicEntropy.IsCurrentlyMeditating)
				{
					rate += 1;
				}
			}
			cachedRecoveryRate = rate * 0.01f;
		}

	}

	// In Dev
	public class Gene_Duplicator_Hivemind : Gene_Duplicator_MeSource, IGeneHivemind, IGeneNonSync
	{

	}

}
