using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_Precept_Social_Hivemind : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			// I am hiver
			if (HivemindUtility.InHivemind(p))
			{
				// My friend non hiver
				if (!HivemindUtility.InHivemind(otherPawn))
				{
					return ThoughtState.ActiveAtStage(1);
				}
				// My friend is hiver too
				return ThoughtState.ActiveAtStage(2);
			}
			// I am not hiver, but my friend..
			if (HivemindUtility.InHivemind(otherPawn))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			// No hivers
			return ThoughtState.Inactive;
		}

	}

	public class ThoughtWorker_Precept_Social_Hivemind_Reviled : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			// I am hiver
			if (HivemindUtility.InHivemind(p))
			{
				// My friend non hiver
				if (!HivemindUtility.InHivemind(otherPawn))
				{
					return ThoughtState.ActiveAtStage(1);
				}
				// My friend is hiver too
				return ThoughtState.ActiveAtStage(2);
			}
			// I am not hiver, but my friend..
			if (HivemindUtility.InHivemind(otherPawn))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			return ThoughtState.ActiveAtStage(3);
		}

	}

	public class ThoughtWorker_Precept_Hivemind : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (HivemindUtility.InHivemind(p))
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

	}

}
