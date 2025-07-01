using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenotypeHolderBasic : GeneCreationDialogBase
	{

		public List<GeneDef> selectedGenes = new();

		protected HashSet<XenotypeHolder> matchingXenotypes = new();

		public bool? selectedCollapsed = false;

		public static readonly Vector2 XenotypeSize = new(87f, 68f);

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		protected override List<GeneDef> SelectedGenes => selectedGenes;

		protected override string Header => "ERR";

		protected override string AcceptButtonLabel
		{
			get
			{
				return "Accept".Translate();
			}
		}

		public XenotypeHolder selectedXenoHolder;

		public List<XenotypeHolder> allXenotypes;

		public List<XenotypeHolder> cachedXenotypeDefsInOrder;

		public bool disabled = false;

		public virtual List<XenotypeHolder> XenotypesInOrder
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

		public Dialog_XenotypeHolderBasic()
		{
			xenotypeName = string.Empty;
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			allXenotypes = ListsUtility.GetAllXenotypesHolders();
			selectedXenoHolder = allXenotypes.First();
			//selectedXenoHolder = allXenotypes.First((XenotypeHolder holder) => holder.xenotypeDef == gene.pawn.genes.Xenotype);
			//OnGenesChanged();
		}

		protected override void DrawGenes(Rect rect)
		{
			GUI.BeginGroup(rect);
			float curY = 0f;
			DrawGenesSection(new Rect(0f, 0f, rect.width, selectedHeight), SelectedGenes, "WVC_SelectedXenotypeGenes".Translate(), ref curY, ref selectedHeight, rect, ref selectedCollapsed);
			Widgets.Label(0f, ref curY, rect.width, "WVC_Xenotypes".Translate().CapitalizeFirst());
			curY += 10f;
			float num2 = curY;
			Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
			Widgets.BeginScrollView(new Rect(0f, curY, rect.width, rect.height - curY), ref scrollPosition, rect2);
			Rect containingRect = rect2;
			containingRect.y = curY + scrollPosition.y;
			containingRect.height = rect.height;
			DrawXenotypeSection(rect, XenotypesInOrder, null, ref curY, ref unselectedHeight, containingRect);
			if (Event.current.type == EventType.Layout)
			{
				scrollHeight = curY - num2;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		private void DrawGenesSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, Rect containingRect, ref bool? collapsed)
		{
			float curX = 4f;
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
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
			Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
			curY += 4f;
			if (!genes.Any())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
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
					if (DrawGene(geneDef, ref curX, curY, num2, containingRect))
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

		private void DrawXenotypeSection(Rect rect, List<XenotypeHolder> xenoHolders, string label, ref float curY, ref float sectionHeight, Rect containingRect)
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
			if (!xenoHolders.Any())
			{
				XaG_UiUtility.MiddleLabel_None(rect3);
			}
			else
			{
				for (int i = 0; i < xenoHolders.Count; i++)
				{
					XenotypeHolder xenotypeDef = xenoHolders[i];
					if (quickSearchWidget.filter.Active && !matchingXenotypes.Contains(xenotypeDef))
					{
						continue;
					}
					if (curX + num2 > num3)
					{
						curX = 4f;
						curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					}
					curX = Mathf.Max(curX, b);
					if (DrawXenotype(xenotypeDef, ref curX, curY, num2, containingRect))
					{
						if (selectedXenoHolder == xenotypeDef)
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
							selectedXenoHolder = allXenotypes.First((XenotypeHolder holder) => holder.Baseliner);
						}
						else
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
							selectedXenoHolder = xenotypeDef;
						}
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

		private bool DrawGene(GeneDef geneDef, ref float curX, float curY, float packWidth, Rect containingRect)
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
			GeneUIUtility.DrawGeneDef(geneRect: new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y), gene: geneDef, geneType: selectedXenoHolder.inheritable ? GeneType.Endogene : GeneType.Xenogene, extraTooltip: null, doBackground: false, clickable: false, overridden: false);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		public virtual bool DrawXenotype(XenotypeHolder xenotypeHolder, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, XenotypeSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = selectedXenoHolder == xenotypeHolder;
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			bool trueForm = xenotypeHolder.isTrueShiftForm;
			DrawBiostats(xenotypeHolder, ref curX, curY, 4f);
			Rect xenoRect = new(curX, curY + 4f, XenotypeSize.x, XenotypeSize.y);
			DrawXenotypeBasics(xenotypeHolder, xenoRect, disabled && !trueForm || xenotypeHolder.isOverriden);
			if (Mouse.IsOver(xenoRect))
			{
				string text = xenotypeHolder.Description;
				text += "\n\n" + (selected ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
				TooltipHandler.TipRegion(xenoRect, text);
			}
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

		public virtual void DrawXenotypeBasics(XenotypeHolder xenotypeHolder, Rect geneRect, bool overridden)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = new(0.85f, 0.85f, 0.85f);
			if (overridden)
			{
				iconColor.a = 0.5f;
				GUI.color = ColoredText.SubtleGrayColor;
			}
			Def iconDef = xenotypeHolder.xenotypeDef;
			if (xenotypeHolder.CustomXenotype)
			{
				iconDef = xenotypeHolder.iconDef;
			}
			CachedTexture cachedTexture = xenotypeHolder.inheritable ? EndotypeBackground : XenotypeBackground;
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			XaG_UiUtility.XaG_DefIcon(rect2, iconDef, 0.9f, iconColor);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(xenotypeHolder.LabelCap, rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			Widgets.Label(rect3, xenotypeHolder.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.EndGroup();
		}

		public virtual void DrawBiostats(XenotypeHolder xenotypeHolder, ref float curX, float curY, float margin = 6f)
		{
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
			selectedGenes = selectedXenoHolder.genes;
			selectedGenes.SortGeneDefs();
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
			float num = rect.width * 0.25f - Margin - 10f;
			float num2 = num - 24f - 10f;
			Rect rect6 = new(rect5.xMax + Margin, num4, 32f, 4f);
			Rect rect7 = new(rect6.xMin, rect6.y + 32f, num, 4f);
			rect7.xMax = rect2.xMax - Margin - 17f - num2 * 0.25f;
			Rect rect9 = new(rect7.x, rect7.yMax + 4f, num2 * 0.75f - 4f, 24f);
			Rect rect10 = new(rect9.xMax + 4f, rect9.y, num2 * 0.25f, 24f);
			Rect rect11 = new(rect10.xMax + 10f, rect9.y, 24f, 24f);
			postXenotypeHeight = rect11.yMax - num4;
			PostXenotypeOnGUI(rect6.xMin, rect9.y + 24f);
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		protected override bool CanAccept()
		{
			return true;
		}

		protected override void Accept()
		{
			//Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), StartChange));
		}

		protected override void PostXenotypeOnGUI(float curX, float curY)
		{
		}

		protected override void UpdateSearchResults()
		{
			quickSearchWidget.noResultsMatched = false;
			matchingXenotypes.Clear();
			if (!quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (XenotypeHolder item in XenotypesInOrder)
			{
				if (item != selectedXenoHolder)
				{
					if (quickSearchWidget.filter.Matches(item.LabelCap))
					{
						matchingXenotypes.Add(item);
					}
				}
			}
			quickSearchWidget.noResultsMatched = !matchingXenotypes.Any();
		}

	}

}
