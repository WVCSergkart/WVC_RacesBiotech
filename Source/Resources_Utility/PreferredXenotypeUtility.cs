using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class PreferredXenotypesUtility
	{

		//private static Dictionary<Pawn, Ideo> notPreferredPawns = new();
		//private static Dictionary<Pawn, Ideo> preferredPawns = new();

		public static float ReqMatch => WVC_Biotech.settings.preferredXenotypes_RequiredMinMatch;
		public static bool Enabled => WVC_Biotech.settings.preferredXenotypes_enableTweak;

		private static HashSet<IdeoPawnsHolder> notPreferredPawns = new();
		private static HashSet<IdeoPawnsHolder> preferredPawns = new();

		public class IdeoPawnsHolder
		{

			public Ideo ideo;
			public HashSet<Pawn> pawns = new();

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
			if (InList(preferredPawns, caller, ideo))
			{
				return true;
			}
			if (InList(notPreferredPawns, caller, ideo))
			{
				return false;
			}
			List<Gene> genesListForReading = caller.genes.GenesListForReading;
			foreach (XenotypeDef xenotypeDef in ideo.PreferredXenotypes)
			{
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
			if (XaG_GeneUtility.GenesIsMatch(genesListForReading, genes, ReqMatch))
			{
				Add(preferredPawns, caller, ideo);
			}
			else
			{
				Add(notPreferredPawns, caller, ideo);
			}

			static void Add(HashSet<IdeoPawnsHolder> list, Pawn caller, Ideo ideo)
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

		private static bool InList(HashSet<IdeoPawnsHolder> list, Pawn caller, Ideo ideo)
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
			cachedSameXenotypes = new();
		}


		private static HashSet<SameXenotype> cachedSameXenotypes = new();

		public static bool IsSameXenotype(Pawn caller, Pawn target, int hop = 0)
		{
			if (hop > 2)
			{
				return false;
			}
			if (caller?.genes == null || target?.genes == null)
			{
				return caller?.def == target?.def;
			}
			bool initNewHolder = true;
			foreach (SameXenotype sameXenotype in cachedSameXenotypes)
			{
				if (!sameXenotype.Contains(caller))
				{
					continue;
				}
				initNewHolder = false;
				if (sameXenotype.IsSame(caller, target))
				{
					return true;
				}
			}
			if (initNewHolder)
			{
				cachedSameXenotypes.Add(new(caller));
				return IsSameXenotype(caller, target, hop++);
			}
			return false;
		}

		public static void Debug_LogAllSameXenotypes()
		{
			IReadOnlyList<Pawn> allMapsCaravansAndTravellingTransporters_AliveSpawned = PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned;
			foreach (Pawn target in allMapsCaravansAndTravellingTransporters_AliveSpawned)
			{
				foreach (Pawn target2 in allMapsCaravansAndTravellingTransporters_AliveSpawned)
				{
					PreferredXenotypesUtility.IsSameXenotype(target2, target);
					PreferredXenotypesUtility.IsSameXenotype(target, target2);
				}
			}
			StringBuilder stringBuild2 = new();
			int num = 0;
			foreach (SameXenotype target in cachedSameXenotypes)
			{
				stringBuild2.AppendLine(num.ToString());
				foreach (Pawn pawn in target.pawns)
				{
					stringBuild2.AppendLine(pawn.Name.ToStringShort);
				}
				stringBuild2.AppendLine();
				num++;
			}
			Log.Error(stringBuild2.ToString());
		}

		public class SameXenotype
		{

			private List<GeneDef> geneDefs = new();
			public HashSet<Pawn> pawns = new();
			private HashSet<Pawn> blacklist = new();

			//public SameXenotype()
			//{

			//}

			public SameXenotype(Pawn pawn)
			{
				geneDefs = pawn.genes.GenesListForReading.Where(gene => gene.def.passOnDirectly).ToList().ConvertToDefs();
				pawns.Add(pawn);
			}

			public bool IsSame(Pawn caller, Pawn target)
			{
				if (blacklist.Contains(caller) || blacklist.Contains(target))
				{
					return false;
				}
				bool containsCaller = pawns.Contains(caller);
				bool containsTarget = pawns.Contains(target);
				if (containsCaller && containsTarget)
				{
					return true;
				}
				//Log.Error("1");
				if (geneDefs.Empty())
				{
					if (IsSame_Baseliner(containsCaller, target) || IsSame_Baseliner(containsTarget, caller))
					{
						return true;
					}
				}
				else
				{
					if (IsSame_Sub(containsCaller, target) || IsSame_Sub(containsTarget, caller))
					{
						return true;
					}
				}
				if (!containsCaller)
				{
					blacklist.Add(caller);
				}
				if (!containsTarget)
				{
					blacklist.Add(target);
				}
				return false;

				bool IsSame_Sub(bool contains, Pawn pawn)
				{
					if (contains && XaG_GeneUtility.GenesIsMatch(pawn.genes.GenesListForReading, geneDefs, ReqMatch))
					{
						pawns.Add(pawn);
						return true;
					}
					return false;
				}

				bool IsSame_Baseliner(bool contains, Pawn pawn)
				{
					if (contains && !pawn.genes.GenesListForReading.Any(gene => gene.def.passOnDirectly))
					{
						pawns.Add(pawn);
						return true;
					}
					return false;
				}
			}

			public bool Contains(Pawn pawn)
			{
				return pawns.Contains(pawn);
			}

		}

	}

}