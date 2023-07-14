// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using System.Linq;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class ThoughtWorker_IncestLover : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
		{
			if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
			{
				return false;
			}
			// if (MechanoidizationUtility.IsIncestLover(pawn))
			// {
			// }
			if (pawn.relations.FamilyByBlood.Contains(other))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return false;
		}
	}
}
