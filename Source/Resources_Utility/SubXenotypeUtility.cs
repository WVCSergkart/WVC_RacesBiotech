using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class SubXenotypeUtility
	{

		// ========================================================

		public static void XenotypeShapeShift(Pawn pawn, Gene shapeShiftGene, float xenogermReplicationChance = 0.2f)
		{
			if (TryGetXenotypeDef(pawn, out XenotypeDef subXenotypeDef))
			{
				if (subXenotypeDef is SubXenotypeDef sub && !sub.doubleXenotypeChances.NullOrEmpty())
				{
					SetXenotypeGenes(pawn, sub, xenogermReplicationChance);
				}
				if (subXenotypeDef is ThralltypeDef thrall)
				{
					SetThrallGenes(pawn, thrall);
				}
			}
			// else
			// {
				// ReimplanterUtility.UnknownXenotype(pawn);
			// }
			if (shapeShiftGene != null)
			{
				pawn.genes.RemoveGene(shapeShiftGene);
			}
		}

		public static void SetThrallGenes(Pawn pawn, ThralltypeDef xenotype)
		{
			if (xenotype.thrallDefs.Where((ThrallDef x) => x.mutantDef == null).TryRandomElementByWeight((ThrallDef x) => x.selectionWeight, out var result))
			{
				CompAbilityEffect_ReimplanterThrallMaker.ThrallMaker(pawn, result);
			}
			AddGenes(pawn, xenotype.guaranteedGenes, true, new());
		}

		public static void SetXenotypeGenes(Pawn pawn, SubXenotypeDef xenotype, float xenogermReplicationChance)
		{
			// RemoveRandomGenes(pawn);
			// Log.Error(pawn.Name + " genes remove.");
			if (!xenotype.removeGenes.NullOrEmpty())
			{
				RemoveGenes(pawn, xenotype);
			}
			List<GeneDef> endogenes = GetEndogenesFromXenotypes(xenotype);
			// Log.Error(pawn.Name + " genes add.");
			if (!endogenes.NullOrEmpty())
			{
				AddGenes(pawn, endogenes, true, xenotype.removeGenes);
			}
			// if (!xenotype.xenogenes.NullOrEmpty())
			// {
				// AddGenes(pawn, xenotype.xenogenes, false, xenotype.removeGenes);
			// }
			RemoveAllUnnecessaryGenes(pawn, endogenes, xenotype.genes);
			if (Rand.Chance(xenogermReplicationChance))
			{
				GeneUtility.UpdateXenogermReplication(pawn);
			}
		}

		// ========================================================

		public static void RemoveAllUnnecessaryGenes(Pawn pawn, List<GeneDef> endogenes, List<GeneDef> xenogenes)
		{
			List<GeneDef> xenoTypeGenes = new();
			xenoTypeGenes.AddRange(endogenes);
			xenoTypeGenes.AddRange(xenogenes);
			List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (!xenoTypeGenes.Contains(genesListForReading[i].def))
				{
					pawn.genes?.RemoveGene(genesListForReading[i]);
				}
			}
		}

		public static List<GeneDef> GetEndogenesFromXenotypes(SubXenotypeDef xenotype)
		{
			List<GeneDef> list = new();
			XenotypeDef xenotypeDef = xenotype.doubleXenotypeChances.RandomElementByWeight((XenotypeChance x) => x.chance).xenotype;
			foreach (GeneDef item in xenotypeDef.genes)
			{
				if (!xenotype.removeGenes.Contains(item))
				{
					list.Add(item);
				}
			}
			foreach (GeneDef item in xenotype.endogenes)
			{
				if (!xenotype.removeGenes.Contains(item) && !list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static void AddGenes(Pawn pawn, List<GeneDef> genes, bool inheritable, List<GeneDef> removeGenes)
		{
			List<GeneDef> genesListForReading = XaG_GeneUtility.ConvertGenesInGeneDefs(pawn.genes.GenesListForReading);
			for (int i = 0; i < genes.Count; i++)
			{
				if (!removeGenes.Contains(genes[i]) && !genesListForReading.Contains(genes[i]))
				{
					pawn.genes?.AddGene(genes[i], !inheritable);
				}
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

		// ========================================================

		[Obsolete]
		public static void XenotypeShapeshifter(Pawn pawn)
		{
			//if (WVC_Biotech.settings.allowShapeshiftAfterDeath)
			//{
			//}
			if (PawnXenotypeIsNotCustomXenotype(pawn))
			{
				XenotypeDef xenotype = pawn.genes?.Xenotype;
				ShapeShiftOnDeath(pawn, xenotype);
			}
		}

		[Obsolete]
		public static void ShapeShiftOnDeath(Pawn pawn, XenotypeDef mainXenotype)
		{
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				return;
			}
			if (mainXenotype == null || mainXenotype is not EvotypeDef evotypeDef)
			{
				return;
			}
			if (evotypeDef.subXenotypeDefs.NullOrEmpty() || !evotypeDef.xenotypeCanShapeshiftOnDeath)
			{
				return;
			}
			// if (removeRandomGenes)
			// {
				// RemoveRandomGenes(pawn);
			// }
			if (Rand.Chance(evotypeDef.shapeshiftChance))
			{
				XenotypeDef xenotypeDef = evotypeDef.subXenotypeDefs.RandomElementByWeight((XenotypeDef x) => x is SubXenotypeDef subXenotypeDef ? subXenotypeDef.selectionWeight : 1f);
				ReimplanterUtility.SaveReimplantXenogenesFromXenotype(pawn, xenotypeDef);
			}
		}

		// public static bool ShapeShift_Evolve(Pawn pawn, XenotypeDef mainXenotype)
		// {
			// if (mainXenotype == null || mainXenotype is not EvotypeDef evotypeDef)
			// {
				// return false;
			// }
			// if (evotypeDef.subXenotypeDefs.NullOrEmpty() || !evotypeDef.xenotypeCanEvolveOvertime)
			// {
				// return false;
			// }
			// if (Rand.Chance(evotypeDef.shapeshiftChance))
			// {
				// XenotypeDef xenotypeDef = evotypeDef.subXenotypeDefs.RandomElementByWeight((XenotypeDef x) => x is SubXenotypeDef subXenotypeDef ? subXenotypeDef.selectionWeight : 0.0001f);
				// ReimplanterUtility.SaveReimplantXenogenesFromXenotype(pawn, xenotypeDef);
				// return true;
			// }
			// return false;
		// }

		// ===============================================

		// public static bool XenotypeIsSubXenotype(Pawn pawn, out SubXenotypeDef subXenotypeDef)
		// {
			// subXenotypeDef = null;
			// Pawn_GeneTracker genes = pawn?.genes;
			// if (genes == null || genes.CustomXenotype != null || genes.UniqueXenotype || genes.iconDef != null)
			// {
				// return false;
			// }
			// XenotypeDef pawnXenotype = genes.Xenotype;
			// if (pawnXenotype == null)
			// {
				// return false;
			// }
			// if (pawnXenotype is SubXenotypeDef sub && !sub.doubleXenotypeChances.NullOrEmpty())
			// {
				// subXenotypeDef = sub;
				// return true;
			// }
			// return false;
		// }

		public static bool TryGetXenotypeDef(Pawn pawn, out XenotypeDef subXenotypeDef)
		{
			subXenotypeDef = null;
			Pawn_GeneTracker genes = pawn?.genes;
			// Skip if xenotype is custom
			if (genes == null || genes.CustomXenotype != null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			return (subXenotypeDef = genes.Xenotype) != null;
		}

		// public static bool PawnCanEvolve(Pawn pawn)
		// {
		// Pawn_GeneTracker genes = pawn?.genes;
		// if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
		// {
		// return false;
		// }
		// XenotypeDef pawnXenotype = genes.Xenotype;
		// if (pawnXenotype == null || pawnXenotype is not EvotypeDef evotypeDef)
		// {
		// return false;
		// }
		// if (evotypeDef.xenotypeCanEvolveOvertime)
		// {
		// return true;
		// }
		// return false;
		// }

		[Obsolete]
		public static bool PawnXenotypeIsEvotype(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			if (pawnXenotype == null || pawnXenotype is not EvotypeDef)
			{
				return false;
			}
			return true;
		}

		// Remove genes
		[Obsolete]
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

		// Random genes
		// public static bool GeneIsRandom(GeneDef gene)
		// {
		// if (gene.geneClass == typeof(Gene_XenotypeShapeshifter))
		// {
		// return true;
		// }
		// return false;
		// }
		[Obsolete]
		public static bool GeneIsRandom(GeneDef gene)
		{
			List<Type> geneClasses = GetAllShapeShiftGeneClasses();
			if (geneClasses.Contains(gene.geneClass))
			{
				return true;
			}
			return false;
		}

		[Obsolete]
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

		[Obsolete]
		public static bool TestXenotype_TestGene(GeneDef geneDef)
		{
			if (!GeneIsRandom(geneDef) && !geneDef.defName.Contains("Skin_Melanin"))
			{
				return true;
			}
			return false;
		}

		[Obsolete]
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
			List<GeneDef> pawnGenes = new();
			foreach (Gene gene in pawn.genes?.Endogenes)
			{
				if (TestXenotype_TestGene(gene.def))
				{
					// Log.Error("Pawn contain " + gene.def);
					pawnGenes.Add(gene.def);
				}
			}
			// for (int i = 0; i < pawnGenes.Count; i++)
			// {
				// if (!xenotypeGenes.Contains(pawnGenes[i].def))
				// {
					// return false;
				// }
			// }
			for (int i = 0; i < xenotypeGenes.Count; i++)
			{
				if (pawnGenes.Contains(xenotypeGenes[i]))
				{
					continue;
				}
				return false;
			}
			return true;
		}

		[Obsolete]
		public static bool CheckIfXenotypeIsCorrect(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			// Skip if xenotype is custom
			if (genes == null || genes.CustomXenotype != null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			// Skip if xenotype is not double
			if (pawnXenotype != null && pawnXenotype is SubXenotypeDef subXenotypeDef && !subXenotypeDef.doubleXenotypeChances.NullOrEmpty())
			{
				foreach (XenotypeChance xenotypeChance in subXenotypeDef.doubleXenotypeChances)
				{
					if (CheckIfXenotypeIsCorrect_CheckXenoChances(xenotypeChance, pawn))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Checks if xenotype is modified.
		[Obsolete]
		public static bool PawnXenotypeIsNotCustomXenotype(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			if (pawnXenotype == null || pawnXenotype is not EvotypeDef)
			{
				return false;
			}
			if (XaG_GeneUtility.GenesIsMatch(genes.GenesListForReading, pawnXenotype.genes, 1.0f))
			{
				// Log.Error("match");
				return true;
			}
			// Log.Error("not match");
			return false;
		}

		// public static bool GenesIsMatch(List<Gene> pawnGenes, List<GeneDef> xenotypeGenes)
		// {
			// if (pawnGenes.NullOrEmpty() || xenotypeGenes.NullOrEmpty())
			// {
				// return false;
			// }
			// List<GeneDef> matchingGenes = XaG_GeneUtility.GetMatchingGenesList(pawnGenes, xenotypeGenes);
			// if (matchingGenes.Count == xenotypeGenes.Count)
			// {
				// return true;
			// }
			// return false;
		// }

		[Obsolete]
		public static bool TestXenotype(Pawn pawn)
		{
			Pawn_GeneTracker genes = pawn?.genes;
			// Skip if xenotype is UniqueXenotype
			if (genes == null || genes.UniqueXenotype || genes.iconDef != null)
			{
				return false;
			}
			XenotypeDef pawnXenotype = genes.Xenotype;
			if (pawnXenotype == null || pawnXenotype is not EvotypeDef)
			{
				return false;
			}
			List<GeneDef> pawnXenotypeGenes = new();
			foreach (GeneDef geneDef in pawnXenotype.genes)
			{
				if (TestXenotype_TestGene(geneDef))
				{
					pawnXenotypeGenes.Add(geneDef);
				}
			}
			List<Gene> pawnGenes = new();
			foreach (Gene gene in pawn.genes?.GenesListForReading)
			{
				if (TestXenotype_TestGene(gene.def))
				{
					pawnGenes.Add(gene);
				}
			}
			for (int i = 0; i < pawnGenes.Count; i++)
			{
				if (!pawnXenotypeGenes.Contains(pawnGenes[i].def))
				{
					return false;
				}
			}
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
