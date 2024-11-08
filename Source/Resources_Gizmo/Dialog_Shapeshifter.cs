using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Shapeshifter : Dialog_XenotypeHolderBasic
	{

		public Gene_Shapeshifter gene;

		protected override string Header => gene.LabelCap;

		protected override string AcceptButtonLabel
		{
			get
			{
				return "WVC_XaG_ChimeraApply_Implant".Translate().CapitalizeFirst();
			}
		}

		public GeneExtension_Undead shiftExtension;
		//public bool doubleXenotypeReimplantation = true;
		public bool clearXenogenes = false;

		public Dialog_Shapeshifter(Gene_Shapeshifter shapeshifter)
        {
            xenotypeName = string.Empty;
            gene = shapeshifter;
            //forcePause = true;
            //closeOnAccept = false;
            //absorbInputAroundWindow = true;
            //alwaysUseFullBiostatsTableHeight = true;
            //searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
            //allXenotypes = ListsUtility.GetAllXenotypesHolders();
			ListsUtility.UpdTrueFormHoldersFromList(allXenotypes);
			selectedXenoHolder = allXenotypes.First((XenotypeHolder holder) => holder.xenotypeDef == gene.pawn.genes.Xenotype);
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Undead>();
            disabled = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
            OnGenesChanged();
		}

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
			if (xenotypeHolder.isTrueShiftForm)
			{
				Rect iconRect3 = new(curX, curY + margin + num2, num3, num3);
				DrawStat(iconRect3, XTFTex, xenotypeHolder.isTrueShiftForm.ToStringYesNo(), num3 - 3f);
				Rect rect3 = new(curX, iconRect3.y, baseWidthOffset, num3);
				if (Mouse.IsOver(rect3))
				{
					Widgets.DrawHighlight(rect3);
					TooltipHandler.TipRegion(rect3, "WVC_XaG_TrueForm".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_TrueFormDesc".Translate());
				}
			}
			curX += 34f;
		}


		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (disabled)
			{
				string text = "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
				float x2 = Text.CalcSize(text).x;
				GUI.color = ColorLibrary.RedReadable;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x - x2 - 4f, rect.y, x2, rect.height), text);
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
		}

		protected override bool CanAccept()
		{
			if (selectedXenoHolder.isTrueShiftForm)
			{
				return true;
			}
			if (disabled)
			{
				Messages.Message("WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}

		protected override void Accept()
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), StartChange));
		}

		public void StartChange()
		{
			GeneResourceUtility.TryShapeshift(gene, this);
			Close(doCloseSound: false);
		}

		protected override void PostXenotypeOnGUI(float curX, float curY)
		{
			TaggedString taggedString = "WVC_XaG_GeneShapeshifter_CheckBox_ClearXenogenesBeforeImplant".Translate();
			TaggedString taggedString2 = "WVC_XaG_GeneShapeshifter_CheckBox_ImplantDoubleXenotype".Translate();
			float width = Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4f + 24f;
			Rect rect = new(curX, curY, width, Text.LineHeight);
			Widgets.CheckboxLabeled(rect, taggedString, ref clearXenogenes);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "WVC_XaG_GeneShapeshifter_CheckBox_ClearXenogenesBeforeImplant_Desc".Translate());
			}
			rect.y += Text.LineHeight;
            //Widgets.CheckboxLabeled(rect, taggedString2, ref doubleXenotypeReimplantation);
            //if (Mouse.IsOver(rect))
            //{
            //	Widgets.DrawHighlight(rect);
            //	TooltipHandler.TipRegion(rect, "WVC_XaG_GeneShapeshifter_CheckBox_ImplantDoubleXenotype_Desc".Translate());
            //}
            postXenotypeHeight += rect.yMax - curY;
		}

	}

}
