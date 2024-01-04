using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public static class MiscUtility
	{

		// ShapeShifter

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

		public static float CountAllPlayerXenos()
		{
			float mult = 0f;
			List<XenotypeDef> xenoList = new();
			List<CustomXenotype> xenoListB = new();
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.genes != null && item.genes.Xenotype != null && item.genes.Xenotype != XenotypeDefOf.Baseliner && !xenoList.Contains(item.genes.Xenotype))
					{
						mult += 1;
						xenoList.Add(item.genes.Xenotype);
					}
					else if (item.genes != null && item.genes.CustomXenotype != null && !xenoListB.Contains(item.genes.CustomXenotype))
					{
						mult += 1;
						xenoListB.Add(item.genes.CustomXenotype);
					}
				}
			}
			// Log.Error("Xenos in faction: " + mult.ToString());
			return mult;
		}

		public static float CountAllPlayerNonHumanlikes()
		{
			float mult = 0f;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (!item.RaceProps.Humanlike)
					{
						mult++;
					}
				}
			}
			return mult;
		}

		public static float CountAllPlayerMechs()
		{
			float mult = 0f;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsColonyMech)
					{
						mult += 1;
					}
				}
			}
			return mult;
		}

		public static float CountAllPlayerAnimals()
		{
			float mult = 0f;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
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

		public static bool HasReimplanterAbility(this Pawn pawn)
		{
			List<Ability> allAbilitiesForReading = pawn?.abilities?.AllAbilitiesForReading;
			foreach (Ability ability in allAbilitiesForReading)
			{
				foreach (AbilityComp comp in ability.comps)
				{
					if (comp is CompAbilityEffect_Reimplanter)
					{
						return true;
					}
				}
			}
			return false;
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

	}
}
