using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MechaClotting : Gene_AddOrRemoveHediff
	{
		// private const int ClotCheckInterval = 750;

		private static readonly FloatRange TendingQualityRange = new(0.5f, 1.0f);

		public override void Tick()
		{
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
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
			}
			base.Tick();
		}
	}

	public class Gene_DustClotting : Gene
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
				if (!hediffs[num].TendableNow() || hediffs[num].IsTended() || !UndeadUtility.PawnDowned(pawn))
				{
					continue;
				}
				// Gene_Dust gene_Dust = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
				// if (gene_Dust != null)
				// {
				// }
				UndeadUtility.OffsetNeedFood(pawn, -1f * def.resourceLossPerDay);
				hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
				// hediffs[num].Heal(TendingQualityRange.RandomInRange * 2);
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

}
