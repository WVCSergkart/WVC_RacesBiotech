using System;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_Precept_Social_FamilyByBlood : ThoughtWorker_Precept_Social
	{


		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (p.relations == null)
			{
				return ThoughtState.Inactive;
			}
			if (p.relations.FamilyByBlood.Contains(otherPawn))
			{
				if (p.ideo.Ideo?.GetRole(otherPawn)?.def == PreceptDefOf.IdeoRole_Leader)
				{
					return ThoughtState.ActiveAtStage(1);
				}
				return ThoughtState.ActiveAtStage(0);
			}
			if (otherPawn.IsSlave)
			{
				return ThoughtState.ActiveAtStage(2);
			}
			return ThoughtState.ActiveAtStage(3);
		}

	}

	public class ThoughtWorker_Precept_FamilyByBlood : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.relations == null)
			{
				return ThoughtState.Inactive;
			}
			FamilyUtility.UpdCollection();
			if (FamilyUtility.cachedFamiliesCount > 1)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return FamilyUtility.cachedFamiliesCount - 1;
		}

	}

}
