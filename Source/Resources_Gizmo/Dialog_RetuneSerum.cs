using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_RetuneSerum : Dialog_XenotypeHolderBasic
	{

		public CompUseEffect_XenogermSerum xenotypeForcer;

        protected override string Header => xenotypeForcer.parent.def.LabelCap;

        public Dialog_RetuneSerum(Thing serum)
		{
			xenotypeForcer = serum.TryGetComp<CompUseEffect_XenogermSerum>();
			allXenotypes = ListsUtility.GetWhiteListedXenotypesHolders(true);
			foreach (XenotypeHolder holder in allXenotypes)
            {
				holder.isOverriden = !HasArchites(holder);
			}
			Dialog_XenotypeTree.GetCurrentXenotypeHolder(xenotypeForcer?.xenotypeHolder, ref selectedXenoHolder, allXenotypes);
		}

		protected override void Accept()
		{
			xenotypeForcer.SetupHolder(selectedXenoHolder);
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

		protected override bool CanAccept()
        {
            if (selectedXenoHolder.isOverriden)
            {
                Messages.Message("WVC_XaG_XenotypeHolderCycleStarted_XenotypeIsForbidden".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            return true;
        }

        private bool HasArchites(XenotypeHolder holder)
        {
            if (XaG_GeneUtility.XenotypeHasArchites(holder))
            {
                if (xenotypeForcer.Props.xenotypeType == CompProperties_UseEffect_XenogermSerum.XenotypeType.Archite)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (xenotypeForcer.Props.xenotypeType == CompProperties_UseEffect_XenogermSerum.XenotypeType.Base)
            {
                return true;
            }
            return false;
        }

    }

}
