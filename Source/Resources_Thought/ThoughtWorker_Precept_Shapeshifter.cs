using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ThoughtWorker_Precept_HasAnyXenotypesAndCount : ThoughtWorker_Precept
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
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.genes != null && item.genes.Xenotype != null && item.genes.Xenotype != XenotypeDefOf.Baseliner)
					{
						return ThoughtState.ActiveDefault;
					}
				}
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return MiscUtility.CountAllPlayerXenos();
		}

	}

	public class ThoughtWorker_Precept_HasAnyNonHumanlikeAndCount : ThoughtWorker_Precept
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
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (!item.RaceProps.Humanlike)
					{
						return ThoughtState.ActiveDefault;
					}
				}
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return MiscUtility.CountAllPlayerNonHumanlikes();
		}

	}

	public class ThoughtWorker_Precept_HasAnyMechanoidsAndCount : ThoughtWorker_Precept
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
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.IsColonyMechPlayerControlled)
					{
						return ThoughtState.ActiveDefault;
					}
				}
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return MiscUtility.CountAllPlayerMechs();
		}

	}

	public class ThoughtWorker_Precept_HasAnyAnimalsAndCount : ThoughtWorker_Precept
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
				foreach (Pawn item in maps[i].mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
				{
					if (item.RaceProps.Animal)
					{
						return ThoughtState.ActiveDefault;
					}
				}
			}
			return ThoughtState.Inactive;
		}

		public override float MoodMultiplier(Pawn p)
		{
			return MiscUtility.CountAllPlayerAnimals();
		}

	}

}
