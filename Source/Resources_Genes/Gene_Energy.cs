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

	public class Gene_Rechargeable : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(autoFeed);

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlChargerDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
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


		//===========

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public GeneExtension_Opinion Opinion => def?.GetModExtension<GeneExtension_Opinion>();

        public Building_XenoCharger currentCharger;

		public override void TickInterval(int delta)
		{
			if (!autoFeed)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(2301, delta))
            {
                return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				OffsetNeedFood();
				return;
			}
			if (!pawn.TryGetNeedFood(out Need_Food food))
			{
				return;
			}
			if (food.CurLevelPercentage >= pawn.RaceProps.FoodLevelPercentageWantEat + 0.09f)
            {
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
            TryRecharge(pawn, Props.rechargeableStomachJobDef, Props.xenoChargerDef, MiscUtility.PawnDoIngestJob(pawn));
        }

        public override IEnumerable<Gizmo> GetGizmos()
		{
			//if (DebugSettings.ShowDevGizmos)
			//{
			//	yield return new Command_Action
			//	{
			//		defaultLabel = "DEV: TryRecharge",
			//		action = delegate
			//		{
			//			if (!TryRecharge(pawn, Props.rechargeableStomachJobDef, Props.xenoChargerDef, false))
			//			{
			//				Log.Error("Charger is null");
			//			}
			//		}
			//	};
			//}
			//if (enabled)
			//{
			//	foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes))
			//	{
			//		yield return gizmo;
			//	}
			//}
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public static bool TryRecharge(Pawn pawn, JobDef jobDef, ThingDef thingDef, bool requestQueueing = true)
		{
			if (PawnHaveThisJob(pawn, jobDef))
			{
				return false;
			}
			Building_XenoCharger closestCharger = GetClosestCharger(pawn, forced: false, thingDef);
			if (closestCharger != null)
			{
				Job job = JobMaker.MakeJob(jobDef, closestCharger);
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
			if (Props?.foodPoisoningFromFood != false)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, 0.25f);
			}
		}

		private int chargingTick = 0;

		public void Notify_Charging(float chargePerTick, int tick)
        {
            if (!GeneResourceUtility.CanTick(ref chargingTick, tick, 1))
            {
                return;
            }
            if (pawn.needs?.food != null)
            {
                pawn.needs.food.CurLevel += chargePerTick * tick * Props.chargeSpeedFactor;
            }
            NotifySubGenes_Charging(pawn, chargePerTick, tick, Props.chargeSpeedFactor);
        }

        public static void NotifySubGenes_Charging(Pawn pawn, float chargePerTick, int tick, float chargeSpeedFactor)
        {
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is IGeneChargeable charge && gene.Active)
                {
                    charge.Notify_Charging(chargePerTick, tick, chargeSpeedFactor);
                }
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
			if (Props?.foodPoisoningFromFood == false)
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
			Scribe_Values.Look(ref autoFeed, "autoFeed", defaultValue: true);
			Scribe_References.Look(ref currentCharger, "currentCharger");
		}

	}

	public class Gene_SegmentedTail : Gene
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(77752, delta))
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

		private bool TrySummonRandomMech(float chance = 0.02f)
		{
			if (!Rand.Chance(chance))
			{
				return false;
			}
			if (!Gene_Mechlink.CanDoOrbitalSummon(pawn))
			{
				return false;
			}
			//MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
			if (MechanoidsUtility.TrySummonMechanoids(pawn, 1, Spawner.allowedMechWeightClasses, out List<Thing> summonList))
			{
				Messages.Message("WVC_RB_Gene_Summoner".Translate(), new LookTargets(summonList), MessageTypeDefOf.PositiveEvent);
			}
			return true;
		}

	}

    public class Gene_HemogenRecharge : Gene_HemogenOffset, IGeneChargeable, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(autoCharge);

		public TaggedString RemoteActionDesc => "WVC_XaG_RemoteControlHemogenRechargeDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			autoCharge = !autoCharge;
		}

		public bool autoCharge = true;

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


		//===========

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		[Unsaved(false)]
		private Gene_Rechargeable cachedRechargeGene;

		public Gene_Rechargeable Rechargeable
		{
			get
			{
				if (cachedRechargeGene == null || !cachedRechargeGene.Active)
				{
					cachedRechargeGene = pawn?.genes?.GetFirstGeneOfType<Gene_Rechargeable>();
				}
				return cachedRechargeGene;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!autoCharge)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(3307, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				autoCharge = false;
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (Rechargeable == null || Hemogen == null)
			{
				autoCharge = false;
				return;
			}
			if (!Hemogen.ShouldConsumeHemogenNow())
			{
				return;
			}
			if (pawn.Drafted || pawn.Downed || !pawn.Awake())
			{
				return;
			}
			Gene_Rechargeable.TryRecharge(pawn, Props.rechargeableStomachJobDef, Props.xenoChargerDef);
		}

		public void Notify_Charging(float chargePerTick, int tick, float factor)
		{
			if (Rechargeable == null || Hemogen == null)
			{
				return;
			}
			Hemogen.Value += chargePerTick * tick * factor;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this);
			}
			return null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref autoCharge, "autoFeed", defaultValue: true);
		}

	}

}
