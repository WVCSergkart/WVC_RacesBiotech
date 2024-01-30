using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobGiver_GetSpecialFood : ThinkNode_JobGiver
	{

		public List<GeneDef> geneDefs;

		public List<ThingDef> foodDefs;

		public int ingestAtOnce = 1;

		// private GeneDef cachedGeneDef;

		// private bool cachedNeedSpecialFood = true;

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_GetSpecialFood obj = (JobGiver_GetSpecialFood)base.DeepCopy(resolve);
			obj.geneDefs = geneDefs;
			// obj.cachedNeedSpecialFood = cachedNeedSpecialFood;
			obj.foodDefs = foodDefs;
			obj.ingestAtOnce = ingestAtOnce;
			return obj;
		}

		public override float GetPriority(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return 0f;
			}
			if (pawn.IsBaseliner())
			{
				return 0f;
			}
			if (pawn.Downed)
			{
				return 0f;
			}
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return 0f;
			}
			if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
			{
				return 9.6f;
			}
			return 0f;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			// if (!cachedNeedSpecialFood)
			// {
				// return null;
			// }
			if (pawn.Downed)
			{
				return null;
			}
			// if (!ModsConfig.BiotechActive)
			// {
				// return null;
			// }
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return null;
			}
			// Log.Error(pawn.Name + " try (try get special food)");
			// if (geneDefs.NullOrEmpty())
			// {
				// Log.Error(pawn.Name + " geneDefs is null");
			// }
			if (XaG_GeneUtility.HasAnyActiveGene(geneDefs, pawn))
			{
				// Log.Error(pawn.Name + " try get special food");
				// int num = Mathf.FloorToInt((food.Max - gene_Hemogen.Value) / HemogenPackHemogenGain);
				if (ingestAtOnce > 0)
				{
					for (int j = 0; j < foodDefs.Count; j++)
					{
						Thing specialFood = MiscUtility.GetSpecialFood(pawn, foodDefs[j]);
						if (specialFood != null)
						{
							Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
							// job.count = Mathf.Min(specialFood.stackCount, ingestAtOnce);
							job.count = ingestAtOnce;
							// job.ingestTotalCount = true;
							// Log.Error(pawn.Name + " eat special food");
							// Log.Error(specialFood.def.defName);
							return job;
						}
					}
				}
			}
			// else if (cachedNeedSpecialFood)
			// {
				// cachedNeedSpecialFood = false;
			// }
			// Log.Error(pawn.Name + " special food is null");
			return null;
		}

	}

}
