using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ThoughtWorker_Precept_HasWalkingCorpses : ThoughtWorker_Precept
	{

		protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (p.Faction == null || p.IsSlave)
			{
				return false;
			}
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (!ModsConfig.BiotechActive)
				{
					continue;
				}
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsColonyMechPlayerControlled && WalkingUtility.PawnIsBoneGolem(item))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

}
