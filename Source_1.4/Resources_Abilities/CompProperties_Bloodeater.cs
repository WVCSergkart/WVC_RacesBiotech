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

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn bloodeater = parent.pawn;
			if (bloodeater.IsBloodeater())
			{
				UndeadUtility.OffsetNeedFood(bloodeater, Props.nutritionPerBite * bloodeater.GetStatValue(StatDefOf.RawNutritionFactor) * bloodeater.GetStatValue(StatDefOf.HemogenGainFactor));
			}
		}

	}

}
