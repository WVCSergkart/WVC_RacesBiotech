using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ConditionalStatAffecter_OneManArmy : ConditionalStatAffecter
    {

        public override string Label => "WVC_StatsReport_OneManArmy".Translate();

        public override bool Applies(StatRequest req)
        {
            return StaticCollectionsClass.oneManArmyMode == true;
        }

    }

}
