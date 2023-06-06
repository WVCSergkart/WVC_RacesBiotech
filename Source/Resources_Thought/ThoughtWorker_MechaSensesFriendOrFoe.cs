// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	// public class ThoughtWorker_MechaSkin_FriendOrFoe : ThoughtWorker
	// {
		// protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		// {
			// if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			// {
				// return false;
			// }
			// if (pawn.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareFriendOrFoe))
			// {
				// if (other.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareFriendOrFoe))
				// {
					// return ThoughtState.ActiveAtStage(0);
				// }
			// }
			// return false;
		// }
	// }
	public class ThoughtWorker_MechaSensesFriendOrFoe : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			if (other.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareNaturalEnemy))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.ActiveAtStage(1);
			// if (MechanoidizationUtility.PawnIsMechaskinned(pawn))
			// {
				// if (pawn.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareNaturalEnemy))
				// {
					// if (other.genes.HasGene(WVC_GenesDefOf.WVC_MechaAI_SoftwareNaturalEnemy))
					// {
						// return ThoughtState.ActiveAtStage(0);
					// }
					// if (MechanoidizationUtility.PawnIsMechaskinned(other) || MechanoidizationUtility.PawnIsAndroid(other))
					// {
						// return false;
					// }
					// return ThoughtState.ActiveAtStage(1);
				// }
			// }
			// return false;
		}
	}
}
