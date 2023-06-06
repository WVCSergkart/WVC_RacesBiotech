// RimWorld.ThoughtWorker_Pretty
using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public abstract class ThoughtWorker_Precept_MechanoidLabor : ThoughtWorker_Precept
	{
		protected abstract TimeAssignmentDef AssignmentDef { get; }

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (!ModsConfig.IdeologyActive || !ModsConfig.BiotechActive || !MechanitorUtility.AnyMechanitorInPlayerFaction())
			{
				return ThoughtState.Inactive;
			}
			foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
			{
				if (!item.RaceProps.Humanlike || item.timetable == null)
				{
					continue;
				}
				for (int i = 0; i < 24; i++)
				{
					if (item.timetable.GetAssignment(i) == AssignmentDef)
					{
						return ThoughtState.ActiveAtStage(1);
					}
				}
			}
			return ThoughtState.ActiveAtStage(0);
		}
	}

	public class ThoughtWorker_Precept_MechanoidLabor_AssignedWork : ThoughtWorker_Precept_MechanoidLabor
	{
		protected override TimeAssignmentDef AssignmentDef => TimeAssignmentDefOf.Work;
	}

	// public class ThoughtWorker_Precept_MechanoidLabor_AssignedAnything : ThoughtWorker_Precept_MechanoidLabor
	// {
		// protected override TimeAssignmentDef AssignmentDef => TimeAssignmentDefOf.Anything;
	// }
}
