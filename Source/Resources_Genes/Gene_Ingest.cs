using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DustDrain : Gene
	{

		// private float cachedMaxNutrition = 0f;

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
			{
				return;
			}
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				UndeadUtility.OffsetNeedFood(pawn, (-1f * def.resourceLossPerDay) * nutrition * (float)numTaken);
			}
		}

	}

	public class Gene_Dustogenic : Gene
	{

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(540))
			{
				return;
			}
			if (!WVC_Biotech.settings.useAlternativeDustogenicFoodJob)
			{
				return;
			}
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
			for (int j = 0; j < Props.specialFoodDefs.Count; j++)
			{
				Thing specialFood = MiscUtility.GetSpecialFood(pawn, Props.specialFoodDefs[j]);
				if (specialFood == null)
				{
					continue;
				}
				if (!PawnHaveIngestJob(pawn))
				{
					Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
					job.count = 1;
					pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, pawn.jobs.curJob.def != JobDefOf.Ingest);
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
				if (item.targetA.Thing != null && Props.specialFoodDefs.Contains(item.targetA.Thing.def))
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
				if (!Props.specialFoodDefs.Contains(things[j].def))
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
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				if (Props.specialFoodDefs.Contains(thing.def) || UndeadUtility.PawnDowned(pawn))
				{
					UndeadUtility.OffsetNeedFood(pawn, 10.0f, true);
				}
				else
				{
					UndeadUtility.OffsetNeedFood(pawn, -0.1f * nutrition * (float)numTaken);
				}
			}
		}

	}

	public class Gene_SuperMetabolism : Gene
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
			{
				return;
			}
			// IngestibleProperties ingestible = thing.def.ingestible;
			// float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			// if (ingestible != null && nutrition >= 1f)
			// {
			// DustUtility.OffsetNeedFood(pawn, 10.0f * nutrition * (float)numTaken);
			// }
			UndeadUtility.OffsetNeedFood(pawn, 10.0f, true);
		}

	}

	public class Gene_SuperMetabolism_AddOrRemoveHediff : Gene_AddOrRemoveHediff
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
			{
				return;
			}
			UndeadUtility.OffsetNeedFood(pawn, 10.0f, true);
		}

	}

	// Hemogen
	public class Gene_EternalHunger : Gene_HemogenOffset
	{

		public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		// Gene

		public override void PostAdd()
		{
			base.PostAdd();
			Gene_AddOrRemoveHediff.AddOrRemoveHediff(Props?.hediffDefName, pawn, this);
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
				Gene_AddOrRemoveHediff.AddOrRemoveHediff(Props?.hediffDefName, pawn, this);
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
			if (TryHuntForFood(pawn))
			{
				Messages.Message("WVC_XaG_Gene_EternalHunger_HuntWarning".Translate(pawn.NameShortColored.ToString()), pawn, MessageTypeDefOf.NeutralEvent);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			Gene_AddOrRemoveHediff.RemoveHediff(Props?.hediffDefName, pawn);
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
							Gene_AddOrRemoveHediff.AddOrRemoveHediff(Props?.hediffDefName, pawn, this);
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

		public static bool TryHuntForFood(Pawn pawn)
		{
			List<Pawn> colonists = pawn?.Map?.mapPawns?.SpawnedPawnsInFaction(pawn.Faction);
			colonists.Shuffle();
			for (int j = 0; j < colonists.Count; j++)
			{
				Pawn colonist = colonists[j];
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, colonist))
				{
					continue;
				}
				if (MiscUtility.TryGetAbilityJob(pawn, colonist, WVC_GenesDefOf.Bloodfeed, out Job job))
				{
					if (!PawnHaveBloodHuntJob(pawn, job))
					{
						pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool PawnHaveBloodHuntJob(Pawn pawn, Job job)
		{
			foreach (Job item in pawn.jobs.AllJobs().ToList())
			{
				if (item.def == job.def && item.ability == job.ability)
				{
					return true;
				}
			}
			return false;
		}

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
				UndeadUtility.OffsetNeedFood(pawn, GetHunger(stack, specialFood, need_Food));
				Job job = JobMaker.MakeJob(JobDefOf.Ingest, specialFood);
				job.count = stack + 3;
				pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, true);
				return true;
			}
			return false;
		}

		public float GetHunger(int stack, Thing thing, Need_Food need_Food)
		{
			float nutrition = stack * thing.GetStatValue(StatDefOf.Nutrition) * pawn.GetStatValue(StatDefOf.RawNutritionFactor);
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
			}
			return foodCount;
		}

		public float GetHomegenCountFromFood(Thing thing)
		{
			return 0.175f * thing.GetStatValue(StatDefOf.Nutrition) * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor);
		}

	}

	// Health
	public class Gene_HealthStomach : Gene
	{

		public override void Notify_IngestedThing(Thing thing, int numTaken)
		{
			base.Notify_IngestedThing(thing, numTaken);
			if (!Active)
			{
				return;
			}
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				int count = GetCount(nutrition, numTaken, 0.05f);
				for (int num = 0; num < count; num++)
				{
					HealingUtility.TryHealRandomPermanentWound(pawn, this, true);
					// HealWounds();
				}
			}
		}

		public int GetCount(float nutrition, int numTaken, float percent = 0.05f)
		{
			int count = 0;
			float nutritionTaken = nutrition * numTaken;
			while (nutritionTaken > 0f)
			{
				nutritionTaken -= percent;
				count++;
			}
			return count;
		}

		// public void HealWounds()
		// {
			// List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			// for (int num = 0; num < hediffs.Count; num++)
			// {
				// if (!hediffs[num].TendableNow())
				// {
					// continue;
				// }
				// hediffs[num].Tended(TendingQualityRange.RandomInRange, TendingQualityRange.TrueMax, 1);
				// break;
			// }
		// }

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
			UndeadUtility.OffsetNeedFood(pawn, 0.1f);
		}

	}

	public class Gene_Bloodeater : Gene_Bloodfeeder
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
			Gene_EternalHunger.TryHuntForFood(pawn);
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
				SanguophageUtility.DoBite(pawn, pawns[j], 0.2f, 0.9f * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor), 0.4f, 1f, new (0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
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
			IngestibleProperties ingestible = thing.def.ingestible;
			float nutrition = thing.GetStatValue(StatDefOf.Nutrition);
			if (ingestible != null && nutrition > 0f)
			{
				FoodUtility.AddFoodPoisoningHediff(pawn, thing, FoodPoisonCause.DangerousFoodType);
			}
		}

		public override void Notify_Bloodfeed(Pawn victim)
		{
			base.Notify_Bloodfeed(victim);
			UndeadUtility.OffsetNeedFood(pawn, Props.nutritionPerBite * victim.BodySize * pawn.GetStatValue(StatDefOf.RawNutritionFactor) * pawn.GetStatValue(StatDefOf.HemogenGainFactor));
		}

	}

}
