using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public abstract class JobGiver_NewDryads_CreateAndEnterDryadHolder : ThinkNode_JobGiver
	{

		public int squareRadius = 4;

		public abstract JobDef JobDef { get; }

		public virtual bool ExtraValidator(Pawn pawn, CompGauranlenDryad connectionComp)
		{
			return false;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			CompGauranlenDryad compTreeConnection = pawn?.TryGetComp<CompGauranlenDryad>();
			if (compTreeConnection == null)
			{
				return null;
			}
			Pawn master = compTreeConnection.Master;
			if (master == null)
			{
				return null;
			}
			if (compTreeConnection != null && ExtraValidator(pawn, compTreeConnection) && !master.IsForbidden(pawn) && pawn.CanReach(master, PathEndMode.Touch, Danger.Deadly) && CellFinder.TryFindRandomCellNear(master.Position, pawn.Map, squareRadius, (IntVec3 c) => GauranlenUtility.CocoonAndPodCellValidator(c, pawn.Map), out var _))
			{
				return JobMaker.MakeJob(JobDef, master);
			}
			return null;
		}

	}

	public class JobGiver_NewDryads_CreateAndEnterCocoon : JobGiver_NewDryads_CreateAndEnterDryadHolder
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

	public class JobGiver_NewDryads_CreateAndEnterHealingPod : JobGiver_NewDryads_CreateAndEnterDryadHolder
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

	public abstract class JobDriver_NewDryads_CreateAndEnterDryadHolder: JobDriver
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

	public class JobDriver_NewDryads_CreateAndEnterHealingPod : JobDriver_NewDryads_CreateAndEnterDryadHolder
	{
		public override Toil EnterToil()
		{
			return Toils_General.Do(delegate
			{
				GenSpawn.Spawn(Props.thingDefToSpawn, job.targetB.Cell, pawn.Map).TryGetComp<CompDryadHealingPod_WithGene>().TryAcceptPawn(pawn);
			});
		}
	}

	public class JobDriver_NewDryads_CreateAndEnterCocoon : JobDriver_NewDryads_CreateAndEnterDryadHolder
	{
		public override Toil EnterToil()
		{
			return Toils_General.Do(delegate
			{
				GenSpawn.Spawn(Props.thingDefToSpawn, job.targetB.Cell, pawn.Map).TryGetComp<CompDryadCocoon_WithGene>().TryAcceptPawn(pawn);
			});
		}
	}

	// Queen Dryads

	public class ThinkNode_ConditionalShouldFollowConnectedPawn : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn pawn)
		{
			return ShouldFollowConnectedPawn(pawn);
		}

		public static bool ShouldFollowConnectedPawn(Pawn pawn)
		{
			if (!pawn.Spawned || pawn.playerSettings == null)
			{
				return false;
			}
			Pawn respectedMaster = (Pawn)pawn.connections.ConnectedThings.FirstOrDefault();
			if (respectedMaster == null)
			{
				return false;
			}
			if (respectedMaster.Spawned)
			{
				if (respectedMaster.Drafted && pawn.CanReach(respectedMaster, PathEndMode.OnCell, Danger.Deadly))
				{
					return true;
				}
				if (respectedMaster.mindState.lastJobTag == JobTag.Fieldwork && pawn.CanReach(respectedMaster, PathEndMode.OnCell, Danger.Deadly))
				{
					return true;
				}
			}
			else
			{
				Pawn carriedBy = respectedMaster.CarriedBy;
				if (carriedBy != null && carriedBy.HostileTo(respectedMaster) && pawn.CanReach(carriedBy, PathEndMode.OnCell, Danger.Deadly))
				{
					return true;
				}
			}
			return false;
		}

	}

	public class JobGiver_AIDefendConnectedPawn : JobGiver_AIDefendPawn
	{

		public float radiusUnreleased = 5f;
		public float radiusReleased = 50f;

		protected override Pawn GetDefendee(Pawn pawn)
		{
			return pawn.connections.ConnectedThings.FirstOrDefault() as Pawn;
		}

		protected override float GetFlagRadius(Pawn pawn)
		{
			if (pawn.playerSettings.Master?.playerSettings?.animalsReleased == true && pawn.training.HasLearned(TrainableDefOf.Release))
			{
				return radiusReleased;
			}
			return radiusUnreleased;
		}

	}

	public class JobGiver_AIFollowConnectedPawn : JobGiver_AIFollowPawn
	{

		public float radiusUnreleased = 5f;
		public float radiusReleased = 50f;

		protected override int FollowJobExpireInterval => 200;

		protected override Pawn GetFollowee(Pawn pawn)
		{
			if (pawn.playerSettings == null)
			{
				return null;
			}
			return pawn.connections.ConnectedThings.FirstOrDefault() as Pawn;
		}

		protected override float GetRadius(Pawn pawn)
		{
			if (pawn.playerSettings.Master?.playerSettings?.animalsReleased == true && pawn.training.HasLearned(TrainableDefOf.Release))
			{
				return radiusReleased;
			}
			return radiusUnreleased;
		}

	}

	public class JobGiver_WanderNearConnectedPawn : JobGiver_Wander
	{

		public JobGiver_WanderNearConnectedPawn()
		{
			wanderRadius = 3f;
			ticksBetweenWandersRange = new IntRange(125, 200);
			wanderDestValidator = (Pawn p, IntVec3 c, IntVec3 root) => (!MustUseRootRoom(p) || root.GetRoom(p.Map) == null || WanderRoomUtility.IsValidWanderDest(p, c, root));
		}

		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return WanderUtility.BestCloseWanderRoot(pawn.connections.ConnectedThings.FirstOrDefault().PositionHeld, pawn);
		}

		private bool MustUseRootRoom(Pawn pawn)
		{
			return pawn.playerSettings.Master?.playerSettings?.animalsReleased == true;
		}

	}

}
