// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_PawnIsMechaskinned_SingleThought : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				return false;
			}
			if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
			{
				return false;
			}
			if (MechanoidizationUtility.PawnIsExoskinned(pawn))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}

	// ===============================================================

	public class ThoughtWorker_PawnIsMechaskinned_SingleThought_Social : ThoughtWorker_Precept_Social
	{
		protected override ThoughtState ShouldHaveThought(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
			{
				return false;
			}
			if (MechanoidizationUtility.PawnIsExoskinned(other))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}
}
