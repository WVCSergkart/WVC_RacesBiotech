using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_SpawnOnDeath : CompProperties
	{

		public int filthCount = 5;
		public ThingDef filthDefToSpawn;
		public List<ThingDef> thingDefsToSpawn;

		public int subplantCount = 3;
		public ThingDef subplant;
		public float maxRadius;
		public FloatRange? initialGrowthRange;
		public bool canSpawnOverPlayerSownPlants = true;
		public List<ThingDef> plantsToNotOverwrite;

		// public CompProperties_SpawnOnDeath()
		// {
			// compClass = typeof(CompSpawnOnDeath);
		// }
	}

	public class CompSpawnOnDeath : ThingComp
	{

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, previousMap, Props.filthDefToSpawn, 1);
			}
			// FilthMaker.TryMakeFilth(pawn.Position, previousMap, Props.filthDefToSpawn, filthCount);
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, previousMap, ThingPlaceMode.Near, null, null, default);
		}

	}

	public class CompSpawnOnDeath_Subplants : ThingComp
	{

		private CompProperties_SpawnOnDeath Props => (CompProperties_SpawnOnDeath)props;

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			Pawn pawn = parent as Pawn;
			for (int i = 0; i < Props.filthCount; i++)
			{
				IntVec3 intVec = pawn.Position + GenRadial.RadialPattern[i];
				FilthMaker.TryMakeFilth(intVec, previousMap, Props.filthDefToSpawn, 1);
			}
			for (int i = 0; i < Props.subplantCount; i++)
			{
				CompSpawnSubplantDuration.GrowSubplant(pawn, Props.maxRadius, Props.subplant, Props.plantsToNotOverwrite, Props.initialGrowthRange, previousMap, Props.canSpawnOverPlayerSownPlants);
			}
			// FilthMaker.TryMakeFilth(pawn.Position, previousMap, Props.filthDefToSpawn, filthCount);
			Thing thing = ThingMaker.MakeThing(Props.thingDefsToSpawn.RandomElement());
			GenPlace.TryPlaceThing(thing, pawn.Position, previousMap, ThingPlaceMode.Near, null, null, default);
		}

	}

}
