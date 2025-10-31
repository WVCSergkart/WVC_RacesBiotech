using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class IncidentWorker_XenoGenesTreeSpawn : IncidentWorker_SpecialTreeSpawn
	{

		protected override bool TryFindRootCell(Map map, out IntVec3 cell)
		{
			if (CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => !x.IsPolluted(map) && base.GenStep.CanSpawnAt(x, map), map, out cell))
			{
				return true;
			}
			return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => base.GenStep.CanSpawnAt(x, map), map, out cell);
		}

	}

	public class GenStep_XenoGenesTrees : GenStep_SpecialTrees
	{

		public override int SeedPart => 231455142;

		public override int DesiredTreeCountForMap(Map map)
		{
			PollutionLevel pollutionLevel = Find.WorldGrid[map.Tile].PollutionLevel();
			return pollutionLevel switch
			{
				PollutionLevel.None => pollutionNone,
				PollutionLevel.Light => pollutionLight,
				PollutionLevel.Moderate => pollutionModerate,
				PollutionLevel.Extreme => pollutionExtreme,
				_ => 1,
			};
		}

		protected override float GetGrowth()
		{
			return 1f;
		}

	}

}
