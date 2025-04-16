using RimWorld;
using System;
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
			UpdXenotypHolders();
			selectedXenoHolder = XenotypesInOrder.First();
		}

        public void UpdXenotypHolders()
        {
            List<GeneDef> genes = cycle?.GetGenes();
            foreach (XenotypeHolder item in XenotypesInOrder)
            {
				if (item.genes.NullOrEmpty() || genes.NullOrEmpty())
                {
					item.matchPercent = 0f;
					continue;
				}
				item.matchPercent = (float)Math.Round(CompBiosculpterPod_XenotypeHolderCycle.GetChance(XaG_GeneUtility.GetMatchingGenesList(genes, item.genes).Count, item.genes.Count), 2);
                //item.customEffectsDesc = "WVC_XaG_XenotypeHolderCycleStarted_XenotypeChanceInfo".Translate(item.matchPercent.Value.ToStringPercent());
            }
		}

		public static readonly CachedTexture ChanceTex = new("WVC/UI/XaG_General/ChanceTex_v0");

		public override void DrawBiostats(XenotypeHolder xenotypeHolder, ref float curX, float curY, float margin = 6f)
		{
			float num = GeneCreationDialogBase.GeneSize.y / 3f;
			float num2 = 0f;
			float baseWidthOffset = 38f;
			float num3 = Text.LineHeightOf(GameFont.Small);
			Rect iconRect = new(curX, curY + margin + num2, num3, num3);
			DrawStat(iconRect, XGTex, xenotypeHolder.genes.Count.ToString(), num3);
			Rect rect = new(curX, iconRect.y, baseWidthOffset, num3);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "Genes".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_XenotypeGenesDesc".Translate());
			}
			num2 += num;
			if (xenotypeHolder.matchPercent.HasValue)
			{
				Rect iconRect3 = new(curX, curY + margin + num2, num3, num3);
				DrawStat(iconRect3, ChanceTex, xenotypeHolder.matchPercent.Value.ToStringPercent(), num3 - 6f);
				Rect rect3 = new(curX, iconRect3.y, baseWidthOffset, num3);
				if (Mouse.IsOver(rect3))
				{
					Widgets.DrawHighlight(rect3);
					TooltipHandler.TipRegion(rect3, "WVC_XaG_XenoHolder_GenesMatch".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_DialogBiosculpterPod_ChanceDesc".Translate(xenotypeHolder.matchPercent.Value.ToStringPercent()));
				}
			}
			curX += 34f;
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
