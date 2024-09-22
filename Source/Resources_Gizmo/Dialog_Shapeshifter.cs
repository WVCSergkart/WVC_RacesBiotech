using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Shapeshifter : GeneCreationDialogBase
	{

		public Gene_Shapeshifter gene;

		public List<GeneDef> selectedGenes = new();

		protected HashSet<XenotypeDef> matchingXenotypes = new();
		// protected HashSet<CustomXenotype> matchingCustomXenotypes = new();

		private bool? selectedCollapsed = false;

		// public HashSet<GeneCategoryDef> matchingCategories = new();

		// public Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

		// private bool hoveredAnyGene;

		// private XenotypeDef hoveredGene;

		public static readonly Vector2 XenotypeSize = new(87f, 68f);

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		protected override List<GeneDef> SelectedGenes => selectedGenes;

		protected override string Header => selectedXenotype != null ? selectedXenotype.LabelCap : gene.LabelCap;

		protected override string AcceptButtonLabel
		{
			get
			{
				return "WVC_XaG_ChimeraApply_Implant".Translate().CapitalizeFirst();
			}
		}

		// public List<XenotypeDef> allGenes;

		// public List<GeneDef> eatedGenes;

		// public List<GeneDef> pawnGenes;

		// public List<GeneDef> pawnEndoGenes;

		// public List<GeneDef> pawnXenoGenes;

		public GeneExtension_Undead shiftExtension;
		public bool genesRegrowing = false;
		// public List<XenotypeDef> preferredXenotypes;
		public List<XenotypeDef> trueFormXenotypes = new();
		// public List<CustomXenotype> trueFormCustomtypes = new();
		public bool doubleXenotypeReimplantation = true;
		public bool clearXenogenes = true;
		public XenotypeDef selectedXenotype;
		// public XenotypeDef currentXeno;

		public List<XenotypeDef> allXenotypes;
		// public List<CustomXenotype> customXenotypes;

		private List<XenotypeDef> cachedXenotypeDefsInOrder;
		// private List<CustomXenotype> cachedCustomXenotypesInOrder;

		public List<XenotypeDef> XenotypesInOrder
		{
			get
			{
				if (cachedXenotypeDefsInOrder == null)
				{
					cachedXenotypeDefsInOrder = new();
					foreach (XenotypeDef allDef in allXenotypes)
					{
						if (allDef == XenotypeDefOf.Baseliner)
						{
							continue;
						}
						cachedXenotypeDefsInOrder.Add(allDef);
					}
					cachedXenotypeDefsInOrder.SortBy((XenotypeDef x) => 0f - x.displayPriority - (x.inheritable ? 100000f : 0));
				}
				return cachedXenotypeDefsInOrder;
			}
		}

		// public List<CustomXenotype> CustomtypesInOrder
		// {
			// get
			// {
				// if (cachedCustomXenotypesInOrder == null)
				// {
					// cachedCustomXenotypesInOrder = new();
					// foreach (CustomXenotype allDef in customXenotypes)
					// {
						// cachedCustomXenotypesInOrder.Add(allDef);
					// }
					// cachedCustomXenotypesInOrder.SortBy((CustomXenotype x) => x.inheritable ? 1f : 0);
				// }
				// return cachedCustomXenotypesInOrder;
			// }
		// }

		public Dialog_Shapeshifter(Gene_Shapeshifter shapeshifter)
		{
			xenotypeName = string.Empty;
			gene = shapeshifter;
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			// currentXeno = gene?.pawn?.genes?.Xenotype;
			selectedXenotype = gene?.pawn?.genes?.Xenotype;
			allXenotypes = ListsUtility.GetAllXenotypesExceptAndroids();
			// customXenotypes = XenotypeFilterUtility.AllCustomXenotypesExceptAndroids();
			// preferredXenotypes = ModLister.IdeologyInstalled ? gene.pawn?.ideo?.Ideo?.PreferredXenotypes : null;
			shiftExtension = gene?.def?.GetModExtension<GeneExtension_Undead>();
			genesRegrowing = HediffUtility.HasAnyHediff(shiftExtension?.blockingHediffs, gene.pawn);
			trueFormXenotypes = ListsUtility.GetTrueFormXenotypesFromList(allXenotypes);
			// trueFormCustomtypes = XenotypeFilterUtility.TrueFormXenotypesFromList(customXenotypes);
			OnGenesChanged();
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
			// DrawCustomXenotypeSection(rect, CustomtypesInOrder, null, ref curY, ref unselectedHeight, containingRect);
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

		private void DrawXenotypeSection(Rect rect, List<XenotypeDef> xenotypes, string label, ref float curY, ref float sectionHeight, Rect containingRect)
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
			if (!xenotypes.Any())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
			}
			else
			{
				for (int i = 0; i < xenotypes.Count; i++)
				{
					XenotypeDef xenotypeDef = xenotypes[i];
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
						if (selectedXenotype == xenotypeDef)
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
							selectedXenotype = XenotypeDefOf.Baseliner;
						}
						else
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
							selectedXenotype = xenotypeDef;
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
			// bool selected = !selectedSection && selectedGenes.Contains(geneDef);
			// bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
			Widgets.DrawOptionBackground(rect, false);
			curX += 4f;
			GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
			GeneUIUtility.DrawGeneDef(geneRect: new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y), gene: geneDef, geneType: selectedXenotype.inheritable ? GeneType.Endogene : GeneType.Xenogene, extraTooltip: null, doBackground: false, clickable: false, overridden: false);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
			// if (Mouse.IsOver(rect))
			// {
				// hoveredGene = geneDef;
				// hoveredAnyGene = true;
			// }
			// else if (hoveredGene != null && geneDef.ConflictsWith(hoveredGene))
			// {
				// Widgets.DrawLightHighlight(rect);
			// }
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		private bool DrawXenotype(XenotypeDef xenotypeDef, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, XenotypeSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = selectedXenotype == xenotypeDef;
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			bool trueForm = trueFormXenotypes.Contains(xenotypeDef);
			DrawBiostats(xenotypeDef.AllGenes.Count, trueForm, ref curX, curY, 4f);
			Rect xenoRect = new(curX, curY + 4f, XenotypeSize.x, XenotypeSize.y);
			DrawXenotypeBasics(xenotypeDef, xenoRect, genesRegrowing && !trueForm);
			if (Mouse.IsOver(xenoRect))
			{
				string text = xenotypeDef.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + (!xenotypeDef.descriptionShort.NullOrEmpty() ? xenotypeDef.descriptionShort : xenotypeDef.description);
				text += "\n\n" + ("WVC_Inheritable".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + " " + xenotypeDef.inheritable.ToStringYesNo();
				if (!xenotypeDef.doubleXenotypeChances.NullOrEmpty())
				{
					text += "\n\n" + ("WVC_DoubleXenotypes".Translate() + ":").Colorize(ColoredText.TipSectionTitleColor) + "\n" + xenotypeDef.doubleXenotypeChances.Select((XenotypeChance x) => "WVC_XaG_DoubleXenotypeWithChanceText".Translate(x.xenotype.LabelCap, (x.chance * 100f).ToString()).ToString()).ToLineList(" - ");
				}
				text += "\n\n" + (selected ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
				TooltipHandler.TipRegion(xenoRect, text);
			}
			curX += XenotypeSize.x + 4f;
			// if (Mouse.IsOver(rect))
			// {
				// hoveredGene = xenotypeDef;
				// hoveredAnyGene = true;
			// }
			// else if (hoveredGene != null && selectedXenotype == xenotypeDef)
			// {
				// Widgets.DrawLightHighlight(rect);
			// }
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		public static readonly CachedTexture XGTex = new("WVC/UI/XaG_General/XGTex_v0");
		public static readonly CachedTexture XTFTex = new("WVC/UI/XaG_General/XTFTex_v0");

		public static readonly CachedTexture EndotypeBackground = new("WVC/UI/XaG_General/UI_BackgroundEndotype_v0");
		public static readonly CachedTexture XenotypeBackground = new("WVC/UI/XaG_General/UI_BackgroundXenotype_v0");

		public static void DrawXenotypeBasics(XenotypeDef xenotypeDef, Rect geneRect, bool overridden)
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
			CachedTexture cachedTexture = xenotypeDef.inheritable ? EndotypeBackground : XenotypeBackground;
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			Widgets.DefIcon(rect2, xenotypeDef, null, 0.9f, null, drawPlaceholder: false, iconColor);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(xenotypeDef.LabelCap, rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			Widgets.Label(rect3, xenotypeDef.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.EndGroup();
		}

		public static void DrawBiostats(int genes, bool trueForm, ref float curX, float curY, float margin = 6f)
		{
			float num = GeneCreationDialogBase.GeneSize.y / 3f;
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
			num2 += num;
			if (trueForm)
			{
				Rect iconRect3 = new(curX, curY + margin + num2, num3, num3);
				DrawStat(iconRect3, XTFTex, trueForm.ToStringYesNo(), num3 - 3f);
				Rect rect3 = new(curX, iconRect3.y, baseWidthOffset, num3);
				if (Mouse.IsOver(rect3))
				{
					Widgets.DrawHighlight(rect3);
					TooltipHandler.TipRegion(rect3, "WVC_XaG_TrueForm".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_ShapeshifterDialog_TrueFormDesc".Translate());
				}
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
			selectedGenes = selectedXenotype.AllGenes;
			// selectedGenes.SortBy((GeneDef x) => 0f - x.displayCategory.displayPriorityInGenepack);
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
			// Presets
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
			// Presets
			// PostXenotypeOnGUI(rect5.xMin, rect5.y + 24f);
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		// protected override void DrawSearchRect(Rect rect)
		// {
			// base.DrawSearchRect(rect);
			// if (Widgets.ButtonText(new Rect(rect.xMax - ButSize.x, rect.y, ButSize.x, ButSize.y), "WVC_XaG_CreateChimera_PresetsLoad".Translate()))
			// {
				// if (!gene.geneSetPresets.NullOrEmpty())
				// {
					// Find.WindowStack.Add(new Dialog_ChimeraPresetsList_Load(this));
				// }
				// else
				// {
					// Messages.Message("WVC_XaG_CreateChimera_PresetsIsNull".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				// }
			// }
			// if (Widgets.ButtonText(new Rect(rect.xMax - ButSize.x * 2f - 4f, rect.y, ButSize.x, ButSize.y), "WVC_XaG_CreateChimera_PresetsSave".Translate()))
			// {
				// if (xenotypeName.NullOrEmpty())
				// {
					// Messages.Message("WVC_XaG_CreateChimera_PresetsNameMessageError".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				// }
				// else
				// {
					// if (!gene.geneSetPresets.NullOrEmpty())
					// {
						// foreach (GeneSetPresets geneSetPresets in gene.geneSetPresets.ToList())
						// {
							// if (geneSetPresets.name == xenotypeName)
							// {
								// gene.geneSetPresets.Remove(geneSetPresets);
							// }
						// }
					// }
					// else
					// {
						// gene.geneSetPresets = new();
					// }
					// GeneSetPresets preset = new();
					// preset.name = xenotypeName;
					// preset.geneDefs = selectedGenes;
					// gene.geneSetPresets.Add(preset);
					// Messages.Message("WVC_XaG_CreateChimera_NewPresetSaved".Translate().CapitalizeFirst(), null, MessageTypeDefOf.PositiveEvent, historical: false);
				// }
			// }
		// }

		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			// if (Widgets.ButtonText(new Rect((rect.xMax / 2) - ButSize.x, rect.y, ButSize.x, ButSize.y), "WVC_XaG_ResetButton".Translate()))
			// {
				// selectedGenes = new();
			// }
			// if (Widgets.ButtonText(new Rect((rect.xMax / 2), rect.y, ButSize.x, ButSize.y), "WVC_XaG_ChimeraApply_Eat".Translate()))
			// {
				// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneGeneticThief_EatSelectedGenes".Translate(gene.pawn.LabelCap), EatGenes));
			// }
			if (genesRegrowing)
			{
				// int num = leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
				// GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups[0];
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
			if (trueFormXenotypes.Contains(selectedXenotype))
			{
				return true;
			}
			if (genesRegrowing)
			{
				Messages.Message("WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			// if (!preferredXenotypes.NullOrEmpty() && !preferredXenotypes.Contains(selectedXenotype))
			// {
				// Messages.Message("WVC_XaG_GeneShapeshifter_NotPreferredXenotype".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				// return false;
			// }
			return true;
		}

		protected override void Accept()
		{
			// if (selectedXenotype != currentXeno)
			// {
				// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), StartChange));
			// }
			// else
			// {
				// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), StartChange));
			// }
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneShapeshifter_ShapeshiftWarning".Translate(gene.pawn.LabelCap), StartChange));
		}

		public void StartChange()
		{
			UndeadUtility.TryShapeshift(gene, this);
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
			Widgets.CheckboxLabeled(rect, taggedString2, ref doubleXenotypeReimplantation);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "WVC_XaG_GeneShapeshifter_CheckBox_ImplantDoubleXenotype_Desc".Translate());
			}
			postXenotypeHeight += rect.yMax - curY;
		}

		protected override void UpdateSearchResults()
		{
			quickSearchWidget.noResultsMatched = false;
			matchingXenotypes.Clear();
			// matchingCustomXenotypes.Clear();
			if (!quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (XenotypeDef item in XenotypesInOrder)
			{
				if (item != selectedXenotype)
				{
					if (quickSearchWidget.filter.Matches(item.label))
					{
						matchingXenotypes.Add(item);
					}
				}
			}
			// foreach (CustomXenotype item in CustomtypesInOrder)
			// {
				// if (item != selectedXenotype)
				// {
					// if (quickSearchWidget.filter.Matches(item.name))
					// {
						// matchingCustomXenotypes.Add(item);
					// }
				// }
			// }
			quickSearchWidget.noResultsMatched = !matchingXenotypes.Any();
		}

	}

}
