using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_FoodEfficiency : Gene
	{

		//public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			//base.Notify_IngestedThing(thing, numTaken);
			//if (Undead != null && Undead.specialFoodDefs.Contains(thing.def))
			//{
			//	return;
			//}
			IngestibleProperties ingestible = thing.def?.ingestible;
            if (ingestible == null)
            {
                return;
            }
            float nutrition = ingestible.CachedNutrition;
			if (nutrition > 0f)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * numTaken);
			}
		}

	}

	[Obsolete]
	public class Gene_DustDrain : Gene_FoodEfficiency
	{


	}

	public class Gene_Dustogenic : Gene, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(autoFeed);

		public string RemoteActionDesc => "WVC_XaG_RemoteControlDustogenicDesc".Translate();

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

		[Unsaved(false)]
		private List<IGeneRemoteControl> cachedRemoteControlGenes;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (enabled)
			{
				return XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes);
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
			if (!pawn.TryGetFood(out Need_Food food))
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
					if (things[j].stackCount > 1)
					{
						things[j].stackCount--;
					}
					else
					{
						things[j].Destroy();
					}
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

	public class Gene_SuperMetabolism : Gene
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			GeneResourceUtility.IngestedThingWithFactor(this, thing, pawn, 5 * numTaken);
		}

	}

	public class Gene_SuperMetabolism_AddOrRemoveHediff : Gene_AddOrRemoveHediff
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			GeneResourceUtility.IngestedThingWithFactor(this, thing, pawn, 5 * numTaken);
		}

	}

	// Hemogen
	public class Gene_EternalHunger : Gene_BloodHunter, IGeneOverridden
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

        // Gene

        public override void PostAdd()
		{
			base.PostAdd();
			AddOrRemoveHediff();
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			AddOrRemoveHediff();
		}

		public void Notify_Override()
		{
			AddOrRemoveHediff();
		}

		public void AddOrRemoveHediff()
		{
			HediffUtility.TryAddOrRemoveHediff(Props?.hediffDefName, pawn, this);
		}


		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(6000))
			{
				return;
			}
			if (pawn.IsHashIntervalTick(66000))
			{
				AddOrRemoveHediff();
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted || !pawn.Awake())
			{
				return;
			}
			if (Resource is not Gene_Hemogen gene_hemogen || !gene_hemogen.ShouldConsumeHemogenNow())
			{
				return;
			}
			if (TryGetFood())
			{
				return;
			}
			if (TryHuntForFood())
			{
				Messages.Message("WVC_XaG_Gene_EternalHunger_HuntWarning".Translate(pawn.NameShortColored.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			HediffUtility.TryRemoveHediff(Props?.hediffDefName, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Add Or Remove Hediff",
					action = delegate
					{
						if (Active)
						{
							AddOrRemoveHediff();
						}
					}
				};
			}
		}

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			base.Notify_IngestedThing(thing, numTaken);
			if (thing.def.IsMeat)
			{
				IngestibleProperties ingestible = thing.def.ingestible;
				if (ingestible != null)
				{
					GeneUtility.OffsetHemogen(pawn, GetHemogenCountFromFood(thing) * (float)numTaken * 1.2f);
					// Resource.Value += GetHomegenCountFromFood(thing) * (float)numTaken;
				}
			}
		}

		// Misc

		public override bool Active
		{
			get
			{
				if (!foodNeedDisbled)
				{
					return base.Active;
				}
				return false;
			}
		}

		public bool foodNeedDisbled = false;

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				foodNeedDisbled = pawn.needs?.food == null;
			}
		}

		public bool TryGetFood()
		{
			if (!pawn.TryGetFoodWithRef(out Need_Food need_Food, ref foodNeedDisbled))
			{
				return false;
			}
			List<Thing> meatFood = new();
			foreach (Thing item in pawn.Map.listerThings.AllThings.ToList())
			{
				if (item.def.ingestible != null && item.def.IsMeat)
				{
					meatFood.Add(item);
				}
			}
			for (int j = 0; j < meatFood.Count; j++)
			{
				Thing specialFood = MiscUtility.GetSpecialFood(pawn, meatFood[j].def);
				if (specialFood == null)
				{
					continue;
				}
				int stack = GetFoodCount(specialFood) > 1 ? GetFoodCount(specialFood) : 1;
				GeneResourceUtility.OffsetNeedFood(pawn, GetHunger(stack, specialFood, need_Food));
				Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
				job.count = stack + 3;
				pawn.TryTakeOrderedJob(job, JobTag.Misc, true);
				return true;
			}
			return false;
		}

		public float GetHunger(int stack, Thing thing, Need_Food need_Food)
		{
			float nutrition = stack * thing.def.ingestible.CachedNutrition * pawn.GetStatValue(StatDefOf.RawNutritionFactor, cacheStaleAfterTicks: 360000);
			float currentNeedFood = need_Food.CurLevel;
			float maxNeedFood = need_Food.MaxLevel;
			float targetFoodLevel = maxNeedFood - nutrition;
			float offset = -1 * (currentNeedFood - targetFoodLevel);
			if (offset > 0f)
			{
				offset = 0f;
			}
			return offset;
		}

		public int GetFoodCount(Thing thing)
		{
			int foodCount = 0;
			float hemogen = GetHemogenCountFromFood(thing);
			float currentHemogenLvl = Resource.Value;
			float targetHemogenLvl = Resource.targetValue;
			while (currentHemogenLvl < targetHemogenLvl)
			{
				foodCount++;
				currentHemogenLvl += hemogen;
				if (foodCount >= 75)
				{
					break;
				}
			}
			return foodCount;
		}

		public float GetHemogenCountFromFood(Thing thing)
		{
			return 0.175f *  thing.def.ingestible.CachedNutrition * pawn.GetStatValue(StatDefOf.RawNutritionFactor, cacheStaleAfterTicks: 360000) * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000);
		}

	}

    public class Gene_HungerlessStomach : Gene_AddOrRemoveHediff
    {

        public override void Tick()
        {
            base.Tick();
            if (!pawn.IsHashIntervalTick(2919))
            {
                return;
            }
            GeneResourceUtility.OffsetNeedFood(pawn, 0.1f);
        }

    }

    //[Obsolete]
    //public class Gene_HungerlessStomach : Gene
    //{



    //}

    public class Gene_Bloodeater : Gene_BloodHunter, IGeneBloodfeeder, IGeneFloatMenuOptions, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(canAutoFeed);

		public string RemoteActionDesc => "WVC_XaG_RemoteControlBloodeaterDesc".Translate();

		public void RemoteControl()
		{
			canAutoFeed = !canAutoFeed;
		}

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

		[Unsaved(false)]
		private List<IGeneRemoteControl> cachedRemoteControlGenes;


		//===========

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		// public override void PostAdd()
		// {
		// base.PostAdd();
		// pawn.foodRestriction;
		// }

		public bool canAutoFeed = true;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref canAutoFeed, "canAutoFeed", true);
		}

		public override void Tick()
        {
            base.Tick();
            if (!canAutoFeed)
            {
                return;
            }
            if (!pawn.IsHashIntervalTick(2210))
            {
                return;
			}
			if (pawn.Faction != Faction.OfPlayer)
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
            TryEat();
        }

        private void TryEat(bool queue = false)
        {
            if (pawn.Map == null)
            {
                // In caravan use
                InCaravan();
                return;
            }
            if (pawn.Downed || pawn.Drafted || !pawn.Awake())
            {
                return;
            }
            TryHuntForFood(MiscUtility.PawnDoIngestJob(pawn), queue);
        }

        private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan == null)
			{
				return;
			}
			List<Pawn> pawns = caravan.PawnsListForReading;
			if (pawns.NullOrEmpty())
			{
				return;
			}
			pawns.Shuffle();
			for (int j = 0; j < pawns.Count; j++)
			{
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, pawns[j]))
				{
					continue;
				}
				SanguophageUtility.DoBite(pawn, pawns[j], 0.2f, 0.9f * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000), 0.4f, 1f, new (0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
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
			if (Props.specialFoodDefs.Contains(thing.def))
			{
				return;
			}
			if (thing.def.IsDrug)
			{
				return;
			}
			IngestibleProperties ingestible = thing?.def?.ingestible;
			if (ingestible != null && ingestible.CachedNutrition > 0f)
			{
				if (ingestible.foodType == FoodTypeFlags.Fluid)
				{
					return;
				}
				MiscUtility.TryAddFoodPoisoningHediff(pawn, thing);
			}
		}

		public void Notify_Bloodfeed(Pawn victim)
		{
			GeneResourceUtility.OffsetNeedFood(pawn, Props.nutritionPerBite * victim.BodySize * pawn.GetStatValue(StatDefOf.MaxNutrition));
			MiscUtility.TryFinalizeAllIngestJobs(pawn);
			if (pawn.TryGetFood(out Need_Food food) && food.CurLevelPercentage < 0.85f)
			{
				//Log.Error(food.CurLevelPercentage.ToString());
				TryEat(true);
			}
        }

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, selPawn))
			{
				yield return new FloatMenuOption("WVC_NotEnoughBlood".Translate(), null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_FeedWithBlood".Translate() + " " + pawn.LabelShort, delegate
			{
				Job job = JobMaker.MakeJob(Props.bloodeaterFeedingJobDef, pawn);
				selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			}), selPawn, pawn);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo item in base.GetGizmos())
			{
				yield return item;
			}
			if (enabled)
			{
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, this, cachedRemoteControlGenes))
				{
					yield return gizmo;
				}
			}
			if (XaG_GeneUtility.ActiveDowned(pawn, this))
			{
				yield break;
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_FeedDownedBloodeaterForced".Translate(),
				defaultDesc = "WVC_FeedDownedBloodeaterForcedDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<Pawn> list2 = pawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					for (int i = 0; i < list2.Count; i++)
					{
						Pawn absorber = list2[i];
						if (absorber.genes != null 
							&& GeneFeaturesUtility.CanBloodFeedNowWith(pawn, absorber))
						{
							list.Add(new FloatMenuOption(absorber.LabelShort, delegate
							{
								Job job = JobMaker.MakeJob(Props.bloodeaterFeedingJobDef, pawn);
								absorber.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
							}, absorber, Color.white));
						}
					}
					if (!list.Any())
					{
						list.Add(new FloatMenuOption("WVC_XaG_NoSuitableTargets".Translate(), null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			};
		}

	}

}
