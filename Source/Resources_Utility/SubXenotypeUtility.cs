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

		public static void ShapeShift(Pawn pawn, XenotypeDef mainXenotype, Gene shapeShiftGene)
		{
			// List<Gene> pawnGenes = pawn.genes?.GenesListForReading;
			// for (int i = 0; i < pawnGenes.Count; i++)
			// {
				// Log.Error("Pawn contain " + pawnGenes[i].def + " " + (i + 1) + "/" + pawnGenes.Count);
			// }
			if (mainXenotype != null)
			{
				XenotypeExtension_SubXenotype modExtension = mainXenotype.GetModExtension<XenotypeExtension_SubXenotype>();
				if (modExtension != null)
				{
					if (Rand.Chance(modExtension.subXenotypeChance))
					{
						SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
						RandomXenotype(pawn, subXenotypeDef, mainXenotype);
						// pawn.genes.RemoveGene(shapeShiftGene);
					}
					else if (modExtension.mainSubXenotypeDef != null)
					{
						SubXenotypeDef subXenotypeDef = modExtension.mainSubXenotypeDef;
						MainXenotype(pawn, subXenotypeDef, mainXenotype);
					}
				}
			}
			if (shapeShiftGene != null)
			{
				pawn.genes.RemoveGene(shapeShiftGene);
			}
		}

		public static void RandomXenotype(Pawn pawn, SubXenotypeDef xenotype, XenotypeDef mainXenotype)
		{
			if (xenotype.removeGenes != null)
			{
				RemoveGenes(pawn, xenotype);
			}
			if (xenotype.mainGenes != null)
			{
				MainXenotype(pawn, xenotype, mainXenotype);
			}
			// if (xenotype.genes != null)
			// {
				// pawn.genes?.RemoveGene(WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter);
			// }
			Pawn_GeneTracker genes = pawn.genes;
			List<GeneDef> geneDefs = xenotype.genes;
			if (xenotype.overrideExistingGenes)
			{
				if (!xenotype.inheritable)
				{
					for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
					{
						pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
					}
				}
				if (xenotype.inheritable)
				{
					for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
					{
						pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
					}
				}
			}
			if ((xenotype.inheritable && genes.Endogenes.Count <= 0) || (!xenotype.inheritable && genes.Xenogenes.Count <= 0) || xenotype.ignoreExistingGenes)
			{
				for (int i = 0; i < geneDefs.Count; i++)
				{
					pawn.genes?.AddGene(geneDefs[i], !xenotype.inheritable);
				}
			}
			if (xenotype.xenotypeIconDef != null)
			{
				pawn.genes.iconDef = xenotype.xenotypeIconDef;
			}
		}

		public static void MainXenotype(Pawn pawn, SubXenotypeDef xenotype, XenotypeDef mainXenotype)
		{
			List<GeneDef> geneDefs = xenotype.mainGenes;
			for (int i = 0; i < geneDefs.Count; i++)
			{
				pawn.genes?.AddGene(geneDefs[i], !mainXenotype.inheritable);
			}
		}

		public static void RemoveGenes(Pawn pawn, SubXenotypeDef xenotype)
		{
			List<GeneDef> geneDefs = xenotype.removeGenes;
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (geneDefs.Contains(genesListForReading[i].def))
				{
					pawn.genes?.RemoveGene(genesListForReading[i]);
				}
			}
		}

		// ===============================================

		public static void XenotypeShapeshifter(Pawn pawn)
		{
			if (WVC_Biotech.settings.allowShapeshiftAfterDeath)
			{
				Pawn_GeneTracker genes = pawn.genes;
				if (genes != null && !genes.UniqueXenotype && genes.Xenotype != null)
				{
					XenotypeDef xenotype = pawn.genes?.Xenotype;
					XenotypeExtension_SubXenotype modExtension = xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
					if (modExtension != null && modExtension.xenotypeCanShapeshiftOnDeath)
					{
						// GeneDef shapeGene = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
						// if (MechanoidizationUtility.HasActiveGene(shapeGene, pawn))
						// {
							// ShapeShift(pawn, xenotype, pawn.genes?.GetGene(shapeGene));
						// }
						if (TestXenotype(pawn))
						{
							// Clear the xenotype from random genes.
							// This is necessary so that they do not produce duplicates indefinitely.
							RemoveRandomGenes(pawn);
							pawn.genes?.AddGene(WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter, !xenotype.inheritable);
						}
					}
				}
			}
		}

		public static bool GeneIsRandom(GeneDef gene)
		{
			if (gene.geneClass == typeof(Gene_Shuffle) || gene.geneClass == typeof(Gene_Randomizer) || gene.geneClass == typeof(Gene_FacelessShuffle) || gene.geneClass == typeof(Gene_XenotypeShapeshifter))
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
			if (!GeneIsRandom(geneDef) && !geneDef.defName.Contains("Skin_Melanin") && !geneDef.passOnDirectly)
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
