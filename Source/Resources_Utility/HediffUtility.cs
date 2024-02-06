using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class HediffUtility
	{

		public static void AddHediffsFromList(Pawn pawn, List<HediffDef> hediffDefs)
		{
			if (hediffDefs.NullOrEmpty() || pawn?.health?.hediffSet == null)
			{
				return;
			}
			foreach (HediffDef item in hediffDefs.ToList())
			{
				if (pawn.health.hediffSet.HasHediff(item))
				{
					continue;
				}
				pawn.health.AddHediff(item);
			}
		}

		public static void RemoveHediffsFromList(Pawn pawn, List<HediffDef> hediffDefs)
		{
			List<Hediff> hediffs = pawn?.health?.hediffSet?.hediffs;
			if (hediffs.NullOrEmpty() || hediffDefs.NullOrEmpty())
			{
				return;
			}
			foreach (Hediff item in hediffs.ToList())
			{
				if (!hediffDefs.Contains(item.def))
				{
					continue;
				}
				pawn.health.RemoveHediff(item);
			}
		}

		public static Hediff GetFirstHediffPreventsPregnancy(List<Hediff> hediffs)
		{
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.preventsPregnancy)
				{
					return hediffs[i];
				}
			}
			return null;
		}

		public static bool HasAnyHediff(List<HediffDef> hediffDefs, Pawn pawn)
		{
			List<Hediff> hediffs = pawn?.health?.hediffSet?.hediffs;
			if (hediffs.NullOrEmpty() || hediffDefs.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < hediffDefs.Count; i++)
			{
				for (int j = 0; j < hediffs.Count; j++)
				{
					if (hediffs[j].def == hediffDefs[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static HediffDef GetAnyHediffFromList(List<HediffDef> hediffDefs, Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffDefs.Count; i++)
			{
				for (int j = 0; j < hediffs.Count; j++)
				{
					if (hediffs[j].def == hediffDefs[i])
					{
						return hediffDefs[i];
					}
				}
			}
			return null;
		}

	}
}
