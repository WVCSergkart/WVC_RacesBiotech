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

  //      public string RemoteActionName
  //      {
  //          get
  //          {
  //              if (isActive)
  //              {
  //                  return "WVC_XaG_Gene_DustMechlink_On".Translate();
  //              }
  //              return "WVC_XaG_Gene_DustMechlink_Off".Translate();
  //          }
  //      }

  //      public override bool Active
		//{
		//	get
		//	{
		//		if (!isActive)
		//		{
		//			return false;
		//		}
		//		return base.Active;
		//	}
		//}

		//private bool isActive = true;

		//public void RemoteСontrol()
		//{
		//	isActive = !isActive;
		//	XaG_GeneUtility.Notify_GenesChanged(pawn);
		//}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref isActive, "isActive", defaultValue: true);
		//}

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
