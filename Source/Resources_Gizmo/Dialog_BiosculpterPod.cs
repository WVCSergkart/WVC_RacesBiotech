using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_BiosculpterPod : Dialog_XenotypeHolderBasic
	{

		public CompBiosculpterPod_XenotypeHolderCycle cycle;

		public List<GeneDef> genes;

		protected override string Header => cycle.parent.LabelCap;

		public override List<XenotypeHolder> XenotypesInOrder
		{
			get
			{
				if (cachedXenotypeDefsInOrder == null)
				{
					cachedXenotypeDefsInOrder = new();
					foreach (XenotypeHolder allDef in allXenotypes)
					{
						if (allDef.shouldSkip)
                        {
							continue;
                        }
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((XenotypeHolder x) => 0f - x.displayPriority);
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		public Dialog_BiosculpterPod(CompBiosculpterPod_XenotypeHolderCycle cycle, List<GeneDef> genes)
		{
			this.cycle = cycle;
			this.genes = genes;
			UpdXenotypHolders();
			if (XenotypesInOrder.NullOrEmpty())
			{
				Messages.Message("WVC_XaG_XenotypeHolderCycleStarted_NonXenotypes".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				Close();
				return;
			}
			selectedXenoHolder = XenotypesInOrder.First();
		}

		public void UpdXenotypHolders()
        {
            //List<XenotypeDef> whiteList = ListsUtility.GetWhiteListedXenotypes(true);
            //foreach (XenotypeHolder allDef in allXenotypes)
            //{
            //    if (!allDef.CustomXenotype && !whiteList.Contains(allDef.xenotypeDef))
            //    {
            //        allDef.shouldSkip = true;
            //    }
            //}
            foreach (XenotypeHolder item in allXenotypes)
            {
                item.isOverriden = !XaG_GeneUtility.GenesIsMatch(genes, item.genes, 0.4f);
            }
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

		protected override void Accept()
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_XenotypeHolderCycleStarted_Dialog".Translate(cycle.parent.LabelCap), StartChange));
		}

		//private int GetCycleDays()
		//{
		//	return (int)(Dialog_XenotypeGestator.GetXenotype_Cpx(selectedXenoHolder) * 0.5f);
		//}

		public void StartChange()
		{
			cycle.SetupHolder(selectedXenoHolder);
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

	}

}
