using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public abstract class ThoughtWorker_Precept_Shapeshifter : ThoughtWorker_Precept
	//{
	//}

	public class ThoughtWorker_Precept_Shapeshifter_Social : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			return GeneshiftUtility.IsShapeshifter(otherPawn);
		}

	}

	[Obsolete]
	public class ThoughtWorker_Precept_ShapeshifterPresent : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (GeneshiftUtility.AnyShapeshifters)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

	}

	public class ThoughtWorker_Precept_ShapeshifterColonist : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction == null || GeneshiftUtility.IsShapeshifter(p))
			{
				return ThoughtState.Inactive;
			}
			if (GeneshiftUtility.GetShapeshifterLeader(p))
			{
				return ThoughtState.ActiveAtStage(2);
			}
			if (GeneshiftUtility.AnyShapeshifters)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}

	[Obsolete]
	public class ThoughtWorker_Precept_IsShapeshifter : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			return GeneshiftUtility.IsShapeshifter(p);
		}
	}

}
