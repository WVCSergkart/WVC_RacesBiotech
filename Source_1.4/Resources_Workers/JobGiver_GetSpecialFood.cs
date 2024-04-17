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

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_GetSpecialFood obj = (JobGiver_GetSpecialFood)base.DeepCopy(resolve);
			obj.geneDefs = geneDefs;
			obj.foodDefs = foodDefs;
			obj.ingestAtOnce = ingestAtOnce;
			return obj;
		}

		public override float GetPriority(Pawn pawn)
		{
			Need_Food food = pawn?.needs?.food;
			if (food == null)
			{
				return 0f;
			}
			if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
			{
				if (pawn?.genes == null)
				{
					return 0f;
				}
				if (pawn.Downed)
				{
					return 0f;
				}
				if (pawn.IsBaseliner())
				{
					return 0f;
				}
				return 9.6f;
			}
			return 0f;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			// if (pawn.Downed)
			// {
				// return null;
			// }
			// Need_Food food = pawn.needs.food;
			// if (food == null)
			// {
				// return null;
			// }
			if (!XaG_GeneUtility.HasAnyActiveGene(geneDefs, pawn))
			{
				return null;
			}
			if (ingestAtOnce <= 0)
			{
				return null;
			}
			for (int j = 0; j < foodDefs.Count; j++)
			{
				Thing specialFood = MiscUtility.GetSpecialFood(pawn, foodDefs[j]);
				if (specialFood == null)
				{
					continue;
				}
				Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
				job.count = ingestAtOnce;
				return job;
			}
			return null;
		}

	}

}
