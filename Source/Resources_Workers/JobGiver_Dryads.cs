using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public abstract class JobGiver_CreateAndEnterDryadHolder_Gene : ThinkNode_JobGiver
	{

		public int squareRadius = 4;

		public abstract JobDef JobDef { get; }

		public virtual bool ExtraValidator(Pawn pawn, CompGauranlenDryad connectionComp)
		{
			return false;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (pawn.connections == null || pawn.connections.ConnectedThings.NullOrEmpty())
			{
				return null;
			}
			CompGauranlenDryad compTreeConnection = pawn.TryGetComp<CompGauranlenDryad>();
			foreach (Thing connectedThing in pawn.connections.ConnectedThings)
			{
				if (connectedThing is not Pawn master)
				{
					continue;
				}
				if (compTreeConnection != null && ExtraValidator(pawn, compTreeConnection) && !master.IsForbidden(pawn) && pawn.CanReach(master, PathEndMode.Touch, Danger.Deadly) && CellFinder.TryFindRandomCellNear(master.Position, pawn.Map, squareRadius, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, pawn.Map), out var _))
				{
					return JobMaker.MakeJob(JobDef, master);
				}
			}
			return null;
		}

	}

	public class JobGiver_CreateAndEnterCocoon_Gene : JobGiver_CreateAndEnterDryadHolder_Gene
	{

		public JobDef cocoonJobDef;

		public override JobDef JobDef => cocoonJobDef;

		public override bool ExtraValidator(Pawn pawn, CompGauranlenDryad connectionComp)
		{
			if (connectionComp.DryadKind != pawn.kindDef)
			{
				return true;
			}
			return base.ExtraValidator(pawn, connectionComp);
		}
	}

	public class JobGiver_CreateAndEnterHealingPod_Gene : JobGiver_CreateAndEnterDryadHolder_Gene
	{

		public JobDef cocoonJobDef;

		public override JobDef JobDef => cocoonJobDef;

		public override bool ExtraValidator(Pawn pawn, CompGauranlenDryad connectionComp)
		{
			if (pawn.mindState != null && pawn.mindState.returnToHealingPod)
			{
				if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
				{
					return true;
				}
				if (pawn.health.hediffSet.GetMissingPartsCommonAncestors().Any())
				{
					return true;
				}
			}
			return base.ExtraValidator(pawn, connectionComp);
		}
	}

	// Driver

	public abstract class JobDriver_CreateAndEnterDryadHolder_Gene : JobDriver
	{

		public GeneExtension_Spawner Props => job?.def?.GetModExtension<GeneExtension_Spawner>();

		public const int ticksToCreate = 200;

		// private CompTreeConnection TreeComp => job.targetA.Thing.TryGetComp<CompTreeConnection>();

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			// this.FailOn(() => TreeComp.ShouldReturnToTree(pawn));
			yield return Toils_General.Do(delegate
			{
				// IntVec3 result = null;
				if (CellFinder.TryFindRandomCellNear(job.GetTarget(TargetIndex.A).Cell, pawn.Map, 4, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, pawn.Map), out var near))
				{
					job.targetB = near;
				}
				else if (CellFinder.TryFindRandomCell(pawn.Map, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, pawn.Map), out var any))
				{
					job.targetB = any;
				}
				// job.targetB = result;
			});
			yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
			yield return Toils_General.Wait(ticksToCreate).WithProgressBarToilDelay(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.B);
			yield return EnterToil();
		}

		public abstract Toil EnterToil();
	}

	public class JobDriver_CreateAndEnterHealingPod_Gene : JobDriver_CreateAndEnterDryadHolder_Gene
	{
		public override Toil EnterToil()
		{
			return Toils_General.Do(delegate
			{
				GenSpawn.Spawn(Props.thingDefToSpawn, job.targetB.Cell, pawn.Map).TryGetComp<CompDryadHealingPod_WithGene>().TryAcceptPawn(pawn);
			});
		}
	}

	public class JobDriver_CreateAndEnterCocoon_Gene : JobDriver_CreateAndEnterDryadHolder_Gene
	{
		public override Toil EnterToil()
		{
			return Toils_General.Do(delegate
			{
				GenSpawn.Spawn(Props.thingDefToSpawn, job.targetB.Cell, pawn.Map).TryGetComp<CompDryadCocoon_WithGene>().TryAcceptPawn(pawn);
			});
		}
	}

}
