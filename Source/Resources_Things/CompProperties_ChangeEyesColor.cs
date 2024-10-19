using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// public class CompProperties_UseEffect_XenogermSerum : CompProperties_UseEffect
	// {



	// }
	public class IngestionOutcomeDoer_ChangeEyesColor : IngestionOutcomeDoer
	{
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
		{
			Gene_Eyes gene = pawn?.genes?.GetFirstGeneOfType<Gene_Eyes>();
			if (gene != null && gene.def.geneClass == typeof(Gene_Eyes))
			{
				gene.ChangeEyesColor();
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			yield break;
		}
	}

	//public class CompUseEffect_ChangeEyesColor : CompUseEffect
	//{

	//	public override void DoEffect(Pawn pawn)
	//	{
	//		Gene_Eyes gene = pawn?.genes?.GetFirstGeneOfType<Gene_Eyes>();
	//		if (gene != null && gene.def.geneClass == typeof(Gene_Eyes))
	//		{
	//			gene.ChangeEyesColor();
	//		}
	//	}

	//}

}
