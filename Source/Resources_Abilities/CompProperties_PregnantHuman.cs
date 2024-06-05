using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    // public class CompProperties_AbilityPawnNutritionCost : CompProperties_AbilityEffect
	// {
		// public float nutritionCost = 0.2f;

		// public CompProperties_AbilityPawnNutritionCost()
		// {
			// compClass = typeof(CompAbilityEffect_PawnNutritionCost);
		// }

		// public override IEnumerable<string> ExtraStatSummary()
		// {
			// yield return (string)("WVC_XaG_AbilityPawnNutritionCost".Translate() + ": ") + nutritionCost;
		// }
	// }

	public class CompAbilityEffect_PregnantHuman : CompAbilityEffect
	{

		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = parent.pawn;
			if (pawn?.genes != null && pawn.ageTracker.AgeBiologicalYears >= 13)
			{
				Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, pawn);
				hediff_Pregnant.Severity = PregnancyUtility.GeneratedPawnPregnancyProgressRange.TrueMin;
				GeneSet newGeneSet = new();
				HediffComp_TrueParentGenes.AddParentGenes(pawn, newGeneSet);
				// GeneSet inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(null, pawn, out success);
				newGeneSet.SortGenes();
				hediff_Pregnant.SetParents(pawn, null, newGeneSet);
				pawn.health.AddHediff(hediff_Pregnant);
			}
		}

	}

}
