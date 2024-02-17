using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// Harvester

	public class Gene_BloodHarvester : Gene
	{

		public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		public int ticksUntilSpawn;

		private Gene_Hemogen cachedHemogenGene = null;

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

		public override void Tick()
		{
			base.Tick();
			ticksUntilSpawn--;
			if (ticksUntilSpawn > 0)
			{
				return;
			}
			if (pawn.Map != null && Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && Props != null)
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
			MiscUtility.TryHarvest(pawn, Props.thingDefToSpawn, Props.stackCount);
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Harvest " + Props.thingDefToSpawn.label,
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
