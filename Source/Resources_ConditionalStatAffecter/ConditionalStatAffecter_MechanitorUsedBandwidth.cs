using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ConditionalStatAffecter_MechanitorUsedBandwidth : ConditionalStatAffecter
    {

        public float controlledPawns;

        public override string Label => "WVC_StatsReport_MechanitorTotalBandwidth".Translate(controlledPawns);

        public override bool Applies(StatRequest req)
        {
            return StaticCollectionsClass.cachedColonyMechs >= controlledPawns;
        }

    }

}
