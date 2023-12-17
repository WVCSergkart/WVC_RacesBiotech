using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class XaG_GeneUtility
	{

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
			if (xenotypeDef?.genes == null)
			{
				return false;
			}
			List<GeneDef> genesListForReading = xenotypeDef.genes;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].defName.Contains("VREA_SyntheticBody"))
				{
					return true;
				}
			}
			return false;
		}

		public static bool PawnCannotUseSerums(Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
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
			if (pawn?.genes == null)
			{
				return false;
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
			// for (int i = 0; i < geneDefs.Count; i++)
			// {
				// if (geneDefs[i] != null)
				// {
					// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
					// for (int j = 0; j < genesListForReading.Count; j++)
					// {
						// if (genesListForReading[j].Active == true && genesListForReading[j].def == geneDefs[i])
						// {
							// return true;
						// }
					// }
				// }
			// }
			for (int i = 0; i < geneDefs.Count; i++)
			{
				if (HasActiveGene(geneDefs[i], pawn))
				{
					return true;
				}
			}
			return false;
		}

		public static bool HasActiveGene(GeneDef geneDef, Pawn pawn)
		{
			if (geneDef == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true && genesListForReading[i].def == geneDef)
				{
					return true;
				}
			}
			return false;
		}

		// ============================= Getter =============================

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

		public static int GetXenotype_Cpx(XenotypeDef xenotypeDef)
		{
			List<GeneDef> genes = xenotypeDef.genes;
			if (genes.NullOrEmpty())
			{
				return 0;
			}
			int cpx = 0;
			foreach (GeneDef item in xenotypeDef.genes)
			{
				cpx += item.biostatCpx;
			}
			return cpx;
		}

	}
}
