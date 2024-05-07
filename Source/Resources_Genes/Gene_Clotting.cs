using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ClottingWithHediff : Gene_AddOrRemoveHediff
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			if (!Active)
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

	[Obsolete]
	public class Gene_MechaClotting : Gene_ClottingWithHediff
	{


	}

	public class Gene_DustClotting : Gene
	{

		private static readonly FloatRange TendingQualityRange = new(0.8f, 1.0f);

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(3200))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int num = 0; num < hediffs.Count; num++)
			{
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended() || !UndeadUtility.PawnDowned(pawn))
				{
					continue;
				}
				UndeadUtility.OffsetNeedFood(pawn, -1f * def.resourceLossPerDay);
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
			}
		}

	}

	public class Gene_ResurgentClotting : Gene_ResurgentDependent
	{
		// private const int ClotCheckInterval = 750;

		private static readonly FloatRange TendingQualityRange = new(0.5f, 1.0f);

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(1500))
			{
				return;
			}
			if (!Active)
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
				// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (Resurgent == null)
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

	public class Gene_ScarifierClotting : Gene
	{

		private static readonly FloatRange TendingQualityRange = new(0.7f, 1.0f);

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(2000))
			{
				return;
			}
			if (!Active)
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
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
				if (!ShouldScarify())
				{
					continue;
				}
				Gene_Scarifier gene_Scarifier = pawn.genes?.GetFirstGeneOfType<Gene_Scarifier>();
				if (gene_Scarifier != null && (gene_Scarifier == null || !gene_Scarifier.CanScarify))
				{
					continue;
				}
				Gene_Scarifier.Scarify(pawn);
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

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(2789))
			{
				return;
			}
			List<Pawn> connectedThings = Gauranlen?.AllDryads;
			foreach (Pawn dryad in connectedThings)
			{
				Gene_ClottingWithHediff.WoundsClotting(dryad, new(0.4f, 0.8f));
			}
		}

	}

	// Health
	public class Gene_HealingStomach : Gene
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(2317))
			{
				return;
			}
			EatWounds();
		}

		public void EatWounds()
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			float eatedDamage = 0f;
			foreach (Hediff hediff in hediffs)
			{
				if (hediff is not Hediff_Injury injury)
				{
					continue;
				}
				if (hediff.def == HediffDefOf.Scarification && WVC_Biotech.settings.totalHealingIgnoreScarification)
				{
					continue;
				}
				eatedDamage += 0.005f;
				injury.Heal(0.5f);
			}
			UndeadUtility.OffsetNeedFood(pawn, eatedDamage);
		}

	}

	// Health
	public class Gene_ArchiteSkin : Gene
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(11821))
			{
				return;
			}
			if (TryHealWounds())
			{
				TryRepairApparel();
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

		public bool TryRepairApparel()
		{
			List<Apparel> apparels = pawn.apparel.WornApparel;
			bool repairApparels = false;
			foreach (Apparel apparel in apparels)
			{
				if (apparel.def.useHitPoints && apparel.HitPoints < apparel.MaxHitPoints)
				{
					apparel.HitPoints++;
					repairApparels = true;
				}
			}
			return repairApparels;
		}

	}

}
