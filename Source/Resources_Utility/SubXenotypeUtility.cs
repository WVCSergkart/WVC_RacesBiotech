using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI.Group;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public static class SubXenotypeUtility
	{

		public static void XenotypeShapeshifter(Pawn pawn)
		{
			if (WVC_Biotech.settings.allowShapeshiftAfterDeath)
			{
				XenotypeDef xenotype = pawn.genes?.Xenotype;
				if (xenotype != null)
				{
					XenotypeExtension_SubXenotype modExtension = xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
					if (modExtension != null && modExtension.xenotypeCanShapeshiftOnDeath)
					{
						if (TestXenotype(pawn))
						{
							// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
							RemoveRandomGenes(pawn);
							pawn.genes?.AddGene(WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter, !xenotype.inheritable);
						}
					}
				}
			}
		}

		public static bool GeneIsRandom(GeneDef gene)
		{
			if (gene.geneClass == typeof(Gene_Shuffle) || gene.geneClass == typeof(Gene_Randomizer) || gene.geneClass == typeof(Gene_FacelessShuffle))
			{
				return true;
			}
			return false;
		}

		public static void RemoveRandomGenes(Pawn pawn)
		{
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (GeneIsRandom(genesListForReading[i].def))
				{
					pawn.genes?.RemoveGene(genesListForReading[i]);
				}
			}
		}

		public static bool TestXenotype_TestGene(GeneDef geneDef)
		{
			if (!GeneIsRandom(geneDef) && geneDef.geneClass != typeof(Gene_XenotypeShapeshifter) && !geneDef.defName.Contains("Skin_Melanin") && !geneDef.passOnDirectly)
			{
				return true;
			}
			return false;
		}

		// Checks if xenotype is modified.
		public static bool TestXenotype(Pawn pawn)
		{
			// Log.Error("0");
			if (pawn?.genes == null)
			{
				return false;
			}
			// Log.Error("1");
			XenotypeDef pawnXenotype = pawn.genes?.Xenotype;
			if (pawnXenotype == null)
			{
				return false;
			}
			// Log.Error("2");
			XenotypeExtension_SubXenotype modExtension = pawnXenotype.GetModExtension<XenotypeExtension_SubXenotype>();
			if (modExtension == null)
			{
				return false;
			}
			// Log.Error("3");
			List<GeneDef> pawnXenotypeGenes = new();
			foreach (GeneDef geneDef in pawnXenotype.genes)
			{
				if (TestXenotype_TestGene(geneDef))
				{
					pawnXenotypeGenes.Add(geneDef);
				}
			}
			// Log.Error("4");
			List<Gene> pawnGenes = new();
			foreach (Gene gene in pawn.genes?.GenesListForReading)
			{
				if (TestXenotype_TestGene(gene.def))
				{
					pawnGenes.Add(gene);
				}
			}
			// Log.Error("5");
			for (int i = 0; i < pawnGenes.Count; i++)
			{
				if (!pawnXenotypeGenes.Contains(pawnGenes[i].def))
				{
					// Log.Error("Pawn contain " + pawnGenes[i].def);
					return false;
				}
			}
			// Log.Error("6");
			return true;
		}

	}
}
