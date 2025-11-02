using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ConditionalStatAffecter_HivemindSize : ConditionalStatAffecter
	{

		public float size;

		public override string Label => "WVC_StatsReport_HivemindCount".Translate(size);

		public override bool Applies(StatRequest req)
		{
			//return HivemindUtility.HivemindPawns.Count >= controlledPawns;
			return HivemindUtility.SafePawnsCount >= size;
		}

	}

}
