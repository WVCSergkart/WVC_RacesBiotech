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

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			if (!Active)
			{
				return;
			}
			base.Notify_IngestedThing(thing, numTaken);
			if (Undead != null && Undead.specialFoodDefs.Contains(thing.def))
			{
				return;
			}
			IngestibleProperties ingestible = thing.def.ingestible;
			// thing.def.ingestible.CachedNutrition
			float nutrition = ingestible.CachedNutrition;
			if (ingestible != null && nutrition > 0f)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * (float)numTaken);
			}
		}

	}

	[Obsolete]
	public class Gene_DustDrain : Gene_FoodEfficiency
	{


	}

	public class Gene_Dustogenic : Gene
	{

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(539))
			{
				return;
			}
			// if (!WVC_Biotech.settings.useAlternativeDustogenicFoodJob)
			// {
				// return;
			// }
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			Need_Food food = pawn?.needs?.food;
			if (food == null)
			{
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
			for (int j = 0; j < Undead.specialFoodDefs.Count; j++)
			{
				Thing specialFood = MiscUtility.GetSpecialFood(pawn, Undead.specialFoodDefs[j]);
				if (specialFood == null)
				{
					continue;
				}
				if (!PawnHaveIngestJob(pawn))
				{
					Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
					job.count = 1;
					pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, MiscUtility.PawnDoIngestJob(pawn));
				}
				break;
			}
		}

		public bool PawnHaveIngestJob(Pawn pawn)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def != JobDefOf.Ingest)
				{
					continue;
				}
				if (item.targetA.Thing != null && Undead.specialFoodDefs.Contains(item.targetA.Thing.def))
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
				if (!Undead.specialFoodDefs.Contains(things[j].def))
				{
					continue;
				}
				Notify_IngestedThing(things[j], 1);
				if (things[j] != null)
				{
					things[j].Destroy();
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
			base.Notify_IngestedThing(thing, numTaken);
			if (Undead.specialFoodDefs.Contains(thing.def) || GeneResourceUtility.PawnDowned(pawn))
			{
				GeneResourceUtility.OffsetNeedFood(pawn, 10.0f, true);
				MiscUtility.TryDebugEaterGene(pawn);
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
			if (pawn.Map == null)
			{
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			if (Resource is not Gene_Hemogen gene_hemogen || !gene_hemogen.ShouldConsumeHemogenNow())
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
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
			base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
			{
				return;
			}
			if (thing.def.IsMeat)
			{
				IngestibleProperties ingestible = thing.def.ingestible;
				if (ingestible != null)
				{
					GeneUtility.OffsetHemogen(pawn, GetHomegenCountFromFood(thing) * (float)numTaken * 1.2f);
					// Resource.Value += GetHomegenCountFromFood(thing) * (float)numTaken;
				}
			}
		}

		// Misc

		public bool TryGetFood()
		{
			Need_Food need_Food = pawn.needs?.food;
			if (need_Food == null)
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
			float hemogen = GetHomegenCountFromFood(thing);
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

		public float GetHomegenCountFromFood(Thing thing)
		{
			return 0.175f *  thing.def.ingestible.CachedNutrition * pawn.GetStatValue(StatDefOf.RawNutritionFactor, cacheStaleAfterTicks: 360000) * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000);
		}

	}

	public class Gene_HungerlessStomach : Gene_AddOrRemoveHediff
	{

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(900))
			{
				return;
			}
			GeneResourceUtility.OffsetNeedFood(pawn, 0.1f);
		}

	}

	public class Gene_Bloodeater : Gene_BloodHunter, IGeneBloodfeeder, IGeneFloatMenuOptions
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// pawn.foodRestriction;
		// }

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(2210))
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
				return;
			}
			if (pawn.Map == null)
			{
				// In caravan use
				InCaravan();
				return;
			}
			if (pawn.Downed || pawn.Drafted)
			{
				return;
			}
			TryHuntForFood(MiscUtility.PawnDoIngestJob(pawn));
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
			GeneResourceUtility.OffsetNeedFood(pawn, Props.nutritionPerBite * victim.BodySize * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000));
			MiscUtility.TryDebugEaterGene(pawn);
		}

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!pawn.Downed)
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
			if (!pawn.Downed)
			{
				yield break;
			}
			if (XaG_GeneUtility.SelectorDraftedActiveFactionMap(pawn, this))
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
