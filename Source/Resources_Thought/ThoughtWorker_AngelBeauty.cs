// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class ThoughtWorker_AngelBeauty : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                return false;
            }
            if (GeneFeaturesUtility.IsAngelBeauty(pawn))
            {
                return false;
            }
            float psySens = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
            if (psySens > 0f && GeneFeaturesUtility.IsAngelBeauty(other))
            {
                return ThoughtState.ActiveAtStage(0);
            }
            return false;
        }
    }
}
