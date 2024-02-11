using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

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

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(540))
			{
				return;
			}
			if (!WVC_Biotech.settings.useAlternativeDustogenicFoodJob)
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			Need_Food food = pawn?.needs?.food;
			if (food == null)
			{
				return;
			}
			if (food.CurLevelPercentage >= pawn.RaceProps.FoodLevelPercentageWantEat + 0.12f)
			{
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			for (int j = 0; j < Props.specialFoodDefs.Count; j++)
			{
				Thing specialFood = MiscUtility.GetSpecialFood(pawn, Props.specialFoodDefs[j]);
				if (specialFood == null)
				{
					continue;
				}
				if (!PawnHaveIngestJob(pawn))
				{
					Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
					job.count = 1;
					pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, pawn.jobs.curJob.def != JobDefOf.Ingest);
				}
				break;
			}
		}

		public bool PawnHaveIngestJob(Pawn pawn)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def != JobDefOf.Ingest)
				{
					continue;
				}
				if (item.targetA.Thing != null && Props.specialFoodDefs.Contains(item.targetA.Thing.def))
				{
					// continue;
					return true;
				}
				// return true;
			}
			return false;
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Thing> things = caravan.AllThings.ToList();
			if (things.NullOrEmpty())
			{
				return;
			}
			for (int j = 0; j < things.Count; j++)
			{
				if (!Props.specialFoodDefs.Contains(things[j].def))
				{
					continue;
				}
				Notify_IngestedThing(things[j], 1);
				if (things[j] != null)
				{
					things[j].Destroy();
				}
				break;
			}
		}

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
				if (Props.specialFoodDefs.Contains(thing.def) || DustUtility.PawnInPronePosition(pawn))
				{
					DustUtility.OffsetNeedFood(pawn, 10.0f);
				}
				else
				{
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
