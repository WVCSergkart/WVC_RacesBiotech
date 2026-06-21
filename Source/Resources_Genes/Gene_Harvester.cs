using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// Harvester

	public class Gene_BloodHarvester : XaG_Gene
	{

		//public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		//[Unsaved(false)]
		//private GeneExtension_Spawner cachedGeneExtension;
		//public GeneExtension_Spawner Spawner
		//{
		//	get
		//	{
		//		if (cachedGeneExtension == null)
		//		{
		//			cachedGeneExtension = def.GetModExtension<GeneExtension_Spawner>();
		//		}
		//		return cachedGeneExtension;
		//	}
		//}

		public int ticksUntilSpawn;

		[Unsaved(false)]
		private Gene_Hemogen cachedHemogenGene;
		public Gene_Hemogen Gene_Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			ticksUntilSpawn -= delta;
			if (ticksUntilSpawn > 0)
			{
				return;
			}
			if (pawn.Map != null && Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && Extension_Spawner != null)
			{
				Harvest();
			}
			ResetInterval();
		}

		private void Harvest()
		{
			if (Gene_Hemogen?.hemogenPacksAllowed != true)
			{
				return;
			}
			GeneFeaturesUtility.TryHarvest(pawn, Extension_Spawner.thingDefToSpawn, Extension_Spawner.stackCount, styleDef: Extension_Spawner.styleDef);
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Extension_Spawner.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Harvest " + Extension_Spawner.thingDefToSpawn.label,
					defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					action = delegate
					{
						if (Active)
						{
							Harvest();
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
