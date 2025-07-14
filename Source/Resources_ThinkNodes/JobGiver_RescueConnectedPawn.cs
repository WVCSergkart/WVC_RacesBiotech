using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class JobGiver_RescueConnectedPawn : ThinkNode_JobGiver
	{

		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn queen = MiscUtility.GetConnectedPawn(pawn);
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
