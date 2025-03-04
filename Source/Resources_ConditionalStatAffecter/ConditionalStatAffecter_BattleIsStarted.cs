using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
    // ======================================================================================================
    public class ConditionalStatAffecter_Drafted : ConditionalStatAffecter
    {
        //public bool invert = false;

        public override string Label => "WVC_StatsReport_Drafted".Translate();

        public override bool Applies(StatRequest req)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                return pawn.Drafted;
            }
            return false;
        }

        //private string Invert()
        //{
        //    if (invert == true)
        //    {
        //        return "WVC_StatsReport_NonGenes".Translate() + "WVC_StatsReport_Drafted".Translate();
        //    }
        //    return "WVC_StatsReport_Drafted".Translate();
        //}
    }
}
