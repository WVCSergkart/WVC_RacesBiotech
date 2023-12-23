using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_Precept_HasXenoTreeAnyHomeMap : ThoughtWorker_Precept
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
				if (maps[i].IsPlayerHome)
				{
					foreach (Thing item in maps[i].listerThings.AllThings)
					{
						if (item.IsXenoTree())
						{
							return ThoughtState.ActiveDefault;
						}
					}
				}
			}
			return ThoughtState.Inactive;
		}

	}

	public class ThoughtWorker_Precept_HasResurgentTreeAnyHomeMap : ThoughtWorker_Precept
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
				if (maps[i].IsPlayerHome)
				{
					foreach (Thing item in maps[i].listerThings.AllThings)
					{
						if (item.IsResurgentTree())
						{
							return ThoughtState.ActiveDefault;
						}
					}
				}
			}
			return ThoughtState.Inactive;
		}

	}

}
