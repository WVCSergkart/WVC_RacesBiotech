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

		public static bool CanBleed(this Pawn pawn)
		{
			if (!pawn.health.CanBleed)
            {
				return false;
            }
			return !pawn.RaceProps.Dryad;
		}

		public static bool InSpace(this Pawn pawn)
		{
			return pawn.Map?.Biome == BiomeDefOf.Space;
		}

		public static bool GameStarted()
		{
			return !GameNotStarted();
		}

		public static bool GameNotStarted()
		{
			return Current.ProgramState != ProgramState.Playing;
		}

		//public static void BackCompatability()
		//{
		//	foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList())
		//	{
		//		if (pawn.genes == null)
		//		{
		//			continue;
		//		}
		//		List<GeneDef> allDefsListForReading = DefDatabase<GeneDef>.AllDefsListForReading;
		//		foreach (Gene gene in pawn.genes.GenesListForReading.ToList())
		//		{
		//			bool xenogene = pawn.genes.IsXenogene(gene);
		//			string backCompt = MiscUtility.BackCompatibleDefName(gene.def.GetType(), gene.def.defName);
		//			if (backCompt != null)
		//			{
		//				pawn.genes.RemoveGene(gene);
		//				pawn.genes.AddGene(allDefsListForReading.Where((def) => def.defName == backCompt).ToList().First(), xenogene);
		//			}
		//		}
		//		ReimplanterUtility.PostImplantDebug(pawn);
		//	}
		//}

		//public static string BackCompatibleDefName(Type defType, string defName)
		//{
		//	if (defType == typeof(GeneDef))
		//	{
		//		if (defName == "WVC_CrossPlate_Yellow" || defName == "WVC_FacelessEyes_ffffff_HEX" || defName == "WVC_FacelessEyes_afafaf_HEX" || defName == "WVC_FacelessEyes_afafaf_HEX" || defName == "WVC_FacelessEyes_7c69ce_HEX" || defName == "WVC_FacelessEyes_69a2ce_HEX" || defName == "WVC_FacelessEyes_69ce7a_HEX" || defName == "WVC_FacelessEyes_c8ce69_HEX" || defName == "WVC_FacelessEyes_ce69c3_HEX" || defName == "WVC_FacelessEyes_ce6969_HEX" || defName == "WVC_FacelessEyes_ceae69_HEX" || defName == "WVC_FacelessEyes_69cec6_HEX")
		//		{
		//			return "WVC_Eyes_Holoeyes";
		//		}
		//		if (defName == "WVC_Eyes_ffffff_HEX" || defName == "WVC_Eyes_afafaf_HEX" || defName == "WVC_Eyes_7c69ce_HEX" || defName == "WVC_Eyes_69a2ce_HEX" || defName == "WVC_FacelessEyes_7c69ce_HEX" || defName == "WVC_Eyes_69ce7a_HEX" || defName == "WVC_Eyes_c8ce69_HEX" || defName == "WVC_Eyes_ce69c3_HEX" || defName == "WVC_Eyes_ce6969_HEX" || defName == "WVC_Eyes_ceae69_HEX" || defName == "WVC_Eyes_69cec6_HEX")
		//		{
		//			return "WVC_Eyes_Colorful";
		//		}
		//		if (defName == "WVC_GenePackSpawner_Vanilla" || defName == "WVC_GenePackSpawner_Base" || defName == "WVC_GenePackSpawner_Ultra" || defName == "WVC_GenePackSpawner_Mecha" || defName == "WVC_GenePackSpawner_AlphaBase" || defName == "WVC_GenePackSpawner_AlphaMixed" || defName == "WVC_GenePackSpawner_Disable" || defName == "WVC_XenotypeSerumSpawner_Random" || defName == "WVC_XenotypeSerumSpawner_HybridRandom")
		//		{
		//			return "WVC_Genemaker";
		//		}
		//		if (defName == "WVC_NaturalUndead")
		//		{
		//			return "WVC_Undead";
		//		}
		//		if (defName == "WVC_ReimplanterXenotype")
		//		{
		//			return "WVC_StorageImplanter";
		//		}
		//		if (defName == "WVC_HairColor_DarkGray")
		//		{
		//			return "WVC_HairColor_Slate";
		//		}
		//		if (defName == "WVC_SkinColor_DarkGray")
		//		{
		//			return "WVC_SkinColor_Slate";
		//		}
		//		if (defName == "WVC_Mecha_NoEars")
		//		{
		//			return "Headbone_Human";
		//		}
		//	}
		//	if (defType == typeof(AbilityDef))
		//	{
		//		if (defName == "WVC_ReimplanterXenotype")
		//		{
		//			return "WVC_StorageImplanter";
		//		}
		//	}
		//	if (defType == typeof(ScenarioDef))
		//	{
		//		if (defName == "WVC_XenotypesAndGenes_Blank")
		//		{
		//			return "WVC_XenotypesAndGenes_Meca";
		//		}
		//	}
		//	if (defType == typeof(HediffDef))
		//	{
		//		if (defName == "WVC_GeneSavant")
		//		{
		//			return "TraumaSavant";
		//		}
		//	}
		//	return null;
		//}

		public static void GetModExtensions(Def def, out GeneExtension_General geneExtension_General, out GeneExtension_Giver geneExtension_Giver)
		{
			geneExtension_General = null;
			geneExtension_Giver = null;
			if (def.modExtensions == null)
			{
				return;
			}
			foreach (DefModExtension extension in def.modExtensions)
            {
				if (extension is GeneExtension_General general)
                {
					geneExtension_General = general;
				}
				else if (extension is GeneExtension_Giver giver)
				{
					geneExtension_Giver = giver;
				}
				if (geneExtension_General != null && geneExtension_Giver != null)
                {
					break;
                }
			}
		}

		public static string Reverse(this string text)
		{
			char[] cArray = text.ToCharArray();
			string reverse = "";
			for (int i = cArray.Length - 1; i > -1; i--)
			{
				reverse += cArray[i];
			}
			return reverse;
		}

		public static bool TryGetAndDestroyCorpse_WithPosition(Pawn pawn, out Map mapHeld, out IntVec3 positionHeld)
		{
			mapHeld = null;
			positionHeld = default;
			Corpse corpse = pawn.Corpse;
			if (corpse == null)
			{
				return false;
			}
			mapHeld = corpse.MapHeld;
			positionHeld = corpse.PositionHeld;
			corpse.Destroy();
			return true;
		}

		public static void Notify_DebugPawn(Pawn pawn)
		{
			//PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn);
			pawn.needs?.AddOrRemoveNeedsAsAppropriate();
			pawn.health?.hediffSet?.DirtyCache();
			pawn.skills?.DirtyAptitudes();
			pawn.Notify_DisabledWorkTypesChanged();
			XaG_GeneUtility.ResetGenesInspectString(pawn);
			pawn.Drawer?.renderer?.SetAllGraphicsDirty();
			ReimplanterUtility.NotifyGenesChanged(pawn);
		}

		public static void DoShapeshiftEffects_OnPawn(Pawn pawn)
		{
            if (ModsConfig.AnomalyActive)
            {
                //HediffUtility.MutationMeatSplatter(pawn, false, FleshbeastUtility.MeatExplosionSize.Small);
				MiscUtility.MeatSplatter(pawn, FleshbeastUtility.MeatExplosionSize.Small);
			}
            MainDefOf.WVC_ShapeshiftBurst.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
		}

		public static void MeatSplatter(Pawn pawn, FleshbeastUtility.MeatExplosionSize size, int bloodDropSize = 3)
		{
			for (int i = 0; i < bloodDropSize; i++)
			{
				pawn.health.DropBloodFilth();
			}
			FleshbeastUtility.MeatSplatter(3, pawn.PositionHeld, pawn.MapHeld, size);
		}

		public static void DoSkipEffects(IntVec3 spawnCell, Map map)
		{
			map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(spawnCell, map), spawnCell, 60);
			SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(spawnCell, map));
		}

		public static bool CanStartPregnancy_Gestator(Pawn pawn, GeneExtension_Giver giver = null, bool throwMessage = true)
        {
            if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(pawn, throwMessage))
            {
                return false;
            }
            if (HediffUtility.GetFirstHediffPreventsPregnancy(pawn.health.hediffSet.hediffs) != null)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_Gene_SimpleGestatorFailMessage".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
                return false;
            }
            if (giver != null && giver.gender != Gender.None && giver.gender != pawn.gender)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_AbilityGeneIsActive_PawnWrongGender".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
                return false;
            }
            if ((pawn.ageTracker?.CurLifeStage?.reproductive) == false)
			{
				if (throwMessage)
				{
					Messages.Message("WVC_XaG_Gene_SimpleGestator_ToYoungMessage".Translate(pawn.LabelShortCap).CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
            }
            return true;
		}

		public static bool TryUpdChildGenes(Pawn pawn)
		{
			if (pawn.health.hediffSet.TryGetHediff(HediffDefOf.PregnantHuman, out Hediff hediff))
			{
				if (hediff is Hediff_Pregnant pregnant)
				{
					GeneSet newGeneSet = new();
					HediffUtility.AddParentGenes(pawn, newGeneSet);
					newGeneSet.SortGenes();
					pregnant.geneSet = newGeneSet;
					return true;
				}
				else
				{
					pawn.health.RemoveHediff(hediff);
				}
			}
			return false;
		}

		public static void TryImpregnateOrUpdChildGenes(Pawn pawn)
		{
			if (!TryUpdChildGenes(pawn))
			{
				MiscUtility.Impregnate(pawn);
			}
		}

		public static void Impregnate(Pawn pawn)
		{
			if (pawn?.genes != null)
			{
				Hediff_Pregnant hediff_Pregnant = (Hediff_Pregnant)HediffMaker.MakeHediff(HediffDefOf.PregnantHuman, pawn);
				hediff_Pregnant.Severity = PregnancyUtility.GeneratedPawnPregnancyProgressRange.TrueMin;
				GeneSet newGeneSet = new();
				HediffUtility.AddParentGenes(pawn, newGeneSet);
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

		public static bool TrySummonDropPod(Map map, List<Thing> list)
		{
			if (map == null || map.IsUnderground())
			{
				return false;
			}
			IntVec3 cell = DropCellFinder.TradeDropSpot(map);
			SummonDropPod(map, list, cell);
			return true;
		}

		public static void SummonDropPod(Map map, List<Thing> list, IntVec3 cell, bool leaveSlug = false)
		{
			ActiveTransporterInfo activeDropPodInfo = new();
			activeDropPodInfo.leaveSlag = leaveSlug;
			activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list);
			DropPodUtility.MakeDropPodAt(cell, map, activeDropPodInfo);
		}

		public static bool TryAddFoodPoisoningHediff(Pawn pawn, Thing thing)
        {
            //if (FoodUtility.GetFoodPoisonChanceFactor(pawn) <= 0f)
            //{
            //	return false;
            //}
            float chance = GetFoodPoisonChance(pawn, thing);
            if (Rand.Chance(chance))
            {
                FoodUtility.AddFoodPoisoningHediff(pawn, thing, FoodPoisonCause.DangerousFoodType);
                return true;
            }
            return false;
        }

        public static float GetFoodPoisonChance(Pawn pawn, Thing thing)
        {
            return FoodUtility.TryGetFoodPoisoningChanceOverrideFromTraits(pawn, thing, out float poisonChanceOverride) ? poisonChanceOverride : (pawn.GetStatValue(StatDefOf.FoodPoisonChanceFixedHuman) * FoodUtility.GetFoodPoisonChanceFactor(pawn));
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

		public static List<AbilityDef> ConvertToDefs(this List<Ability> abilities)
		{
			List<AbilityDef> list = new();
			foreach (Ability item in abilities)
			{
				list.Add(item.def);
			}
			return list;
		}

		public static List<BodyPartDef> ConvertToDefs(this List<BodyPartRecord> abilities)
		{
			List<BodyPartDef> list = new();
			foreach (BodyPartRecord item in abilities)
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
			//if (ModsConfig.AnomalyActive)
			//{
			//	return corpse is UnnaturalCorpse;
			//}
			//return false;
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

		public static bool TryGetAbilityJob(Pawn biter, Corpse victim, AbilityDef abilityDef, out Job job)
		{
			job = null;
			if (victim.InnerPawn == biter)
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

		//public static bool HasAnyTraits(List<TraitDef> traitDefs, Pawn pawn)
		//{
		//	List<Trait> traits = pawn?.story?.traits?.allTraits;
		//	if (traits.NullOrEmpty() || traitDefs.NullOrEmpty())
		//	{
		//		return false;
		//	}
		//	for (int i = 0; i < traitDefs.Count; i++)
		//	{
		//		for (int j = 0; j < traits.Count; j++)
		//		{
		//			if (traits[j].def == traitDefs[i])
		//			{
		//				return true;
		//			}
		//		}
		//	}
		//	return false;
		//}

		//public static bool TraitHasAnyConflicts(List<Trait> traits, Trait trait)
		//{
		//	foreach (Trait item in traits)
		//	{
		//		if (item.def.ConflictsWith(trait))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

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

		//public static List<ThingDef> GetAllThingInStuffCategory(StuffCategoryDef stuffCategoryDef)
		//{
		//	List<ThingDef> list = new();
		//	foreach (ThingDef item in DefDatabase<ThingDef>.AllDefsListForReading)
		//	{
		//		if (item.stuffProps != null && item.stuffProps.categories.Contains(stuffCategoryDef))
		//		{
		//			list.Add(item);
		//		}
		//	}
		//	return list;
		//}

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

		//[Obsolete]
		//public static int CountAllPlayerXenos()
		//{
		//	int mult = 0;
		//	foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
		//	{
		//		if (!item.IsHuman())
		//		{
		//			continue;
		//		}
		//		if (XaG_GeneUtility.PawnIsBaseliner(item))
		//		{
		//			continue;
		//		}
		//		mult++;
		//	}
		//	return mult;
		//}

		public static void CountAllPlayerControlledPawns_StaticCollection()
		{
			int colonists = 0;
			int xenos = 0;
			int nonHumans = 0;
			int colonyMechs = 0;
			//int mutants = 0;
			bool anyAssignedWork = false;
			//bool leaderIsShapeshifterOrSimilar = false;
			//bool presentShapeshifter = false;
			//bool leaderIsUndead = false;
			//bool presentUndead = false;
			List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
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
                    else
                    {
                        SubHumansCount(ref colonists, ref nonHumans, item);
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
                    if (item.mutant.Def.thinkTree == null && item.mutant.Def.thinkTreeConstant == null)
					{
						Colonists(ref colonists, ref xenos, item);
					}
                    //mutants++;
                    //SubHumansCount(ref colonists, ref nonHumans, item);
                    continue;
                }
                Colonists(ref colonists, ref xenos, item);
				if (anyAssignedWork)
                {
					continue;
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
			StaticCollectionsClass.oneManArmyMode = colonists <= 1;

            static void SubHumansCount(ref int colonists, ref int nonHumans, Pawn item)
            {
                if (!item.IsSubhuman)
                {
                    nonHumans++;
                }
                else
                {
                    colonists++;
                }
			}
			static void Colonists(ref int colonists, ref int xenos, Pawn item)
			{
				if (!XaG_GeneUtility.PawnIsBaseliner(item) && item.IsHuman())
				{
					xenos++;
				}
				if (!item.IsDuplicate && !item.Deathresting && !item.IsPrisoner)
				{
					colonists++;
				}
			}
			//StaticCollectionsClass.oneManArmyMode = nonHumans <= 0 && colonists <= 1;
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
				Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_OneManArmy, Faction.OfPlayer.Named(HistoryEventArgsNames.Doer)));
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
				if (!victim.CanBleed())
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

		public static StatModifier CopyStatModifier(StatModifier statModifier)
		{
			StatModifier newStatMod = new();
			newStatMod.value = statModifier.value;
			newStatMod.stat = statModifier.stat;
			return newStatMod;
		}

		// Inherit

		public static void CopyFromGeneDef(GeneDef childGeneDef, GeneralHolder holder)
		{
			// Tags
			string phase = "initial";
			try
			{
				phase = "exclusionTags";
				if (holder.copyExclusionTags)
					CopyExclusionTags(childGeneDef, holder.copyFromGeneDef);
				phase = "makeImmuneTo";
				if (holder.copyMakeImmuneTo)
					CopyMakeImunne(childGeneDef, holder.copyFromGeneDef);
				phase = "hediffGiversCannotGive";
				if (holder.copyHediffGiversCannotGive)
					CopyHediffGivers(childGeneDef, holder.copyFromGeneDef);
				phase = "suppressedTraits";
				if (holder.copySuppressedTraits)
					CopySuppressedTraits(childGeneDef, holder.copyFromGeneDef);
				phase = "forcedTraits";
				if (holder.copyForcedTraits)
					CopyForcedTraits(childGeneDef, holder.copyFromGeneDef);
				phase = "conditionalStatAffecters";
				if (holder.copyConditionalStatAffecters)
					CopyStatAffecters(childGeneDef, holder.copyFromGeneDef);
				phase = "statFactors";
				if (holder.copyStatFactors)
					CopyStatFactors(childGeneDef, holder.copyFromGeneDef);
				phase = "statOffsets";
				if (holder.copyStatOffsets)
					CopyStatOffsets(childGeneDef, holder.copyFromGeneDef);
				phase = "symbolPack";
				if (holder.copySymbolPack)
					CopySymbolPack(childGeneDef, holder.copyFromGeneDef);
			}
			catch (Exception arg)
			{
				Log.Error("Failed inherit from gene: " + holder.copyFromGeneDef.defName + ". For gene: " + childGeneDef.defName + ". On phase: " + phase + ". Reason: " + arg);
			}
		}

		public static void CopyFromGeneDef(GeneDef childGeneDef, GeneDef parentGeneDef)
        {
			// Tags
			string phase = "initial";
            try
            {
                phase = "exclusionTags";
                CopyExclusionTags(childGeneDef, parentGeneDef);
                phase = "makeImmuneTo";
                CopyMakeImunne(childGeneDef, parentGeneDef);
                phase = "hediffGiversCannotGive";
                CopyHediffGivers(childGeneDef, parentGeneDef);
                phase = "suppressedTraits";
                CopySuppressedTraits(childGeneDef, parentGeneDef);
                phase = "forcedTraits";
                CopyForcedTraits(childGeneDef, parentGeneDef);
                phase = "conditionalStatAffecters";
                CopyStatAffecters(childGeneDef, parentGeneDef);
                phase = "statFactors";
                CopyStatFactors(childGeneDef, parentGeneDef);
                phase = "statOffsets";
                CopyStatOffsets(childGeneDef, parentGeneDef);
                phase = "symbolPack";
                CopySymbolPack(childGeneDef, parentGeneDef);
            }
            catch (Exception arg)
            {
				Log.Error("Failed inherit from gene: " + parentGeneDef.defName + ". For gene: " + childGeneDef.defName + ". On phase: " + phase + ". Reason: " + arg);
            }
        }

        private static void CopyExclusionTags(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyMakeImunne(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyHediffGivers(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopySuppressedTraits(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyForcedTraits(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyStatAffecters(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyStatFactors(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopyStatOffsets(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
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
        }

        private static void CopySymbolPack(GeneDef targetGeneDef, GeneDef sourceGeneDef)
        {
            if (sourceGeneDef.symbolPack != null)
            {
                if (targetGeneDef.symbolPack == null)
                {
                    targetGeneDef.symbolPack = new();
                }
                // Log.Error("9");
                //phase = "symbolPack prefixSymbols";
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
                //phase = "symbolPack suffixSymbols";
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
                //phase = "symbolPack wholeNameSymbols";
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
    }
}
