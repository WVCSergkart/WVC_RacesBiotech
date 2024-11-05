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
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((XenotypeHolder x) => 0f - x.displayPriority);
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		public Dialog_BiosculpterPod(CompBiosculpterPod_XenotypeHolderCycle cycle)
		{
			this.cycle = cycle;
			selectedXenoHolder = XenotypesInOrder.First();
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

		private int GetCycleDays()
		{
			return (int)(Dialog_XenotypeGestator.GetXenotype_Cpx(selectedXenoHolder) * 0.5f);
		}

		public void StartChange()
		{
			cycle.SetupHolder(selectedXenoHolder, GetCycleDays());
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}

	}

}
