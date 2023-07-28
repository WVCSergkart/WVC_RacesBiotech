using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Blank : Gene
    {

        public override bool Active
        {
            get
            {
                // if (Overridden)
                // {
                // return false;
                // }
                if (base.Active && pawn?.genes != null)
                {
                    List<Gene> genesListForReading = pawn.genes.Xenogenes;
                    if (genesListForReading.Count > 0)
                    {
                        return false;
                    }
                }
                return base.Active;
            }
        }

    }

}
