using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class PreferredXenotypesUtility
	{

		//private static Dictionary<Pawn, Ideo> notPreferredPawns = new();
		//private static Dictionary<Pawn, Ideo> preferredPawns = new();

		public static float ReqMatch => WVC_Biotech.settings.preferredXenotypes_RequiredMinMatch;

		private static List<IdeoPawnsHolder> notPreferredPawns = new();
		private static List<IdeoPawnsHolder> preferredPawns = new();

		public class IdeoPawnsHolder
		{

			public Ideo ideo;
			public List<Pawn> pawns = new();

			public void Add(Pawn pawn)
			{
				if (!Contains(pawn))
				{
					pawns.Add(pawn);
				}
			}

			public bool Contains(Pawn pawn)
			{
				return pawns.Contains(pawn);
			}

		}

		public static bool IsPreferredXenotype(Pawn caller, Ideo ideo)
		{
			if (InList(notPreferredPawns, caller, ideo))
			{
				return false;
			}
			if (InList(preferredPawns, caller, ideo))
			{
				return true;
			}
			List<Gene> genesListForReading = caller.genes.GenesListForReading;
			foreach (XenotypeDef xenotypeDef in ideo.PreferredXenotypes)
			{
				//if (!XaG_GeneUtility.GenesIsMatch(genesListForReading, xenotypeDef.genes, ReqMatch))
				//{
				//	notPreferredPawns.Add(caller, ideo);
				//}
				//else
				//{
				//	preferredPawns.Add(caller, ideo);
				//}
				UpdLists(caller, ideo, genesListForReading, xenotypeDef.genes);
			}
			foreach (CustomXenotype customXenotype in ideo.PreferredCustomXenotypes)
			{
				UpdLists(caller, ideo, genesListForReading, customXenotype.genes);
			}
			return true;
		}

		public static void UpdLists(Pawn caller, Ideo ideo, List<Gene> genesListForReading, List<GeneDef> genes)
		{
			if (!XaG_GeneUtility.GenesIsMatch(genesListForReading, genes, ReqMatch))
			{
				Add(notPreferredPawns, caller, ideo);
			}
			else
			{
				Add(preferredPawns, caller, ideo);
			}

			static void Add(List<IdeoPawnsHolder> list, Pawn caller, Ideo ideo)
			{
				foreach (IdeoPawnsHolder item in list)
				{
					if (item.ideo == ideo)
					{
						item.Add(caller);
						return;
					}
				}
				IdeoPawnsHolder pawnIdeo = new();
				pawnIdeo.ideo = ideo;
				pawnIdeo.Add(caller);
				list.Add(pawnIdeo);
			}
		}

		private static bool InList(List<IdeoPawnsHolder> list, Pawn caller, Ideo ideo)
		{
			foreach (IdeoPawnsHolder item in list)
			{
				if (item.ideo == ideo && item.Contains(caller))
				{
					return true;
				}
			}
			return false;
		}

		public static void UpdCollection()
		{
			preferredPawns = new();
			notPreferredPawns = new();
			//cachedSameXenotypes = null;
		}

		//private static List<SameXenotype> cachedSameXenotypes = new();

		//public static bool IsSameXenotype(Pawn caller, Pawn other)
		//{
		//	if (cachedSameXenotypes == null)
		//	{
		//		InitSameXenotypes();
		//	}
		//	if (IsSameXenotype(cachedSameXenotypes, caller, other))
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		//private static void InitSameXenotypes()
		//{
		//	cachedSameXenotypes = new();
		//	foreach (XenotypeHolder xenotypeHolder in ListsUtility.GetAllXenotypesHolders())
		//	{
		//		SameXenotype sameXenotype = new();
		//		sameXenotype.xenotypeHolder = xenotypeHolder;
		//		sameXenotype.pawns = new();
		//		cachedSameXenotypes.Add(sameXenotype);
		//	}
		//}

		//private static bool IsSameXenotype(List<SameXenotype> list, Pawn caller, Pawn other)
		//{
		//	foreach (SameXenotype item in list)
		//	{
		//		if (item.TryAdd(caller) && item.Contains(other))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//public class SameXenotype
		//{

		//	public XenotypeHolder xenotypeHolder;
		//	public List<Pawn> pawns;

		//	public bool Contains(Pawn pawn)
		//	{
		//		if (pawns != null)
		//		{
		//			return pawns.Contains(pawn);
		//		}
		//		return false;
		//	}

		//	public bool TryAdd(Pawn pawn)
		//	{
		//		if (Contains(pawn))
		//		{
		//			return true;
		//		}
		//		if (!pawn.genes.UniqueXenotype && xenotypeHolder.xenotypeDef == pawn.genes.Xenotype)
		//		{
		//			pawns.Add(pawn);
		//			return true;
		//		}
		//		if (XaG_GeneUtility.GenesIsMatch(pawn.genes.GenesListForReading, xenotypeHolder.genes, 1f))
		//		{
		//			pawns.Add(pawn);
		//			return true;
		//		}
		//		return false;
		//	}

		//}

	}

}