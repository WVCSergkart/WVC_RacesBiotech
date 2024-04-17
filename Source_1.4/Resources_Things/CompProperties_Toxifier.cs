// RimWorld.CompProperties_Toxifier
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_Toxifier : CompProperties
    {
        public float radius = 5.9f;

        public IntRange pollutionIntervalTicks = new(60000, 120000);

        public int cellsToPollute = 1;

        public CompProperties_Toxifier()
        {
            compClass = typeof(CompToxifier);
        }
    }

}
