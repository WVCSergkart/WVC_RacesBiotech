using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityBloodeater : CompProperties_AbilityEffect
	{

		public float nutritionPerBite = 0.8f;

		public CompProperties_AbilityBloodeater()
		{
			compClass = typeof(CompAbilityEffect_Bloodeater);
		}

	}

	public class CompAbilityEffect_Bloodeater : CompAbilityEffect
	{

		public new CompProperties_AbilityBloodeater Props => (CompProperties_AbilityBloodeater)props;

		[Unsaved(false)]
		private Gene_Bloodeater cachedBloodeaterGene;

		public Gene_Bloodeater Bloodeater
		{
			get
			{
				if (cachedBloodeaterGene == null || !cachedBloodeaterGene.Active)
				{
					cachedBloodeaterGene = parent.pawn?.genes?.GetFirstGeneOfType<Gene_Bloodeater>();
				}
				return cachedBloodeaterGene;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			// base.Apply(target, dest);
			if (Bloodeater != null)
			{
				Pawn pawn = parent.pawn;
				UndeadUtility.OffsetNeedFood(pawn, Props.nutritionPerBite * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor));
			}
		}

	}

}
