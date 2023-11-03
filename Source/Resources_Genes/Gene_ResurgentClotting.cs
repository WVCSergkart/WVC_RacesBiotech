using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

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
			if (Active)
			{
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int num = hediffs.Count - 1; num >= 0; num--)
				{
					if (hediffs[num].TendableNow() && !hediffs[num].IsTended())
					{
						Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
						if (gene_Resurgent != null)
						{
							if (gene_Resurgent.woundClottingAllowed)
							{
								if ((gene_Resurgent.Value - def.resourceLossPerDay) >= 0f)
								{
									gene_Resurgent.Value -= def.resourceLossPerDay;
									hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
								}
							}
						}
					}
				}
			}
		}
	}

}
