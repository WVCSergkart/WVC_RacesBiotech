using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class JobGiver_AIDefendConnectedPawn : JobGiver_AIDefendPawn
	{

		public float radiusUnreleased = 5f;
		public float radiusReleased = 50f;

		protected override Pawn GetDefendee(Pawn pawn)
		{
			return MiscUtility.GetConnectedPawn(pawn);
		}

		protected override float GetFlagRadius(Pawn pawn)
		{
			if (pawn.playerSettings.Master?.playerSettings?.animalsReleased == true && pawn.training.HasLearned(TrainableDefOf.Release))
			{
				return radiusReleased;
			}
			return radiusUnreleased;
		}

	}

}
