using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    //[Obsolete]
    //public class Gene_DustDrain : Gene_FoodEfficiency
    //{


    //}

    public class Gene_Dustogenic : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(autoFeed);

		public string RemoteActionDesc => "WVC_XaG_RemoteControlDustogenicDesc".Translate();

		public void RemoteControl_Action()
		{
			autoFeed = !autoFeed;
		}

		public bool autoFeed = true;

		public bool RemoteControl_Hide => !Active;

		public bool RemoteControl_Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
				remoteControllerCached = false;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.SetAllRemoteControllersTo(pawn);
		}

		public bool enabled = true;
		public bool remoteControllerCached = false;

		public void RemoteControl_Recache()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref remoteControllerCached, ref enabled);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}


		//===========

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		//public override bool Active
		//{
		//	get
		//	{
		//		if (!foodNeedDisbled)
		//		{
		//			return base.Active;
		//		}
		//		return false;
		//	}
		//}

		//public bool foodNeedDisbled = false;

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref autoFeed, "autoFeed", defaultValue: true);
		//	if (Scribe.mode == LoadSaveMode.PostLoadInit)
		//	{
		//		foodNeedDisbled = pawn.needs?.food == null;
		//	}
		//}

		private int nextTick = 539;

		public override void Tick()
		{
			//base.Tick();
			if (!autoFeed)
			{
				return;
			}
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			nextTick = 539;
			if (!pawn.TryGetNeedFood(out Need_Food food))
			{
				autoFeed = false;
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
			if (pawn.Downed || pawn.Drafted || !pawn.Awake())
			{
				nextTick = 900;
				return;
			}
			//for (int j = 0; j < Undead.specialFoodDefs.Count; j++)
			//{
			//	Thing specialFood = MiscUtility.GetSpecialFood(pawn, Undead.specialFoodDefs[j]);
			//	if (specialFood == null)
			//	{
			//		continue;
			//	}
			//	if (!PawnHaveIngestJob(pawn))
			//	{
			//		Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
			//		job.count = 1;
			//		pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, MiscUtility.PawnDoIngestJob(pawn));
			//	}
			//	break;
			//}
			Thing specialFood = GetDustFood(pawn);
			if (specialFood == null)
			{
				nextTick = 900;
				return;
			}
			if (!PawnHaveIngestJob(pawn))
			{
				Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
				job.count = 1;
				pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, MiscUtility.PawnDoIngestJob(pawn));
			}
		}

		public static Thing GetDustFood(Pawn pawn)
		{
			Thing carriedThing = pawn.carryTracker.CarriedThing;
			if (carriedThing != null && carriedThing.IsDustogenicFood())
			{
				return carriedThing;
			}
			for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
			{
				if (pawn.inventory.innerContainer[i].IsDustogenicFood())
				{
					return pawn.inventory.innerContainer[i];
				}
			}
			return GetBestDustFoodStack(pawn, false);
		}

		public static Thing GetBestDustFoodStack(Pawn pawn, bool forced)
		{
			Danger danger = (forced ? Danger.Deadly : Danger.Some);
			return (Thing)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree), PathEndMode.Touch, TraverseParms.For(pawn, danger), 9999f, delegate (Thing thing)
			{
				if (!pawn.CanReach(thing, PathEndMode.InteractionCell, danger))
				{
					return false;
				}
				return !thing.IsForbidden(pawn) && pawn.CanReserve(thing, 1, -1, null, forced) && thing.IsDustogenicFood();
			});
		}

		public bool PawnHaveIngestJob(Pawn pawn)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def != JobDefOf.Ingest)
				{
					continue;
				}
				if (item.targetA.Thing.IsDustogenicFood())
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
				if (!things[j].IsDustogenicFood())
				{
					continue;
				}
				Notify_IngestedThing(things[j], 1);
				if (things[j] != null)
				{
					things[j].ReduceStack();
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
			//base.Notify_IngestedThing(thing, numTaken);
			if (thing.IsDustogenicFood() || GeneResourceUtility.PawnDowned(pawn))
			{
				GeneResourceUtility.OffsetNeedFood(pawn, 10.0f, true);
				MiscUtility.TryFinalizeAllIngestJobs(pawn);
			}
		}

	}

}
