using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    //[Obsolete]
    //public class Gene_HungerlessStomach : Gene
    //{



    //}

    public class Gene_Bloodeater : Gene_BloodHunter, IGeneBloodfeeder, IGeneFloatMenuOptions, IGeneRemoteControl
	{
		public string RemoteActionName => XaG_UiUtility.OnOrOff(canAutoFeed);

		public string RemoteActionDesc => "WVC_XaG_RemoteControlBloodeaterDesc".Translate();

		public void RemoteControl_Action(Dialog_GenesSettings genesSettings)
		{
			canAutoFeed = !canAutoFeed;
		}

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
			if (!pawn.TryGetNeedFood(out Need_Food food))
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
            Gene_BloodHunter.TryHuntForFood(pawn, MiscUtility.PawnDoIngestJob(pawn), queue);
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
			//if (Props.specialFoodDefs.Contains(thing.def))
			//{
			//	return;
			//}
			if (thing.IsHemogenPack(out _))
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
			if (pawn.TryGetNeedFood(out Need_Food food) && food.CurLevelPercentage < 0.85f)
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
				foreach (Gizmo gizmo in XaG_UiUtility.GetRemoteControllerGizmo(pawn, remoteControllerCached, this))
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
