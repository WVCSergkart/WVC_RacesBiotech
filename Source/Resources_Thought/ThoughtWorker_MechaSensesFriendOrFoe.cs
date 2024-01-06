// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_MechaSensesFriendOrFoe : ThoughtWorker
	{

		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			// Log.Error("MechaSense Check");
			if (XaG_GeneUtility.HasActiveGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareNaturalEnemy, other))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.ActiveAtStage(1);
		}

	}

}
