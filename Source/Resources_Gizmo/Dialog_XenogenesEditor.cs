using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_XenogenesEditor : GeneCreationDialogBase
	{

		public IGeneXenogenesEditor gene;

		public List<GeneDef> selectedGenes = new();

		private bool? selectedCollapsed = false;

		public HashSet<GeneCategoryDef> matchingCategories = new();

		public Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

		private bool hoveredAnyGene;

		private GeneDef hoveredGene;

		private List<GeneDef> overridenGenes;

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		protected override List<GeneDef> SelectedGenes => selectedGenes;

		private string cachedHeader = null;
		protected override string Header
		{
			get
			{
				if (cachedHeader == null)
				{
					if (limitDisabled)
					{
						cachedHeader = gene.LabelCap;
					}
					else
					{
						cachedHeader = "WVC_Complexity".Translate().CapitalizeFirst() + " (" + ConsumedLimit_Cpx + "/" + ComplexityLimit.ToString() + ") | " + "WVC_Archites".Translate().CapitalizeFirst() + " (" + ConsumedLimit_Arc + "/" + ArchitesLimit.ToString() + ")";
					}
				}
				return cachedHeader;
			}
		}

		protected override string AcceptButtonLabel
		{
			get
			{
				if (selectedGenes.NullOrEmpty())
				{
					return "WVC_XaG_ChimeraApply_Clear".Translate().CapitalizeFirst();
				}
				return "Apply".Translate().CapitalizeFirst();
			}
		}

		private List<GeneDef> allGenes;
		private List<GeneDef> pawnEndoGenes;
		private List<GeneDef> pawnXenoGenes;

		private List<GeneDef> cachedGeneDefsInOrder;
		public List<GeneDef> GenesInOrder
		{
			get
			{
				if (cachedGeneDefsInOrder.NullOrEmpty())
				{
					cachedGeneDefsInOrder = new();
					foreach (GeneDef allDef in allGenes)
					{
						cachedGeneDefsInOrder.Add(allDef);
					}
					cachedGeneDefsInOrder.SortBy((GeneDef x) => 0f - x.displayCategory.displayPriorityInXenotype, (GeneDef x) => x.displayCategory.label, (GeneDef x) => x.displayOrderInCategory);
				}
				return cachedGeneDefsInOrder;
			}
		}

		public bool subActionsDisabled = false;
		private bool limitDisabled = false;
		private bool isContainer = false;

		public Dialog_XenogenesEditor(IGeneXenogenesEditor chimera)
		{
			xenotypeName = string.Empty;
			gene = chimera;
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefsListForReading)
			{
				collapsedCategories.Add(allDef, value: false);
			}
			limitDisabled = !Gene_Chimera.ChimeraGenesLimit || (gene.ComplexityLimit == 999 && gene.ArchiteLimit == 999);
			isContainer = gene.IsContainer;
			UpdateGenesInforamtion();
			OnGenesChanged();
		}

		private void GetCustomEater()
		{
			geneCustomChimeraEater = null;
			chimeraApplyEater = "WVC_XaG_IGeneXenogenesEditor_Disable";
			chimeraApplyEaterWarning = "WVC_XaG_IGeneXenogenesEditor_DisableGenes";
			foreach (Gene pawnGene in gene.Pawn.genes.GenesListForReading)
			{
				if (pawnGene is IGeneCustomChimeraEater customEater && pawnGene.Active)
				{
					geneCustomChimeraEater = customEater;
					chimeraApplyEater = customEater.ChimeraEater_Name;
					chimeraApplyEaterWarning = customEater.ChimeraEater_Desc.ToString();
					break;
				}
			}
		}

		private string chimeraApplyEater = "WVC_XaG_IGeneXenogenesEditor_Disable";
		private TaggedString chimeraApplyEaterWarning = "WVC_XaG_IGeneXenogenesEditor_DisableGenes";
		private IGeneCustomChimeraEater geneCustomChimeraEater;

		private void GenesEater()
		{
			MiscUtility.UpdateStaticCollection();
			if (geneCustomChimeraEater != null)
			{
				geneCustomChimeraEater.ChimeraEater(ref selectedGenes);
			}
			else
			{
				BasicEater();
			}
			//if (gene.Extension_Undead != null && !gene.Extension_Undead.soundDefOnImplant.NullOrUndefined())
			//{
			//}
			gene.Extension_Undead?.soundDefOnImplant?.PlayOneShot(SoundInfo.InMap(gene.Pawn));
			gene.Debug_RemoveDupes();
			UpdateGenesInforamtion();
			OnGenesChanged();
		}

		private void BasicEater()
		{
			float bonusChanceForGameDays = (Find.TickManager.TicksGame / 60000) * 0.01f / 7;
			if (bonusChanceForGameDays > 0.3f)
			{
				bonusChanceForGameDays = 0.3f;
			}
			foreach (GeneDef geneDef in selectedGenes)
			{
				if (!gene.TryDisableGene(geneDef))
				{
					continue;
				}
				try
				{
					if ((!Rand.Chance(0.07f + bonusChanceForGameDays) || !gene.TryGetToolGene()) && Rand.Chance(0.04f + bonusChanceForGameDays))
					{
						gene.TryGetUniqueGene();
					}
				}
				catch (Exception arg)
				{
					Log.Error("Failed obtaine gene. Reason: " + arg);
				}
			}
		}

		// public override void PostOpen()
		// {
		// if (!ModLister.CheckBiotech("xenotype creation"))
		// {
		// Close(doCloseSound: false);
		// }
		// else
		// {
		// base.PostOpen();
		// }
		// }

		protected override void DrawGenes(Rect rect)
		{
			hoveredAnyGene = false;
			GUI.BeginGroup(rect);
			float curY = 0f;
			DrawSection(new Rect(0f, 0f, rect.width, selectedHeight), selectedGenes, "SelectedGenes".Translate(), ref curY, ref selectedHeight, adding: false, rect, ref selectedCollapsed);
			if (!selectedCollapsed.Value)
			{
				curY += 10f;
			}
			float num = curY;
			Widgets.Label(0f, ref curY, rect.width, "Genes".Translate().CapitalizeFirst());
			curY += 10f;
			float height = curY - num - 4f;
			if (Widgets.ButtonText(new Rect(rect.width - 150f - 16f, num, 150f, height), "CollapseAllCategories".Translate()))
			{
				SoundDefOf.TabClose.PlayOneShotOnCamera();
				foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
				{
					collapsedCategories[allDef] = true;
				}
			}
			if (Widgets.ButtonText(new Rect(rect.width - 300f - 4f - 16f, num, 150f, height), "ExpandAllCategories".Translate()))
			{
				SoundDefOf.TabOpen.PlayOneShotOnCamera();
				foreach (GeneCategoryDef allDef2 in DefDatabase<GeneCategoryDef>.AllDefs)
				{
					collapsedCategories[allDef2] = false;
				}
			}
			float num2 = curY;
			Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
			Widgets.BeginScrollView(new Rect(0f, curY, rect.width, rect.height - curY), ref scrollPosition, rect2);
			Rect containingRect = rect2;
			containingRect.y = curY + scrollPosition.y;
			containingRect.height = rect.height;
			bool? collapsed = null;
			DrawSection(rect, GenesInOrder, null, ref curY, ref unselectedHeight, adding: true, containingRect, ref collapsed);
			if (Event.current.type == EventType.Layout)
			{
				scrollHeight = curY - num2;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
			if (!hoveredAnyGene)
			{
				hoveredGene = null;
			}
		}

		private void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref bool? collapsed)
		{
			float curX = 4f;
			Dialog_XenogenesEditor.DrawGenesSections_Label(ref rect, label, ref curY, adding, ref collapsed);
			if (collapsed == true)
			{
				if (Event.current.type == EventType.Layout)
				{
					sectionHeight = 0f;
				}
				return;
			}
			Dialog_XenogenesEditor.DrawGenesSection_DrawRectFast(ref rect, ref curY, sectionHeight, adding, out float num, out bool flag, out float num2, out float num3, out float b, out Rect rect3);
			DrawGenesSection_Local(rect, genes, ref curY, ref sectionHeight, adding, containingRect, ref curX, num, ref flag, num2, num3, b, rect3);
		}

		public static void DrawGenesSections_Label(ref Rect rect, string label, ref float curY, bool adding, ref bool? collapsed)
		{
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
				rect2.xMax -= (adding ? 16f : (Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f));
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
				if (!adding)
				{
					Text.Anchor = TextAnchor.UpperRight;
					GUI.color = ColoredText.SubtleGrayColor;
					Widgets.Label(new Rect(rect2.xMax - 18f, curY, rect.width - rect2.width, Text.LineHeight), "ClickToAddOrRemove".Translate());
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
				}
				curY += Text.LineHeight + 3f;
			}
		}

		private void DrawGenesSection_Local(Rect rect, List<GeneDef> genes, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref float curX, float num, ref bool flag, float num2, float num3, float b, Rect rect3)
		{
			if (!genes.Any())
			{
				XaG_UiUtility.MiddleLabel_None(rect3);
			}
			else
			{
				GeneCategoryDef geneCategoryDef = null;
				int num5 = 0;
				for (int i = 0; i < genes.Count; i++)
				{
					GeneDef geneDef = genes[i];
					if ((adding && quickSearchWidget.filter.Active && (!matchingGenes.Contains(geneDef) || selectedGenes.Contains(geneDef)) && !matchingCategories.Contains(geneDef.displayCategory)))
					{
						continue;
					}
					bool flag2 = false;
					if (curX + num2 > num3)
					{
						curX = 4f;
						curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
						flag2 = true;
					}
					bool flag3 = quickSearchWidget.filter.Active && (matchingGenes.Contains(geneDef) || matchingCategories.Contains(geneDef.displayCategory));
					bool flag4 = collapsedCategories[geneDef.displayCategory] && !flag3;
					if (adding && geneCategoryDef != geneDef.displayCategory)
					{
						if (!flag2 && flag)
						{
							curX = 4f;
							curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
						}
						geneCategoryDef = geneDef.displayCategory;
						Rect rect4 = new(curX, curY, rect.width - 8f, Text.LineHeight);
						if (!flag3)
						{
							Rect position2 = new(rect4.x, rect4.y + (rect4.height - 18f) / 2f, 18f, 18f);
							GUI.DrawTexture(position2, flag4 ? TexButton.Reveal : TexButton.Collapse);
							if (Widgets.ButtonInvisible(rect4))
							{
								collapsedCategories[geneDef.displayCategory] = !collapsedCategories[geneDef.displayCategory];
								if (collapsedCategories[geneDef.displayCategory])
								{
									SoundDefOf.TabClose.PlayOneShotOnCamera();
								}
								else
								{
									SoundDefOf.TabOpen.PlayOneShotOnCamera();
								}
							}
							if (num5 % 2 == 1)
							{
								Widgets.DrawLightHighlight(rect4);
							}
							if (Mouse.IsOver(rect4))
							{
								Widgets.DrawHighlight(rect4);
							}
							rect4.xMin += position2.width;
						}
						Widgets.Label(rect4, geneCategoryDef.LabelCap);
						curY += rect4.height;
						if (!flag4)
						{
							GUI.color = Color.grey;
							Widgets.DrawLineHorizontal(curX, curY, rect.width - 8f);
							GUI.color = Color.white;
							curY += 10f;
						}
						num5++;
					}
					if (adding && flag4)
					{
						flag = false;
						if (Event.current.type == EventType.Layout)
						{
							sectionHeight = curY - num;
						}
						continue;
					}
					curX = Mathf.Max(curX, b);
					flag = true;
					if (DrawGene(geneDef, !adding, ref curX, curY, num2, containingRect))
					{
						if (selectedGenes.Contains(geneDef))
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
							selectedGenes.Remove(geneDef);
						}
						else
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
							selectedGenes.Add(geneDef);
						}
						if (!xenotypeNameLocked)
						{
							xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
						}
						OnGenesChanged();
						break;
					}
				}
			}
			if (!adding || flag)
			{
				curY += GeneCreationDialogBase.GeneSize.y + 12f;
			}
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num;
			}
		}

		public static void DrawGenesSection_DrawRectFast(ref Rect rect, ref float curY, float sectionHeight, bool adding, out float num, out bool flag, out float num2, out float num3, out float b, out Rect rect3)
		{
			num = curY;
			flag = false;
			num2 = 34f + GeneCreationDialogBase.GeneSize.x + 8f;
			num3 = rect.width - 16f;
			float num4 = num2 + 4f;
			b = (num3 - num4 * Mathf.Floor(num3 / num4)) / 2f;
			rect3 = new(0f, curY, rect.width, sectionHeight);
			if (!adding)
			{
				Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
			}
			curY += 4f;
		}

		private bool DrawGene(GeneDef geneDef, bool selectedSection, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = !selectedSection && selectedGenes.Contains(geneDef);
			bool overridden = GetOverridden(geneDef);
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
			GeneUIUtility.DrawGeneDef(geneRect: new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y), gene: geneDef, geneType: GeneType.Xenogene, extraTooltip: () => GeneTip(geneDef, selectedSection), doBackground: false, clickable: false, overridden: overridden);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
			if (Mouse.IsOver(rect))
			{
				hoveredGene = geneDef;
				hoveredAnyGene = true;
			}
			else if (hoveredGene != null && geneDef.ConflictsWith(hoveredGene))
			{
				Widgets.DrawLightHighlight(rect);
			}
			if (Widgets.ButtonInvisible(rect))
			{
				result = true;
			}
			if (!gene.CollectedGenes.Contains(geneDef))
			{
				Rect genelineRect = new(rect.xMax - 25f, rect.yMax - 72f, 22f, 22f);
				Widgets.DrawTextureFitted(genelineRect, XaG_UiUtility.GenelineIconMark.Texture, 1f, 1f);
				if (Mouse.IsOver(genelineRect))
				{
					Widgets.DrawHighlight(genelineRect);
					TooltipHandler.TipRegion(genelineRect, "WVC_XaG_GenelineMarkTooltip".Translate());
				}
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		private bool GetOverridden(GeneDef geneDef)
		{
			if (isContainer)
			{
				return overridenGenes.Contains(geneDef);
			}
			if (geneDef.endogeneCategory == EndogeneCategory.Melanin)
			{
				return true;
			}
			if (pawnEndoGenes.Contains(geneDef))
			{
				return true;
			}
			//if (lockedGenes.Contains(geneDef))
			//{
			//	return true;
			//}
			return overridenGenes.Contains(geneDef);
		}

		private string GeneTip(GeneDef geneDef, bool selectedSection)
		{
			string text = null;
			if (selectedSection)
			{
				if (overridenGenes.Contains(geneDef))
				{
					text = "WVC_XaG_ChimeraDialog_ConflictWith".Translate(geneDef.label).Colorize(ColoredText.TipSectionTitleColor) + "\n" + (overridenGenes.Where((gene) => gene != geneDef && gene.ConflictsWith(geneDef)).Select((GeneDef x) => x.LabelCap.ToString()).ToLineList("  - ")).Colorize(ColorLibrary.RedReadable);
				}
			}
			if (pawnEndoGenes.Contains(geneDef))
			{
				text = "WVC_XaG_ChimeraDialog_PawnHasEndogene".Translate(geneDef.label).Colorize(ColoredText.TipSectionTitleColor);
			}
			else if (geneDef.endogeneCategory == EndogeneCategory.Melanin)
			{
				text = "WVC_XaG_ChimeraDialog_NonPassGene".Translate(geneDef.label).Colorize(ColoredText.TipSectionTitleColor);
			}
			if (selectedGenes.Contains(geneDef) && geneDef.prerequisite != null && !selectedGenes.Contains(geneDef.prerequisite) && !pawnEndoGenes.Contains(geneDef.prerequisite))
			{
				if (!text.NullOrEmpty())
				{
					text += "\n\n";
				}
				text += ("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
			}
			if (!text.NullOrEmpty())
			{
				text += "\n\n";
			}
			return text + (selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
			//static string GroupInfo(GeneLeftChosenGroup group)
			//{
			//	if (group == null)
			//	{
			//		return null;
			//	}
			//	return ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
			//}
		}

		private int? cachedComplexityLimit_Consumed;
		public int ConsumedLimit_Cpx
		{
			get
			{
				if (!cachedComplexityLimit_Consumed.HasValue)
				{
					GetLimitAndCost();
				}
				return cachedComplexityLimit_Consumed.Value;
			}
		}

		private int? cachedComplexityLimit;
		public int ComplexityLimit
		{
			get
			{
				if (!cachedComplexityLimit.HasValue)
				{
					GetLimitAndCost();
				}
				return cachedComplexityLimit.Value;
			}
		}

		private int? cachedArchitesLimit;
		public int ArchitesLimit
		{
			get
			{
				if (!cachedArchitesLimit.HasValue)
				{
					GetLimitAndCost();
				}
				return cachedArchitesLimit.Value;
			}
		}

		private int? cachedLimitConsumed_Arc;
		public int ConsumedLimit_Arc
		{
			get
			{
				if (!cachedLimitConsumed_Arc.HasValue)
				{
					GetLimitAndCost();
				}
				return cachedLimitConsumed_Arc.Value;
			}
		}

		private void GetLimitAndCost()
		{
			int biostatCpxCost = 0;
			//float genesLimit = 0;
			//float genesLimitFactor = 1f;
			//StatDef statDef = gene.ChimeraLimitStatDef;
			int biostatArcCost = 0;
			//foreach (GeneDef item in pawnEndoGenes)
			//{
			//	if (XaG_GeneUtility.ConflictWith(item, SelectedGenes))
			//	{
			//		continue;
			//	}
			//	GetStatFromStatModifiers(statDef, item.statOffsets, item.statFactors, out float offset, out float factor);
			//	genesLimit += offset;
			//	genesLimitFactor *= factor;
			//}
			foreach (GeneDef item in SelectedGenes)
			{
				//GetStatFromStatModifiers(statDef, item.statOffsets, item.statFactors, out float offset, out float factor);
				biostatCpxCost += item.biostatCpx;
				biostatArcCost += item.biostatArc;
				//genesLimit += offset;
				//genesLimitFactor *= factor;
			}
			//foreach (Gene pawnGene in gene.Pawn.genes.GenesListForReading)
			//{
			//	if (!SelectedGenes.Contains(pawnGene.def) && pawnGene.pawn.genes.Xenogenes.Contains(pawnGene))
			//	{
			//		continue;
			//	}
			//	if (pawnGene is not Gene_ChimeraHediff chimeraHediffGene)
			//	{
			//		continue;
			//	}
			//	if (chimeraHediffGene.ChimeraHediff != null)
			//	{
			//		GetStatFromStatModifiers(statDef, chimeraHediffGene.ChimeraHediff?.CurStage?.statOffsets, chimeraHediffGene.ChimeraHediff?.CurStage?.statFactors, out float offset, out float factor);
			//		genesLimit += offset;
			//		genesLimitFactor *= factor;
			//	}
			//}
			//genesLimit *= genesLimitFactor;
			cachedComplexityLimit_Consumed = biostatCpxCost;
			//
			cachedLimitConsumed_Arc = biostatArcCost;
			//
			if (limitDisabled)
			{
				cachedComplexityLimit = 999;
				cachedArchitesLimit = 999;
			}
			else
			{
				cachedComplexityLimit = gene.ComplexityLimit;
				cachedArchitesLimit = gene.ArchiteLimit;
			}

			//void GetStatFromStatModifiers(StatDef statDef, List<StatModifier> statOffsets, List<StatModifier> statFactors, out float offset, out float factor)
			//{
			//	offset = 0;
			//	factor = 1;
			//	if (statDef == null)
			//	{
			//		return;
			//	}
			//	if (statOffsets != null)
			//	{
			//		foreach (StatModifier statModifier in statOffsets)
			//		{
			//			if (statModifier.stat == statDef)
			//			{
			//				offset += statModifier.value;
			//			}
			//		}
			//	}
			//	if (statFactors != null)
			//	{
			//		foreach (StatModifier statModifier in statFactors)
			//		{
			//			if (statModifier.stat == statDef)
			//			{
			//				factor *= statModifier.value;
			//			}
			//		}
			//	}
			//}
		}


		//private List<GeneDef> lockedGenes;
		//private void UpdLockedGenes()
		//{
		//	lockedGenes = new();
		//	//foreach (GeneDef item in gene.EatedGenes)
		//	//{
		//	//	lockedGenes.Add(item);
		//	//}
		//	foreach (GeneDef item in gene.DestroyedGenes)
		//	{
		//		lockedGenes.Add(item);
		//	}
		//}

		protected override void OnGenesChanged()
		{
			overridenGenes = new();
			gcx = 0;
			met = 0;
			arc = 0;
			foreach (GeneDef geneDef in selectedGenes)
			{
				foreach (GeneDef otherGeneDef in selectedGenes)
				{
					if (geneDef != otherGeneDef && geneDef.ConflictsWith(otherGeneDef))
					{
						if (!overridenGenes.Contains(geneDef))
						{
							overridenGenes.Add(geneDef);
						}
					}
				}
				gcx += geneDef.biostatCpx;
				met += geneDef.biostatMet;
				arc += geneDef.biostatArc;
			}
			selectedGenes.SortGeneDefs();
			//UpdLockedGenes();
			//base.OnGenesChanged();
			foreach (GeneDef item in pawnEndoGenes)
			{
				if (XaG_GeneUtility.ConflictWith(item, selectedGenes))
				{
					continue;
				}
				gcx += item.biostatCpx;
				met += item.biostatMet;
				arc += item.biostatArc;
			}
			cachedComplexityLimit = null;
			cachedComplexityLimit_Consumed = null;
			cachedArchitesLimit = null;
			cachedLimitConsumed_Arc = null;
			cachedHeader = null;
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
			PresetNameField(rect, rect2, num4, rect5);
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		private void PresetNameField(Rect rect, Rect rect2, float num4, Rect rect5)
		{
			if (gene.GeneSetPresets == null)
			{
				return;
			}
			// Presets
			float num = rect.width * 0.25f - Margin - 10f;
			float num2 = num - 24f - 10f;
			string text = "WVC_XaG_CreateChimera_PresetsNameTextField".Translate().CapitalizeFirst() + ":";
			Rect rect6 = new(rect5.xMax + Margin, num4, Text.CalcSize(text).x, Text.LineHeight);
			Widgets.Label(rect6, text);
			Rect rect7 = new(rect6.xMin, rect6.y + Text.LineHeight, num, Text.LineHeight);
			rect7.xMax = rect2.xMax - Margin - 17f - num2 * 0.25f;
			string text2 = xenotypeName;
			xenotypeName = Widgets.TextField(rect7, xenotypeName, 40, new("^[\\p{L}0-9 '\\-]*$"));
			if (text2 != xenotypeName)
			{
				if (xenotypeName.Length > text2.Length && xenotypeName.Length > 3)
				{
					xenotypeNameLocked = true;
				}
				else if (xenotypeName.Length == 0)
				{
					xenotypeNameLocked = false;
				}
			}
			Rect rect9 = new(rect7.x, rect7.yMax + 4f, num2 * 0.75f - 4f, 24f);
			if (Widgets.ButtonText(rect9, "Randomize".Translate()))
			{
				if (SelectedGenes.Count == 0)
				{
					Messages.Message("SelectAGeneToRandomizeName".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					GUI.FocusControl(null);
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
				}
			}
			Rect rect10 = new(rect9.xMax + 4f, rect9.y, num2 * 0.25f, 24f);
			if (Widgets.ButtonText(rect10, "..."))
			{
				if (SelectedGenes.Count > 0)
				{
					List<string> list = new();
					int num5 = 0;
					while (list.Count < 20)
					{
						string text3 = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
						if (text3.NullOrEmpty())
						{
							break;
						}
						if (list.Contains(text3) || text3 == xenotypeName)
						{
							num5++;
							if (num5 >= 1000)
							{
								break;
							}
						}
						else
						{
							list.Add(text3);
						}
					}
					List<FloatMenuOption> list2 = new();
					for (int j = 0; j < list.Count; j++)
					{
						string i = list[j];
						list2.Add(new FloatMenuOption(i, delegate
						{
							xenotypeName = i;
						}));
					}
					if (list2.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list2));
					}
				}
				else
				{
					Messages.Message("SelectAGeneToChooseAName".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			Rect rect11 = new(rect10.xMax + 10f, rect9.y, 24f, 24f);
			if (Widgets.ButtonImage(rect11, xenotypeNameLocked ? LockedTex : UnlockedTex))
			{
				xenotypeNameLocked = !xenotypeNameLocked;
				if (xenotypeNameLocked)
				{
					SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
				}
			}
			if (Mouse.IsOver(rect11))
			{
				string text4 = "LockNameButtonDesc".Translate() + "\n\n" + (xenotypeNameLocked ? "LockNameOn" : "LockNameOff").Translate();
				TooltipHandler.TipRegion(rect11, text4);
			}
			// Presets
		}

		public void SetPreset(GeneSetPreset geneSetPresets)
		{
			xenotypeName = geneSetPresets.name;
			selectedGenes = new();
			foreach (GeneDef geneDef in geneSetPresets.geneDefs)
			{
				if (GenesInOrder.Contains(geneDef))
				{
					selectedGenes.Add(geneDef);
				}
			}
			OnGenesChanged();
		}

		protected override void DrawSearchRect(Rect rect)
		{
			base.DrawSearchRect(rect);
			if (gene.GeneSetPresets == null)
			{
				return;
			}
			if (Widgets.ButtonText(new Rect(rect.xMax - ButSize.x, rect.y, ButSize.x, ButSize.y), "Load".Translate()))
			{
				if (!gene.GeneSetPresets.NullOrEmpty())
				{
					Find.WindowStack.Add(new Dialog_ChimeraPresetsList_Load(this));
				}
				else
				{
					Messages.Message("WVC_XaG_CreateChimera_PresetsIsNull".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			if (Widgets.ButtonText(new Rect(rect.xMax - ButSize.x * 2f - 4f, rect.y, ButSize.x, ButSize.y), "Save".Translate()))
			{
				if (xenotypeName.NullOrEmpty())
				{
					Messages.Message("WVC_XaG_CreateChimera_PresetsNameMessageError".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					if (!gene.GeneSetPresets.NullOrEmpty())
					{
						foreach (GeneSetPreset geneSetPresets in gene.GeneSetPresets.ToList())
						{
							if (geneSetPresets.name == xenotypeName)
							{
								gene.GeneSetPresets.Remove(geneSetPresets);
							}
						}
					}
					else
					{
						gene.GeneSetPresets = new();
					}
					GeneSetPreset preset = new();
					preset.name = xenotypeName;
					preset.geneDefs = selectedGenes;
					gene.GeneSetPresets.Add(preset);
					Messages.Message("WVC_XaG_CreateChimera_NewPresetSaved".Translate().CapitalizeFirst(), null, MessageTypeDefOf.PositiveEvent, historical: false);
				}
			}
		}

		//private void StorageImplanterSet()
		//{
		//	if (Gene_StorageImplanter.CanStoreGenes(gene.Pawn, out Gene_StorageImplanter implanter))
		//	{
		//		implanter.SetupHolder(XenotypeDefOf.Baseliner, selectedGenes, false, null, xenotypeName?.Trim());
		//		foreach (GeneDef geneDef in selectedGenes)
		//		{
		//			gene.RemoveCollectedGene_Storage(geneDef);
		//		}
		//		Close();
		//	}
		//}

		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (subActionsDisabled)
			{
				if (Widgets.ButtonText(new Rect((rect.xMax / 2) - (ButSize.x / 2), rect.y, ButSize.x, ButSize.y), "WVC_Reset".Translate()))
				{
					selectedGenes = new();
				}
				return;
			}
			if (Widgets.ButtonText(new Rect((rect.xMax / 2) - ButSize.x, rect.y, ButSize.x, ButSize.y), "WVC_Reset".Translate()))
			{
				selectedGenes = new();
			}
			if (Widgets.ButtonText(new Rect((rect.xMax / 2), rect.y, ButSize.x, ButSize.y), chimeraApplyEater.Translate()))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(chimeraApplyEaterWarning.ToString().Translate(gene.Pawn.LabelCap), GenesEater));
			}
			//if (Widgets.ButtonText(new Rect(rect.xMax - (ButSize.x * 2), rect.y, ButSize.x, ButSize.y), "WVC_XaG_StorageImplanter_Apply".Translate()))
			//{
			//	Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_StorageImplanter_Warning".Translate(gene.Pawn.LabelCap), StorageImplanterSet));
			//}
		}

		protected override bool CanAccept()
		{
			//if (isContainer)
			//{
			//	return true;
			//}
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.Pawn))
			{
				return false;
			}
			if (ComplexityLimit < ConsumedLimit_Cpx || ArchitesLimit < ConsumedLimit_Arc)
			{
				Messages.Message("WVC_XaG_Gene_Chimera_LimitMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			foreach (GeneDef selectedGene in SelectedGenes)
			{
				if (selectedGene.prerequisite != null && !selectedGenes.Contains(selectedGene.prerequisite) && !pawnEndoGenes.Contains(selectedGene.prerequisite))
				{
					Messages.Message("MessageGeneMissingPrerequisite".Translate(selectedGene.label).CapitalizeFirst() + ": " + selectedGene.prerequisite.LabelCap, null, MessageTypeDefOf.RejectInput, historical: false);
					return false;
				}
			}
			if (overridenGenes.Any())
			{
				Messages.Message("WVC_XaG_GeneGeneticThief_ConflictingGenesMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			IntRange reqMetRange = gene.ReqMetRange;
			if (reqMetRange.TrueMin > met || reqMetRange.TrueMax < met)
			{
				Messages.Message("WVC_XaG_ChimeraBadMetabol_Message".Translate(reqMetRange.TrueMin, reqMetRange.TrueMax, met), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return true;
		}

		protected override void Accept()
		{
			if (isContainer)
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_IGeneXenogenesEditor_ApplyWarning".Translate(gene.LabelCap), SimpleChange));
				return;
			}
			if (selectedGenes.NullOrEmpty())
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_IGeneXenogenesEditor_ClearGenes".Translate(gene.LabelCap), ClearXenogenes));
			}
			else
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_IGeneXenogenesEditor_ApplyGenes".Translate(gene.Pawn.LabelCap), StartChange));
			}
		}

		public void SimpleChange()
		{
			if (gene is IGeneXenogenesContainer container)
			{
				container.ResetContainer();
			}
			foreach (GeneDef geneDef in selectedGenes)
			{
				try
				{
					gene.AddGene_Editor(geneDef);
				}
				catch (Exception arg)
				{
					Log.Error("Error during packaging in def: " + geneDef.defName + ". Reason: " + arg.Message);
				}
			}
			gene.UpdateCache();
			gene.UpdSubHediffs();
			Close();
		}

		public void StartChange()
		{
			ClearGenes(selectedGenes);
			List<GeneDef> implantedGenes = new();
			ReimplanterUtility.UnknownChimerkin(gene.Pawn);
			foreach (GeneDef geneDef in selectedGenes)
			{
				try
				{
					if (!XaG_GeneUtility.HasGene(geneDef, gene.Pawn))
					{
						gene.AddGene_Editor(geneDef);
						implantedGenes.Add(geneDef);
					}
				}
				catch (Exception arg)
				{
					Log.Error("Error during implantation in def: " + geneDef.defName + ". Reason: " + arg.Message);
				}
			}
			RemoveOverridenGenes(ref implantedGenes);
			ReimplanterUtility.PostImplantDebug(gene.Pawn);
			UpdateChimeraXenogerm(implantedGenes);
			UpdateOther();
			Close(doCloseSound: false);
		}

		private void UpdateOther()
		{
			if (gene is IGeneWithEffects geneWithEffects)
			{
				geneWithEffects.DoEffects();
			}
			if (gene is IGeneMetabolism geneMetabolism)
			{
				geneMetabolism.UpdateMetabolism();
			}
			gene.UpdSubHediffs();
		}

		public virtual void UpdateChimeraXenogerm(List<GeneDef> implantedGenes)
		{
			Hediff firstHediffOfDef = gene.Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
			if (firstHediffOfDef != null)
			{
				List<Ability> xenogenesAbilities = MiscUtility.GetXenogenesAbilities(gene.Pawn);
				foreach (Ability ability in xenogenesAbilities)
				{
					if (!ability.HasCooldown)
					{
						continue;
					}
					ability.StartCooldown(ability.def.cooldownTicksRange.RandomInRange);
				}
				//pawn.health.RemoveHediff(firstHediffOfDef);
			}
			if (implantedGenes.Empty())
			{
				return;
			}
			XaG_GeneUtility.GetBiostatsFromList(implantedGenes, out int cpx, out int met, out int _);
			int architeCount = implantedGenes.Where((geneDef) => geneDef.biostatArc != 0).ToList().Count;
			int nonArchiteCount = implantedGenes.Count - architeCount;
			int days = Mathf.Clamp(nonArchiteCount + (architeCount * 3) - met + (int)(cpx * 0.2f), 0, 999);
			int ticks = days * (gene.ReqCooldown ? 30000 : 120000);
			if (ticks < 30000 && implantedGenes.Count > 0)
			{
				ticks = 30000;
			}
			//int count = (implantedGenes.Count + 1) * 180000;
			ReimplanterUtility.XenogermReplicating_WithCustomDuration(gene.Pawn, new((int)(ticks * 0.8f), (int)(ticks * 1.1f)), firstHediffOfDef);
			// pawn.health.AddHediff(HediffDefOf.XenogermReplicating);
		}

		private void RemoveOverridenGenes(ref List<GeneDef> implantedGenes)
		{
			bool postImplantRemoveMessage = false;
			foreach (Gene gene in gene.Pawn.genes.Xenogenes.ToList())
			{
				if (gene.Overridden && gene.overriddenByGene != null)
				{
					gene.pawn.genes.RemoveGene(gene);
					if (implantedGenes.Contains(gene.def))
					{
						implantedGenes.Remove(gene.def);
					}
					postImplantRemoveMessage = true;
				}
				if (pawnEndoGenes.Contains(gene.def))
				{
					gene.pawn.genes.RemoveGene(gene);
					if (implantedGenes.Contains(gene.def))
					{
						implantedGenes.Remove(gene.def);
					}
				}
			}
			if (postImplantRemoveMessage)
			{
				Messages.Message("WVC_XaG_GeneChimera_PostImplantRemove".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}

		private void UpdateGenesInforamtion()
		{
			subActionsDisabled = gene.DisableSubActions;
			GetCustomEater();
			//selectedGenes = new();
			cachedGeneDefsInOrder = new();
			List<GeneDef> genelinedGenes = null;
			if (isContainer)
			{
				this.pawnEndoGenes = new();
				this.pawnXenoGenes = new();
			}
			else
			{
				ConvertToDefsAndGetGeneline(gene.Pawn.genes.Endogenes, out List<GeneDef> pawnEndoGenes, ref genelinedGenes);
				ConvertToDefsAndGetGeneline(gene.Pawn.genes.Xenogenes, out List<GeneDef> pawnXenoGenes, ref genelinedGenes);
				this.pawnEndoGenes = pawnEndoGenes;
				this.pawnXenoGenes = pawnXenoGenes;
			}
			if (genelinedGenes == null)
			{
				allGenes = gene.CollectedGenes;
			}
			else
			{
				allGenes = genelinedGenes;
			}
			//nonPassedGenes = new();
			//foreach (GeneDef item in allGenes)
			//{
			//	if (!item.passOnDirectly)
			//	{
			//		nonPassedGenes.Add(item);
			//	}
			//}
			selectedGenes = this.pawnXenoGenes;
			if (gene is IGeneXenogenesContainer container && container.XenotypeHolder != null)
			{
				selectedGenes = container.XenotypeHolder.genes;
			}
			UpdateSearchResults();
		}

		private void ConvertToDefsAndGetGeneline(List<Gene> pawnGenes, out List<GeneDef> geneDefs, ref List<GeneDef> genelinedGenes)
		{
			geneDefs = new();
			foreach (Gene item in pawnGenes)
			{
				geneDefs.Add(item.def);
				if (!gene.UseGeneline || item is not Gene_ChimeraGeneline geneline)
				{
					continue;
				}
				if (genelinedGenes == null)
				{
					genelinedGenes = geneline.GenelineGenes;
				}
				else
				{
					//genelinedGenes.AddRange(geneline.GenelineGenes);
					foreach (GeneDef geneDef in geneline.GenelineGenes)
					{
						if (genelinedGenes.Contains(geneDef))
						{
							continue;
						}
						genelinedGenes.Add(geneDef);
					}
				}
			}
		}

		private void ClearGenes(List<GeneDef> nonRemoveGenes = null)
		{
			try
			{
				foreach (Gene gene in gene.Pawn.genes.Xenogenes.ToList())
				{
					if (nonRemoveGenes != null && nonRemoveGenes.Contains(gene.def))
					{
						continue;
					}
					gene.pawn?.genes?.RemoveGene(gene);
				}
				if (!XaG_GeneUtility.HasGene(gene.Def, gene.Pawn))
				{
					gene.Pawn.genes.AddGene(gene.Def, false);
				}
			}
			catch
			{
				Log.Warning("Error during genes removing. Broken PostRemove() in some gene?");
			}
			//ReimplanterUtility.NotifyGenesChanged(gene.pawn);
			ReimplanterUtility.PostImplantDebug(gene.Pawn);
		}

		public void ClearXenogenes()
		{
			// gene.ClearChimeraXenogerm();
			ClearGenes();
			// XaG_GeneUtility.UpdateXenogermReplication(gene.pawn, false);
			ReimplanterUtility.PostImplantDebug(gene.Pawn);
			//if (gene is IGeneWithEffects geneWithEffects)
			//{
			//	geneWithEffects.DoEffects();
			//}
			//if (gene is IGeneMetabolism geneMetabolism)
			//{
			//	geneMetabolism.UpdateMetabolism();
			//}
			UpdateOther();
			Close(doCloseSound: false);
		}

		// private IEnumerable<string> GetWarnings()
		// {
		// if (ignoreRestrictions)
		// {
		// if (arc > 0 && eatAllSelectedGenes)
		// {
		// yield return "XenotypeBreaksLimits_Archites".Translate();
		// }
		// if (met > GeneTuning.BiostatRange.TrueMax)
		// {
		// yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX")).CapitalizeFirst();
		// }
		// else if (met < GeneTuning.BiostatRange.TrueMin)
		// {
		// yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN")).CapitalizeFirst();
		// }
		// }
		// }

		protected override void UpdateSearchResults()
		{
			quickSearchWidget.noResultsMatched = false;
			matchingGenes.Clear();
			matchingCategories.Clear();
			if (!quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (GeneDef item in GenesInOrder)
			{
				if (!selectedGenes.Contains(item))
				{
					if (quickSearchWidget.filter.Matches(item.label))
					{
						matchingGenes.Add(item);
					}
					if (quickSearchWidget.filter.Matches(item.displayCategory.label))
					{
						matchingCategories.Add(item.displayCategory);
					}
				}
			}
			quickSearchWidget.noResultsMatched = !matchingGenes.Any() && !matchingCategories.Any();
		}

	}

}
