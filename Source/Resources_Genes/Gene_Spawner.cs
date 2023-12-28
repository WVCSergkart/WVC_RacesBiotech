using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Spawner : Gene
	{
		public ThingDef ThingDefToSpawn => def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;
		public int StackCount => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public int ticksUntilSpawn;

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
					SpawnItems(pawn);
				}
				ResetInterval();
			}
		}

		private void SpawnItems(Pawn pawn)
		{
			Thing thing = ThingMaker.MakeThing(ThingDefToSpawn);
			int countedStack = (int)(StackCount * GetStackModifier(pawn));
			thing.stackCount = countedStack <= 1 ? 1 : countedStack;
			GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
		}

		private float GetStackModifier(Pawn pawn)
		{
			float modifier = 1f;
			List<Gene> genes = pawn?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				return modifier;
			}
			int met = 0;
			foreach (Gene item in genes)
			{
				met += item.def.biostatMet;
			}
			return modifier + (met * 0.1f);
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
							SpawnItems(pawn);
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
