// RimWorld.CompProperties_Toxifier
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompSpawnSubplantDuration : ThingComp
	{
		private int nextSubplantTick;

		public CompProperties_SpawnSubplant Props => (CompProperties_SpawnSubplant)props;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				SetupNextSubplantTick();
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			Tick();
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			Tick();
		}

		public override void CompTickLong()
		{
			base.CompTickRare();
			Tick();
		}

		public void Tick()
		{
			if (Find.TickManager.TicksGame >= nextSubplantTick)
			{
				DoGrowSubplant();
				SetupNextSubplantTick();
			}
		}

		public void SetupNextSubplantTick()
		{
			nextSubplantTick = Find.TickManager.TicksGame + (int)(60000f * Props.subplantSpawnDays);
		}

		public void DoGrowSubplant(bool force = false)
		{
			if (!ModLister.CheckIdeology("Subplant duration spawning") || (!force && ((Plant)parent).Growth < Props.minGrowthForSpawn))
			{
				return;
			}
			GrowSubplant(parent, Props.maxRadius, Props.subplant, Props.initialGrowthRange, parent.Map, Props.canSpawnOverPlayerSownPlants);
		}

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

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn Subplant",
					action = delegate
					{
						DoGrowSubplant();
						SetupNextSubplantTick();
					}
				};
			}
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref nextSubplantTick, "nextSubplantTick", 0);
		}
	}

}
