using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	// public class Command_ActionWithCooldown : Command_Action
	// {
		// private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.6f));

		// private int lastUsedTick;

		// private int cooldownTicks;

		// public Command_ActionWithCooldown(int lastUsedTick, int cooldownTicks)
		// {
			// this.lastUsedTick = lastUsedTick;
			// this.cooldownTicks = cooldownTicks;
		// }

		// public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		// {
			// Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			// GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
			// if (lastUsedTick > 0)
			// {
				// int num = Find.TickManager.TicksGame - lastUsedTick;
				// if (num < cooldownTicks)
				// {
					// float value = Mathf.InverseLerp(cooldownTicks, 0f, num);
					// Widgets.FillableBar(rect, Mathf.Clamp01(value), cooldownBarTex, null, doBorder: false);
				// }
			// }
			// if (result.State == GizmoState.Interacted)
			// {
				// return result;
			// }
			// return new GizmoResult(result.State);
		// }
	// }

	public static class MiscUtility
	{

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
			if ((pawn.IsColonist || pawn.IsSlaveOfColony) && (shouldBeAdult || pawn.ageTracker.CurLifeStage.reproductive))
			{
				return true;
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
