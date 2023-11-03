using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

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
			if (Active)
			{
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int num = hediffs.Count - 1; num >= 0; num--)
				{
					if (hediffs[num].TendableNow() && !hediffs[num].IsTended())
					{
						hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
						if (Rand.Chance(0.2f))
						{
							Gene_Scarifier gene_Scarifier = pawn.genes?.GetFirstGeneOfType<Gene_Scarifier>();
							if (gene_Scarifier == null || (gene_Scarifier != null && gene_Scarifier.CanScarifyCheck()))
							{
								Gene_Scarifier.Scarify(pawn);
							}
						}
					}
				}
			}
		}
	}

}
