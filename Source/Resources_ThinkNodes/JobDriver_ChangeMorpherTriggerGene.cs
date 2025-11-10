using System.Collections.Generic;
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

		public GeneDef geneDef;

		public bool consumeStack;

		public float factor;

		public bool implantEndogenes;
		public bool implantXenogenes;

		//public new void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Defs.Look(ref geneDef, "geneDef");
		//}

	}

	public abstract class JobDriver_XaGJob_General : JobDriver
	{

		public GeneDef geneDef;

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
			Scribe_Values.Look(ref consumeStack, "consumeStack", false);
			Scribe_Values.Look(ref factor, "factor", 1f);
			Scribe_Values.Look(ref implantEndogenes, "implantEndogenes", false);
			Scribe_Values.Look(ref implantXenogenes, "implantXenogenes", false);
		}

	}

	public class JobDriver_ChangeMorpherTriggerGene : JobDriver_XaGJob_General
	{

		public Thing Target => job.targetA.Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.WaitWith(TargetIndex.A, 320, useProgressBar: true).WithEffect(EffecterDefOf.ButcherMechanoid, TargetIndex.A);
			yield return Toils_General.Do(delegate
			{
				Target.ReduceStack();
				if (job is XaG_Job xaG_Job)
				{
					Gene_Morpher morpher = pawn.genes?.GetFirstGeneOfType<Gene_Morpher>();
					morpher?.UpdToolGenes(true, geneDef);
					morpher?.DoEffects(pawn);
				}
			});
		}

	}

}
