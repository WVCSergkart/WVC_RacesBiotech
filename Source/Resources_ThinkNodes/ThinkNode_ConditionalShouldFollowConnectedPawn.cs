using RimWorld;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
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
			Pawn respectedMaster = MiscUtility.GetConnectedPawn(pawn);
			if (respectedMaster == null)
			{
				return false;
			}
			if (ModsConfig.OdysseyActive && respectedMaster.InSpace())
			{
				return false;
			}
			if (respectedMaster.Spawned)
			{
				if ((respectedMaster.Drafted || (respectedMaster.Downed && !respectedMaster.InBed())) && pawn.CanReach(respectedMaster, PathEndMode.OnCell, Danger.Deadly))
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

}
