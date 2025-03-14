using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;
using Verse.Sound;
using static Verse.GeneSymbolPack;

namespace WVC_XenotypesAndGenes
{

	public static class MiscUtility
	{

		public static void DoSkipEffects(IntVec3 spawnCell, Map map)
		{
			map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(spawnCell, map), spawnCell, 60);
			SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(spawnCell, map));
		}

		public static bool CanStartPregnancy_Gestator(Pawn pawn, GeneExtension_Giver giver = null)
		{
			return HediffUtility.GetFirstHediffPreventsPregnancy(pawn.health.hediffSet.hediffs) == null 
				&& (giver == null || giver.gender == Gender.None || giver.gender == pawn.gender)
				&& pawn.ageTracker?.CurLifeStage?.reproductive != false;
		}

		public static void Impregnate(Pawn pawn)
		{
			if (pawn?.genes != null)
			{
				Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, pawn);
				hediff_Pregnant.Severity = PregnancyUtility.GeneratedPawnPregnancyProgressRange.TrueMin;
				GeneSet newGeneSet = new();
				HediffComp_TrueParentGenes.AddParentGenes(pawn, newGeneSet);
				// GeneSet inheritedGeneSet = PregnancyUtility.GetInheritedGeneSet(null, pawn, out success);
				newGeneSet.SortGenes();
				hediff_Pregnant.SetParents(pawn, null, newGeneSet);
				pawn.health.AddHediff(hediff_Pregnant);
			}
		}

		public static void MakeJobWithGeneDef(Pawn pawn, JobDef jobDef, GeneDef geneDef, Thing target)
		{
			XaG_Job xaG_Job = new(JobMaker.MakeJob(jobDef, target));
			xaG_Job.geneDef = geneDef;
			pawn.jobs.TryTakeOrderedJob(xaG_Job, JobTag.Misc);
		}

		public static bool PawnDoIngestJob(Pawn pawn)
		{
			return pawn.jobs?.curJob?.def != JobDefOf.Ingest;
		}

		public static void TryTakeOrderedJob(this Pawn pawn, Job job, JobTag tag, bool requestQueueing)
		{
			if (requestQueueing && tag == JobTag.SatisfyingNeeds)
			{
				TryFinalizeAllIngestJobs(pawn, false);
				if (!pawn.jobs.curJob.playerForced && pawn.jobs.curJob.def.suspendable)
				{
					pawn.jobs.SuspendCurrentJob(JobCondition.InterruptForced);
					pawn.jobs.StartJob(job, tag: tag);
					return;
				}
				pawn.jobs.jobQueue.EnqueueFirst(job, tag);
				return;
			}
            pawn.jobs.TryTakeOrderedJob(job, tag, requestQueueing);
		}

        public static void TryFinalizeAllIngestJobs(Pawn pawn, bool finalize = true)
        {
			pawn?.jobs?.jobQueue?.RemoveAll(pawn, (Job j) => j.def == JobDefOf.Ingest || (finalize && j.GetCachedDriverDirect is IJobCustomEater eater && eater.ShouldFinalize));
        }

        public static void SummonDropPod(Map map, List<Thing> list)
		{
			if (map == null || map.IsUnderground())
			{
				return;
			}
			ActiveDropPodInfo activeDropPodInfo = new();
			activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list);
			IntVec3 cell = DropCellFinder.TradeDropSpot(map);
            DropPodUtility.MakeDropPodAt(cell, map, activeDropPodInfo);
		}

		public static bool TryAddFoodPoisoningHediff(Pawn pawn, Thing thing)
		{
			if (FoodUtility.GetFoodPoisonChanceFactor(pawn) <= 0f)
			{
				return false;
			}
			FoodUtility.AddFoodPoisoningHediff(pawn, thing, FoodPoisonCause.DangerousFoodType);
			return true;
		}

		public static bool FurskinHasMask(FurDef furDef)
		{
			foreach (BodyTypeGraphicData item in furDef.bodyTypeGraphicPaths)
			{
				if (TextureHasMask(item.texturePath))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TextureHasMask(string path)
		{
			Texture2D north = ContentFinder<Texture2D>.Get(path + "_northm", reportFailure: false);
			Texture2D east = ContentFinder<Texture2D>.Get(path + "_eastm", reportFailure: false);
			Texture2D south = ContentFinder<Texture2D>.Get(path + "_southm", reportFailure: false);
			Texture2D west = ContentFinder<Texture2D>.Get(path + "_westm", reportFailure: false);
			return west != null || south != null || east != null || north != null;
		}

		public static List<AbilityDef> ConvertAbilitiesInAbilityDefs(List<Ability> abilities)
		{
			List<AbilityDef> list = new();
			foreach (Ability item in abilities)
			{
				list.Add(item.def);
			}
			return list;
		}

		// Map

		public static bool IsUnderground(this Map map)
		{
			return map?.generatorDef?.isUnderground == true;
		}

		// Misc

		public static bool IsUnnaturalCorpse(this Thing corpse)
		{
			return corpse is UnnaturalCorpse;
		}

		public static void ReduceStack(this Thing implant)
		{
			if (implant.stackCount > 1)
			{
				implant.stackCount--;
			}
			else
			{
				implant.Destroy();
			}
		}

		// Job Misc

		public static bool TryGetAbilityJob(Pawn biter, Pawn victim, AbilityDef abilityDef, out Job job)
		{
			job = null;
			if (biter == victim)
			{
				return false;
			}
			if (biter.CurJobDef == abilityDef.jobDef)
			{
				return false;
			}
			Ability ability = biter.abilities?.GetAbility(abilityDef);
			if (ability == null || !ability.CanCast)
			{
				return false;
			}
			LocalTargetInfo target = victim;
			if (!target.IsValid)
			{
				return false;
			}
			if (!ability.CanApplyOn(target))
			{
				return false;
			}
			job = ability.GetJob(target, target);
			return true;
		}

		public static List<Ability> GetXenogenesAbilities(Pawn pawn)
		{
			List<Ability> list = new();
			List<Ability> pawnAbilities = pawn.abilities.abilities;
			List<Gene> xenogenes = pawn.genes.Xenogenes;
			foreach (Gene gene in xenogenes)
			{
				if (gene.def.abilities.NullOrEmpty())
				{
					continue;
				}
				foreach (Ability pawnAbility in pawnAbilities)
				{
					foreach (AbilityDef geneAbility in gene.def.abilities)
					{
						if (pawnAbility.def == geneAbility)
						{
							list.Add(pawnAbility);
						}
					}
				}
			}
			return list;
		}

		// Spawner

		public static void SpawnItems(Pawn pawn, ThingDef thingDef, int stack, bool showMessage = false, string message = "MessageCompSpawnerSpawnedItem")
		{
			Thing thing = ThingMaker.MakeThing(thingDef);
			thing.stackCount = stack;
			if (pawn.Map == null && pawn.Corpse != null)
			{
				GenPlace.TryPlaceThing(thing, pawn.Corpse.Position, pawn.Corpse.Map, ThingPlaceMode.Near, null, null, default);
			}
			else
			{
				GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
			}
			if (showMessage)
			{
				Messages.Message(message.Translate(thing.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
			}
		}

		// Food

		public static Thing GetSpecialFood(Pawn pawn, ThingDef foodDef)
		{
			Thing carriedThing = pawn.carryTracker.CarriedThing;
			if (carriedThing != null && carriedThing.def == foodDef)
			{
				return carriedThing;
			}
			for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
			{
				if (pawn.inventory.innerContainer[i].def == foodDef)
				{
					return pawn.inventory.innerContainer[i];
				}
			}
			return GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.listerThings.ThingsOfDef(foodDef), PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, (Thing t) => pawn.CanReserve(t) && !t.IsForbidden(pawn));
		}

		public static bool IsPsychicSensitive(this Pawn pawn)
		{
			//return pawn?.GetStatValue(StatDefOf.PsychicSensitivity, cacheStaleAfterTicks: 30000) > 0f;
			return pawn.psychicEntropy?.IsPsychicallySensitive != false;
		}

		// Precepts

		// public static List<XenotypeDef> GetAllPreferredXenotypes(Pawn pawn)
		// {
			// if (!ModLister.CheckIdeology("Precepts"))
			// {
				// return null;
			// }
			// return pawn?.ideo?.Ideo?.PreferredXenotypes;
		// }

		// IntRange

		public static bool Includes(this IntRange range, int val)
		{
			if (val >= range.min)
			{
				return val <= range.max;
			}
			return false;
		}

		// Traits

		public static bool HasAnyTraits(List<TraitDef> traitDefs, Pawn pawn)
		{
			List<Trait> traits = pawn?.story?.traits?.allTraits;
			if (traits.NullOrEmpty() || traitDefs.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < traitDefs.Count; i++)
			{
				for (int j = 0; j < traits.Count; j++)
				{
					if (traits[j].def == traitDefs[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool TraitHasAnyConflicts(List<Trait> traits, Trait trait)
		{
			foreach (Trait item in traits)
			{
				if (item.def.ConflictsWith(trait))
				{
					return true;
				}
			}
			return false;
		}

		// public static void TransferTraits(Pawn target, Pawn source)
		// {
			// foreach (Trait trait in target.story.traits.allTraits.ToList())
			// {
				// target.story.traits.RemoveTrait(trait, true);
				// if (target.story.traits.allTraits.Contains(trait) && trait.def.GetGenderSpecificCommonality(target.gender) > 0f)
				// {
					// target.story.traits.allTraits.Remove(trait);
				// }
			// }
			// foreach (Trait trait in source.story.traits.allTraits.ToList())
			// {
				// if (trait.suppressedByGene != null || trait.sourceGene != null || trait.def.GetGenderSpecificCommonality(target.gender) <= 0f)
				// {
					// continue;
				// }
				// target.story.traits.GainTrait(trait, true);
			// }
		// }

		// Shape

		// public static List<TraitDef> GetAllShiftProhibitedTraits()
		// {
			// List<TraitDef> list = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// list.AddRange(item.shapeShift_ProhibitedTraits);
			// }
			// return list;
		// }

		// public static List<PreceptDef> GetAllShiftProhibitedPrecepts()
		// {
			// List<PreceptDef> list = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// list.AddRange(item.shapeShift_ProhibitedPrecepts);
			// }
			// return list;
		// }

		// Gene Spawner

		public static List<ThingDef> GetAllThingInStuffCategory(StuffCategoryDef stuffCategoryDef)
		{
			List<ThingDef> list = new();
			foreach (ThingDef item in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (item.stuffProps != null && item.stuffProps.categories.Contains(stuffCategoryDef))
				{
					list.Add(item);
				}
			}
			return list;
		}

		// Researchs

		public static bool AllProjectsFinished(List<ResearchProjectDef> researchProjects, out ResearchProjectDef nonResearched)
		{
			nonResearched = null;
			if (!researchProjects.NullOrEmpty())
			{
				for (int i = 0; i < researchProjects?.Count; i++)
				{
					if (researchProjects[i] == null || !researchProjects[i].IsFinished)
					{
						nonResearched = researchProjects[i];
						return false;
					}
				}
				return true;
			}
			return true;
		}

		// Pawns

		public static List<Pawn> GetAllPlayerControlledMapPawns_ForBloodfeed(Pawn pawn)
		{
			List<Pawn> targets = new();
			// =
			List<Pawn> prisoners = GetAndSortPrisoners(pawn);
			targets.AddRange(prisoners);
			// =
			List<Pawn> slaves = pawn?.Map?.mapPawns?.SlavesOfColonySpawned;
			slaves.Shuffle();
			targets.AddRange(slaves);
			// =
			List<Pawn> colonists = pawn?.Map?.mapPawns?.FreeColonists;
			colonists.Shuffle();
			targets.AddRange(colonists);
			return targets;
		}

		public static List<Pawn> GetAndSortPrisoners(Pawn pawn)
		{
			List<Pawn> allPawns = new();
			List<Pawn> prisoners = pawn?.Map?.mapPawns?.PrisonersOfColony;
			List<Pawn> nonBloodfeedPrisoners = new();
			List<Pawn> bloodfeedPrisoners = new();
			foreach (Pawn prisoner in prisoners)
			{
				if (prisoner.guest.IsInteractionDisabled(PrisonerInteractionModeDefOf.Bloodfeed))
				{
					nonBloodfeedPrisoners.Add(prisoner);
				}
				else
				{
					bloodfeedPrisoners.Add(prisoner);
				}
			}
			bloodfeedPrisoners.Shuffle();
			allPawns.AddRange(bloodfeedPrisoners);
			nonBloodfeedPrisoners.Shuffle();
			allPawns.AddRange(nonBloodfeedPrisoners);
			return allPawns;
		}

		public static Pawn GetConnectedPawn(Pawn dryad)
		{
			return dryad.TryGetComp<CompGestatedDryad>()?.Master;
		}

		[Obsolete]
		public static int CountAllPlayerXenos()
		{
			int mult = 0;
			// List<Map> maps = Find.Maps;
			// for (int i = 0; i < maps.Count; i++)
			// {
				// foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				// {
					// if (!item.IsHuman())
					// {
						// continue;
					// }
					// if (XaG_GeneUtility.PawnIsBaseliner(item))
					// {
						// continue;
					// }
					// mult++;
				// }
			// }
			foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
			{
				if (!item.IsHuman())
				{
					continue;
				}
				if (XaG_GeneUtility.PawnIsBaseliner(item))
				{
					continue;
				}
				mult++;
			}
			return mult;
		}

		public static void CountAllPlayerControlledPawns_StaticCollection()
		{
			int colonists = 0;
			int xenos = 0;
			int nonHumans = 0;
			int colonyMechs = 0;
			bool anyAssignedWork = false;
			//bool leaderIsShapeshifterOrSimilar = false;
			//bool presentShapeshifter = false;
			//bool leaderIsUndead = false;
			//bool presentUndead = false;
			List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction;
			foreach (Pawn item in pawns)
			{
				if (item.IsQuestLodger())
				{
					continue;
				}
				if (!item.RaceProps.Humanlike)
				{
					if (!item.IsMutant)
					{
						nonHumans++;
					}
					if (item.RaceProps.IsMechanoid)
					{
						colonyMechs++;
					}
					continue;
				}
				if (item.IsMutant)
				{
					if (item.IsGhoul)
					{
						nonHumans++;
					}
					continue;
				}
				if (!XaG_GeneUtility.PawnIsBaseliner(item) && item.IsHuman())
				{
					xenos++;
					//if (item.IsShapeshifterChimeraOrMorpher())
					//{
					//	presentShapeshifter = true;
					//	Precept_Role precept_Role = item.Ideo?.GetRole(item);
					//	if (precept_Role != null && precept_Role.ideo == item.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
					//	{
					//		leaderIsShapeshifterOrSimilar = true;
					//	}
					//}
					//if (item.IsUndead())
					//{
					//	presentUndead = true;
					//	Precept_Role precept_Role = item.Ideo?.GetRole(item);
					//	if (precept_Role != null && precept_Role.ideo == item.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
					//	{
					//		leaderIsUndead = true;
					//	}
					//}
				}
				if (!item.IsDuplicate && !item.Deathresting && !item.IsPrisoner)
				{
					colonists++;
				}
				for (int i = 0; i < 24; i++)
				{
					if (item.timetable.GetAssignment(i) == TimeAssignmentDefOf.Work)
					{
						anyAssignedWork = true;
						break;
					}
				}
			}
			StaticCollectionsClass.cachedColonistsCount = colonists;
			StaticCollectionsClass.cachedXenotypesCount = xenos;
			StaticCollectionsClass.cachedNonHumansCount = nonHumans;
			StaticCollectionsClass.haveAssignedWork = anyAssignedWork;
			StaticCollectionsClass.cachedColonyMechs = colonyMechs;
			//StaticCollectionsClass.leaderIsShapeshifter = leaderIsShapeshifterOrSimilar;
			//StaticCollectionsClass.leaderIsShapeshifter = presentShapeshifter;
			//StaticCollectionsClass.leaderIsShapeshifter = leaderIsUndead;
			//StaticCollectionsClass.leaderIsShapeshifter = presentUndead;
			StaticCollectionsClass.oneManArmyMode = nonHumans <= 0 && colonists <= 1;
			//Log.Error("Colonists: " + colonists + ". Xenos: " + xenos + ". Non-humans: " + nonHumans + ". Mechs: " + colonyMechs);
		}

		public static void ForeverAloneDevelopmentPoints()
		{
			if (StaticCollectionsClass.cachedColonistsCount > 1 || !ModLister.IdeologyInstalled)
			{
				return;
			}
			foreach (Ideo item in Faction.OfPlayer.ideos.AllIdeos)
			{
				if (!item.PreceptsListForReading.Any((precept) => precept is Precept_Ritual))
				{
					break;
				}
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(WVC_HistoryEventDefOf.WVC_OneManArmy, Faction.OfPlayer.Named(HistoryEventArgsNames.Doer)));
			}
		}

		//[Obsolete]
		//public static int CountAllPlayerControlledColonistsExceptClonesAndQuests()
		//{
		//	int mult = 0;
		//	List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists;
		//	foreach (Pawn item in pawns)
		//	{
		//		if (item.IsQuestLodger())
		//		{
		//			continue;
		//		}
		//		if (item.IsDuplicate)
		//		{
		//			continue;
		//		}
		//		if (item.IsPrisoner)
		//		{
		//			continue;
		//		}
		//		mult++;
		//	}
		//	return mult;
		//}

		//[Obsolete]
		//public static int CountAllPlayerNonHumanlikes()
		//{
		//	int mult = 0;
		//	List<Map> maps = Find.Maps;
		//	for (int i = 0; i < maps.Count; i++)
		//	{
		//		foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
		//		{
		//			if (item.IsQuestLodger())
		//			{
		//				continue;
		//			}
		//			if (!item.RaceProps.Humanlike)
		//			{
		//				mult++;
		//			}
		//		}
		//	}
		//	return mult;
		//}

		//[Obsolete]
		//public static int CountAllPlayerMechs()
		//{
		//	int mult = 0;
		//	List<Map> maps = Find.Maps;
		//	for (int i = 0; i < maps.Count; i++)
		//	{
		//		foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
		//		{
		//			if (item.IsQuestLodger())
		//			{
		//				continue;
		//			}
		//			if (item.IsColonyMech)
		//			{
		//				mult += 1;
		//			}
		//		}
		//	}
		//	return mult;
		//}

		//[Obsolete]
		//public static int CountAllPlayerAnimals()
		//{
		//	int mult = 0;
		//	List<Map> maps = Find.Maps;
		//	for (int i = 0; i < maps.Count; i++)
		//	{
		//		foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
		//		{
		//			if (item.IsQuestLodger())
		//			{
		//				continue;
		//			}
		//			if (item.RaceProps.Animal)
		//			{
		//				mult += 1;
		//			}
		//		}
		//	}
		//	return mult;
		//}

		//public static bool PawnIsColonistOrSlave(Pawn pawn, bool shouldBeAdult = false)
		//{
		//	if ((pawn.IsColonist || pawn.IsSlaveOfColony) && (!shouldBeAdult || pawn.ageTracker.CurLifeStage.reproductive))
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		// public static bool HasReimplanterAbility(Pawn pawn)
		// {
			// List<Ability> allAbilitiesForReading = pawn?.abilities?.AllAbilitiesForReading;
			// foreach (Ability ability in allAbilitiesForReading)
			// {
				// foreach (AbilityComp comp in ability.comps)
				// {
					// if (comp is CompAbilityEffect_Reimplanter)
					// {
						// return true;
					// }
				// }
			// }
			// return false;
		// }

		public static bool BasicTargetValidation(Pawn biter, Pawn victim, bool shouldBleed = true)
		{
			if (victim == null || biter == null)
			{
				return false;
			}
			if (!ReimplanterUtility.IsHuman(victim))
			{
				return false;
			}
			if (shouldBleed)
			{
				if (ModsConfig.AnomalyActive && victim.IsMutant && !victim.mutant.Def.canBleed)
				{
					return false;
				}
			}
			if (victim.Faction != null && !victim.IsSlaveOfColony && !victim.IsPrisonerOfColony)
			{
				if (victim.Faction.HostileTo(biter.Faction))
				{
					if (!victim.Downed)
					{
						return false;
					}
				}
				else if (victim.IsQuestLodger() || victim.Faction != biter.Faction)
				{
					return false;
				}
			}
			if (victim.IsWildMan() && !victim.IsPrisonerOfColony && !victim.Downed)
			{
				return false;
			}
			if (victim.InMentalState)
			{
				return false;
			}
			return true;
		}

		// Plants

		public static void GrowSubplant(ThingWithComps parent, float maxRadius, ThingDef subplant, FloatRange? initialGrowthRange, Map map, bool canSpawnOverPlayerSownPlants = true)
		{
			IntVec3 position = parent.Position;
			int num = GenRadial.NumCellsInRadius(maxRadius);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = position + GenRadial.RadialPattern[i];
				if (!intVec.InBounds(map) || !WanderUtility.InSameRoom(position, intVec, map))
				{
					continue;
				}
				bool flag = false;
				List<Thing> thingList = intVec.GetThingList(map);
				foreach (Thing item in thingList)
				{
					if (item.def == subplant)
					{
						flag = true;
						break;
					}
					List<ThingDef> plantsToNotOverwrite = PlantsToNotOverwrite();
					if (plantsToNotOverwrite.NullOrEmpty())
					{
						continue;
					}
					for (int j = 0; j < plantsToNotOverwrite.Count; j++)
					{
						if (item.def == plantsToNotOverwrite[j])
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				if (!canSpawnOverPlayerSownPlants)
				{
					Plant plant = intVec.GetPlant(map);
					Zone zone = map.zoneManager.ZoneAt(intVec);
					if (plant != null && plant.sown && zone != null && zone is Zone_Growing)
					{
						continue;
					}
				}
				if (!subplant.CanEverPlantAt(intVec, map, canWipePlantsExceptTree: true))
				{
					continue;
				}
				for (int num2 = thingList.Count - 1; num2 >= 0; num2--)
				{
					if (thingList[num2].def.category == ThingCategory.Plant)
					{
						thingList[num2].Destroy();
					}
				}
				Plant plant2 = (Plant)GenSpawn.Spawn(subplant, intVec, map);
				if (initialGrowthRange.HasValue)
				{
					plant2.Growth = initialGrowthRange.Value.RandomInRange;
				}
				break;
			}
		}

		public static List<ThingDef> PlantsToNotOverwrite()
		{
			List<ThingDef> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.plantsToNotOverwrite_SpawnSubplant);
			}
			return list;
		}

		public static StatModifier MakeStatModifier(StatModifier statModifier)
		{
			StatModifier newStatMod = new();
			newStatMod.value = statModifier.value;
			newStatMod.stat = statModifier.stat;
			return newStatMod;
		}

		// Inherit

		public static void CopyStatsFromGeneDef(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
			// Tags
			string phase = "initial";
            try
            {
				phase = "exclusionTags";
				if (sourceGeneDef.exclusionTags != null)
                {
                    if (targetGeneDef.exclusionTags == null)
                    {
                        targetGeneDef.exclusionTags = new();
                    }
                    foreach (string item in sourceGeneDef.exclusionTags)
                    {
                        targetGeneDef.exclusionTags.Add(item);
                    }
                }
				// Log.Error("1");
				// Immunity
				phase = "makeImmuneTo";
				if (sourceGeneDef.makeImmuneTo != null)
                {
                    if (targetGeneDef.makeImmuneTo == null)
                    {
                        targetGeneDef.makeImmuneTo = new();
                    }
                    foreach (HediffDef item in sourceGeneDef.makeImmuneTo)
                    {
                        if (!targetGeneDef.makeImmuneTo.Contains(item))
                        {
                            targetGeneDef.makeImmuneTo.Add(item);
                        }
                    }
				}
				phase = "hediffGiversCannotGive";
				// Log.Error("2");
				if (sourceGeneDef.hediffGiversCannotGive != null)
                {
                    if (targetGeneDef.hediffGiversCannotGive == null)
                    {
                        targetGeneDef.hediffGiversCannotGive = new();
                    }
                    foreach (HediffDef item in sourceGeneDef.hediffGiversCannotGive)
                    {
                        if (!targetGeneDef.hediffGiversCannotGive.Contains(item))
                        {
                            targetGeneDef.hediffGiversCannotGive.Add(item);
                        }
                    }
                }
				// Log.Error("3");
				// Traits
				phase = "suppressedTraits";
				if (sourceGeneDef.suppressedTraits != null)
                {
                    if (targetGeneDef.suppressedTraits == null)
                    {
                        targetGeneDef.suppressedTraits = new();
                    }
                    foreach (GeneticTraitData item in sourceGeneDef.suppressedTraits)
                    {
						//GeneticTraitData newData = new();
						//newData.def = item.def;
						//newData.degree = item.degree;
						//geneDef.suppressedTraits.Add(newData);
                        if (!targetGeneDef.suppressedTraits.Contains(item))
                        {
                            targetGeneDef.suppressedTraits.Add(item);
                        }
                    }
                }
				// Log.Error("4");
				phase = "forcedTraits";
				if (sourceGeneDef.forcedTraits != null)
                {
                    if (targetGeneDef.forcedTraits == null)
                    {
                        targetGeneDef.forcedTraits = new();
                    }
                    foreach (GeneticTraitData item in sourceGeneDef.forcedTraits)
					{
						//GeneticTraitData newData = new();
						//newData.def = item.def;
						//newData.degree = item.degree;
						//geneDef.forcedTraits.Add(newData);
                        if (!targetGeneDef.forcedTraits.Contains(item))
                        {
                            targetGeneDef.forcedTraits.Add(item);
                        }
                    }
                }
				// Log.Error("5");
				// Stats
				phase = "conditionalStatAffecters";
				if (sourceGeneDef.conditionalStatAffecters != null)
                {
                    if (targetGeneDef.conditionalStatAffecters == null)
                    {
                        targetGeneDef.conditionalStatAffecters = new();
                    }
                    foreach (ConditionalStatAffecter item in sourceGeneDef.conditionalStatAffecters)
                    {
						targetGeneDef.conditionalStatAffecters.Add(item);
                    }
                }
				// Log.Error("6");
				phase = "statFactors";
				if (sourceGeneDef.statFactors != null)
                {
                    if (targetGeneDef.statFactors == null)
                    {
                        targetGeneDef.statFactors = new();
                    }
                    foreach (StatModifier item in sourceGeneDef.statFactors)
                    {
                        if (!targetGeneDef.statFactors.Contains(item))
                        {
                            targetGeneDef.statFactors.Add(item);
                        }
                    }
                }
				// Log.Error("7");
				phase = "statOffsets";
				if (sourceGeneDef.statOffsets != null)
                {
                    if (targetGeneDef.statOffsets == null)
                    {
                        targetGeneDef.statOffsets = new();
                    }
                    foreach (StatModifier item in sourceGeneDef.statOffsets)
                    {
                        if (!targetGeneDef.statOffsets.Contains(item))
                        {
                            targetGeneDef.statOffsets.Add(item);
                        }
                    }
                }
				// Log.Error("8");
				// Symbols
				phase = "symbolPack";
				if (sourceGeneDef.symbolPack != null)
                {
                    if (targetGeneDef.symbolPack == null)
                    {
                        targetGeneDef.symbolPack = new();
                    }
					// Log.Error("9");
					phase = "symbolPack prefixSymbols";
					if (sourceGeneDef.symbolPack.prefixSymbols != null)
					{
						if (targetGeneDef.symbolPack.prefixSymbols == null)
						{
							targetGeneDef.symbolPack.prefixSymbols = new();
						}
						foreach (WeightedSymbol item in sourceGeneDef.symbolPack.prefixSymbols)
                        {
                            targetGeneDef.symbolPack.prefixSymbols.Add(item);
                        }
                    }
					// Log.Error("10");
					phase = "symbolPack suffixSymbols";
					if (sourceGeneDef.symbolPack.suffixSymbols != null)
					{
						if (targetGeneDef.symbolPack.suffixSymbols == null)
						{
							targetGeneDef.symbolPack.suffixSymbols = new();
						}
						foreach (WeightedSymbol item in sourceGeneDef.symbolPack.suffixSymbols)
                        {
                            targetGeneDef.symbolPack.suffixSymbols.Add(item);
                        }
                    }
					// Log.Error("11");
					phase = "symbolPack wholeNameSymbols";
					if (sourceGeneDef.symbolPack.wholeNameSymbols != null)
					{
						if (targetGeneDef.symbolPack.wholeNameSymbols == null)
						{
							targetGeneDef.symbolPack.wholeNameSymbols = new();
						}
						foreach (WeightedSymbol item in sourceGeneDef.symbolPack.wholeNameSymbols)
                        {
                            targetGeneDef.symbolPack.wholeNameSymbols.Add(item);
                        }
                    }
                }
            }
            catch (Exception arg)
            {
				Log.Error("Failed inherit from gene: " + sourceGeneDef.defName + ". For gene: " + targetGeneDef.defName + ". On phase: " + phase + ". Reason: " + arg);
            }
        }

    }
}
