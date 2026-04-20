using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public abstract class JobDriver_XaGJob_General : JobDriver
	{

		public GeneDef geneDef;
		public XenotypeDef xenotypeDef;

		public bool consumeStack;

		public float factor = 1f;

		public bool implantEndogenes = false;
		public bool implantXenogenes = false;

		public override void Notify_Starting()
		{
			base.Notify_Starting();
			if (job is XaG_Job xaG_Job)
			{
				if (geneDef == null)
				{
					geneDef = xaG_Job.geneDef;
				}
				if (xenotypeDef == null)
				{
					xenotypeDef = xaG_Job.xenotypeDef;
				}
				consumeStack = xaG_Job.consumeStack;
				factor = xaG_Job.factor;
				implantEndogenes = xaG_Job.implantEndogenes;
				implantXenogenes = xaG_Job.implantXenogenes;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref geneDef, "geneDef");
			Scribe_Defs.Look(ref xenotypeDef, "xenotypeDef");
			Scribe_Values.Look(ref consumeStack, "consumeStack", false);
			Scribe_Values.Look(ref factor, "factor", 1f);
			Scribe_Values.Look(ref implantEndogenes, "implantEndogenes", false);
			Scribe_Values.Look(ref implantXenogenes, "implantXenogenes", false);
		}

	}

}
