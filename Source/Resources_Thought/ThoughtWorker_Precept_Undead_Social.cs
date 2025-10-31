using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class ThoughtWorker_Precept_Resurrected_Social : ThoughtWorker_Precept_Social
	//{

	//	protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
	//	{
	//		return otherPawn.IsUndead();
	//	}

	//}

	public class ThoughtWorker_Precept_Undead_Social : ThoughtWorker_Precept_Social
	{
		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			return otherPawn.IsUndead();
		}
	}

	public class ThoughtWorker_Precept_UndeadPresent : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.IsUndead())
			{
				return ThoughtState.Inactive;
			}
			foreach (Pawn item in p.MapHeld.mapPawns.AllPawnsSpawned)
			{
				if (item.IsUndead() && (item.IsPrisonerOfColony || item.IsSlaveOfColony || item.IsColonist))
				{
					return ThoughtState.ActiveDefault;
				}
			}
			return ThoughtState.Inactive;
		}
	}

	public class ThoughtWorker_Precept_UndeadColonist : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.IsUndead() || p.Faction == null)
			{
				return ThoughtState.Inactive;
			}
			_ = p.Ideo;
			bool flag = false;
			foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
			{
				if (item.IsUndead())
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

	public class ThoughtWorker_Precept_IsUndead : ThoughtWorker_Precept
	{
		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			return p.IsUndead();
		}
	}

}
