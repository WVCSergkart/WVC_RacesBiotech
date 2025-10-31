using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class ThinkNode_ConditionalAllowWanderNearMaster : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.playerSettings?.AreaRestrictionInPawnCurrentMap == pawn.TryGetComp<CompGestatedDryad>()?.Master?.playerSettings?.AreaRestrictionInPawnCurrentMap;
		}

	}

}
