using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public abstract class ThoughtWorker_Precept_Shapeshifter : ThoughtWorker_Precept
    {

		public static List<Pawn> shapeshifters = new();
		public static List<Pawn> nonShapeshifters = new();

		public static bool? anyShapeshiftersInFaction;
		public static bool AnyShapeshifters
		{
			get
			{
				if (!anyShapeshiftersInFaction.HasValue)
				{
					foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
					{
						anyShapeshiftersInFaction = false;
						if (IsShapeshifter(item))
						{
							anyShapeshiftersInFaction = true;
							break;
						}
					}
				}
				return anyShapeshiftersInFaction.Value;
			}
		}

		public static bool? shapeshifterLeader;
        public static bool GetShapeshifterLeader(Pawn caller)
        {
			UpdLeader();
			if (!shapeshifterLeader.HasValue)
            {
                shapeshifterLeader = false;
                foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
                {
                    if (IsShapeshifter(item))
                    {
                        Precept_Role precept_Role = item.Ideo?.GetRole(item);
                        if (precept_Role != null && precept_Role.ideo == caller.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
                        {
                            shapeshifterLeader = true;
                            break;
                        }
                    }
                }
            }
            return shapeshifterLeader.Value;
        }

        public static bool IsShapeshifter(Pawn pawn)
		{
			if (nonShapeshifters.Contains(pawn))
			{
				return false;
			}
			if (shapeshifters.Contains(pawn))
			{
				return true;
			}
            if (pawn.IsShapeshifterChimeraOrMorpher())
            {
                shapeshifters.Add(pawn);
				return true;
			}
            else
            {
                nonShapeshifters.Add(pawn);
			}
            return false;
		}

		public static void UpdCollection()
        {
			shapeshifters = new();
			nonShapeshifters = new();
			shapeshifterLeader = null;
			anyShapeshiftersInFaction = null;
		}

		public static int lastRecacheTick = -1;
		public static void UpdLeader()
		{
			if (lastRecacheTick < Find.TickManager.TicksGame)
			{
				shapeshifterLeader = null;
				lastRecacheTick = Find.TickManager.TicksGame + 7232;
			}
		}

	}

    public class ThoughtWorker_Precept_Shapeshifter_Social : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			return ThoughtWorker_Precept_Shapeshifter.IsShapeshifter(otherPawn);
		}

	}

	[Obsolete]
	public class ThoughtWorker_Precept_ShapeshifterPresent : ThoughtWorker_Precept_Shapeshifter
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (AnyShapeshifters)
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

	}

	public class ThoughtWorker_Precept_ShapeshifterColonist : ThoughtWorker_Precept_Shapeshifter
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction == null || IsShapeshifter(p))
			{
				return ThoughtState.Inactive;
			}
			if (GetShapeshifterLeader(p))
			{
				return ThoughtState.ActiveAtStage(2);
			}
			if (AnyShapeshifters)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}

	[Obsolete]
	public class ThoughtWorker_Precept_IsShapeshifter : ThoughtWorker_Precept_Shapeshifter
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			return IsShapeshifter(p);
		}
	}

}
