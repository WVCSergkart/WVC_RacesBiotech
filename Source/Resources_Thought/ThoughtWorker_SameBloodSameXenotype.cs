// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class ThoughtWorker_SameBloodSameXenotype : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (pawn.genes?.Xenotype != null && other.genes?.Xenotype != null)
            {
                if (pawn.genes?.Xenotype == other.genes?.Xenotype)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            }
            else if (pawn.genes?.CustomXenotype != null && other.genes?.CustomXenotype != null)
            {
                if (pawn.genes?.CustomXenotype == other.genes?.CustomXenotype)
                {
                    return ThoughtState.ActiveAtStage(1);
                }
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
