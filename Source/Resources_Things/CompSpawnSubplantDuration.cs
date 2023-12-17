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
			MiscUtility.GrowSubplant(parent, Props.maxRadius, Props.subplant, Props.initialGrowthRange, parent.Map, Props.canSpawnOverPlayerSownPlants);
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
