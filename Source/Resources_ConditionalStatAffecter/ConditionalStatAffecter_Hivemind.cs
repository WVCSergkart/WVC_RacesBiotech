using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// Cause loop. Req save version
	public class ConditionalStatAffecter_Hivemind : ConditionalStatAffecter
	{

		public float controlledPawns;

		public override string Label => "WVC_StatsReport_HivemindCount".Translate(controlledPawns);

		public override bool Applies(StatRequest req)
		{
			return HivemindUtility.HivemindPawns.Count >= controlledPawns;
		}

	}

}
