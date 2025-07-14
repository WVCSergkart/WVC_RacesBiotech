using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class JobGiver_AIFollowConnectedPawn : JobGiver_AIFollowPawn
	{

		public float radiusUnreleased = 5f;
		public float radiusReleased = 50f;

		protected override int FollowJobExpireInterval => 200;

		protected override Pawn GetFollowee(Pawn pawn)
		{
			if (pawn.playerSettings == null)
			{
				return null;
			}
			return MiscUtility.GetConnectedPawn(pawn);
		}

		protected override float GetRadius(Pawn pawn)
		{
			if (pawn.playerSettings.Master?.playerSettings?.animalsReleased == true && pawn.training.HasLearned(TrainableDefOf.Release))
			{
				return radiusReleased;
			}
			return radiusUnreleased;
		}

	}

}
