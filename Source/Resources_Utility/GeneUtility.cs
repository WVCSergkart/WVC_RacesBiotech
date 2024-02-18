using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class XaG_GeneUtility
	{

		// Gene Restoration

		// public static void XenogermRestoration(Pawn pawn)
		// {
			// List<HediffDef> list = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// list.AddRange(item.hediffsRemovedByGenesRestorationSerum);
			// }
			// HediffUtility.RemoveHediffsFromList(pawn, list);
		// }

		// Misc

		public static List<GeneDef> ConvertGenesInGeneDefs(List<Gene> genes)
		{
			List<GeneDef> geneDefs = new();
			foreach (Gene item in genes)
			{
				geneDefs.Add(item.def);
			}
			return geneDefs;
		}

		public static bool AbilityIsGeneAbility(Ability ability)
		{
			List<GeneDef> genes = DefDatabase<GeneDef>.AllDefsListForReading;
			for (int i = 0; i < genes.Count; i++)
			{
				if (!genes[i].abilities.NullOrEmpty() && genes[i].abilities.Contains(ability.def))
				{
					return true;
				}
			}
			return false;
		}

		// ============================= Anti-Bug =============================

		public static bool PawnIsAndroid(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].def.defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		public static bool XenotypeIsAndroid(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genesListForReading = xenotypeDef?.genes;
			if (genesListForReading.NullOrEmpty())
			{
				return false;
			}
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		[Obsolete]
		public static bool PawnCannotUseSerums(Pawn pawn)
		{
			// if (pawn?.RaceProps?.Humanlike == false)
			// {
				// return true;
			// }
			if (pawn?.genes == null)
			{
				return true;
			}
			List<Def> blackListedThings = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				blackListedThings.AddRange(item.blackListedDefsForSerums);
			}
			if (blackListedThings.Contains(pawn.def))
			{
				return true;
			}
			List<GeneDef> nonCandidates = ReimplanterUtility.GenesNonCandidatesForSerums();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// nonCandidates.AddRange(item.nonCandidatesForSerums);
			// }
			for (int i = 0; i < nonCandidates.Count; i++)
			{
				if (HasActiveGene(nonCandidates[i], pawn))
				{
					return true;
				}
			}
			return false;
		}

		// ============================= Checker =============================

		public static bool HasAnyActiveGene(List<GeneDef> geneDefs, Pawn pawn)
		{
			if (geneDefs.NullOrEmpty() || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int j = 0; j < genesListForReading.Count; j++)
			{
				if (!genesListForReading[j].Active)
				{
					continue;
				}
				for (int i = 0; i < geneDefs.Count; i++)
				{
					if (genesListForReading[j].def == geneDefs[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool HasActiveGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null || pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true && genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		public static bool GenesIsMatchForPawns(List<Pawn> pawns, List<GeneDef> xenotypeGenes, float percent)
		{
			if (xenotypeGenes.NullOrEmpty() || percent <= 0f)
			{
				return true;
			}
			List<Gene> genes = new();
			foreach (Pawn pawn in pawns)
			{
				List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
				if (!genesListForReading.NullOrEmpty())
				{
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (!genes.Contains(gene))
						{
							genes.Add(gene);
						}
					}
				}
			}
			if (GenesIsMatch(genes, xenotypeGenes, percent))
			{
				return true;
			}
			return false;
		}

		public static bool GenesIsMatch(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes, float percent)
		{
			if (xenotypeGenes.NullOrEmpty() || percent <= 0f)
			{
				return true;
			}
			if (pawnGenes.NullOrEmpty())
			{
				return false;
			}
			List<GeneDef> matchingGenes = GetMatchingGenesList(pawnGenes, xenotypeGenes);
			if (matchingGenes.Count >= xenotypeGenes.Count * percent)
			{
				return true;
			}
			return false;
		}

		// public static bool PawnIsBaseliner(Pawn pawn)
		// {
			// if (pawn.genes == null)
			// {
				// return true;
			// }
			// if (pawn.genes.CustomXenotype != null)
			// {
				// return false;
			// }
			// if (pawn.genes.Xenotype == XenotypeDefOf.Baseliner)
			// {
				// return true;
			// }
			// return false;
		// }

		// ============================= Getter =============================

		public static GeneDef GetFirstGeneDefOfType(List<GeneDef> genes, Type type)
		{
			if (genes.NullOrEmpty())
			{
				return null;
			}
			for (int i = 0; i < genes.Count; i++)
			{
				if (genes[i].geneClass == type)
				{
					return genes[i];
				}
			}
			return null;
		}

		// public static Gene GetFirstGeneOfType(List<Gene> genes, Type type)
		// {
			// for (int i = 0; i < genes.Count; i++)
			// {
				// if (genes[i] is type)
				// {
					// return genes[i];
				// }
			// }
			// return null;
		// }

		public static List<XenotypeDef> GetAllMatchedXenotypes_ForPawns(List<Pawn> pawns, List<XenotypeDef> xenotypeDefs, float percent = 0.6f)
		{
			if (pawns.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			List<XenotypeDef> allMatched = new();
			// foreach (Pawn item in pawns)
			// {
				// List<XenotypeDef> matched = GetAllMatchedXenotypes(item, xenotypeDefs, percent);
				// foreach (XenotypeDef xeno in matched)
				// {
					// if (!allMatched.Contains(xeno))
					// {
						// allMatched.Add(xeno);
					// }
				// }
			// }
			List<Gene> genes = new();
			foreach (Pawn pawn in pawns)
			{
				List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
				if (!genesListForReading.NullOrEmpty())
				{
					foreach (Gene gene in pawn.genes.GenesListForReading)
					{
						if (!genes.Contains(gene))
						{
							genes.Add(gene);
						}
					}
				}
			}
			foreach (XenotypeDef item in xenotypeDefs)
			{
				if (GenesIsMatch(genes, item.genes, percent))
				{
					if (!allMatched.Contains(item))
					{
						allMatched.Add(item);
					}
				}
			}
			return allMatched;
		}

		public static List<XenotypeDef> GetAllMatchedXenotypes(Pawn pawn, List<XenotypeDef> xenotypeDefs, float percent = 0.6f)
		{
			List<Gene> pawnGenes = pawn?.genes?.GenesListForReading;
			if (pawnGenes.NullOrEmpty() || xenotypeDefs.NullOrEmpty())
			{
				return null;
			}
			List<XenotypeDef> matched = new();
			foreach (XenotypeDef item in xenotypeDefs)
			{
				if (GenesIsMatch(pawnGenes, item.genes, percent))
				{
					matched.Add(item);
				}
			}
			return matched;
		}

		public static List<GeneDef> GetMatchingGenesList(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes)
		{
			if (pawnGenes.NullOrEmpty() || xenotypeGenes.NullOrEmpty())
			{
				return null;
			}
			List<Gene> genes = new();
			foreach (Gene item in pawnGenes)
			{
				if (item.def.endogeneCategory == EndogeneCategory.Melanin)
				{
					continue;
				}
				// if (item.def.minMelanin >= 0f)
				// {
					// continue;
				// }
				// if (item.def.defName.Contains("Skin_Melanin"))
				// {
					// continue;
				// }
				genes.Add(item);
			}
			List<GeneDef> geneDef = new();
			foreach (Gene item in genes)
			{
				if (xenotypeGenes.Contains(item.def))
				{
					geneDef.Add(item.def);
				}
			}
			return geneDef;
		}

		public static GeneDef GetAnyActiveGeneFromList(List<GeneDef> geneDefs, Pawn pawn)
		{
			for (int i = 0; i < geneDefs.Count; i++)
			{
				if (geneDefs[i] != null)
				{
					List<Gene> genesListForReading = pawn.genes.GenesListForReading;
					for (int j = 0; j < genesListForReading.Count; j++)
					{
						if (genesListForReading[j].Active == true && genesListForReading[j].def == geneDefs[i])
						{
							return geneDefs[i];
						}
					}
				}
			}
			return null;
		}

		// Xenotype Cost

		public static float XenotypeCost(XenotypeDef xenotype)
		{
			return (float)((GetXenotype_Arc(xenotype) * 1.2) + (GetXenotype_Cpx(xenotype) * 0.2) + (-1 * (GetXenotype_Met(xenotype) * 0.4)));
		}

		public static int GetXenotype_Cpx(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef?.genes;
			if (genes.NullOrEmpty())
			{
				return 0;
			}
			int num = 0;
			foreach (GeneDef item in genes)
			{
				num += item.biostatCpx;
			}
			return num;
		}

		public static int GetXenotype_Met(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef?.genes;
			if (genes.NullOrEmpty())
			{
				return 0;
			}
			int num = 0;
			foreach (GeneDef item in genes)
			{
				num += item.biostatMet;
			}
			return num;
		}

		public static int GetXenotype_Arc(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef?.genes;
			if (genes.NullOrEmpty())
			{
				return 0;
			}
			int num = 0;
			foreach (GeneDef item in genes)
			{
				num += item.biostatArc;
			}
			return num;
		}

		// XaG test

		public static bool IsXenoGenesGene(this GeneDef geneDef)
		{
			return geneDef?.modContentPack != null && geneDef.modContentPack.PackageId.Contains("wvc.sergkart.races.biotech");
		}

	}
}
