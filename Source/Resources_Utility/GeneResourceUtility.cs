using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class GeneResourceUtility
	{

		public static void UpdMetabolism(Pawn pawn)
		{
			if (pawn.health.hediffSet.TryGetHediff(out HediffWithComps_Metabolism metabolism))
			{
				metabolism.Reset();
				//Log.Error("Upd");
			}
		}

		public static int GetThrallsLimit(Pawn caller, float cellsPerDay)
		{
			float limit = 0f;
			List<Pawn> colonists = caller?.Map?.mapPawns?.SpawnedPawnsInFaction(caller.Faction);
			// colonists.Shuffle();
			foreach (Pawn colonist in colonists)
			{
				Gene_Resurgent gene = colonist?.genes?.GetFirstGeneOfType<Gene_Resurgent>();
				if (gene == null)
				{
					continue;
				}
				foreach (IGeneResourceDrain drainGene in gene.GetDrainGenes)
				{
					if (drainGene.CanOffset)
					{
						// Log.Error("drainGene.ResourceLossPerDay: " + drainGene.ResourceLossPerDay.ToString());
						// limit += (int)(drainGene.ResourceLossPerDay * -100);
						limit += drainGene.ResourceLossPerDay * -1;
					}
				}
			}
			// Log.Error("Limit v0: " + limit.ToString());
			if (limit <= 0f)
			{
				return 0;
			}
			limit = (int)(Math.Round(((limit / (cellsPerDay > 0f ? cellsPerDay : 0.01f)) * 100) - 1, 0, MidpointRounding.ToEven));
			// Log.Error("Limit v1: " + limit.ToString());
			if (limit >= 1000f)
			{
				return 999;
			}
			return (int)limit;
		}

		[Obsolete]
		public static List<Gene_GeneticThrall> GetAllThralls(Pawn pawn)
		{
			List<Gene_GeneticThrall> list = new();
			List<Pawn> colonists = pawn?.Map?.mapPawns?.SpawnedPawnsInFaction(pawn.Faction);
			// colonists.Shuffle();
			if (!colonists.NullOrEmpty())
			{
				foreach (Pawn colonist in colonists)
				{
					if (colonist.Dead)
					{
						continue;
					}
					Gene_GeneticThrall thralls = colonist?.genes?.GetFirstGeneOfType<Gene_GeneticThrall>();
					if (thralls == null)
					{
						continue;
					}
					list.Add(thralls);
				}
			}
			return list;
		}

		public static void TryAddMechlinkRandomly(Pawn pawn, int delta, float chance = 0.02f)
		{
			if (!pawn.IsHashIntervalTick(71712, delta))
			{
				return;
			}
			if (!Rand.Chance(chance))
			{
				return;
			}
			if (TryAddMechlink(pawn))
			{
				if (pawn.Faction == Faction.OfPlayer)
				{
					Find.LetterStack.ReceiveLetter("WVC_XaG_GeneNaturalMechlinkLetterLabel".Translate(), "WVC_XaG_GeneNaturalMechlinkLetterDesc".Translate(pawn.Named("PAWN")), LetterDefOf.PositiveEvent, pawn);
				}
			}
		}

		public static bool TryAddMechlink(Pawn pawn)
		{
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
				return true;
			}
			return false;
		}

		// Psylinks

		public static void TryAddPsylinkRandomly(Pawn pawn, int delta, float chance = 0.02f)
		{
			if (!pawn.IsHashIntervalTick(71712, delta))
			{
				return;
			}
			if (!Rand.Chance(chance))
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
				if (pawn.Faction == Faction.OfPlayer)
				{
					Find.LetterStack.ReceiveLetter("WVC_XaG_GeneNaturalPsylinkLetterLabel".Translate(), "WVC_XaG_GeneNaturalPsylinkLetterDesc".Translate(pawn.Named("PAWN")), LetterDefOf.PositiveEvent, pawn);
				}
			}
		}

		public static void AddPsylink(Pawn pawn)
		{
			if (!WVC_Biotech.settings.link_addedPsylinkWithGene)
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier))
			{
				pawn.health.AddHediff(HediffDefOf.PsychicAmplifier, pawn.health.hediffSet.GetBrain());
				ChangePsylinkLevel(pawn);
			}
			//else
			//{
			//	pawnHadPsylinkBefore = true;
			//}
		}

		public static void ChangePsylinkLevel(Pawn pawn)
		{
			if (pawn.Spawned)
			{
				return;
			}
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier);
			if (Rand.Chance(0.34f) && firstHediffOfDef != null)
			{
				IntRange level = new(1, 5);
				((Hediff_Level)firstHediffOfDef).ChangeLevel(level.RandomInRange);
			}
		}

		public static bool CanTick(ref int nextTick, int updFreq, int delta)
		{
			nextTick -= delta;
			if (nextTick > 0)
			{
				return false;
			}
			nextTick = updFreq;
			return true;
		}

		public static bool CanDo_General(Pawn pawn, bool throwMessage = true)
		{
			if (pawn.InMentalState)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_InMentalState".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.IsQuestLodger())
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_PawnIsQuestLodgerMessage".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		public static bool CanDo_GeneralGeneticStuff(Pawn pawn, bool throwMessage = true)
		{
			if (!CanDo_General(pawn, throwMessage))
			{
				return false;
			}
			if (!pawn.health.capacities.CanBeAwake)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_IsUnconscious".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

		public static bool CanDo_ShifterGeneticStuff(Pawn pawn, bool throwMessage = true)
		{
			if (!CanDo_GeneralGeneticStuff(pawn, throwMessage))
			{
				return false;
			}
            if (pawn.health.InPainShock)
            {
				if (throwMessage)
				{
					Messages.Message("WVC_InPainShock".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
                return false;
            }
            if (pawn.Deathresting)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_IsDeathresting".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
                return false;
            }
            if (pawn.Downed && !LifeStageUtility.AlwaysDowned(pawn))
			{
				if (throwMessage)
				{
					Messages.Message("WVC_IsIncapacitated".Translate(pawn.LabelShort).CapitalizeFirst(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
                return false;
            }
            return true;
		}

		// Dust

		public static void IngestedThingWithFactor(Gene gene, Thing thing, Pawn pawn, float factor)
		{
			if (!gene.Active || thing?.def?.ingestible == null)
			{
				return;
			}
			float nutrition = thing.def.ingestible.CachedNutrition;
			if (nutrition > 0f)
			{
				GeneResourceUtility.OffsetNeedFood(pawn, nutrition * factor);
			}
		}

		public static bool TryGetNeedFood(this Pawn pawn, out Need_Food food)
		{
			food = pawn?.needs?.food;
			return food != null;
		}

		public static bool TryGetNeedFood_WithRef(this Pawn pawn, out Need_Food food, ref bool foodDisabled)
		{
			//food = pawn?.needs?.food;
			foodDisabled = !pawn.TryGetNeedFood(out food);
			return !foodDisabled;
		}

		public static bool TryOffsetNeedFood(Pawn pawn, float offset, float minOffset = 0f)
		{
			if (pawn.TryGetNeedFood(out Need_Food need_Food))
			{
                float value = Mathf.Clamp(need_Food.CurLevel + offset, 0f, need_Food.MaxLevel);
				//Log.Error(value.ToString());
				if (value < need_Food.MaxLevel - minOffset)
				{
					need_Food.CurLevel = value;
					return true;
				}
			}
			return false;
		}

		public static void OffsetNeedFood(Pawn pawn, float offset, bool alwaysMaxValue = false)
		{
			if (pawn.TryGetNeedFood(out Need_Food need_Food))
			{
				if (alwaysMaxValue)
				{
					need_Food.CurLevel = need_Food.MaxLevel;
				}
				else
				{
					// need_Food.CurLevel += need_Food.MaxLevel >= (need_Food.CurLevel + offset) ? offset : (need_Food.MaxLevel - need_Food.CurLevel);
					need_Food.CurLevel = Mathf.Clamp(need_Food.CurLevel + offset, 0f, need_Food.MaxLevel);
				}
			}
		}

		public static bool DownedSleepOrInBed(Pawn pawn)
		{
			if (pawn.Downed || pawn.Deathresting || RestUtility.InBed(pawn) || !pawn.Awake())
			{
				return true;
			}
			return false;
		}

		// Resources

		public static bool IsRawMeat(this ThingDef thingDef)
		{
			if (thingDef.category == ThingCategory.Item && thingDef.thingCategories != null)
			{
				return IsRawMeatCyclic(thingDef.thingCategories);
			}
			return thingDef.IsMeat;
		}

		private static bool IsRawMeatCyclic(List<ThingCategoryDef> thingCategories)
		{
			foreach (ThingCategoryDef category in thingCategories)
			{
				if (category == ThingCategoryDefOf.MeatRaw)
				{
					return true;
				}
				if (IsRawMeatCyclic(category))
                {
					return true;
                }
            }
			return false;
		}

		private static bool IsRawMeatCyclic(ThingCategoryDef thingCategories)
		{
			if (thingCategories == null)
			{
				return false;
			}
			if (thingCategories.parent == ThingCategoryDefOf.MeatRaw)
			{
				return true;
			}
			if (IsRawMeatCyclic(thingCategories.parent))
			{
				return true;
			}
			return false;
		}

		public static bool IsDustogenicFood(this Thing thing)
		{
			if (thing?.def?.ingestible == null)
			{
				return false;
			}
			return thing.def.GetModExtension<GeneExtension_General>()?.isDustogenic == true;
		}

		public static bool IsHemogenPack(this ThingDef thingDef)
		{
			return thingDef.IsHemogenPack(out _);
		}

		public static bool IsHemogenPack(this ThingDef thingDef, out float offsetHemogen)
		{
			offsetHemogen = 0f;
			IngestionOutcomeDoer doer = null;
			if (thingDef?.ingestible?.outcomeDoers?.TryRandomElement((IngestionOutcomeDoer outcome) => outcome is IngestionOutcomeDoer_OffsetHemogen, out doer) == true)
			{
				offsetHemogen = (doer as IngestionOutcomeDoer_OffsetHemogen).offset;
				return true;
			}
			return false;
		}

		public static bool IsHemogenPack(this Thing thing, out float offsetHemogen)
		{
            offsetHemogen = 0f;
			if (thing?.def?.IsHemogenPack(out offsetHemogen) == true)
            {
				return true;
			}
			return false;
		}

		// Shape

		//public static int GetCachedColonistsFromStaticCollection()
		//{
		//	return StaticCollectionsClass.cachedColonistsCount > 1 ? StaticCollectionsClass.cachedColonistsCount : 1;
		//}

		public static void Notify_PreShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			foreach (Gene gene in shapeshiftGene.pawn.genes.GenesListForReading)
			{
				if (gene is IGeneShapeshift geneShapeshifter && gene.Active)
				{
					geneShapeshifter.Notify_PreShapeshift(shapeshiftGene);
				}
			}
		}

		public static void Notify_PostShapeshift(Gene_Shapeshifter shapeshiftGene)
		{
			foreach (Gene gene in shapeshiftGene.pawn.genes.GenesListForReading)
			{
				if (gene is IGeneShapeshift geneShapeshifter && gene.Active)
				{
					geneShapeshifter.Notify_PostShapeshift(shapeshiftGene);
				}
			}
		}

		//[Obsolete]
		//public static void Notify_PostShapeshift_Traits(Gene_Shapeshifter shapeshiftGene)
		//{
		//	foreach (Trait trait in shapeshiftGene.pawn.story.traits.allTraits)
		//	{
		//		GeneExtension_Undead extension = trait?.def?.GetModExtension<GeneExtension_Undead>();
		//		if (extension == null)
		//		{
		//			continue;
		//		}
		//		if (!extension.thoughtDefs.NullOrEmpty())
		//		{
		//			foreach (ThoughtDef thoughtDef in extension.thoughtDefs)
		//			{
		//				shapeshiftGene.pawn.needs?.mood?.thoughts?.memories.TryGainMemory(thoughtDef);
		//			}
		//		}
		//		if (!extension.hediffDefs.NullOrEmpty())
		//		{
		//			foreach (HediffDef hediff in extension.hediffDefs)
		//			{
		//				HediffUtility.TryAddHediff(hediff, shapeshiftGene.pawn, null);
		//			}
		//		}
		//	}
		//}

		//[Obsolete]
		//public static void AddRandomTraitFromListWithChance(Pawn pawn, GeneExtension_Undead geneExtension)
		//{
		//	TraitSet traitSet = pawn.story.traits;
		//	if (geneExtension == null || traitSet.allTraits.Count > 1)
		//	{
		//		return;
		//	}
		//	GeneralHolder traitDefWithWeight = geneExtension.possibleTraits.RandomElementByWeight((GeneralHolder x) => x.weight);
		//	float chance = traitDefWithWeight.weight;
		//	Trait trait = new(traitDefWithWeight.traitDef);
		//	if (traitSet.allTraits.Contains(trait) || MiscUtility.TraitHasAnyConflicts(traitSet.allTraits, trait))
		//	{
		//		return;
		//	}
		//	if (Rand.Chance(chance))
		//	{
		//		traitSet.GainTrait(trait);
		//	}
		//}

		// Instability

		public static void OffsetInstabilityTick(Pawn pawn, int ticks)
		{
			Gene_GeneticInstability gene = pawn?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>();
			if (gene != null)
			{
				gene.nextTick += ticks;
			}
		}

		// Coma TEST

		public static void GeneUndeadResurrection(Pawn pawn, Gene_Undead gene)
		{
			// Brain test
			if (pawn?.health?.hediffSet?.GetBrain() == null)
			{
				//DuplicateUtility.NullifyBackstory(pawn);
				DuplicateUtility.NullifySkills(pawn);
			}
			// Undead Resurrect
			if (!TryResurrectWithSickness(pawn, true, 0.77f))
			{
				return;
			}
			pawn.health.AddHediff(MainDefOf.WVC_Resurgent_UndeadResurrectionRecovery);
			if (ModLister.IdeologyInstalled)
			{
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_UndeadResurrection, pawn.Named(HistoryEventArgsNames.Doer)));
			}
			// Morph
			//pawn.genes?.GetFirstGeneOfType<Gene_UndeadMorph>()?.TryMorphWithChance(null, 0.2f);
			// Letter
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				//string shapeshiftXenotype = pawn?.genes?.Xenotype != null ? pawn.genes.Xenotype.LabelCap : "ERROR";
				Find.LetterStack.ReceiveLetter(gene.LabelCap, "WVC_LetterTextSecondChance_GeneUndead".Translate(pawn.Named("PAWN"), gene.LabelCap), MainDefOf.WVC_XaG_UndeadEvent, new LookTargets(pawn));
			}
			gene.SetWorkSettings();
		}

		// Resurrection

		public static void ResurrectionSicknessWithCustomTick(Pawn innerPawn, IntRange intRange)
		{
			Hediff cooldownHediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, innerPawn);
			HediffComp_Disappears hediffComp_Disappears = cooldownHediff.TryGetComp<HediffComp_Disappears>();
			if (hediffComp_Disappears != null)
			{
				hediffComp_Disappears.ticksToDisappear = intRange.RandomInRange;
			}
			innerPawn.health.AddHediff(cooldownHediff);
		}

		public static bool TryResurrectWithSickness(Pawn pawn, bool resurrectionSickness = true, float scarsChance = 0.2f)
		{
			ResurrectionParams resurrectionParams = new();
			resurrectionParams.restoreMissingParts = true;
			resurrectionParams.noLord = true;
			resurrectionParams.removeDiedThoughts = true;
			resurrectionParams.canPickUpOpportunisticWeapons = false;
			resurrectionParams.gettingScarsChance = scarsChance;
			resurrectionParams.canKidnap = false;
			resurrectionParams.canSteal = false;
			resurrectionParams.breachers = false;
			if (ResurrectionUtility.TryResurrect(pawn, resurrectionParams) == true)
			{
				if (resurrectionSickness)
				{
					pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
				}
				//if (resurrectThought != null)
				//{
				//	pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(resurrectThought);
				//}
				return true;
			}
			else
			{
				Log.Error("Failed resurrect " + pawn.Name.ToString());
			}
			return false;
		}

		public static void ResurrectWithSickness(Pawn pawn)
		{
			TryResurrectWithSickness(pawn);
		}

		// Resource

		public static void OffsetResurgentCells(Pawn pawn, float offset)
		{
			Gene_Resurgent gene_Hemogen = pawn?.genes?.GetFirstGeneOfType<Gene_Resurgent>();
			if (gene_Hemogen != null)
			{
				OffsetResource(gene_Hemogen, offset);
			}
		}

		// ResourceUtility

		public static bool TryHuntForFood(Pawn pawn, bool requestQueueing = true, bool queue = false)
		{
			if (!queue && Gene_Rechargeable.PawnHaveThisJob(pawn, MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee))
			{
				return false;
			}
			// =
			List<Pawn> targets = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
			// =
			foreach (Pawn colonist in targets)
			{
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, colonist))
				{
					continue;
				}
				if (colonist.IsForbidden(pawn) || !pawn.CanReserveAndReach(colonist, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				//if (colonist.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
				//{
				//	continue;
				//}
				if (!MiscUtility.TryGetAbilityJob(pawn, colonist, MainDefOf.Bloodfeed, out Job job))
				{
					continue;
				}
				job.def = MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee;
				pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, requestQueueing);
				return true;
			}
			return false;
		}

		public static void TickResourceDrain(IGeneResourceDrain drain, int tick, int delta)
		{
			if (drain?.Resource != null && drain.CanOffset)
			{
				OffsetResource(drain, ((0f - drain.ResourceLossPerDay) * (float)delta / 60000f) * tick);
			}
		}

		public static void TickHemogenDrain(IGeneResourceDrain drain, int tick, int delta, bool canOffset = false)
		{
			// Log.Error("1 TickHemogenDrain " + tick.ToString() + " | 120");
			if (drain.Resource != null && canOffset)
			{
				// Log.Error("TickHemogenDrain ticks: " + tick.ToString());
				GeneResourceDrainUtility.OffsetResource(drain, ((0f - drain.ResourceLossPerDay) * (float)delta / 60000f) * tick);
			}
		}

		public static void OffsetResource(IGeneResourceDrain drain, float amnt)
		{
			// float value = drain.Resource.Value;
			drain.Resource.Value += amnt;
			// PostResourceOffset(drain, value);
		}

		public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
		{
			if (DebugSettings.ShowDevGizmos && drain.Resource != null)
			{
				Gene_Resource resource = drain.Resource;
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " -10",
					action = delegate
					{
						OffsetResource(drain, -0.1f);
					}
				};
				yield return command_Action;
				Command_Action command_Action2 = new()
				{
					defaultLabel = "DEV: " + resource.ResourceLabel + " +10",
					action = delegate
					{
						OffsetResource(drain, 0.1f);
					}
				};
				yield return command_Action2;
			}
		}

		// public static void PostResourceOffset(IGeneResourceDrain drain, float oldValue)
		// {
		// if (oldValue > 0f && drain.Resource.Value <= 0f)
		// {
		// Pawn pawn = drain.Pawn;
		// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.ResurrectionSickness))
		// {
		// pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
		// }
		// }
		// }

		public static bool IsUndead(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Undead>() != null;
		}

		public static bool IsShapeshifter(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>() != null;
		}

		public static bool IsMorpher(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Morpher>() != null;
		}

		public static bool IsChimerkin(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Chimera>() != null;
		}

		public static bool IsBloodeater(this Pawn pawn)
		{
			return pawn?.genes?.GetFirstGeneOfType<Gene_Bloodeater>() != null;
		}

		public static bool GetUndeadGene(this Pawn pawn, out Gene_Undead gene)
		{
			return (gene = pawn?.genes?.GetFirstGeneOfType<Gene_Undead>()) != null;
		}

		public static bool IsShapeshifterChimeraOrMorpher(this Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			bool isShapeshifter = false;
			foreach (Gene gene in pawn.genes.GenesListForReading)
			{
				if ((gene is Gene_Morpher || gene is Gene_Shapeshifter || gene is Gene_Chimera) && gene.Active)
				{
					isShapeshifter = true;
					break;
				}
			}
			return isShapeshifter;
		}

	}
}
