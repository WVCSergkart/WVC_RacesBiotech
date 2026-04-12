using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class ThoughtWorker_ArtificialBeauty : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (Gene_ArtificialBeauty.thoughtDisabled)
			{
				return false;
			}
			if (Gene_AbominationallyUgly.LeperPawns.Contains(other))
			{
				if (Blind(pawn))
				{
					return false;
				}
				if (Gene_AbominationallyUgly.LeperPawns.Contains(pawn))
				{
					return ThoughtState.ActiveAtStage(2);
				}
				return ThoughtState.ActiveAtStage(1);
			}
			if (Gene_ArtificialBeauty.AngelPawns.Contains(other))
			{
				if (Blind(pawn))
				{
					return false;
				}
				return ThoughtState.ActiveAtStage(0);
			}
			return false;

			// Performance is better if blindness is checked after the pawns lists.
			bool Blind(Pawn pawn)
			{
				return PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn);
			}
		}
	}

	//public class ThoughtWorker_AbominationallyUgly : ThoughtWorker
	//{
	//	protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
	//	{
	//		if (Gene_AbominationallyUgly.thoughtDisabled)
	//		{
	//			return false;
	//		}
	//		if (!Gene_AbominationallyUgly.LeperPawns.Contains(other))
	//		{
	//			return false;
	//		}
	//		if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
	//		{
	//			return false;
	//		}
	//		if (Gene_AbominationallyUgly.LeperPawns.Contains(pawn))
	//		{
	//			return ThoughtState.ActiveAtStage(1);
	//		}
	//		return ThoughtState.ActiveAtStage(0);
	//	}
	//}

}
