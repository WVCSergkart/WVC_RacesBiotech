using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DustDrain : Gene
	{

		// private float cachedMaxNutrition = 0f;

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			// if (cachedMaxNutrition <= 0f)
			// {
				// cachedMaxNutrition = pawn.GetStatValue(StatDefOf.MaxNutrition);
			// }
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				DustUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * (float)numTaken);
				// Log.Error(def.defName + " " + ((-1f * def.resourceLossPerDay) * nutrition * (float)numTaken) + " nutrition gain");
			}
		}

	}

	public class Gene_Dust : Gene
	{

		public List<ThingDef> SpecialFoodDefs => def.GetModExtension<GeneExtension_Giver>().specialFoodDefs;

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
			if (!Active)
			{
				return;
			}
			base.Notify_IngestedThing(thing, numTaken);
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				if (SpecialFoodDefs.Contains(thing.def) || DustUtility.PawnInPronePosition(pawn))
				{
					// It is necessary to ensure that the food bar is filled
					DustUtility.OffsetNeedFood(pawn, 10.0f);
				}
				else
				{
					// DustUtility.OffsetNeedFood(pawn, 0.3f);
					DustUtility.OffsetNeedFood(pawn, -0.1f * nutrition * (float)numTaken);
				}
			}
		}

	}

	public class Gene_SuperMetabolism : Gene
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			// IngestibleProperties ingestible = thing.def.ingestible;
			// float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			// if (ingestible != null && nutrition >= 1f)
			// {
				// DustUtility.OffsetNeedFood(pawn, 10.0f * nutrition * (float)numTaken);
			// }
			DustUtility.OffsetNeedFood(pawn, 10.0f * (float)numTaken);
		}

	}

	public class Gene_SuperMetabolism_AddOrRemoveHediff : Gene_AddOrRemoveHediff
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			DustUtility.OffsetNeedFood(pawn, 10.0f * (float)numTaken);
		}

	}

}
