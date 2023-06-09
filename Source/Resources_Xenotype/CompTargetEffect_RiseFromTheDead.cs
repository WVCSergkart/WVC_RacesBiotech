using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompTargetEffect_DoJobOnTarget : CompTargetEffect
	{
		public CompProperties_TargetEffect_DoJobOnTarget Props => (CompProperties_TargetEffect_DoJobOnTarget)props;

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
	}

}
