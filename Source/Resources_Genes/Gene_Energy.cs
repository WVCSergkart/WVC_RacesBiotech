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

		public string RemoteActionDesc => "WVC_XaG_RemoteControlChargerDesc".Translate();

		public void RemoteControl()
		{
			autoFeed = !autoFeed;
		}

		public bool autoFeed = true;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.ResetAllRemoteControllers(ref cachedRemoteControlGenes);
		}

		public void RecacheGenes()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref cachedRemoteControlGenes, ref enabled);
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		private List<IGeneRemoteControl> cachedRemoteControlGenes;


		//===========

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		public GeneExtension_Opinion Opinion => def?.GetModExtension<GeneExtension_Opinion>();

        public Building_XenoCharger currentCharger;

		public override void Tick()
		{
			if (!autoFeed)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(2301))
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
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes);
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

		public void Notify_Charging(float chargePerTick, int tick = 60)
		{
			if (!GeneResourceUtility.CanTick(ref chargingTick, tick))
			{
				return;
			}
			if (pawn.needs?.food != null)
			{
				pawn.needs.food.CurLevel += chargePerTick * tick * Props.chargeSpeedFactor;
			}
			foreach (Gene gene in pawn.genes.GenesListForReading)
            {
				if (gene is IGeneChargeable charge && gene.Active)
                {
					charge.Notify_Charging(chargePerTick, tick, Props.chargeSpeedFactor);
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

    public class Gene_HemogenRecharge : Gene_HemogenOffset, IGeneChargeable, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(autoFeed);

		public string RemoteActionDesc => "WVC_XaG_RemoteControlChargerDesc".Translate();

		public void RemoteControl()
		{
			autoFeed = !autoFeed;
		}

		public bool autoFeed = true;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			XaG_UiUtility.ResetAllRemoteControllers(ref cachedRemoteControlGenes);
		}

		public void RecacheGenes()
		{
			XaG_UiUtility.RecacheRemoteController(pawn, ref cachedRemoteControlGenes, ref enabled);
		}

		public bool enabled = true;

		public void RemoteControl_Recache()
		{
			RecacheGenes();
		}

		private List<IGeneRemoteControl> cachedRemoteControlGenes;


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

		public override void Tick()
		{
			if (!autoFeed)
			{
				return;
			}
			if (!pawn.IsHashIntervalTick(3307))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (Rechargeable == null || Hemogen == null)
			{
				autoFeed = false;
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
            Hemogen.Value += chargePerTick * tick * factor;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes);
			}
			return null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref autoFeed, "autoFeed", defaultValue: true);
		}

	}

}
