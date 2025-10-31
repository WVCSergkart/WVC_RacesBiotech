using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class JobGiver_WanderNearConnectedPawn : JobGiver_Wander
	{

		public JobGiver_WanderNearConnectedPawn()
		{
			wanderRadius = 3f;
			ticksBetweenWandersRange = new IntRange(125, 200);
			wanderDestValidator = (Pawn p, IntVec3 c, IntVec3 root) => (!MustUseRootRoom(p) || root.GetRoom(p.Map) == null || WanderRoomUtility.IsValidWanderDest(p, c, root));
		}

		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return WanderUtility.BestCloseWanderRoot(MiscUtility.GetConnectedPawn(pawn).PositionHeld, pawn);
		}

		private bool MustUseRootRoom(Pawn pawn)
		{
			return pawn.playerSettings.Master?.playerSettings?.animalsReleased == true;
		}

	}

}
