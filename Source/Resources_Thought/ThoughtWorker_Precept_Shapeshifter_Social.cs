using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ThoughtWorker_Precept_Shapeshifter_Social : ThoughtWorker_Precept_Social
	{
		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive)
			{
				return ThoughtState.Inactive;
			}
			return otherPawn.IsShapeshifter();
		}
	}

	public class ThoughtWorker_Precept_ShapeshifterPresent : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive || p.IsShapeshifter())
			{
				return ThoughtState.Inactive;
			}
			foreach (Pawn item in p.MapHeld.mapPawns.AllPawnsSpawned)
			{
				if (item.IsShapeshifter() && (item.IsPrisonerOfColony || item.IsSlaveOfColony || item.IsColonist))
				{
					return ThoughtState.ActiveDefault;
				}
			}
			return ThoughtState.Inactive;
		}
	}

	public class ThoughtWorker_Precept_ShapeshifterColonist : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive || p.IsShapeshifter() || p.Faction == null)
			{
				return ThoughtState.Inactive;
			}
			_ = p.Ideo;
			bool flag = false;
			foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
			{
				if (item.IsShapeshifter())
				{
					flag = true;
					Precept_Role precept_Role = item.Ideo?.GetRole(item);
					if (precept_Role != null && precept_Role.ideo == p.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
					{
						return ThoughtState.ActiveAtStage(2);
					}
				}
			}
			if (flag)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}

	public class ThoughtWorker_Precept_IsShapeshifter : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive)
			{
				return ThoughtState.Inactive;
			}
			return p.IsShapeshifter();
		}
	}

}
