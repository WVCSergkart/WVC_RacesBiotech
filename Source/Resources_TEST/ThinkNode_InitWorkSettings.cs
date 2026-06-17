using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class ThinkNode_InitWorkSettings : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn pawn)
		{
			if (pawn.workSettings == null)
			{
				pawn.workSettings = new(pawn);
				pawn.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
			}
			return pawn.workSettings != null && pawn.workSettings.EverWork;
		}

	}

}
