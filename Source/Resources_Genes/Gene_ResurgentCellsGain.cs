using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentCellsGain : Gene, IGeneResourceDrain
	{
		[Unsaved(false)]
		private Gene_ResurgentCells cachedHemogenGene;

		public Gene_Resource Resource
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes.GetFirstGeneOfType<Gene_ResurgentCells>();
				}
				return cachedHemogenGene;
			}
		}

		public bool CanOffset
		{
			get
			{
				if (Active)
				{
					return true;
				}
				return false;
			}
		}

		public float ResourceLossPerDay => def.resourceLossPerDay;

		public Pawn Pawn => pawn;

		public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

		public override void Tick()
		{
			base.Tick();
			UndeadUtility.TickResourceDrain(this);
		}

	}

	public class Gene_ResurgentFungus : Gene_ResurgentCellsGain
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public int timeForNextSummon = -1;
		// public int cachedMapTreesCount = 0;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetSummonInterval();
		}

		public override void Tick()
		{
			base.Tick();
			timeForNextSummon--;
			if (timeForNextSummon > 0)
			{
				return;
			}
			ResetSummonInterval();
			if (pawn.Map == null)
			{
				return;
			}
			GenerateIncident();
		}

		public void ResetSummonInterval()
		{
			if (Spawner == null)
			{
				return;
			}
			timeForNextSummon = Spawner.spawnIntervalRange.RandomInRange;
		}

		public void GenerateIncident()
		{
			if (Spawner?.incidentDef == null)
			{
				return;
			}
			int cachedMapTreesCount = CountSpecialTrees();
			if (cachedMapTreesCount >= Spawner.specialTreesMax)
			{
				return;
			}
			IncidentParms parms = StorytellerUtility.DefaultParmsNow(Spawner.incidentDef.category, pawn.Map);
			IncidentDef result = Spawner.incidentDef;
			if (result != null)
			{
				if (!result.Worker.CanFireNow(parms) && cachedMapTreesCount > Spawner.specialTreesMin)
				{
					return;
				}
				TryFire(new(result, null, parms));
			}
		}

		public bool TryFire(FiringIncident fi)
		{
			if (fi.def.Worker.CanFireNow(fi.parms) && fi.def.Worker.TryExecute(fi.parms))
			{
				fi.parms.target.StoryState.Notify_IncidentFired(fi);
				return true;
			}
			return false;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Fire incident",
					action = delegate
					{
						GenerateIncident();
					}
				};
			}
		}

		private int CountSpecialTrees()
		{
			int trees = 0;
			foreach (Thing item in pawn.Map.listerThings.AllThings)
			{
				if (item?.def != Spawner.incidentDef.treeDef)
				{
					continue;
				}
				trees++;
			}
			return trees;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			// Scribe_Values.Look(ref cachedMapTreesCount, "cachedMapTreesCount", 0);
		}

	}

}
