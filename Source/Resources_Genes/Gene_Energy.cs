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

		//public override bool Active
		//{
		//	get
		//	{
		//		if (!isActive)
		//		{
		//			return false;
		//		}
		//		return base.Active;
		//	}
		//}

		//private bool isActive = true;

		//public void RemoteÑontrol()
		//{
		//	isActive = !isActive;
		//	XaG_GeneUtility.Notify_GenesChanged(pawn);
		//}

  //      public string RemoteActionName
  //      {
  //          get
  //          {
  //              if (isActive)
  //              {
  //                  return "WVC_XaG_Gene_DustMechlink_On".Translate();
  //              }
  //              return "WVC_XaG_Gene_DustMechlink_Off".Translate();
  //          }
		//}

  //      public string RemoteActionDesc => "WVC_XaG_RemoteControlEnergyDesc".Translate();

        public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public GeneExtension_Opinion Opinion => def?.GetModExtension<GeneExtension_Opinion>();

        public Building_XenoCharger currentCharger;

		public override void Tick()
        {
            if (!pawn.IsHashIntervalTick(2301))
            {
                return;
			}
			if (!pawn.TryGetFood(out Need_Food food))
			{
				return;
			}
			if (food.CurLevelPercentage >= pawn.RaceProps.FoodLevelPercentageWantEat + 0.09f)
            {
                return;
            }
            if (pawn.Faction != Faction.OfPlayer)
            {
                OffsetNeedFood();
                return;
            }
            if (pawn.Map == null)
            {
                // In caravan use
                OffsetNeedFood();
                return;
            }
            if (pawn.Drafted)
            {
                return;
            }
            if (pawn.Downed || !pawn.Awake())
            {
                OffsetNeedFood();
                return;
            }
            TryRecharge(MiscUtility.PawnDoIngestJob(pawn));
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
			if (PawnHaveThisJob(pawn, Props.rechargeableStomachJobDef))
			{
				return false;
			}
			Building_XenoCharger closestCharger = GetClosestCharger(pawn, forced: false, Props.xenoChargerDef);
			if (closestCharger != null)
			{
				Job job = JobMaker.MakeJob(Props.rechargeableStomachJobDef, closestCharger);
				job.overrideFacing = Rot4.South;
				pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, requestQueueing);
				return true;
			}
			return false;
		}

		public static bool PawnHaveThisJob(Pawn pawn, JobDef job)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def == job)
				{
					return true;
				}
			}
			return false;
		}

		public static Building_XenoCharger GetClosestCharger(Pawn mech, bool forced, ThingDef thingDef)
		{
			Danger danger = (forced ? Danger.Deadly : Danger.Some);
			return (Building_XenoCharger)GenClosest.ClosestThingReachable(mech.Position, mech.Map, ThingRequest.ForDef(thingDef), PathEndMode.InteractionCell, TraverseParms.For(mech, danger), 9999f, delegate(Thing t)
			{
				Building_XenoCharger building_MechCharger = (Building_XenoCharger)t;
				if (!mech.CanReach(t, PathEndMode.InteractionCell, danger))
				{
					return false;
				}
				return !t.IsForbidden(mech) && mech.CanReserve(t, 1, -1, null, forced) && building_MechCharger.CanPawnChargeCurrently(mech);
			});
		}

		private void OffsetNeedFood()
		{
			if (Props?.foodPoisoningFromFood != false || !WVC_Biotech.settings.rechargeable_enablefoodPoisoningFromFood)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, 0.25f);
			}
		}

		public void Notify_StopCharging()
		{
			currentCharger = null;
			if (Opinion?.MeAboutThoughtDef != null)
			{
				pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.MeAboutThoughtDef);
			}
			MiscUtility.TryFinalizeAllIngestJobs(pawn);
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
            {
				return;
            }
			if (Props?.foodPoisoningFromFood == false || !WVC_Biotech.settings.rechargeable_enablefoodPoisoningFromFood)
			{
				return;
			}
			//IngestibleProperties ingestible = thing?.def?.ingestible;
			//if (ingestible != null && ingestible.CachedNutrition > 0f)
			//{
			//	GeneResourceUtility.OffsetNeedFood(pawn, -1 * ingestible.CachedNutrition);
			//}
			if (!thing.def.IsDrug)
			{
				MiscUtility.TryAddFoodPoisoningHediff(pawn, thing);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			//Scribe_Values.Look(ref isActive, "isActive", defaultValue: true);
			Scribe_References.Look(ref currentCharger, "currentCharger");
		}

	}

	public class Gene_SegmentedTail : Gene
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public override void Tick()
		{
			if (!pawn.IsHashIntervalTick(77752))
			{
				return;
			}
			TrySummonRandomMech();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryRandomSummon",
					action = delegate
					{
						if (!TrySummonRandomMech(1f))
						{
							Log.Error("Failed summon");
						}
					}
				};
			}
		}

		public bool CanDoOrbitalSummon()
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (pawn.Map == null)
			{
				return false;
			}
			if (!MechanitorUtility.IsMechanitor(pawn))
			{
				return false;
			}
			return true;
		}

		private bool TrySummonRandomMech(float chance = 0.02f)
		{
			if (!Rand.Chance(chance))
			{
				return false;
			}
			if (!CanDoOrbitalSummon())
			{
				return false;
			}
			MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
			Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
			return true;
		}

	}

}
