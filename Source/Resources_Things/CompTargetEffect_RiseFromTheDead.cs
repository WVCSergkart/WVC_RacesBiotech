using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompTargetEffect_DoJobOnTarget_XenogermSerum : CompTargetEffect
	{
		public XenotypeDef xenotypeDef = null;

		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

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
					List<XenotypeDef> xenotypeDefs = ListsUtility.GetWhiteListedXenotypes(true, true);
					xenotypeDef = xenotypeDefs.RandomElement();
				}
			}
		}

		public virtual void Notify_SerumCrafted(Pawn pawn)
		{
			xenotypeDef = null;
		}

		public override bool AllowStackWith(Thing other)
		{
			// CompTargetEffect_DoJobOnTarget otherXeno = other.TryGetComp<CompTargetEffect_DoJobOnTarget>();
			// if (otherXeno != null && otherXeno.xenotypeDef != null && otherXeno.xenotypeDef == xenotypeDef)
			// {
				// return true;
			// }
			return false;
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
				return parent.def.label + " (" + "WVC_XaG_Untuned".Translate() + ")";
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
