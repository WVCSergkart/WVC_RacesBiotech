using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Rechargeable : Gene
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public Building_XenoCharger currentCharger;

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(2301))
			{
				return;
			}
			Need_Food food = pawn?.needs?.food;
			if (food == null)
			{
				return;
			}
			if (food.CurLevelPercentage >= pawn.RaceProps.FoodLevelPercentageWantEat + 0.09f)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				UndeadUtility.OffsetNeedFood(pawn, 0.2f);
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			if (pawn.Drafted)
			{
				return;
			}
			if (pawn.Downed)
			{
				UndeadUtility.OffsetNeedFood(pawn, 0.1f);
				return;
			}
			TryRecharge();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryRecharge",
					// defaultLabel = "WVC_ForceRecharge".Translate(),
					// defaultDesc = "WVC_ForceRechargeDesc".Translate(),
					// icon = ContentFinder<Texture2D>.Get(def.iconPath),
					action = delegate
					{
						if (!TryRecharge(false))
						{
							Log.Error("Charger is null");
						}
					}
				};
			}
		}

		public virtual bool TryRecharge(bool requestQueueing = true)
		{
			Building_XenoCharger closestCharger = GetClosestCharger(pawn, pawn, forced: false, Props.xenoChargerDef);
			if (closestCharger != null)
			{
				Job job = JobMaker.MakeJob(Props.rechargeableStomachJobDef, closestCharger);
				job.overrideFacing = Rot4.South;
				if (PawnHaveThisJob(pawn, job))
				{
					return false;
				}
				pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, requestQueueing);
				return true;
			}
			return false;
		}

		public static bool PawnHaveThisJob(Pawn pawn, Job job)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def == job.def)
				{
					return true;
				}
			}
			return false;
		}

		public static Building_XenoCharger GetClosestCharger(Pawn mech, Pawn carrier, bool forced, ThingDef thingDef)
		{
			Danger danger = (forced ? Danger.Deadly : Danger.Some);
			return (Building_XenoCharger)GenClosest.ClosestThingReachable(mech.Position, mech.Map, ThingRequest.ForDef(thingDef), PathEndMode.InteractionCell, TraverseParms.For(carrier, danger), 9999f, delegate(Thing t)
			{
				Building_XenoCharger building_MechCharger = (Building_XenoCharger)t;
				if (!carrier.CanReach(t, PathEndMode.InteractionCell, danger))
				{
					return false;
				}
				return !t.IsForbidden(carrier) && carrier.CanReserve(t, 1, -1, null, forced) && building_MechCharger.CanPawnChargeCurrently(mech);
			});
		}

		private void InCaravan()
		{
			UndeadUtility.OffsetNeedFood(pawn, 0.25f);
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (Props?.foodPoisoningFromFood == false || !WVC_Biotech.settings.rechargeable_enablefoodPoisoningFromFood)
			{
				return;
			}
			IngestibleProperties ingestible = thing?.def?.ingestible;
			if (ingestible != null && ingestible.CachedNutrition > 0f)
			{
				UndeadUtility.OffsetNeedFood(pawn, -1 * ingestible.CachedNutrition);
			}
			if (!thing.def.IsDrug)
			{
				FoodUtility.AddFoodPoisoningHediff(pawn, thing, FoodPoisonCause.DangerousFoodType);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref currentCharger, "currentCharger");
		}

	}

}