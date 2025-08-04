using System.Collections.Generic;
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
                    cachedGenelineGenes = Giver.geneDefs;
                }
                return cachedGenelineGenes;
            }
        }

        public override bool EnableCooldown => true;
        public override bool DisableSubActions => true;

    }

}
