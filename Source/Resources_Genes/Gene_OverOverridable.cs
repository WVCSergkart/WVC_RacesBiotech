using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_OverOverridable : Gene, IGeneOverridden
    {

        private int overrideTries = 0;

        public void Notify_OverriddenBy(Gene overriddenBy)
        {
            if (!WVC_Biotech.settings.enable_OverOverridableGenesMechanic)
            {
                return;
            }
            if (overriddenBy is not Gene_OverOverridable && overrideTries < 100 && overriddenBy.def.ConflictsWith(def))
            {
                this.OverrideBy(null);
                overriddenBy.OverrideBy(this);
            }
            overrideTries++;
        }

        public void Notify_Override()
        {
        }

    }

}
