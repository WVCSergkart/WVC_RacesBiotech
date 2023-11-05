using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DustDrain : Gene
	{

		private float cachedMaxNutrition = 0f;

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (cachedMaxNutrition <= 0f)
			{
				cachedMaxNutrition = pawn.GetStatValue(StatDefOf.MaxNutrition);
			}
			// As planned, the bonus should only apply to special food, while the debuff should apply to regular food. 
			// But since the special food is not ready yet, the bonus and debuff are common to all food.
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				DustUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * (float)numTaken * cachedMaxNutrition);
				// Log.Error(def.defName + " " + ((-1f * def.resourceLossPerDay) * nutrition * (float)numTaken * cachedMaxNutrition) + " nutrition gain");
			}
		}

	}

	public class Gene_Dust : Gene_DustDrain
	{

		// private float cachedMaxNutrition = 0f;

		// public override void Notify_IngestedThing(Thing thing, int numTaken)
		// {
			// if (cachedMaxNutrition <= 0f)
			// {
				// cachedMaxNutrition = pawn.GetStatValue(StatDefOf.MaxNutrition);
			// }
			// IngestibleProperties ingestible = thing.def.ingestible;
			// float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			// if (ingestible != null && nutrition > 0f)
			// {
				// DustUtility.OffsetNeedFood(pawn, nutrition * (float)numTaken * cachedMaxNutrition);
				// Log.Error(def.defName + " " + (nutrition * (float)numTaken * cachedMaxNutrition) + " nutrition gain");
			// }
		// }

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			base.Notify_IngestedThing(thing, numTaken);
			if (DustUtility.PawnInPronePosition(pawn))
			{
				// It is necessary to ensure that the food bar is filled
				DustUtility.OffsetNeedFood(pawn, 10.0f);
			}
			else
			{
				DustUtility.OffsetNeedFood(pawn, 0.3f);
			}
		}

	}

	// public class Gene_SuperMetabolism : Gene
	// {

		// public override void Notify_IngestedThing(Thing thing, int numTaken)
		// {
			// IngestibleProperties ingestible = thing.def.ingestible;
			// float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			// if (ingestible != null && nutrition >= 1f)
			// {
				// DustUtility.OffsetNeedFood(pawn, 10.0f * nutrition * (float)numTaken);
			// }
		// }

	// }

}
