using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class XaG_Job : Job
	{

		public XaG_Job()
		{
		}

		public XaG_Job(Job job)
		{
			this.def = job.def;
			this.targetA = job.targetA;
		}

		public XaG_Job(JobDef jobDef, LocalTargetInfo localTargetInfo)
		{
			this.def = jobDef;
			this.targetA = localTargetInfo;
		}

		public GeneDef geneDef;
		public XenotypeDef xenotypeDef;

		public bool consumeStack;

		public float factor;

		public bool implantEndogenes;
		public bool implantXenogenes;

		public Gene gene;

		public new void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref gene, "gene");
		}

	}

}
