using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Randomizer : Gene
    {

        public List<GeneDef> RandomizerGenesList => def.GetModExtension<GeneExtension_Giver>().randomizerGenesList;
        // public IntRange IntervalRange => def.GetModExtension<GeneExtension_Giver>().intRange;

        public override void PostAdd()
        {
            base.PostAdd();
            GeneDef geneDef = RandomizerGenesList.RandomElement();
            // List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            // if (pawn.genes.HasGene(item))
            // {
            // }
            RandomGene(geneDef, pawn, this);
            // pawn.genes.RemoveGene(this);
        }

        public static void RandomGene(GeneDef geneDef, Pawn pawn, Gene gene)
        {
            bool geneIsXenogene = true;
            List<Gene> endogenes = pawn.genes.Endogenes;
            // for (int i = 0; i < endogenes.Count; i++)
            // {
            // if (endogenes[i].def.defName == gene.def.defName)
            // {
            // geneIsXenogene = false;
            // }
            // }
            if (endogenes.Contains(gene))
            {
                geneIsXenogene = false;
            }
            // List<GeneDef> list = Enumerable.Take(count: intRange.RandomInRange, source: randomizerGenesList.Where((GeneDef x) => x != gene.def).OrderBy((GeneDef x) => Rand.Value)).ToList();
            // List<GeneDef> list = Enumerable.Take(count: intRange.RandomInRange, source: randomizerGenesList.Where((GeneDef x) => x != gene.def)).ToList();
            // if (item != null)
            // {
            // }
            // if (endCycle)
            // {
            // geneDef.geneClass = typeof(Gene);
            // }
            if (!pawn.genes.HasEndogene(geneDef))
            {
                pawn.genes.AddGene(geneDef, xenogene: geneIsXenogene);
                pawn.genes.RemoveGene(gene);
            }
            // AddGene(pawn, item, xenogene: geneIsXenogene, randomized);
            // Log.Error("GeneDef item: " + item.defName + " " + "Shuffle gene: " + gene.def.defName);
            // foreach (GeneDef item in list)
            // {
            // }
            // List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            // if (pawn.genes.HasGene(item))
            // {
            // }
        }

        // public static Gene AddGene(Pawn pawn, GeneDef geneDef, bool xenogene, bool randomized)
        // {
        // if (xenogene && !ModLister.BiotechInstalled)
        // {
        // return null;
        // }
        // if (!xenogene && pawn.genes.HasEndogene(geneDef))
        // {
        // return null;
        // }
        // return pawn.genes.AddGene(Randomizer_MakeGene(geneDef, pawn, randomized), xenogene);
        // }

        // public static Gene_Randomizer Randomizer_MakeGene(GeneDef def, Pawn pawn, bool randomized)
        // {
        // Gene_Randomizer obj = (Gene_Randomizer)Activator.CreateInstance(def.geneClass);
        // obj.geneIsRandomized = randomized;
        // obj.def = def;
        // obj.pawn = pawn;
        // obj.loadID = Find.UniqueIDsManager.GetNextGeneID();
        // obj.PostMake();
        // return obj;
        // }

    }

}
