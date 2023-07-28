using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Shuffle : Gene
    {

        public List<GeneDef> RandomizerGenesList => def.GetModExtension<GeneExtension_Giver>().randomizerGenesList;
        // XenotypeDef endoXenotype = DefDatabase<XenotypeDef>.AllDefs.Where((XenotypeDef randomXenotypeDef) => randomXenotypeDef != pawn.genes.Xenotype && !blackListedXenotypes.Contains(randomXenotypeDef.defName) && randomXenotypeDef.inheritable).RandomElement();

        // public static List<GeneDef> RandomizerGenesList = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName.Contains("WVC_") && randomGeneDef.exclusionTags != null).ToList();

        public override void PostAdd()
        {
            base.PostAdd();
            // ResetChance();
            // if (HaveGenes(RandomizerGenesList, pawn, this))
            // {
            // pawn.genes.RemoveGene(this);
            // return;
            // }
            // if (Overridden)
            // {
            // if (RandomizerGenesList.Contains(overriddenByGene.def))
            // {
            // pawn.genes.RemoveGene(this);
            // return;
            // }
            // }
            if (Rand.Chance(0.2f))
            // if (!GeneIsRandomized)
            {
                // GeneDef geneDef = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName != def.defName && randomGeneDef.exclusionTags == def.exclusionTags).RandomElement();
                // List<GeneDef> RandomizerGenesList = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName.Contains("WVC_") && randomGeneDef.exclusionTags != null && randomGeneDef.exclusionTags == def.exclusionTags).ToList();
                GeneDef geneDef = RandomizerGenesList.Where((GeneDef randomGeneDef) => randomGeneDef != def).RandomElement();
                // GeneDef geneDef = RandomizerGenesList.RandomElement();
                // RandomTaggedGene(def, pawn, this);
                // geneIsRandomized = true;
                Gene_Randomizer.RandomGene(geneDef, pawn, this);
                // pawn.genes.RemoveGene(this);
            }
        }

        // public static void RandomTaggedGene(GeneDef def, Pawn pawn, Gene gene)
        // {
        // bool geneIsXenogene = true;
        // List<Gene> endogenes = pawn.genes.Endogenes;
        // if (endogenes.Contains(gene))
        // {
        // geneIsXenogene = false;
        // }
        // GeneDef geneDef = DefDatabase<GeneDef>.AllDefs.Where((GeneDef randomGeneDef) => randomGeneDef != null && randomGeneDef.geneClass == def.geneClass && randomGeneDef.exclusionTags != null && randomGeneDef.exclusionTags == def.exclusionTags && randomGeneDef != def).RandomElement();
        // if (!pawn.genes.HasEndogene(geneDef))
        // {
        // pawn.genes.AddGene(geneDef, xenogene: geneIsXenogene);
        // pawn.genes.RemoveGene(gene);
        // }
        // }

        // public static bool HaveGenes(List<GeneDef> genesList, Pawn pawn, Gene gene)
        // {
        // for (int i = 0; i < genesList.Count; i++)
        // {
        // if (pawn.genes.HasGene(genesList[i]) && genesList[i] != gene.def)
        // {
        // return true;
        // }
        // }
        // return false;
        // }

    }

    public class Gene_FacelessShuffle : Gene_Faceless
    {

        public List<GeneDef> RandomizerGenesList => def.GetModExtension<GeneExtension_Giver>().randomizerGenesList;
        // public GeneDef RandomizerGene => def.GetModExtension<GeneExtension_Giver>().randomizerGene;
        // public IntRange IntervalRange => def.GetModExtension<GeneExtension_Giver>().intRange;
        // public bool GeneIsRandomized => def.GetModExtension<GeneExtension_Giver>().geneIsRandomized;

        // public List<GeneDef> RandomizerGenesList = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName.Contains("WVC_")).ToList();

        // private IntRange spawnCountRange = new(1, 2);

        // private int chanceRange;

        // private void ResetChance()
        // {
        // chanceRange = spawnCountRange.RandomInRange;
        // }

        public override void PostAdd()
        {
            base.PostAdd();
            // ResetChance();
            // if (Gene_Shuffle.HaveGenes(RandomizerGenesList, pawn, this))
            // {
            // pawn.genes.RemoveGene(this);
            // return;
            // }
            // if (Overridden)
            // {
            // if (RandomizerGenesList.Contains(overriddenByGene.def))
            // {
            // pawn.genes.RemoveGene(this);
            // return;
            // }
            // }
            if (Rand.Chance(0.2f))
            {
                // GeneDef geneDef = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName != def.defName && randomGeneDef.exclusionTags == def.exclusionTags).RandomElement();
                // List<GeneDef> RandomizerGenesList = DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef randomGeneDef) => randomGeneDef.defName.Contains("WVC_") && randomGeneDef.exclusionTags != null && randomGeneDef.exclusionTags == def.exclusionTags).ToList();
                GeneDef geneDef = RandomizerGenesList.Where((GeneDef randomGeneDef) => randomGeneDef != def).RandomElement();
                // GeneDef geneDef = DefDatabase<GeneDef>.AllDefs.Where((GeneDef randomGeneDef) => randomGeneDef != null && randomGeneDef.geneClass == def.geneClass && randomGeneDef.exclusionTags != null && randomGeneDef.exclusionTags == def.exclusionTags && randomGeneDef != def).RandomElement();
                // GeneDef geneDef = Gene_Shuffle.GenesList(def);
                // Gene_Shuffle.RandomTaggedGene(def, pawn, this);
                // GeneDef geneDef = RandomizerGenesList.RandomElement();
                // Gene_Shuffle.GeneRandomizer(RandomizerGene, pawn, this);
                Gene_Randomizer.RandomGene(geneDef, pawn, this);
                // pawn.genes.RemoveGene(this);
            }
            // if (Overridden)
            // {
            // pawn.genes.RemoveGene(this);
            // }
        }

    }

}
