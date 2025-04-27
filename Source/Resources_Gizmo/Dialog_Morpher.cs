using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Morpher : GeneCreationDialogBase
	{

		public Gene_Morpher gene;

		public List<GeneDef> selectedEndogenes = new();
		public List<GeneDef> selectedXenogenes = new();

		private bool? selectedCollapsedEndogenes = false;
		private bool? selectedCollapsedXenogenes = false;

		public static readonly Vector2 XenotypeSize = new(87f, 68f);

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

        protected override List<GeneDef> SelectedGenes
        {
            get
            {
				List<GeneDef> genes = new();
				if (!selectedEndogenes.NullOrEmpty())
				{
					genes.AddRange(selectedEndogenes);
				}
				if (!selectedXenogenes.NullOrEmpty())
				{
					genes.AddRange(selectedXenogenes);
				}
				return genes;
            }
        }

        protected override string Header => gene.LabelCap;

		protected override string AcceptButtonLabel
		{
			get
			{
				return "WVC_XaG_ChimeraApply_Implant".Translate().CapitalizeFirst();
			}
		}

		public PawnGeneSetHolder selectedGeneSetHolder;

		public List<PawnGeneSetHolder> allXenotypes;

		private List<PawnGeneSetHolder> cachedXenotypeDefsInOrder;

		public List<PawnGeneSetHolder> XenotypesInOrder
		{
			get
			{
				if (cachedXenotypeDefsInOrder == null)
				{
					cachedXenotypeDefsInOrder = new();
					foreach (PawnGeneSetHolder allDef in allXenotypes)
					{
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((PawnGeneSetHolder x) => 0f - x.formId);
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		public Dialog_Morpher(Gene_Morpher morpher)
		{
			xenotypeName = string.Empty;
			gene = morpher;
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			// currentXeno = gene?.pawn?.genes?.Xenotype;
			doCloseButton = true;
			selectedGeneSetHolder = null;
			allXenotypes = gene.SavedGeneSets;
			OnGenesChanged();
		}

		protected override void DrawGenes(Rect rect)
		{
			GUI.BeginGroup(rect);
			float curY = 0f;
			if (selectedGeneSetHolder != null)
			{
				DrawGenesSection(new Rect(0f, 0f, rect.width, selectedHeight), selectedEndogenes, "WVC_XaG_SelectedEndoGeneSetHolder".Translate(), ref curY, ref selectedHeight, rect, ref selectedCollapsedEndogenes);
				DrawGenesSection(new Rect(0f, 0f, rect.width, selectedHeight), selectedXenogenes, "WVC_XaG_SelectedXenoGeneSetHolder".Translate(), ref curY, ref selectedHeight, rect, ref selectedCollapsedXenogenes, true);
			}
			Widgets.Label(0f, ref curY, rect.width, "WVC_Xenotypes".Translate().CapitalizeFirst());
			curY += 10f;
			float num2 = curY;
			Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
			Widgets.BeginScrollView(new Rect(0f, curY, rect.width, rect.height - curY), ref scrollPosition, rect2);
			Rect containingRect = rect2;
			containingRect.y = curY + scrollPosition.y;
			containingRect.height = rect.height;
			DrawXenotypeSection(rect, XenotypesInOrder, null, ref curY, ref unselectedHeight, containingRect);
			// DrawCustomXenotypeSection(rect, CustomtypesInOrder, null, ref curY, ref unselectedHeight, containingRect);
			if (Event.current.type == EventType.Layout)
			{
				scrollHeight = curY - num2;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		private void DrawGenesSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, Rect containingRect, ref bool? collapsed, bool xenogene = false)
		{
			float curX = 4f;
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
				// rect2.xMax -= Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f;
				if (collapsed.HasValue)
				{
					Rect position = new(rect2.x, rect2.y + (rect2.height - 18f) / 2f, 18f, 18f);
					GUI.DrawTexture(position, collapsed.Value ? TexButton.Reveal : TexButton.Collapse);
					if (Widgets.ButtonInvisible(rect2))
					{
						collapsed = !collapsed;
						if (collapsed.Value)
						{
							SoundDefOf.TabClose.PlayOneShotOnCamera();
						}
						else
						{
							SoundDefOf.TabOpen.PlayOneShotOnCamera();
						}
					}
					if (Mouse.IsOver(rect2))
					{
						Widgets.DrawHighlight(rect2);
					}
					rect2.xMin += position.width;
				}
				Widgets.Label(rect2, label);
				curY += Text.LineHeight + 3f;
			}
			if (collapsed == true)
			{
				if (Event.current.type == EventType.Layout)
				{
					sectionHeight = 0f;
				}
				return;
			}
			float num = curY;
			float num2 = 34f + GeneCreationDialogBase.GeneSize.x + 8f;
			float num3 = rect.width - 16f;
			float num4 = num2 + 4f;
			float b = (num3 - num4 * Mathf.Floor(num3 / num4)) / 2f;
			Rect rect3 = new(0f, curY, rect.width, sectionHeight);
			//Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
			curY += 4f;
			if (!genes.Any())
			{
				XaG_UiUtility.MiddleLabel_None(rect3);
			}
			else
			{
				for (int i = 0; i < genes.Count; i++)
				{
					GeneDef geneDef = genes[i];
					if (curX + num2 > num3)
					{
						curX = 4f;
						curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					}
					curX = Mathf.Max(curX, b);
					if (DrawGene(geneDef, ref curX, curY, num2, containingRect, xenogene))
					{
						break;
					}
				}
			}
			curY += GeneCreationDialogBase.GeneSize.y + 12f;
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num;
			}
		}

		private void DrawXenotypeSection(Rect rect, List<PawnGeneSetHolder> geneSets, string label, ref float curY, ref float sectionHeight, Rect containingRect)
		{
			float curX = 4f;
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
				rect2.xMax -= 16f;
				Widgets.Label(rect2, label);
				curY += Text.LineHeight + 3f;
			}
			float num = curY;
			float num2 = 34f + GeneCreationDialogBase.GeneSize.x + 8f;
			float num3 = rect.width - 16f;
			float num4 = num2 + 4f;
			float b = (num3 - num4 * Mathf.Floor(num3 / num4)) / 2f;
			Rect rect3 = new(0f, curY, rect.width, sectionHeight);
			curY += 4f;
			if (!geneSets.Any())
			{
				XaG_UiUtility.MiddleLabel_None(rect3);
			}
			else
			{
				for (int i = 0; i < geneSets.Count; i++)
				{
					PawnGeneSetHolder xenotypeDef = geneSets[i];
					//if (quickSearchWidget.filter.Active && !matchingXenotypes.Contains(xenotypeDef))
					//{
					//	continue;
					//}
					if (curX + num2 > num3)
					{
						curX = 4f;
						curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					}
					curX = Mathf.Max(curX, b);
					if (DrawXenotype(xenotypeDef, ref curX, curY, num2, containingRect))
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						selectedGeneSetHolder = xenotypeDef;
						OnGenesChanged();
						break;
					}
				}
			}
			curY += GeneCreationDialogBase.GeneSize.y + 12f;
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num;
			}
		}

		private bool DrawGene(GeneDef geneDef, ref float curX, float curY, float packWidth, Rect containingRect, bool xenogene)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			Widgets.DrawOptionBackground(rect, false);
			curX += 4f;
			GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
			GeneUIUtility.DrawGeneDef(geneRect: new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y), gene: geneDef, geneType: xenogene ? GeneType.Xenogene : GeneType.Endogene, extraTooltip: null, doBackground: false, clickable: false, overridden: false);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		private bool DrawXenotype(PawnGeneSetHolder geneSet, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, XenotypeSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = selectedGeneSetHolder == geneSet;
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			DrawBiostats(geneSet.AllGenesCount, ref curX, curY, 4f);
			Rect xenoRect = new(curX, curY + 4f, XenotypeSize.x, XenotypeSize.y);
			DrawXenotypeBasics(geneSet, xenoRect);
			curX += XenotypeSize.x + 4f;
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		public static readonly CachedTexture XGTex = new("WVC/UI/XaG_General/XGTex_v0");
		//public static readonly CachedTexture XTFTex = new("WVC/UI/XaG_General/XTFTex_v0");

		public static readonly CachedTexture EndotypeBackground = new("WVC/UI/XaG_General/UI_BackgroundEndotype_v0");
		public static readonly CachedTexture XenotypeBackground = new("WVC/UI/XaG_General/UI_BackgroundXenotype_v0");

		public static void DrawXenotypeBasics(PawnGeneSetHolder geneSet, Rect geneRect)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = new(0.85f, 0.85f, 0.85f);
			//if (overridden)
			//{
			//	iconColor.a = 0.5f;
			//	GUI.color = ColoredText.SubtleGrayColor;
			//}
			CachedTexture cachedTexture = geneSet.xenogenes.NullOrEmpty() ? EndotypeBackground : XenotypeBackground;
			GUI.DrawTexture(rect2, cachedTexture.Texture);
            Def defIcon = geneSet.xenotypeDef;
            if (geneSet.iconDef != null)
            {
				defIcon = geneSet.iconDef;
            }
			XaG_UiUtility.XaG_DefIcon(rect2, defIcon, 0.9f, iconColor);
            Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(geneSet.name.CapitalizeFirst(), rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			Widgets.Label(rect3, geneSet.name.CapitalizeFirst());
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.EndGroup();
		}

        public static void DrawBiostats(int genes, ref float curX, float curY, float margin = 6f)
        {
            // float num = GeneCreationDialogBase.GeneSize.y / 3f;
            float num2 = 0f;
            float baseWidthOffset = 38f;
            float num3 = Text.LineHeightOf(GameFont.Small);
            Rect iconRect = new(curX, curY + margin + num2, num3, num3);
            DrawStat(iconRect, XGTex, genes.ToString(), num3);
            Rect rect = new(curX, iconRect.y, baseWidthOffset, num3);
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, "Genes".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_XenotypeGenesDesc".Translate());
            }
            curX += 34f;
        }

        public static void DrawStat(Rect iconRect, CachedTexture icon, string stat, float iconWidth)
		{
			GUI.DrawTexture(iconRect, icon.Texture);
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.LabelFit(new Rect(iconRect.xMax, iconRect.y, 38f - iconWidth, iconWidth), stat);
			Text.Anchor = TextAnchor.UpperLeft;
		}

		protected override void OnGenesChanged()
		{
			if (selectedGeneSetHolder == null)
			{
				return;
			}
			selectedEndogenes = new();
			if (!selectedGeneSetHolder.endogenes.NullOrEmpty())
			{
				selectedEndogenes.AddRange(XaG_GeneUtility.ConvertToDefs(selectedGeneSetHolder.endogenes));
			}
			else if (!selectedGeneSetHolder.endogeneDefs.NullOrEmpty())
			{
				selectedEndogenes.AddRange(selectedGeneSetHolder.endogeneDefs);
			}
			selectedXenogenes = new();
			if (!selectedGeneSetHolder.xenogenes.NullOrEmpty())
			{
				selectedXenogenes.AddRange(XaG_GeneUtility.ConvertToDefs(selectedGeneSetHolder.xenogenes));
			}
			else if (!selectedGeneSetHolder.xenogeneDefs.NullOrEmpty())
			{
				selectedXenogenes.AddRange(selectedGeneSetHolder.xenogeneDefs);
			}
			if (selectedGeneSetHolder is PawnContainerHolder holder)
			{
				selectedEndogenes.AddRange(XaG_GeneUtility.ConvertToDefs(holder.holded.genes.Endogenes));
				selectedXenogenes.AddRange(XaG_GeneUtility.ConvertToDefs(holder.holded.genes.Xenogenes));
			}
			selectedEndogenes.SortGeneDefs();
			selectedXenogenes.SortGeneDefs();
			gcx = 0;
			met = 0;
			arc = 0;
			foreach (GeneDef item in SelectedGenes)
			{
				gcx += item.biostatCpx;
				met += item.biostatMet;
				arc += item.biostatArc;
			}
		}

		public override void DoWindowContents(Rect rect)
		{
			Rect rect2 = rect;
			rect2.yMax -= ButSize.y + 4f;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 35f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect3, Header);
			Text.Font = GameFont.Small;
			DrawSearchRect(rect);
			rect2.yMin += 39f;
			float num3 = Mathf.Max(BiostatsTable.HeightForBiostats(alwaysUseFullBiostatsTableHeight ? 1 : arc), postXenotypeHeight);
			Rect rect4 = new(rect2.x + Margin, rect2.y, rect2.width - Margin * 2f, rect2.height - num3 - 8f);
			DrawGenes(rect4);
			float num4 = rect4.yMax + 4f;
			Rect rect5 = new(rect2.x + Margin + 10f, num4, rect.width * 0.75f - Margin * 3f - 10f, num3);
			rect5.yMax = rect4.yMax + num3 + 4f;
			BiostatsTable.Draw(rect5, gcx, met, arc, drawMax: true, true, maxGCX);
			// Presets
			//float num = rect.width * 0.25f - Margin - 10f;
			//float num2 = num - 24f - 10f;
			//Rect rect6 = new(rect5.xMax + Margin, num4, 32f, 4f);
			//Rect rect7 = new(rect6.xMin, rect6.y + 32f, num, 4f);
			//rect7.xMax = rect2.xMax - Margin - 17f - num2 * 0.25f;
			//Rect rect9 = new(rect7.x, rect7.yMax + 4f, num2 * 0.75f - 4f, 24f);
			//Rect rect10 = new(rect9.xMax + 4f, rect9.y, num2 * 0.25f, 24f);
			//Rect rect11 = new(rect10.xMax + 10f, rect9.y, 24f, 24f);
			//postXenotypeHeight = rect11.yMax - num4;
			//PostXenotypeOnGUI(rect6.xMin, rect9.y + 24f);
			// Presets
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		protected override void DoBottomButtons(Rect rect)
		{
			if (selectedGeneSetHolder != null && Widgets.ButtonText(new Rect(rect.xMax - (ButSize.x * 2), rect.y, ButSize.x, ButSize.y), "WVC_XaG_StorageImplanter_Apply".Translate()))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_StorageImplanter_Warning".Translate(gene.pawn.LabelCap), StorageImplanterSet));
			}
		}

		private void StorageImplanterSet()
		{
			if (Gene_StorageImplanter.CanStoreGenes(gene.pawn, out Gene_StorageImplanter implanter))
			{
				gene.StoreGeneSet(selectedGeneSetHolder, implanter);
				Close();
			}
		}

		protected override void Accept()
        {

        }

        protected override void UpdateSearchResults()
        {

        }

    }

}
