using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Animalfeeder : Gene_ChimeraDependant, IGeneBloodfeeder
    {

        public void Notify_Bloodfeed(Pawn victim)
        {
            if (victim.IsAnimal)
            {
                Chimera?.GetRandomGene();
            }
        }

    }

}
