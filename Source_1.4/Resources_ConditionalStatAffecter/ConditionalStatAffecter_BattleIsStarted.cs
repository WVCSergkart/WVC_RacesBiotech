using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    // ======================================================================================================
    public class ConditionalStatAffecter_Drafted : ConditionalStatAffecter
    {
        public bool invert = false;

        public override string Label => Invert();

        public override bool Applies(StatRequest req)
        {
            if (!ModsConfig.BiotechActive)
            {
                return false;
            }
            if (req.HasThing && req.Thing is Pawn pawn && pawn.Map != null)
            {
                if ((pawn.Drafted && !invert) || (!pawn.Drafted && invert))
                {
                    return true;
                }
            }
            // if (req.HasThing && req.Thing is Pawn guest && guest.Map != null && !guest.IsColonist)
            // {
            // return true;
            // }
            // if (pawn.Faction != Faction.OfPlayer || !pawn.ageTracker.CurLifeStage.reproductive)
            // {
            // return false;
            // }
            return false;
        }

        private string Invert()
        {
            if (invert == true)
            {
                return "WVC_StatsReport_NonGenes".Translate() + "WVC_StatsReport_Drafted".Translate();
            }
            return "WVC_StatsReport_Drafted".Translate();
        }
    }
}
