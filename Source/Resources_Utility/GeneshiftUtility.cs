using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class GeneshiftUtility
	{

		public static int lastRecacheTick = -1;

		private static bool? anyShapeshiftersInFaction;

		private static bool? shapeshifterLeader;
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

		private static List<Pawn> chimerkins = new();
		private static List<Pawn> nonChimerkins = new();

		public static bool IsChimerkin(this Pawn pawn)
		{
			if (nonChimerkins.Contains(pawn))
			{
				return false;
			}
			if (chimerkins.Contains(pawn))
			{
				return true;
			}
			if (pawn.genes?.GetFirstGeneOfType<Gene_Chimera>() != null)
			{
				chimerkins.Add(pawn);
				return true;
			}
			else
			{
				nonChimerkins.Add(pawn);
			}
			return false;
		}

		private static List<Pawn> shapeshifters = new();
		private static List<Pawn> nonShapeshifters = new();

		public static bool IsShapeshifter(this Pawn pawn)
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

		public static void ResetXenotypesCollection()
		{
			shapeshifters = new();
			nonShapeshifters = new();
			shapeshifterLeader = null;
			anyShapeshiftersInFaction = null;
			chimerkins = new();
			nonChimerkins = new();
			PreferredXenotypesUtility.UpdCollection();
		}
		public static void UpdLeader()
		{
			if (lastRecacheTick < Find.TickManager.TicksGame)
			{
				shapeshifterLeader = null;
				lastRecacheTick = Find.TickManager.TicksGame + 7232;
			}
		}

	}

}