using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class ThinkNode_ConditionalGestatedDryad : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.TryGetComp<CompGestatedDryad>()?.Gestated == true;
		}

	}

}
