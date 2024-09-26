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

		public static bool FurskinHasMask(FurDef furDef)
		{
			bool hasMask = false;
			foreach (BodyTypeGraphicData item in furDef.bodyTypeGraphicPaths)
			{
				if (TextureHasMask(item.texturePath))
				{
					hasMask = true;
				}
			}
			return hasMask;
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

		// Faceplate

		// public static bool HeadTypeIsCorrect(Pawn pawn, List<HeadTypeDef> headTypeDefs)
		// {
			// if (pawn?.genes == null || pawn?.story == null)
			// {
				// return false;
			// }
			// if (headTypeDefs.Contains(pawn.story.headType))
			// {
				// if (HasAnyFaceGraphic(pawn))
				// {
					// return false;
				// }
				// return true;
			// }
			// return false;
		// }

		// public static bool HasAnyFaceGraphic(Pawn pawn)
		// {
			// pawn.Drawer.renderer.renderTree.
			// for (int i = 0; i < missingPart.Count; i++)
			// {
				// if (missingPart[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource))
				// {
					// return true;
				// }
			// }
			// return false;
		// }

		// Settings

		public static void SliderLabeledWithRef(this Listing_Standard ls, string label, ref float val, float min = 0f, float max = 1f, string tooltip = null, int round = 2)
		{
			Rect rect = ls.GetRect(Text.LineHeight);
			Rect rect2 = rect.LeftPart(0.5f).Rounded();
			Rect rect3 = rect.RightPart(0.62f).Rounded().LeftPart(0.97f).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			float _ = (val = Widgets.HorizontalSlider(rect3, val, min, max, middleAlignment: true));
			val = (float)Math.Round(val, round);
			Text.Anchor = TextAnchor.MiddleRight;
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			ls.Gap(ls.verticalSpacing);
		}

		public static void XaG_DefIcon(Rect rect, Def def, float scale = 1f, Color? color = null, Material material = null)
		{
			if (def is ThrallDef thrallDef)
			{
				GUI.color = color ?? Color.white;
				Widgets.DrawTextureFitted(rect, thrallDef.xenotypeIconDef.Icon, scale, material);
				GUI.color = Color.white;
			}
		}

		// public static void XaG_CustomXenotypeIcon(Rect rect, CustomXenotype customXenotype, float scale = 1f, Color? color = null, Material material = null)
		// {
			// GUI.color = color ?? Color.white;
			// Widgets.DrawTextureFitted(rect, customXenotype.IconDef.Icon, scale, material);
			// GUI.color = Color.white;
		// }

		public static void XaG_Icon(Rect rect, Texture icon, float scale = 1f, Color? color = null, Material material = null)
		{
			GUI.color = color ?? Color.white;
			Widgets.DrawTextureFitted(rect, icon, scale, material);
			GUI.color = Color.white;
		}

		// Skills

		// public static void TransferSkills(Pawn student, Pawn teacher)
		// {
			// List<SkillRecord> teacherSkills = teacher?.skills?.skills;
			// if (teacherSkills == null || student?.skills?.skills == null)
			// {
				// return;
			// }
			// foreach (SkillRecord skill in student.skills.skills)
			// {
				// while (skill.levelInt > 0)
				// {
					// skill.Learn(-9999999, true);
				// }
				// skill.xpSinceLastLevel = 0;
			// }
			// foreach (SkillRecord skill in student.skills.skills)
			// {
				// foreach (SkillRecord teacherSkill in teacherSkills)
				// {
					// if (skill.def != teacherSkill.def)
					// {
						// continue;
					// }
					// skill.levelInt = teacherSkill.levelInt;
					// skill.xpSinceLastLevel = 0;
					// break;
				// }
			// }
		// }

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

		public static bool PawnPsychicSensitive(this Pawn pawn)
		{
			return pawn?.GetStatValue(StatDefOf.PsychicSensitivity, cacheStaleAfterTicks: 30000) > 0f;
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

		public static Pawn GetFirstConnectedPawn(Pawn slave)
		{
			if (slave.RaceProps.Dryad)
			{
				return GetDryadQueen(slave);
			}
			List<Thing> things = slave?.connections?.ConnectedThings;
			if (things.NullOrEmpty())
			{
				return null;
			}
			foreach (Thing thing in things)
			{
				if (thing is Pawn master)
				{
					return master;
				}
			}
			return null;
		}

		public static Pawn GetDryadQueen(Pawn dryad)
		{
			return dryad?.TryGetComp<CompGauranlenDryad>()?.Master;
		}

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

		public static int CountAllPlayerControlledColonistsExceptClonesAndQuests()
		{
			int mult = 0;
			List<Pawn> pawns = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists;
			foreach (Pawn item in pawns)
			{
				if (item.IsQuestLodger())
				{
					continue;
				}
				if (item.IsDuplicate)
				{
					continue;
				}
				if (item.IsPrisoner)
				{
					continue;
				}
				mult++;
			}
			return mult;
		}

		public static int CountAllPlayerNonHumanlikes()
		{
			int mult = 0;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsQuestLodger())
					{
						continue;
					}
					if (!item.RaceProps.Humanlike)
					{
						mult++;
					}
				}
			}
			return mult;
		}

		public static int CountAllPlayerMechs()
		{
			int mult = 0;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsQuestLodger())
					{
						continue;
					}
					if (item.IsColonyMech)
					{
						mult += 1;
					}
				}
			}
			return mult;
		}

		public static int CountAllPlayerAnimals()
		{
			int mult = 0;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsQuestLodger())
					{
						continue;
					}
					if (item.RaceProps.Animal)
					{
						mult += 1;
					}
				}
			}
			return mult;
		}

		public static bool PawnIsColonistOrSlave(Pawn pawn, bool shouldBeAdult = false)
		{
			if ((pawn.IsColonist || pawn.IsSlaveOfColony) && (!shouldBeAdult || pawn.ageTracker.CurLifeStage.reproductive))
			{
				return true;
			}
			return false;
		}

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

		public static bool BasicTargetValidation(Pawn biter, Pawn victim, bool canBleed = true)
		{
			if (victim == null || biter == null)
			{
				return false;
			}
			if (!ReimplanterUtility.IsHuman(victim))
			{
				return false;
			}
			if (canBleed)
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

		// Inherit

		public static void InheritGeneDefFrom(GeneDef geneDef, GeneDef inheritGeneDef)
		{
			// Tags
			if (inheritGeneDef.exclusionTags != null)
			{
				if (geneDef.exclusionTags == null)
				{
					geneDef.exclusionTags = new();
				}
				foreach (string item in inheritGeneDef.exclusionTags)
				{
					geneDef.exclusionTags.Add(item);
				}
			}
			// Log.Error("1");
			// Immunity
			if (inheritGeneDef.makeImmuneTo != null)
			{
				if (geneDef.makeImmuneTo == null)
				{
					geneDef.makeImmuneTo = new();
				}
				foreach (HediffDef item in inheritGeneDef.makeImmuneTo)
				{
					if (!geneDef.makeImmuneTo.Contains(item))
					{
						geneDef.makeImmuneTo.Add(item);
					}
				}
			}
			// Log.Error("2");
			if (inheritGeneDef.hediffGiversCannotGive != null)
			{
				if (geneDef.hediffGiversCannotGive == null)
				{
					geneDef.hediffGiversCannotGive = new();
				}
				foreach (HediffDef item in inheritGeneDef.hediffGiversCannotGive)
				{
					if (!geneDef.hediffGiversCannotGive.Contains(item))
					{
						geneDef.hediffGiversCannotGive.Add(item);
					}
				}
			}
			// Log.Error("3");
			// Traits
			if (inheritGeneDef.suppressedTraits != null)
			{
				if (geneDef.suppressedTraits == null)
				{
					geneDef.suppressedTraits = new();
				}
				foreach (GeneticTraitData item in inheritGeneDef.suppressedTraits)
				{
					if (!geneDef.suppressedTraits.Contains(item))
					{
						geneDef.suppressedTraits.Add(item);
					}
				}
			}
			// Log.Error("4");
			if (inheritGeneDef.forcedTraits != null)
			{
				if (geneDef.forcedTraits == null)
				{
					geneDef.forcedTraits = new();
				}
				foreach (GeneticTraitData item in inheritGeneDef.forcedTraits)
				{
					if (!geneDef.forcedTraits.Contains(item))
					{
						geneDef.forcedTraits.Add(item);
					}
				}
			}
			// Log.Error("5");
			// Stats
			if (inheritGeneDef.conditionalStatAffecters != null)
			{
				if (geneDef.conditionalStatAffecters == null)
				{
					geneDef.conditionalStatAffecters = new();
				}
				foreach (ConditionalStatAffecter item in inheritGeneDef.conditionalStatAffecters)
				{
					geneDef.conditionalStatAffecters.Add(item);
				}
			}
			// Log.Error("6");
			if (inheritGeneDef.statFactors != null)
			{
				if (geneDef.statFactors == null)
				{
					geneDef.statFactors = new();
				}
				foreach (StatModifier item in inheritGeneDef.statFactors)
				{
					if (!geneDef.statFactors.Contains(item))
					{
						geneDef.statFactors.Add(item);
					}
				}
			}
			// Log.Error("7");
			if (inheritGeneDef.statOffsets != null)
			{
				if (geneDef.statOffsets == null)
				{
					geneDef.statOffsets = new();
				}
				foreach (StatModifier item in inheritGeneDef.statOffsets)
				{
					if (!geneDef.statOffsets.Contains(item))
					{
						geneDef.statOffsets.Add(item);
					}
				}
			}
			// Log.Error("8");
			// Symbols
			if (inheritGeneDef.symbolPack != null)
			{
				if (geneDef.symbolPack == null)
				{
					geneDef.symbolPack = new();
				}
				if (geneDef.symbolPack.prefixSymbols == null)
				{
					geneDef.symbolPack.prefixSymbols = new();
				}
				if (geneDef.symbolPack.suffixSymbols == null)
				{
					geneDef.symbolPack.suffixSymbols = new();
				}
				if (geneDef.symbolPack.wholeNameSymbols == null)
				{
					geneDef.symbolPack.wholeNameSymbols = new();
				}
				// Log.Error("9");
				if (inheritGeneDef.symbolPack.prefixSymbols != null)
				{
					foreach (WeightedSymbol item in inheritGeneDef.symbolPack.prefixSymbols)
					{
						geneDef.symbolPack.prefixSymbols.Add(item);
					}
				}
				// Log.Error("10");
				if (inheritGeneDef.symbolPack.suffixSymbols != null)
				{
					foreach (WeightedSymbol item in inheritGeneDef.symbolPack.suffixSymbols)
					{
						geneDef.symbolPack.suffixSymbols.Add(item);
					}
				}
				// Log.Error("11");
				if (inheritGeneDef.symbolPack.wholeNameSymbols != null)
				{
					foreach (WeightedSymbol item in inheritGeneDef.symbolPack.wholeNameSymbols)
					{
						geneDef.symbolPack.wholeNameSymbols.Add(item);
					}
				}
			}
		}

	}
}
