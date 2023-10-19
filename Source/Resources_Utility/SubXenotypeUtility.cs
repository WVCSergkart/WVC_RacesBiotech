using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class SubXenotypeUtility
    {

        public static void ShapeShift(Pawn pawn, XenotypeDef mainXenotype, Gene shapeShiftGene, float xenogermReplicationChance = 0.2f, bool removeRandomGenes = false)
        {
            // List<Gene> pawnGenes = pawn.genes?.GenesListForReading;
            // for (int i = 0; i < pawnGenes.Count; i++)
            // {
            // Log.Error("Pawn contain " + pawnGenes[i].def + " " + (i + 1) + "/" + pawnGenes.Count);
            // }
            if (!pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
            {
                if (mainXenotype != null)
                {
                    XenotypeExtension_SubXenotype modExtension = mainXenotype.GetModExtension<XenotypeExtension_SubXenotype>();
                    if (modExtension != null)
                    {
                        if (!modExtension.subXenotypeDefs.NullOrEmpty())
                        {
                            if (removeRandomGenes)
                            {
                                RemoveRandomGenes(pawn);
                            }
                            if (Rand.Chance(modExtension.subXenotypeChance))
                            {
                                SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
                                SetSubXenotype(pawn, subXenotypeDef, xenogermReplicationChance);
                                // pawn.genes.RemoveGene(shapeShiftGene);
                            }
                            // else if (modExtension.mainSubXenotypeDef != null)
                            // {
                                // SubXenotypeDef subXenotypeDef = modExtension.mainSubXenotypeDef;
                                // MainXenotype(pawn, subXenotypeDef, mainXenotype);
                            // }
                        }
                    }
                }
            }
            if (shapeShiftGene != null)
            {
                pawn.genes.RemoveGene(shapeShiftGene);
            }
        }

        public static void SetSubXenotype(Pawn pawn, SubXenotypeDef xenotype, float xenogermReplicationChance)
        {
			if (!xenotype.removeGenes.NullOrEmpty())
			{
				RemoveGenes(pawn, xenotype);
			}
			if (!xenotype.endogenes.NullOrEmpty())
			{
				AddGenes(pawn, xenotype.endogenes, true);
			}
			if (!xenotype.xenogenes.NullOrEmpty())
			{
				AddGenes(pawn, xenotype.xenogenes, false);
			}
            // if (xenotype.genes != null)
            // {
            // pawn.genes?.RemoveGene(WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter);
            // }
            // Pawn_GeneTracker genes = pawn.genes;
            // if (xenotype.overrideExistingGenes)
            // {
                // if (!xenotype.inheritable)
                // {
                    // for (int numXenogenes = genes.Xenogenes.Count - 1; numXenogenes >= 0; numXenogenes--)
                    // {
                        // pawn.genes?.RemoveGene(genes.Xenogenes[numXenogenes]);
                    // }
                // }
                // if (xenotype.inheritable)
                // {
                    // for (int numEndogenes = genes.Endogenes.Count - 1; numEndogenes >= 0; numEndogenes--)
                    // {
                        // pawn.genes?.RemoveGene(genes.Endogenes[numEndogenes]);
                    // }
                // }
            // }
            // if ((xenotype.inheritable && genes.Endogenes.Count <= 0) || (!xenotype.inheritable && genes.Xenogenes.Count <= 0) || xenotype.ignoreExistingGenes)
            // {
            // }
            if (!xenotype.randomGenes.NullOrEmpty())
            {
                foreach (RandomGenes item in xenotype.randomGenes)
                {
                    GeneDef randomGene = item.genes.RandomElement();
                    pawn.genes?.AddGene(randomGene, !item.inheritable);
                }
            }
            if (xenotype.xenotypeIconDef != null)
            {
                pawn.genes.iconDef = xenotype.xenotypeIconDef;
            }
            if (Rand.Chance(xenogermReplicationChance))
            {
                GeneUtility.UpdateXenogermReplication(pawn);
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
                // Pawn_GeneTracker genes = pawn.genes;
                // if (genes != null && !genes.UniqueXenotype && genes.Xenotype != null)
                // {
                // XenotypeDef xenotype = pawn.genes?.Xenotype;
                // XenotypeExtension_SubXenotype modExtension = xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
                // if (modExtension != null && modExtension.xenotypeCanShapeshiftOnDeath)
                // {
                // GeneDef shapeGene = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
                // if (MechanoidizationUtility.HasActiveGene(shapeGene, pawn))
                // {
                // ShapeShift(pawn, xenotype, pawn.genes?.GetGene(shapeGene));
                // }
                // }
                // }
                if (TestXenotype(pawn))
                {
                    // Clear the xenotype from random genes.
                    // This is necessary so that they do not produce duplicates indefinitely.
                    // RemoveRandomGenes(pawn);
                    XenotypeDef xenotype = pawn.genes?.Xenotype;
                    ShapeShift(pawn, xenotype, null, 1.0f, true);
                    // pawn.genes?.AddGene(WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter, !xenotype.inheritable);
                }
            }
        }

		// Random genes
		public static bool GeneIsRandom(GeneDef gene)
		{
            // gene.geneClass == typeof(Gene_Randomizer) || || GeneIsShuffle(gene)
            if (gene.geneClass == typeof(Gene_XenotypeShapeshifter))
			{
				return true;
			}
			return false;
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
            if (modExtension == null || !modExtension.xenotypeCanShapeshiftOnDeath)
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

        public static string GetFirstSubXenotypeName(XenotypeIconDef iconDef, XenotypeExtension_SubXenotype modExtension)
        {
            if (iconDef != null && modExtension != null)
            {
                List<SubXenotypeDef> subDefs = modExtension.subXenotypeDefs;
                for (int i = 0; i < subDefs.Count; i++)
                {
                    if (subDefs[i].label != null && subDefs[i].xenotypeIconDef != null && subDefs[i].xenotypeIconDef == iconDef)
                    {
                        return subDefs[i].label;
                    }
                }
            }
            return null;
        }

        public static string GetFirstSubXenotypeDesc(XenotypeIconDef iconDef, XenotypeExtension_SubXenotype modExtension)
        {
            if (iconDef != null && modExtension != null)
            {
                List<SubXenotypeDef> subDefs = modExtension.subXenotypeDefs;
                for (int i = 0; i < subDefs.Count; i++)
                {
                    if (subDefs[i].description != null && subDefs[i].xenotypeIconDef != null && subDefs[i].xenotypeIconDef == iconDef)
                    {
                        return subDefs[i].description;
                    }
                }
            }
            return null;
        }

        public static bool XenotypeIsSubXenotype(Pawn_GeneTracker geneTracker)
        {
            if (geneTracker.hybrid || geneTracker.CustomXenotype != null)
            {
                return false;
            }
            if (!geneTracker.UniqueXenotype && geneTracker.Xenotype != null && geneTracker.iconDef != null)
            {
                XenotypeExtension_SubXenotype modExtension = geneTracker.Xenotype?.GetModExtension<XenotypeExtension_SubXenotype>();
                if (modExtension != null)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
