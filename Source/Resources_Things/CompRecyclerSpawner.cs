using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompProperties_RecyclerSpawner : CompProperties
	{
		public IntRange ticksUntilSpawn = new(5000, 5000);

		public ThingDef productDef;

		public int productCount = 1;

		public bool writeTimeLeftToSpawn = true;

		public CompProperties_RecyclerSpawner()
		{
			compClass = typeof(CompRecyclerSpawner);
		}
	}

	[Obsolete]
	public class CompRecyclerSpawner : ThingComp
	{

		public int ticksUntilSpawn;

		public CompProperties_RecyclerSpawner Props => (CompProperties_RecyclerSpawner)props;

		public CompAtomizer Atomizer => parent.TryGetComp<CompAtomizer>();

		public bool PowerOn => parent.GetComp<CompPowerTrader>()?.PowerOn ?? false;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				ResetInterval();
			}
		}

		public override void CompTickInterval(int delta)
		{
			base.CompTickInterval(delta);
			if (PowerOn && Atomizer.TicksLeftUntilAllAtomized > 0)
			{
				ticksUntilSpawn -= delta;
				if (ticksUntilSpawn <= 0)
				{
					ResetInterval();
					SpawnItems();
				}
			}
		}

		private void SpawnItems()
		{
			Thing thing = ThingMaker.MakeThing(Props.productDef);
			thing.stackCount = Props.productCount;
			GenPlace.TryPlaceThing(thing, parent.Position, parent.Map, ThingPlaceMode.Near, null, null, default);
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Props.ticksUntilSpawn.RandomInRange;
			// ticksUntilSpawn = Atomizer.TicksPerAtomize;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		}

		public override string CompInspectStringExtra()
		{
			if (PowerOn && Props.writeTimeLeftToSpawn && Atomizer.TicksLeftUntilAllAtomized > 0)
			{
				return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(Props.productDef, null, Props.productCount)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
			return null;
		}
	}

}
