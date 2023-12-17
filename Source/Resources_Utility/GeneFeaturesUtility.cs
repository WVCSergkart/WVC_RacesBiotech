using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class GeneFeaturesUtility
	{

		// ============================= Checker Gene Features =============================

		public static bool PawnIsExoskinned(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<GeneDef> whiteListedGenes = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				whiteListedGenes.AddRange(item.whiteListedExoskinGenes);
			}
			for (int i = 0; i < whiteListedGenes.Count; i++)
			{
				if (XaG_GeneUtility.HasActiveGene(whiteListedGenes[i], pawn))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNotAcceptablePrey(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (!modExtension.canBePredatorPrey)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool PawnSkillsNotDecay(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (modExtension.noSkillDecay)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool IsAngelBeauty(Pawn pawn)
		{
			if (pawn?.genes == null)
			{
				return false;
			}
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (genesListForReading[i].Active == true)
				{
					GeneExtension_General modExtension = genesListForReading[i].def.GetModExtension<GeneExtension_General>();
					if (modExtension != null)
					{
						if (modExtension.geneIsAngelBeauty)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool EyesShouldBeInvisble(Pawn pawn)
		{
			if (pawn?.genes == null || pawn?.story == null || pawn?.story?.headType == null)
			{
				return false;
			}
			if (pawn.story.headType.defName.Contains("WVC_Faceless"))
			{
				return true;
			}
			return false;
		}


	}
}
