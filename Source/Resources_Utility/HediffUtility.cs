using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class HediffUtility
	{

		public static bool TryAddOrRemoveHediff(HediffDef hediffDef, Pawn pawn, Gene gene, List<BodyPartDef> bodyparts = null)
		{
			if (hediffDef == null)
			{
				return false;
			}
			if (gene.Active)
			{
				if (!pawn.health.hediffSet.HasHediff(hediffDef))
				{
					if (!bodyparts.NullOrEmpty())
					{
						Gene_PermanentHediff.BodyPartsGiver(bodyparts, pawn, hediffDef, gene);
						return true;
					}
					// pawn.health.AddHediff(hediffDef);
					Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
					HediffComp_RemoveIfGeneIsNotActive hediff_GeneCheck = hediff.TryGetComp<HediffComp_RemoveIfGeneIsNotActive>();
					if (hediff_GeneCheck != null)
					{
						hediff_GeneCheck.geneDef = gene.def;
					}
					pawn.health.AddHediff(hediff);
					return true;
				}
			}
			else
			{
				TryRemoveHediff(hediffDef, pawn);
			}
			return false;
		}

		public static bool TryRemoveHediff(HediffDef hediffDef, Pawn pawn)
		{
			if (hediffDef == null)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(hediffDef))
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
				return true;
			}
			return false;
		}

		// Heads

		public static bool HeadTypeIsCorrect(Pawn pawn, List<HeadTypeDef> headTypeDefs)
		{
			if (pawn?.genes == null || pawn?.story == null)
			{
				return false;
			}
			if (headTypeDefs.Contains(pawn.story.headType))
			{
				if (pawn?.health != null && pawn?.health?.hediffSet != null)
				{
					if (HasEyesGraphic(pawn) || AnyEyeIsMissing(pawn))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static bool AnyEyeIsMissing(Pawn pawn)
		{
			List<Hediff_MissingPart> missingPart = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
			for (int i = 0; i < missingPart.Count; i++)
			{
				if (missingPart[i].Part.def.tags.Contains(BodyPartTagDefOf.SightSource))
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasEyesGraphic(Pawn pawn)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def.RenderNodeProperties != null || hediffs[i].def.RenderNodeProperties != null)
				{
					return true;
				}
			}
			return false;
		}

		// Add and Remove

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

		// Misc

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
