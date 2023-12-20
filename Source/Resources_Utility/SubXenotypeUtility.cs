using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class SubXenotypeUtility
	{

		public static void XenotypeShapeShift(Pawn pawn, XenotypeDef mainXenotype, Gene shapeShiftGene, float xenogermReplicationChance = 0.2f)
		{
			if (mainXenotype != null && CheckIfXenotypeIsCorrect(pawn))
			{
				// Log.Error(pawn.Name + " xenotype not null.");
				XenotypeExtension_XenotypeShapeShift modExtension = mainXenotype.GetModExtension<XenotypeExtension_XenotypeShapeShift>();
				if (modExtension != null)
				{
					// Log.Error(pawn.Name + " xenotype is correct.");
					if (modExtension.subXenotypeDef != null)
					{
						// Log.Error(pawn.Name + " sub-xenotype is chosen.");
						SetXenotypeGenes(pawn, modExtension.subXenotypeDef, xenogermReplicationChance);
					}
				}
			}
			if (shapeShiftGene != null)
			{
				pawn.genes.RemoveGene(shapeShiftGene);
			}
		}

		public static void SetXenotypeGenes(Pawn pawn, SubXenotypeDef xenotype, float xenogermReplicationChance)
		{
			// Log.Error(pawn.Name + " genes remove.");
			if (!xenotype.removeGenes.NullOrEmpty())
			{
				RemoveGenes(pawn, xenotype);
			}
			// Log.Error(pawn.Name + " genes add.");
			if (!xenotype.endogenes.NullOrEmpty())
			{
				AddGenes(pawn, xenotype.endogenes, true);
			}
			// if (!xenotype.xenogenes.NullOrEmpty())
			// {
				// AddGenes(pawn, xenotype.xenogenes, false);
			// }
			if (Rand.Chance(xenogermReplicationChance))
			{
				GeneUtility.UpdateXenogermReplication(pawn);
			}
		}

		// ========================================================

		public static void ShapeShift(Pawn pawn, XenotypeDef mainXenotype, bool removeRandomGenes = false)
		{
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				if (mainXenotype != null)
				{
					XenotypeExtension_SubXenotype modExtension = mainXenotype.GetModExtension<XenotypeExtension_SubXenotype>();
					if (modExtension != null)
					{
						if (!modExtension.xenotypeDefs.NullOrEmpty() && modExtension.xenotypeCanShapeshiftOnDeath)
						{
							if (removeRandomGenes)
							{
								RemoveRandomGenes(pawn);
							}
							if (Rand.Chance(modExtension.shapeshiftChance))
							{
								XenotypeDef xenotypeDef = modExtension.xenotypeDefs.RandomElementByWeight((XenotypeDef x) => x.GetModExtension<XenotypeExtension_XenotypeShapeShift>().subXenotypeDef.selectionWeight);
								ReimplanterUtility.SaveReimplantXenogenesFromXenotype(pawn, xenotypeDef);
							}
						}
					}
				}
			}
		}

		public static void AddGenes(Pawn pawn, List<GeneDef> genes, bool inheritable)
		{
			// List<GeneDef> geneDefs = genes;
			for (int i = 0; i < genes.Count; i++)
			{
				pawn.genes?.AddGene(genes[i], !inheritable);
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
				if (TestXenotype(pawn))
				{
					XenotypeDef xenotype = pawn.genes?.Xenotype;
					ShapeShift(pawn, xenotype, true);
				}
			}
		}

		// Random genes
		// public static bool GeneIsRandom(GeneDef gene)
		// {
			// if (gene.geneClass == typeof(Gene_XenotypeShapeshifter))
			// {
				// return true;
			// }
			// return false;
		// }
		public static bool GeneIsRandom(GeneDef gene)
		{
			List<Type> geneClasses = GetAllShapeShiftGeneClasses();
			if (geneClasses.Contains(gene.geneClass))
			{
				return true;
			}
			return false;
		}

		public static List<Type> GetAllShapeShiftGeneClasses()
		{
			List<Type> list = new();
			foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			{
				list.AddRange(item.shapeShift_IgnoredGeneClasses);
			}
			return list;
		}

		//public static bool GeneIsShuffle(GeneDef gene)
		//{
		//	if (gene.geneClass == typeof(Gene_Shuffle) || gene.geneClass == typeof(Gene_FacelessShuffle))
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		// Remove genes
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
			if (!GeneIsRandom(geneDef) && !geneDef.defName.Contains("Skin_Melanin"))
			{
				return true;
			}
			return false;
		}

		public static bool CheckIfXenotypeIsCorrect_CheckXenoChances(XenotypeChance xenotypeChance, Pawn pawn)
		{
			// Compare the genes of the original xenotype and the current one to make sure that it can be changed without errors.
			List<GeneDef> xenotypeGenes = new();
			foreach (GeneDef geneDef in xenotypeChance.xenotype.genes)
			{
				// The gene is skipped if it is random or self-deleting.
				// This can be unreliable in some cases, but specifically for Undead it works as it should.
				if (TestXenotype_TestGene(geneDef))
				{
					// Log.Error("Xenotype contain " + geneDef);
					xenotypeGenes.Add(geneDef);
				}
			}
			List<Gene> pawnGenes = new();
			foreach (Gene gene in pawn.genes?.Endogenes)
			{
				if (TestXenotype_TestGene(gene.def))
				{
					// Log.Error("Pawn contain " + gene.def);
					pawnGenes.Add(gene);
				}
			}
			for (int i = 0; i < pawnGenes.Count; i++)
			{
				// Log.Error("Checked gene " + pawnGenes[i].def + " " + (i + 1) + "/" + pawnGenes.Count);
				if (!xenotypeGenes.Contains(pawnGenes[i].def))
				{
					// Log.Error("Pawn contain " + pawnGenes[i].def);
					return false;
				}
			}
			return true;
		}

		// Check if xenotype is correct
		public static bool CheckIfXenotypeIsCorrect(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			// Skip if xenotype is UniqueXenotype
			if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			if (pawnXenotype == null)
			{
				return false;
			}
			bool xenotypeIsCorrect = false;
			foreach (XenotypeChance xenotypeChance in pawnXenotype.doubleXenotypeChances)
			{
				if (CheckIfXenotypeIsCorrect_CheckXenoChances(xenotypeChance, pawn))
				{
					xenotypeIsCorrect = true;
				}
			}
			if (xenotypeIsCorrect)
			{
				return true;
			}
			// Log.Error("6");
			return false;
		}

		// Checks if xenotype is modified.
		public static bool TestXenotype(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			// Skip if xenotype is UniqueXenotype
			if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			if (pawnXenotype == null)
			{
				return false;
			}
			// Check that the xenotype can be shapeshifted.
			XenotypeExtension_SubXenotype modExtension = pawnXenotype.GetModExtension<XenotypeExtension_SubXenotype>();
			if (modExtension == null && modExtension.xenotypeCanShapeshiftOnDeath)
			{
				return false;
			}
			// Compare the genes of the original xenotype and the current one to make sure that it can be changed without errors.
			List<GeneDef> pawnXenotypeGenes = new();
			foreach (GeneDef geneDef in pawnXenotype.genes)
			{
				// The gene is skipped if it is random or self-deleting.
				// This can be unreliable in some cases, but specifically for Undead it works as it should.
				if (TestXenotype_TestGene(geneDef))
				{
					// Log.Error("Xenotype contain " + geneDef);
					pawnXenotypeGenes.Add(geneDef);
				}
			}
			List<Gene> pawnGenes = new();
			foreach (Gene gene in pawn.genes?.GenesListForReading)
			{
				if (TestXenotype_TestGene(gene.def))
				{
					// Log.Error("Pawn contain " + gene.def);
					pawnGenes.Add(gene);
				}
			}
			for (int i = 0; i < pawnGenes.Count; i++)
			{
				// Log.Error("Checked gene " + pawnGenes[i].def + " " + (i + 1) + "/" + pawnGenes.Count);
				if (!pawnXenotypeGenes.Contains(pawnGenes[i].def))
				{
					// Log.Error("Pawn contain " + pawnGenes[i].def);
					return false;
				}
			}
			// Log.Error("6");
			return true;
		}

		// ===============================================

		// public static string GetFirstSubXenotypeName(XenotypeIconDef iconDef, XenotypeExtension_SubXenotype modExtension)
		// {
			// if (iconDef != null && modExtension != null)
			// {
				// List<SubXenotypeDef> subDefs = modExtension.subXenotypeDefs;
				// for (int i = 0; i < subDefs.Count; i++)
				// {
					// if (subDefs[i].label != null && subDefs[i].xenotypeIconDef != null && subDefs[i].xenotypeIconDef == iconDef)
					// {
						// return subDefs[i].label;
					// }
				// }
			// }
			// return null;
		// }

		// public static string GetFirstSubXenotypeDesc(XenotypeIconDef iconDef, XenotypeExtension_SubXenotype modExtension)
		// {
			// if (iconDef != null && modExtension != null)
			// {
				// List<SubXenotypeDef> subDefs = modExtension.subXenotypeDefs;
				// for (int i = 0; i < subDefs.Count; i++)
				// {
					// if (subDefs[i].description != null && subDefs[i].xenotypeIconDef != null && subDefs[i].xenotypeIconDef == iconDef)
					// {
						// return subDefs[i].description;
					// }
				// }
			// }
			// return null;
		// }

		// public static bool XenotypeIsSubXenotype(Pawn_GeneTracker geneTracker)
		// {
			// if (geneTracker.hybrid || geneTracker.CustomXenotype != null)
			// {
				// return false;
			// }
			// if (!geneTracker.UniqueXenotype && geneTracker.Xenotype != null && geneTracker.iconDef != null)
			// {
				// XenotypeExtension_SubXenotype modExtension = geneTracker.Xenotype?.GetModExtension<XenotypeExtension_SubXenotype>();
				// if (modExtension != null)
				// {
					// return true;
				// }
			// }
			// return false;
		// }

	}
}
