using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	[Obsolete]
	public class CompUseEffect_GeneShapeshifterChanger : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			//Gene_Shapeshifter shapeshifter = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
			// if (!ReimplanterUtility.IsHuman(pawn) || shapeshifter == null)
			// {
			// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
			// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			// return;
			// }
			//if (Props.disableShapeshiftComaAfterUse)
			//{
			//	shapeshifter.xenogermComaAfterShapeshift = false;
			//}
			if (Props.disableShapeshiftGenesRegrowAfterUse)
			{
				//shapeshifter.genesRegrowAfterShapeshift = false;
				pawn.genes.AddGene(Props.geneDef, true);
			}
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || !p.IsShapeshifter())
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

}
