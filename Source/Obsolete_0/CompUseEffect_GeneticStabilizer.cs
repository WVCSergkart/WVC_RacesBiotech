using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	[Obsolete]
	public class CompUseEffect_GeneticStabilizer : CompUseEffect_GeneRestoration
	{

		public override void DoEffect(Pawn pawn)
		{
			// Gene_GeneticInstability geneticInstability = pawn?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>();
			// if (!ReimplanterUtility.IsHuman(pawn) || geneticInstability == null)
			// {
			// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
			// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			// return;
			// }
			pawn.genes.GetFirstGeneOfType<Gene_GeneticInstability>().nextTick += 60000 * Props.daysDelay;
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || p?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>() == null)
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

}
