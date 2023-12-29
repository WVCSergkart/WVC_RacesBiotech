using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Spawner : Gene
	{
		public ThingDef ThingDefToSpawn => def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;

		private int StackCount => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public int ticksUntilSpawn;

		// For info
		public int FinalStackCount => GetStackCount();
		private int cachedStackCount;
		// For info

		// public override string Label => GetLabel();

		// public override string LabelCap => GetLabel().CapitalizeFirst();

		// private string GetLabel()
		// {
			// return def.label + " (" + ThingDefToSpawn.LabelCap + ")";
		// }

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksUntilSpawn--;
			if (ticksUntilSpawn <= 0)
			{
				if (pawn.Map != null && Active)
				{
					SpawnItems();
				}
				ResetInterval();
			}
		}

		private void SpawnItems()
		{
			Thing thing = ThingMaker.MakeThing(ThingDefToSpawn);
			thing.stackCount = GetStackModifier();
			GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
		}

		private int GetStackCount()
		{
			if (cachedStackCount != 0)
			{
				return cachedStackCount;
			}
			// Log.Error("_1");
			return GetStackModifier();
		}

		private int GetStackModifier()
		{
			float modifier = 1f;
			List<Gene> genes = pawn?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				cachedStackCount = (int)modifier;
				return cachedStackCount;
			}
			int met = 0;
			foreach (Gene item in genes)
			{
				met += item.def.biostatMet;
			}
			modifier += met * 0.1f;
			int countedStack = (int)(StackCount * modifier);
			cachedStackCount = countedStack <= 1 ? 1 : countedStack;
			return cachedStackCount;
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = def.GetModExtension<GeneExtension_Spawner>().spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn " + ThingDefToSpawn.label,
					defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					action = delegate
					{
						if (pawn.Map != null && Active)
						{
							SpawnItems();
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		}
	}

}
