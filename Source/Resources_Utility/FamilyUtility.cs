using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public static class FamilyUtility
	{

		public static List<Family> families = new();

		public class Family
		{

			public List<Pawn> pawns = new();

			public void AddToFamily(Pawn pawn)
			{
				if (pawn.mutant?.Def?.thinkTree != null)
				{
					return;
				}
				if (!InFamily(pawn))
				{
					pawns.Add(pawn);
				}
			}

			//public void RemoveFromFamily(Pawn pawn)
			//{
			//    if (!InFamily(pawn))
			//    {
			//        pawns.Remove(pawn);
			//    }
			//}

			public bool InFamily(Pawn pawn)
			{
				return pawns.Contains(pawn);
			}

			public void AddRange(List<Pawn> newPawns)
			{
				foreach (Pawn pawn in newPawns)
				{
					AddToFamily(pawn);
				}
			}

			public void AddRange(IEnumerable<Pawn> newPawns)
			{
				foreach (Pawn pawn in newPawns)
				{
					AddToFamily(pawn);
				}
			}

			public bool HasBloodRelationWithAny(Pawn pawn)
			{
				foreach (Pawn item in pawns)
				{
					if (item.relations.FamilyByBlood.Contains(pawn))
					{
						return true;
					}
				}
				return false;
			}

		}

		//public void RemoveFromAllFamilies(Pawn pawn)
		//{
		//    foreach (Family family in families)
		//    {
		//        family.RemoveFromFamily(pawn);
		//    }
		//}

		public static Family GetFamilyOf(Pawn pawn)
		{
			if (pawn.mutant?.Def?.thinkTree != null)
			{
				return null;
			}
			foreach (Family family in families)
			{
				if (family.InFamily(pawn))
				{
					return family;
				}
				else if (family.HasBloodRelationWithAny(pawn))
				{
					//RemoveFromAllFamilies(pawn);
					family.AddToFamily(pawn);
					return family;
				}
			}
			return null;
		}

		public static void AddFamilyOf(Pawn pawn)
		{
			Family family = GetFamilyOf(pawn);
			if (family == null)
			{
				Family newFamily = new();
				newFamily.pawns = new() { pawn };
				newFamily.AddRange(pawn.relations.FamilyByBlood);
				families.Add(newFamily);
			}
			else
			{
				family.AddToFamily(pawn);
			}
		}

		public static void UpdPlayerFamilies()
		{
			families = new();
			List<Pawn> playerPawn = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction.Where((pawn) => pawn.relations != null && pawn.RaceProps.Humanlike && !pawn.IsSlave && !pawn.IsPrisoner && !pawn.IsQuestLodger() && !pawn.Suspended).ToList();
			foreach (Pawn pawn in playerPawn)
			{
				AddFamilyOf(pawn);
			}
			cachedFamiliesCount = families.Count();
		}

		public static int lastRecacheTick = -1;
		public static int cachedFamiliesCount = 0;

		public static void UpdCollection()
		{
			if (lastRecacheTick < Find.TickManager.TicksGame)
			{
				UpdPlayerFamilies();
				lastRecacheTick = Find.TickManager.TicksGame + 6000;
			}
		}

	}

}
