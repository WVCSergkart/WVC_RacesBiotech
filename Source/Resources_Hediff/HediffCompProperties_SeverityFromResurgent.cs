using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_SeverityFromResurgent : HediffCompProperties
    {

        // public float severityPerHourEmpty;

        // public float severityPerHour;

        // 1500 = 1 hour
        public int refreshTicks = 1500;

        public HediffCompProperties_SeverityFromResurgent()
        {
            compClass = typeof(HediffComp_SeverityFromResurgent);
        }

    }

}
