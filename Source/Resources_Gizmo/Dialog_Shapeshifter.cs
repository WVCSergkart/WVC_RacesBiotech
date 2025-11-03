using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Shapeshifter : Dialog_XenotypeHolderBasic
	{

		public Gene_Shapeshifter gene;
		public float minGenesMatch;

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
			minGenesMatch = WVC_Biotech.settings.shapeshifer_BaseGenesMatch;
			SetupAvailableHolders(allXenotypes);
			selectedXenoHolder = allXenotypes.First((XenotypeHolder holder) => holder.xenotypeDef == gene.pawn.genes.Xenotype);
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Undead>();
			disabled = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
			//GetMatchForAllXenos();
			OnGenesChanged();
		}

		private void SetupAvailableHolders(List<XenotypeHolder> xenotypes)
		{
			foreach (XenotypeHolder item in xenotypes)
			{
				foreach (GeneDef geneDef in item.genes)
				{
					if (geneDef.IsGeneDefOfType<Gene_Shapeshift_TrueForm>())
					{
						item.isTrueShiftForm = true;
						item.matchPercent = 1f;
						break;
					}
				}
				if (!item.isTrueShiftForm)
				{
					item.isOverriden = !Dialog_XenotypeGestator.GenesIsMatch(gene.pawn?.genes?.GenesListForReading, item.genes, minGenesMatch, out float matchPercent, true, gene.GeneticMaterial * 0.01f);
					item.matchPercent = matchPercent;
				}
			}
			UpdGenesMatchUpToMinOneXenotype(xenotypes);
		}

		private void UpdGenesMatchUpToMinOneXenotype(List<XenotypeHolder> xenotypes)
		{
			if (!disabled && xenotypes.Where((holder) => !holder.isOverriden && !holder.shouldSkip && !holder.isTrueShiftForm).ToList().Count <= 1)
			{
				minGenesMatch -= 0.05f;
				if (DebugSettings.ShowDevGizmos)
				{
					Log.Warning("Required genes match decreased. New required match: " + minGenesMatch);
				}
				UpdHoldersOverride(xenotypes);
			}
		}

		private void UpdHoldersOverride(List<XenotypeHolder> xenotypes)
		{
			foreach (XenotypeHolder item in xenotypes)
			{
				if (item.isOverriden && item.matchPercent >= minGenesMatch)
				{
					item.isOverriden = false;
				}
			}
			UpdGenesMatchUpToMinOneXenotype(xenotypes);
		}

		// public override void DrawBiostats(XenotypeHolder xenotypeHolder, ref float curX, float curY, float margin = 6f)
		//{
		//	float num = GeneCreationDialogBase.GeneSize.y / 3f;
		//	float num2 = 0f;
		//	float baseWidthOffset = 38f;
		//	float num3 = Text.LineHeightOf(GameFont.Small);
		//	Rect iconRect = new(curX, curY + margin + num2, num3, num3);
		//	DrawStat(iconRect, XGTex, xenotypeHolder.genes.Count.ToString(), num3);
		//	Rect rect = new(curX, iconRect.y, baseWidthOffset, num3);
		//	if (Mouse.IsOver(rect))
		//	{
		//		Widgets.DrawHighlight(rect);
		//		TooltipHandler.TipRegion(rect, "Genes".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_XenotypeGenesDesc".Translate());
		//	}
		//	num2 += num;
		//	if (xenotypeHolder.isTrueShiftForm)
		//	{
		//		Rect iconRect3 = new(curX, curY + margin + num2, num3, num3);
		//		DrawStat(iconRect3, XTFTex, xenotypeHolder.isTrueShiftForm.ToStringYesNo(), num3 - 3f);
		//		Rect rect3 = new(curX, iconRect3.y, baseWidthOffset, num3);
		//		if (Mouse.IsOver(rect3))
		//		{
		//			Widgets.DrawHighlight(rect3);
		//			TooltipHandler.TipRegion(rect3, "WVC_XaG_TrueForm".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_TrueFormDesc".Translate());
		//		}
		//	}
		//	curX += 34f;
		//}


		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (disabled)
			{
				DisabledText(rect, "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate());
			}
			else if (selectedXenoHolder.isOverriden)
			{
				DisabledText(rect, "WVC_XaG_GeneShapeshifter_MinMatchPercent".Translate(minGenesMatch * 100));
			}
			else
			{
				Rect storeButton = new(rect.xMax - (ButSize.x * 2), rect.y, ButSize.x, ButSize.y);
				if (Widgets.ButtonText(storeButton, "WVC_XaG_StorageImplanter_Apply".Translate()))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_StorageImplanter_Warning".Translate(gene.pawn.LabelCap), StorageImplanterSet));
				}
				//Rect clearXenogenes = new(storeButton.xMax - ButSize.x, storeButton.y, storeButton.width, storeButton.height);
				//if (Widgets.ButtonText(clearXenogenes, "WVC_Xenogerm".Translate()))
				//{
				//	Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_ClearGenesWarning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), StorageImplanterSet));
				//}
				//Rect clearEndogenes = new(clearXenogenes.xMax - ButSize.x, clearXenogenes.y, clearXenogenes.width, clearXenogenes.height);
				//if (Widgets.ButtonText(clearEndogenes, "WVC_Germline".Translate()))
				//{
				//	Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_ClearGenesWarning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), StorageImplanterSet));
				//}
			}
		}

		private static void DisabledText(Rect rect, string text)
		{
			//string text = "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			float x2 = Text.CalcSize(text).x;
			GUI.color = ColorLibrary.RedReadable;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x - x2 - 4f, rect.y, x2, rect.height), text);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}

		private void StorageImplanterSet()
		{
			if (Gene_StorageImplanter.CanStoreGenes(gene.pawn, out Gene_StorageImplanter implanter))
			{
				implanter.SetupHolder(selectedXenoHolder);
				Close();
			}
		}

		protected override bool CanAccept()
		{
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.pawn))
			{
				return false;
			}
			if (selectedXenoHolder.isTrueShiftForm)
			{
				return true;
			}
			if (selectedXenoHolder.isOverriden)
			{
				Messages.Message("WVC_XaG_GeneShapeshifter_MinMatchPercent".Translate(minGenesMatch * 100), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
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
			TryShapeshift(gene, this);
			Close(doCloseSound: false);
		}

		// Shapeshift
		private bool TryShapeshift(Gene_Shapeshifter geneShapeshifter, Dialog_Shapeshifter dialog)
		{
			int num = 0;
			try
			{
				num = 1;
				geneShapeshifter.PreShapeshift(geneShapeshifter, dialog.disabled);
				num = 2;
				geneShapeshifter.Shapeshift(dialog.selectedXenoHolder, dialog.disabled || dialog.clearXenogenes, xenotypeHybridization && !dialog.disabled);
				num = 3;
				geneShapeshifter.PostShapeshift(geneShapeshifter, dialog.disabled);
				num = 4;
				Find.LetterStack.ReceiveLetter("WVC_XaG_GeneShapeshifter_ShapeshiftLetterLabel".Translate(), "WVC_XaG_GeneShapeshifter_ShapeshiftLetterDesc".Translate(geneShapeshifter.pawn.Named("TARGET"), dialog.selectedXenoHolder.Label)
				+ "\n\n" + dialog.selectedXenoHolder.Description,
				MainDefOf.WVC_XaG_UndeadEvent, new LookTargets(geneShapeshifter.pawn));
				num = 5;
				ReimplanterUtility.PostImplantDebug(geneShapeshifter.pawn);
				return true;
			}
			catch (Exception arg)
			{
				// Log.Error(geneShapeshifter.pawn.Name.ToString() + " critical error during shapeshift. " + geneShapeshifter.LabelCap + " | " + geneShapeshifter.def.defName);
				Log.Error($"Error while shapeshifting {geneShapeshifter.ToStringSafe()} during phase {num}: {arg} (Gene: " + geneShapeshifter.LabelCap + " | " + geneShapeshifter.def.defName + ")");
			}
			return false;
		}

		private bool xenotypeHybridization = false;

		protected override void PostXenotypeOnGUI(float curX, float curY)
		{
			TaggedString taggedString = "WVC_XaG_GeneShapeshifter_CheckBox_ClearXenogenesBeforeImplant".Translate();
			TaggedString taggedString2 = "WVC_XaG_DialogShapeshifter_xenotypeHybridization".Translate();
			float width = Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4f + 24f;
			Rect rect = new(curX, curY, width, Text.LineHeight);
			Widgets.CheckboxLabeled(rect, taggedString, ref clearXenogenes);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "WVC_XaG_GeneShapeshifter_CheckBox_ClearXenogenesBeforeImplant_Desc".Translate());
			}
			rect.y += Text.LineHeight;
			Widgets.CheckboxLabeled(rect, taggedString2, ref xenotypeHybridization);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "WVC_XaG_DialogShapeshifterDesc_xenotypeHybridization".Translate());
			}
			postXenotypeHeight += rect.yMax - curY;
		}

	}

}
