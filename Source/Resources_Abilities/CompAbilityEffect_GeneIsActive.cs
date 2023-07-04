using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_GeneIsActive : CompAbilityEffect
	{
		public new CompProperties_AbilityGeneIsActive Props => (CompProperties_AbilityGeneIsActive)props;

		public override bool GizmoDisabled(out string reason)
		{
			Pawn pawn = parent.pawn;
			if (pawn?.genes == null)
			{
				reason = "WVC_XaG_AbilityGeneIsActive_PawnBaseliner".Translate(pawn);
				return true;
			}
			if (Props.eachOfGenes != null && Props.eachOfGenes.Count > 0)
			{
				foreach (GeneDef allSelectedItem in Props.eachOfGenes)
				{
					if (!MechanoidizationUtility.HasActiveGene(allSelectedItem, pawn))
					{
						reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn) + ": " + allSelectedItem.label;
						return true;
					}
				}
			}
			if (Props.anyOfGenes != null && Props.anyOfGenes.Count > 0)
			{
				foreach (GeneDef allSelectedItem in Props.anyOfGenes)
				{
					if (MechanoidizationUtility.HasActiveGene(allSelectedItem, pawn))
					{
						reason = null;
						return false;
					}
				}
				reason = "WVC_XaG_AbilityGeneIsActive_PawnNotHaveGene".Translate(pawn);
				return true;
			}
			reason = null;
			return false;
		}
	}

}
