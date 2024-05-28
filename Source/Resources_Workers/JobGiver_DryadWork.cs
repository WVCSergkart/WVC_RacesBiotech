using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class ThinkNode_ConditionalGestatedDryad : ThinkNode_Conditional
	{

		protected override bool Satisfied(Pawn pawn)
		{
			return pawn.TryGetComp<CompGauranlenDryad>() != null;
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
			Pawn respectedMaster = MiscUtility.GetFirstConnectedPawn(pawn);
			if (respectedMaster == null)
			{
				return false;
			}
			if (respectedMaster.Spawned)
			{
				if ((respectedMaster.Drafted || respectedMaster.Downed) && pawn.CanReach(respectedMaster, PathEndMode.OnCell, Danger.Deadly))
				{
					return true;
				}
				if (respectedMaster.mindState.lastJobTag == JobTag.Fieldwork && pawn.CanReach(respectedMaster, PathEndMode.OnCell, Danger.Deadly) && pawn.training.HasLearned(TrainableDefOf.Obedience))
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
			return MiscUtility.GetFirstConnectedPawn(pawn);
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
			return MiscUtility.GetFirstConnectedPawn(pawn);
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
			return WanderUtility.BestCloseWanderRoot(MiscUtility.GetFirstConnectedPawn(pawn).PositionHeld, pawn);
		}

		private bool MustUseRootRoom(Pawn pawn)
		{
			return pawn.playerSettings.Master?.playerSettings?.animalsReleased == true;
		}

	}

	public class JobGiver_RescueConnectedPawn : ThinkNode_JobGiver
	{

		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn queen = MiscUtility.GetFirstConnectedPawn(pawn);
			if (queen?.Downed != true)
			{
				return null;
			}
			if (!HealthAIUtility.CanRescueNow(pawn, queen))
			{
				return null;
			}
			Building_Bed building_Bed = RestUtility.FindBedFor(queen, pawn, checkSocialProperness: false, ignoreOtherReservations: false, queen.GuestStatus);
			if (building_Bed == null || !queen.CanReserve(building_Bed))
			{
				return null;
			}
			Job job = JobMaker.MakeJob(JobDefOf.Rescue, queen, building_Bed);
			job.count = 1;
			return job;
		}

	}

}
