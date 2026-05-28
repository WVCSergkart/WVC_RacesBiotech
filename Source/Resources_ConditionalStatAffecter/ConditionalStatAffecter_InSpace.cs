using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ConditionalStatAffecter_InSpace : ConditionalStatAffecter
	{

		public override string Label => "WVC_StatsReport_InSpace".Translate();

		public override bool Applies(StatRequest req)
		{
			return req.HasThing && req.Thing is Pawn pawn && pawn.InSpace();
		}

	}

}
