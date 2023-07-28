using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_TargetEffect_DoJobOnTarget : CompProperties
    {
        public JobDef jobDef;

        public ThingDef moteDef;

        public XenotypeDef xenotypeDef = null;

        public CompProperties_TargetEffect_DoJobOnTarget()
        {
            compClass = typeof(CompTargetEffect_DoJobOnTarget);
        }
    }

}
