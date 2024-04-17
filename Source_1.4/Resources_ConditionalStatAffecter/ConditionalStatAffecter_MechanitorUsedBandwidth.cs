using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ConditionalStatAffecter_MechanitorUsedBandwidth : ConditionalStatAffecter
    {
        public float controlledPawns;

        public override string Label => "WVC_StatsReport_MechanitorTotalBandwidth".Translate() + " " + controlledPawns;

        public override bool Applies(StatRequest req)
        {
            // if (!ModsConfig.BiotechActive)
            // {
            // return false;
            // }
            if (req.HasThing && req.Thing is Pawn pawn && MechanitorUtility.IsMechanitor(pawn))
            {
                int connectedThingsCount = pawn.mechanitor.ControlledPawns.Count;
                if (connectedThingsCount >= controlledPawns)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
