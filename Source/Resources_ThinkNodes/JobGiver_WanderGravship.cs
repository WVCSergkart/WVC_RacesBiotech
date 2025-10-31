using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class JobGiver_WanderGravship : JobGiver_Wander
	{
		public JobGiver_WanderGravship()
		{
			wanderRadius = 7f;
			locomotionUrgency = LocomotionUrgency.Walk;
			ticksBetweenWandersRange = new IntRange(125, 200);
			wanderDestValidator = (Pawn p, IntVec3 c, IntVec3 root) => c.GetVacuum(p.Map) < 0.5f && (Find.CurrentGravship == null || Find.CurrentGravship.Bounds.Cells.Contains(c));
		}

		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return pawn.Position;
		}
	}

}
