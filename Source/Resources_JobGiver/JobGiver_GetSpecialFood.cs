using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class JobGiver_GetSpecialFood : ThinkNode_JobGiver
	{

		public List<GeneDef> geneDefs;

		public List<ThingDef> foodDefs;

		public int ingestAtOnce = 1;

		public override float GetPriority(Pawn pawn)
		{
			if (!ModsConfig.BiotechActive)
			{
				return 0f;
			}
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return 0f;
			}
			if (!pawn.RaceProps.Humanlike || pawn.genes == null || pawn.genes.Xenotype == XenotypeDefOf.Baseliner)
			{
				return 0f;
			}
			if (food.CurLevelPercentage < pawn.RaceProps.FoodLevelPercentageWantEat)
			{
				return 11.5f;
			}
			return 0f;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!ModsConfig.BiotechActive)
			{
				return null;
			}
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return null;
			}
			if (!geneDefs.NullOrEmpty() && MechanoidizationUtility.HasAnyActiveGene(geneDefs, pawn))
			{
				// int num = Mathf.FloorToInt((food.Max - gene_Hemogen.Value) / HemogenPackHemogenGain);
				if (ingestAtOnce > 0)
				{
					for (int j = 0; j < foodDefs.Count; j++)
					{
						Thing specialFood = GetSpecialFood(pawn, foodDefs[j]);
						if (specialFood != null)
						{
							Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
							job.count = Mathf.Min(specialFood.stackCount, ingestAtOnce);
							job.ingestTotalCount = true;
							return job;
						}
					}
				}
			}
			return null;
		}

		private Thing GetSpecialFood(Pawn pawn, ThingDef foodDef)
		{
			Thing carriedThing = pawn.carryTracker.CarriedThing;
			if (carriedThing != null && carriedThing.def == foodDef)
			{
				return carriedThing;
			}
			for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
			{
				if (pawn.inventory.innerContainer[i].def == foodDef)
				{
					return pawn.inventory.innerContainer[i];
				}
			}
			return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(foodDef), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
		}

	}

}
