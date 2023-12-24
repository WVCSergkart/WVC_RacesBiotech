using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class CompTargetEffect_DoJobOnTarget : CompTargetEffect
    {
        public XenotypeDef xenotypeDef = null;

        public CompProperties_TargetEffect_DoJobOnTarget Props => (CompProperties_TargetEffect_DoJobOnTarget)props;

        public override void PostPostMake()
        {
            base.PostPostMake();
            if (xenotypeDef == null)
            {
                if (Props.xenotypeDef != null)
                {
                    xenotypeDef = Props.xenotypeDef;
                }
                else
                {
                    List<XenotypeDef> xenotypeDefs = XenotypeFilterUtility.WhiteListedXenotypes(true, true);
                    xenotypeDef = xenotypeDefs.RandomElement();
                }
            }
        }

        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (user.IsColonistPlayerControlled && user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                Job job = JobMaker.MakeJob(Props.jobDef, target, parent);
                job.count = 1;
                user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                // Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
            }
        }

        public override string TransformLabel(string label)
        {
            if (xenotypeDef == null)
            {
                return parent.def.label + " (" + "ERR" + ")";
            }
            return parent.def.label + " (" + xenotypeDef.label + ")";
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
        }
    }

}
