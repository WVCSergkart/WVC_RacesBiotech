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

		public CompXenosculpterPod cycle;

		protected override string Header => cycle.parent.LabelCap;

		public Dialog_BiosculpterPod(CompXenosculpterPod cycle)
		{
			this.cycle = cycle;
			//UpdXenotypHolders();
			selectedXenoHolder = XenotypesInOrder.First();
		}

		//public void UpdXenotypHolders()
		//{
		//	List<GeneDef> genes = cycle.GetGenes();
		//	foreach (XenotypeHolder item in allXenotypes)
		//	{
		//		item.isOverriden = !XaG_GeneUtility.GenesIsMatch(genes, item.genes, 0.4f);
		//	}
		//}

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
