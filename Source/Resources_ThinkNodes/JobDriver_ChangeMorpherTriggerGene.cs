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

		//public ChimeraDef chimeraDef;

		//public new void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Defs.Look(ref geneDef, "geneDef");
		//}

	}

	public abstract class JobDriver_XaGJob_General : JobDriver
	{

		public GeneDef geneDef;

		//public ChimeraDef chimeraDef;

		public override void Notify_Starting()
		{
			base.Notify_Starting();
			if (geneDef == null && job is XaG_Job xaG_Job)
			{
				geneDef = xaG_Job.geneDef;
				//chimeraDef = xaG_Job.chimeraDef;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref geneDef, "geneDef");
			//Scribe_Defs.Look(ref chimeraDef, "chimeraDef");
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
