using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ClottingWithHediff : Gene_AddOrRemoveHediff
	{

		public override void TickInterval(int delta)
		{
			base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(1500, delta))
			{
				return;
			}
			WoundsClotting(pawn, new(0.5f, 1.0f));
		}

		public static void WoundsClotting(Pawn pawn, FloatRange tendingQualityRange)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended())
				{
					continue;
				}
				hediffs[num].Tended(tendingQualityRange.RandomInRange, tendingQualityRange.TrueMax, 1);
			}
		}

	}

	//[Obsolete]
	//public class Gene_MechaClotting : Gene_ClottingWithHediff
	//{


	//}

	public class Gene_DustClotting : Gene
	{

		private static readonly FloatRange TendingQualityRange = new(0.8f, 1.0f);

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(1500, delta))
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended() || !GeneResourceUtility.DownedSleepOrInBed(pawn))
				{
					continue;
				}
				GeneResourceUtility.OffsetNeedFood(pawn, -1f * def.resourceLossPerDay);
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
			}
		}

	}

	public class Gene_ResurgentClotting : Gene_ResurgentDependent
	{
		// private const int ClotCheckInterval = 750;

		private static readonly FloatRange TendingQualityRange = new(0.5f, 1.0f);

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(1500, delta))
			{
				return;
			}
			if (Resurgent == null)
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended())
				{
					continue;
				}
				if (!Resurgent.woundClottingAllowed || (Resurgent.Value - def.resourceLossPerDay) < 0f)
				{
					continue;
				}
				Resurgent.Value -= def.resourceLossPerDay;
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
			}
		}
	}

	public class Gene_ScarifierClotting : Gene, IGeneNotifyGenesChanged
	{

		[Unsaved(false)]
		private Gene_Scarifier cachedScarifierGene;

		public Gene_Scarifier Scarifier
		{
			get
			{
				if (cachedScarifierGene == null || !cachedScarifierGene.Active)
				{
					cachedScarifierGene = pawn?.genes?.GetFirstGeneOfType<Gene_Scarifier>();
				}
				return cachedScarifierGene;
			}
		}

		private bool? scarifierIsNull;

		private static readonly FloatRange TendingQualityRange = new(0.7f, 1.0f);

		public void Notify_GenesChanged(Gene changedGene)
		{
			scarifierIsNull = null;
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(1500, delta))
			{
				return;
			}
			if (!scarifierIsNull.HasValue)
			{
				scarifierIsNull = Scarifier == null;
			}
			bool canScarify = scarifierIsNull.Value || Scarifier.CanScarify;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended())
				{
					continue;
				}
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
				if (!canScarify)
				{
					continue;
				}
				if (!ShouldScarify())
				{
					continue;
				}
				Gene_Scarifier.Scarify(pawn);
				canScarify = scarifierIsNull.Value || Scarifier.CanScarify;
			}
		}

		private bool ShouldScarify()
		{
			int scars = pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification);
			if (Rand.Chance(scars > 0 ? 0.2f / scars : 0.2f))
			{
				return true;
			}
			return false;
		}

	}

	public class Gene_GauranlenDryads_Clotting : Gene_DryadQueen_Dependant
	{

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(2789, delta))
			{
				return;
			}
			List<Pawn> connectedThings = Gauranlen?.DryadsListForReading;
			foreach (Pawn dryad in connectedThings)
			{
				Gene_ClottingWithHediff.WoundsClotting(dryad, new(0.4f, 0.8f));
			}
		}

	}

	// Health
	public class Gene_ArchiteSkin : Gene
	{

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!pawn.IsHashIntervalTick(11821, delta))
			{
				return;
			}
			if (TryHealWounds())
			{
				RepairApparel();
			}
		}

		public bool TryHealWounds()
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			bool repairApparels = true;
			foreach (Hediff hediff in hediffs)
			{
				if (hediff is not Hediff_Injury injury)
				{
					continue;
				}
				if (injury.Part?.depth != BodyPartDepth.Outside)
				{
					continue;
				}
				if (hediff.def == HediffDefOf.Scarification && WVC_Biotech.settings.totalHealingIgnoreScarification)
				{
					continue;
				}
				repairApparels = false;
				injury.Heal(0.5f);
			}
			return repairApparels;
		}

		public void RepairApparel()
		{
			List<Apparel> apparels = pawn.apparel.WornApparel;
			foreach (Apparel apparel in apparels)
			{
				if (apparel.def.useHitPoints && apparel.HitPoints < apparel.MaxHitPoints)
				{
					apparel.HitPoints++;
				}
			}
		}

	}

	public class Gene_BleedStopper : Gene
	{

		public override void TickInterval(int delta)
		{
			//base.TickInterval(delta);
			if (!pawn.IsHashIntervalTick(220, delta))
			{
				return;
			}
			BleedStopper(pawn);
		}

		public static void BleedStopper(Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].Bleeding)
				{
					continue;
				}
				hediffs[num].ageTicks = 60000 * 15;
			}
		}

	}

}
