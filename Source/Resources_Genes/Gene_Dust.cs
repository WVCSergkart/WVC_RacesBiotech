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
			DustUtility.OffsetNeedFood(pawn, 0.3f);
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
