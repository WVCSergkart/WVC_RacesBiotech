using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_ChimeraGeneline : Gene_ChimeraDependant
    {

        public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

        private List<GeneDef> cachedGenelineGenes;
        public virtual List<GeneDef> GenelineGenes
        {
            get
            {
                if (cachedGenelineGenes == null)
                {
                    List<GeneDef> geneDefs = new();
                    if (Giver.geneDefs != null)
                    {
                        foreach (GeneDef geneDef in Giver.geneDefs)
                        {
                            AddGene(geneDefs, geneDef);
                        }
                    }
                    if (Giver.geneCategoryDefs != null)
                    {
                        foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading.Where((gene) => Giver.geneCategoryDefs.Contains(gene.displayCategory)))
                        {
                            AddGene(geneDefs, geneDef);
                        }
                    }
                    cachedGenelineGenes = geneDefs;
                }
                return cachedGenelineGenes;

                static void AddGene(List<GeneDef> geneDefs, GeneDef geneDef)
                {
                    if (!geneDefs.Contains(geneDef))
                    {
                        geneDefs.Add(geneDef);
                    }
                }
            }
        }

        //public override bool EnableCooldown => true;
        //public override bool DisableSubActions => true;
        //public override IntRange? ReqMetRange => new(0, 0);

    }

    public class Gene_Chimera_GeneDatabase : Gene_ChimeraGeneline
    {


        private List<GeneDef> cachedGenelineGenes;
        public override List<GeneDef> GenelineGenes
        {
            get
            {
                if (cachedGenelineGenes == null)
                {
                    cachedGenelineGenes = DefDatabase<GeneDef>.AllDefsListForReading.Where((def) => !def.IsAndroid()).ToList();
                }
                return cachedGenelineGenes;
            }
        }

    }

}
