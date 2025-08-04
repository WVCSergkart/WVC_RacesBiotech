using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_CreateChimera : GeneCreationDialogBase
	{
		// private int generationRequestIndex = 1;

		// private Action callback;

		public Gene_Chimera gene;

		public List<GeneDef> selectedGenes = new();

		// public string presetName;

		// private bool eatAllSelectedGenes;

		private bool? selectedCollapsed = false;

		public HashSet<GeneCategoryDef> matchingCategories = new();

		public Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

		private bool hoveredAnyGene;

		private GeneDef hoveredGene;

		// private static bool ignoreRestrictionsConfirmationSent;

		//private const int MaxCustomXenotypes = 200;

		//private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		protected override List<GeneDef> SelectedGenes => selectedGenes;

		protected override string Header => "WVC_XaG_CreateChimera".Translate().CapitalizeFirst() + " (" + ConsumedLimit + "/" + XenogenesLimit.ToString() + ")";

        protected override string AcceptButtonLabel
		{
			get
			{
				if (selectedGenes.NullOrEmpty())
				{
					return "WVC_XaG_ChimeraApply_Clear".Translate().CapitalizeFirst();
				}
				// else if (eatAllSelectedGenes)
				// {
					// return "WVC_XaG_ChimeraApply_Eat".Translate().CapitalizeFirst();
				// }
				return "WVC_XaG_ChimeraApply_Implant".Translate().CapitalizeFirst();
			}
		}

		private List<GeneDef> allGenes;

		//private List<GeneDef> eatedGenes;

		//public List<GeneDef> pawnGenes;

		private List<GeneDef> pawnEndoGenes;

		private List<GeneDef> pawnXenoGenes;

		private List<GeneDef> cachedGeneDefsInOrder;

		// public List<GeneDef> choosenGenes = new();

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

		//public Gene_StorageImplanter storageImplanter = null;
		public bool subActionsDisabled = false;

		public Dialog_CreateChimera(Gene_Chimera chimera)
        {
            // generationRequestIndex = index;
            // this.callback = callback;
            xenotypeName = string.Empty;
            gene = chimera;
            //storageImplanter = gene.pawn.genes.GetFirstGeneOfType<Gene_StorageImplanter>();
            forcePause = true;
            closeOnAccept = false;
            absorbInputAroundWindow = true;
            alwaysUseFullBiostatsTableHeight = true;
            searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
            foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefsListForReading)
            {
                collapsedCategories.Add(allDef, value: false);
            }
			//pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.GenesListForReading);
			//pawnXenoGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.Xenogenes);
			//pawnEndoGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.Endogenes);
			//allGenes = gene.CollectedGenes;
			//eatedGenes = gene.EatedGenes;
			//selectedGenes = pawnXenoGenes;
            UpdateGenesInforamtion();
            OnGenesChanged();
        }

        private void GetCustomEater()
        {
            foreach (Gene pawnGene in gene.pawn.genes.GenesListForReading)
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

        private string chimeraApplyEater = "WVC_XaG_ChimeraApply_Eat";
		private TaggedString chimeraApplyEaterWarning = "WVC_XaG_GeneGeneticThief_EatSelectedGenes";
		private IGeneCustomChimeraEater geneCustomChimeraEater;

		private void GenesEater()
		{
            if (geneCustomChimeraEater != null)
            {
                geneCustomChimeraEater.ChimeraEater(ref selectedGenes);
            }
			else
            {
                BasicEater();
			}
			if (!gene.Props.soundDefOnImplant.NullOrUndefined())
			{
				gene.Props.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(gene.pawn));
			}
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
                if (!gene.TryEatGene(geneDef))
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
			Dialog_CreateChimera.DrawGenesSections_Label(ref rect, label, ref curY, adding, ref collapsed);
            if (collapsed == true)
            {
                if (Event.current.type == EventType.Layout)
                {
                    sectionHeight = 0f;
                }
                return;
            }
            Dialog_CreateChimera.DrawGenesSection_DrawRectFast(ref rect, ref curY, sectionHeight, adding, out float num, out bool flag, out float num2, out float num3, out float b, out Rect rect3);
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
			bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
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
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		private string GeneTip(GeneDef geneDef, bool selectedSection)
		{
			string text = null;
			if (selectedSection)
			{
				if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneDef))
				{
					text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneDef));
				}
				else if (cachedOverriddenGenes.Contains(geneDef))
				{
					text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef)));
				}
				else if (randomChosenGroups.ContainsKey(geneDef))
				{
					text = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[geneDef].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
				}
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
			static string GroupInfo(GeneLeftChosenGroup group)
			{
				if (group == null)
				{
					return null;
				}
				return ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
			}
		}

		private int? cachedLimitConsumed;
		public int ConsumedLimit
		{
			get
			{
				if (!cachedLimitConsumed.HasValue)
				{
					GetLimitAndCost();
				}
				return cachedLimitConsumed.Value;
			}
		}

		private int? cachedXenogenesLimit;
		public int XenogenesLimit
		{
			get
			{
				if (!cachedXenogenesLimit.HasValue)
                {
                    GetLimitAndCost();
                }
                return cachedXenogenesLimit.Value;
			}
		}

        private void GetLimitAndCost()
        {
			int limitCost = 0;
            float genesLimit = 0;
			float genesLimitFactor = 1f;
			StatDef statDef = gene.ChimeraLimitStatDef;
            //foreach (Hediff item in gene.pawn.health.hediffSet.hediffs)
            //{
            //    gene.GetStatFromStatModifiers(statDef, item.CurStage?.statOffsets, item.CurStage?.statFactors, out float offset, out float factor);
            //    genesLimit += offset;
            //    genesLimitFactor *= factor;
            //}
            foreach (GeneDef item in pawnEndoGenes)
            {
                if (XaG_GeneUtility.ConflictWith(item, SelectedGenes))
                {
                    continue;
                }
				gene.GetStatFromStatModifiers(statDef, item.statOffsets, item.statFactors, out float offset, out float factor);
				genesLimit += offset;
				genesLimitFactor *= factor;
			}
			foreach (GeneDef item in SelectedGenes)
			{
				gene.GetStatFromStatModifiers(statDef, item.statOffsets, item.statFactors, out float offset, out float factor);
				limitCost++;
				genesLimit += offset;
				genesLimitFactor *= factor;
			}
			foreach (Gene pawnGene in gene.pawn.genes.GenesListForReading)
            {
                if (!SelectedGenes.Contains(pawnGene.def) && pawnGene.pawn.genes.Xenogenes.Contains(pawnGene))
                {
                    continue;
                }
                if (pawnGene is not Gene_ChimeraHediff chimeraHediffGene)
                {
                    continue;
                }
                if (chimeraHediffGene.ChimeraHediff != null)
				{
                    gene.GetStatFromStatModifiers(statDef, chimeraHediffGene.ChimeraHediff?.CurStage?.statOffsets, chimeraHediffGene.ChimeraHediff?.CurStage?.statFactors, out float offset, out float factor);
                    genesLimit += offset;
                    genesLimitFactor *= factor;
                }
			}
            genesLimit *= genesLimitFactor;
			cachedLimitConsumed = limitCost;
			cachedXenogenesLimit = (int)genesLimit;
        }

        protected override void OnGenesChanged()
		{
			selectedGenes.SortGeneDefs();
			base.OnGenesChanged();
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
			cachedXenogenesLimit = null;
			cachedLimitConsumed = null;
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
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		public void SetPreset(GeneSetPresets geneSetPresets)
		{
			xenotypeName = geneSetPresets.name;
			selectedGenes = new();
			foreach (GeneDef geneDef in geneSetPresets.geneDefs)
			{
				if (gene.CollectedGenes.Contains(geneDef))
				{
					selectedGenes.Add(geneDef);
				}
			}
			OnGenesChanged();
		}

		protected override void DrawSearchRect(Rect rect)
		{
			base.DrawSearchRect(rect);
			if (Widgets.ButtonText(new Rect(rect.xMax - ButSize.x, rect.y, ButSize.x, ButSize.y), "Load".Translate()))
			{
				if (!gene.geneSetPresets.NullOrEmpty())
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
					if (!gene.geneSetPresets.NullOrEmpty())
					{
						foreach (GeneSetPresets geneSetPresets in gene.geneSetPresets.ToList())
						{
							if (geneSetPresets.name == xenotypeName)
							{
								gene.geneSetPresets.Remove(geneSetPresets);
							}
						}
					}
					else
					{
						gene.geneSetPresets = new();
					}
					GeneSetPresets preset = new();
					preset.name = xenotypeName;
					preset.geneDefs = selectedGenes;
					gene.geneSetPresets.Add(preset);
					Messages.Message("WVC_XaG_CreateChimera_NewPresetSaved".Translate().CapitalizeFirst(), null, MessageTypeDefOf.PositiveEvent, historical: false);
				}
			}
			// if (!Widgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x * 2f - 4f, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadPremade".Translate()))
			// {
			// return;
			// }
			// List<FloatMenuOption> list = new List<FloatMenuOption>();
			// foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefs.OrderBy((XenotypeDef c) => 0f - c.displayPriority))
			// {
			// XenotypeDef xenotype2 = item;
			// list.Add(new FloatMenuOption(xenotype2.LabelCap, delegate
			// {
			// xenotypeName = xenotype2.label;
			// selectedGenes.Clear();
			// selectedGenes.AddRange(xenotype2.genes);
			// eatAllSelectedGenes = xenotype2.eatAllSelectedGenes;
			// OnGenesChanged();
			// ignoreRestrictions = selectedGenes.Any((GeneDef g) => g.biostatArc > 0) || !WithinAcceptableBiostatLimits(showMessage: false);
			// }, xenotype2.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate(Rect r)
			// {
			// TooltipHandler.TipRegion(r, xenotype2.descriptionShort ?? xenotype2.description);
			// }));
			// }
			// Find.WindowStack.Add(new FloatMenu(list));
		}

		private void StorageImplanterSet()
		{
			if (Gene_StorageImplanter.CanStoreGenes(gene.pawn, out Gene_StorageImplanter implanter))
			{
				implanter.SetupHolder(XenotypeDefOf.Baseliner, selectedGenes, false, null, xenotypeName?.Trim());
				foreach (GeneDef geneDef in selectedGenes)
				{
					gene.RemoveCollectedGene(geneDef);
				}
				Close();
			}
		}

		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (Widgets.ButtonText(new Rect((rect.xMax / 2) - ButSize.x, rect.y, ButSize.x, ButSize.y), "WVC_Reset".Translate()))
			{
				selectedGenes = new();
			}
			if (subActionsDisabled)
            {
				return;
            }
			if (Widgets.ButtonText(new Rect((rect.xMax / 2), rect.y, ButSize.x, ButSize.y), chimeraApplyEater.Translate()))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(chimeraApplyEaterWarning.ToString().Translate(gene.pawn.LabelCap), GenesEater));
			}
			if (Widgets.ButtonText(new Rect(rect.xMax - (ButSize.x * 2), rect.y, ButSize.x, ButSize.y), "WVC_XaG_StorageImplanter_Apply".Translate()))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_StorageImplanter_Warning".Translate(gene.pawn.LabelCap), StorageImplanterSet));
			}
			// if (leftChosenGroups.Any())
			// {
			// int num = leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
			// GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups[0];
			// string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1)) : string.Empty);
			// float x2 = Text.CalcSize(text).x;
			// GUI.color = ColorLibrary.RedReadable;
			// Text.Anchor = TextAnchor.MiddleLeft;
			// Widgets.Label(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x - x2 - 4f, rect.y, x2, rect.height), text);
			// Text.Anchor = TextAnchor.UpperLeft;
			// GUI.color = Color.white;
			// }
		}

		protected override bool CanAccept()
		{
			// if (eatAllSelectedGenes)
			// {
			// return true;
			// }
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.pawn))
			{
				return false;
			}
			if (XenogenesLimit < ConsumedLimit)
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
			// if (!base.CanAccept())
			// {
				// return false;
			// }
			// if (!selectedGenes.Any())
			// {
				// Messages.Message("MessageNoSelectedGenes".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				// return false;
			// }
			// if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
			// {
				// Messages.Message("MessageTooManyCustomXenotypes", null, MessageTypeDefOf.RejectInput, historical: false);
				// return false;
			// }
			if (leftChosenGroups.Any())
			{
				Messages.Message("WVC_XaG_GeneGeneticThief_ConflictingGenesMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			//if (ReimplanterUtility.TryGetXenogermReplicatingDuration(gene.pawn, out HediffComp_Disappears hediffComp_Disappears))
			//{
			//	if (hediffComp_Disappears.disappearsAfterTicks > 10 * 60000)
			//	{
			//		Messages.Message("WVC_XaG_GeneChimera_TooBigPenaltyMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			//		return false;
			//	}
			//}
			return true;
		}

		protected override void Accept()
		{
			// IEnumerable<string> warnings = GetWarnings();
			if (selectedGenes.NullOrEmpty())
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneGeneticThief_ClearGeneSet".Translate(gene.pawn.LabelCap), ClearXenogenes));
			}
			else
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneGeneticThief_ImplantGeneSet".Translate(gene.pawn.LabelCap), StartChange));
			}
		}

		// private void AcceptInner()
		// {
			// CustomXenotype customXenotype = new CustomXenotype();
			// customXenotype.name = xenotypeName?.Trim();
			// customXenotype.genes.AddRange(selectedGenes);
			// customXenotype.eatAllSelectedGenes = eatAllSelectedGenes;
			// customXenotype.iconDef = iconDef;
			// string text = GenFile.SanitizedFileName(customXenotype.name);
			// string absPath = GenFilePaths.AbsFilePathForXenotype(text);
			// LongEventHandler.QueueLongEvent(delegate
			// {
				// GameDataSaveLoader.SaveXenotype(customXenotype, absPath);
			// }, "SavingLongEvent", doAsynchronously: false, null);
			// if (generationRequestIndex >= 0)
			// {
				// PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(generationRequestIndex);
				// generationRequest.ForcedXenotype = null;
				// generationRequest.ForcedCustomXenotype = customXenotype;
				// StartingPawnUtility.SetGenerationRequest(generationRequestIndex, generationRequest);
			// }
			// callback?.Invoke();
			// Close();
		// }

		public void StartChange()
        {
            ClearGenes(selectedGenes);
            //if (gene.Props.xenotypeDef != null)
            //{
            //	if (gene.pawn.genes.Xenotype != gene.Props.xenotypeDef)
            //	{
            //		ReimplanterUtility.SetXenotypeDirect(null, gene.pawn, gene.Props.xenotypeDef);
            //	}
            //}
            //else
            //{
            //	ReimplanterUtility.UnknownXenotype(gene.pawn);
            //}
            //if (gene.pawn.genes.Xenotype?.GetModExtension<GeneExtension_General>()?.isChimerkin != true)
            //{
            //}
            List<GeneDef> implantedGenes = new();
            ReimplanterUtility.UnknownChimerkin(gene.pawn);
            foreach (GeneDef geneDef in selectedGenes)
            {
                if (!XaG_GeneUtility.HasGene(geneDef, gene.pawn))
                {
                    gene.ImplantGene(geneDef);
                    implantedGenes.Add(geneDef);
                }
            }
            RemoveOverridenGenes(ref implantedGenes);
            ReimplanterUtility.PostImplantDebug(gene.pawn);
            gene.UpdateChimeraXenogerm(implantedGenes);
            gene.DoEffects();
            gene.UpdateMetabolism();
			gene.UpdSubHediffs();
			Close(doCloseSound: false);
        }

        private void RemoveOverridenGenes(ref List<GeneDef> implantedGenes)
        {
            bool postImplantRemoveMessage = false;
            foreach (Gene gene in gene.pawn.genes.Xenogenes.ToList())
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
			subActionsDisabled = gene.EaterDisabled;
			GetCustomEater();
			//selectedGenes = new();
			cachedGeneDefsInOrder = new();
			List<GeneDef> genelinedGenes = null;
			ConvertToDefsAndGetGeneline(gene.pawn.genes.Endogenes, out List<GeneDef> pawnEndoGenes, ref genelinedGenes);
			this.pawnEndoGenes = pawnEndoGenes;
			ConvertToDefsAndGetGeneline(gene.pawn.genes.Xenogenes, out List<GeneDef> pawnXenoGenes, ref genelinedGenes);
			this.pawnXenoGenes = pawnXenoGenes;
			if (genelinedGenes == null)
			{
				allGenes = gene.CollectedGenes;
			}
			else
			{
				allGenes = genelinedGenes;
			}
            //eatedGenes = gene.EatedGenes;
            selectedGenes = this.pawnXenoGenes;
			UpdateSearchResults();
        }

		private void ConvertToDefsAndGetGeneline(List<Gene> pawnGenes, out List<GeneDef> geneDefs, ref List<GeneDef> genelinedGenes)
        {
			geneDefs = new();
			foreach (Gene item in pawnGenes)
			{
				geneDefs.Add(item.def);
				if (item is Gene_ChimeraGeneline geneline)
				{
					if (genelinedGenes == null)
					{
						genelinedGenes = geneline.GenelineGenes;
					}
					else
					{
						genelinedGenes.AddRange(geneline.GenelineGenes);
					}
				}
			}
		}

        private void ClearGenes(List<GeneDef> nonRemoveGenes = null)
		{
			foreach (Gene gene in gene.pawn.genes.Xenogenes.ToList())
			{
				if (nonRemoveGenes != null && nonRemoveGenes.Contains(gene.def))
				{
					continue;
				}
				gene.pawn?.genes?.RemoveGene(gene);
			}
			if (!XaG_GeneUtility.HasGene(gene.def, gene.pawn))
			{
				gene.pawn.genes.AddGene(gene.def, false);
			}
			//ReimplanterUtility.NotifyGenesChanged(gene.pawn);
			ReimplanterUtility.PostImplantDebug(gene.pawn);
		}

		public void ClearXenogenes()
		{
			// gene.ClearChimeraXenogerm();
			ClearGenes();
			// XaG_GeneUtility.UpdateXenogermReplication(gene.pawn, false);
			ReimplanterUtility.PostImplantDebug(gene.pawn);
			gene.DoEffects();
			gene.UpdateMetabolism();
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
