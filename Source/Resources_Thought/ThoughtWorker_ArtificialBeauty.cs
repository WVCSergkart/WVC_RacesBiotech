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
			if (!Gene_ArtificialBeauty.AngelPawns.Contains(other))
			{
				return false;
			}
			if (PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
			{
				return false;
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}

}
