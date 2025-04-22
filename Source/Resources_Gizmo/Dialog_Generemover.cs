using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	//public class Dialog_Generemover_Simple : Window
	//{

	//	public List<Gene> genes;
	//	public Gene ignoredGene;

	//	public Dialog_Generemover_Simple(Pawn pawn, Gene ignoredGene = null)
	//	{
	//		this.ignoredGene = ignoredGene;
	//		this.genes = pawn.genes.GenesListForReading;
	//		forcePause = true;
	//		doCloseButton = true;
	//	}

	//	protected Vector2 scrollPosition;
	//	protected float bottomAreaHeight;

	//	public override void DoWindowContents(Rect inRect)
	//	{
	//		Vector2 vector = new(inRect.width - 16f, 40f);
	//		float y = vector.y;
	//		float height = (float)genes.Count * y;
	//		Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
	//		float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
	//		Rect outRect = inRect.TopPartPixels(num);
	//		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
	//		float num2 = 0f;
	//		int num3 = 0;
	//		foreach (Gene gene in genes.ToList())
	//		{
	//			if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
	//			{
	//				Rect rect = new(0f, num2, vector.x, vector.y);
	//				TooltipHandler.TipRegion(rect, gene.def.description);
	//				if (num3 % 2 == 0)
	//				{
	//					Widgets.DrawAltRect(rect);
	//				}
	//				Widgets.BeginGroup(rect);
	//				GUI.color = Color.white;
	//				Text.Font = GameFont.Small;
	//				Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
	//				if (Widgets.ButtonText(rect3, "Remove".Translate()))
	//				{
	//					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneRemover_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
	//					{
	//						gene.pawn.genes.RemoveGene(gene);
	//						gene.pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>()?.TryOffsetResource(gene);
	//						genes = gene.pawn.genes.GenesListForReading;
	//						if (gene == ignoredGene)
	//						{
	//							Close();
	//						}
	//						ReimplanterUtility.PostImplantDebug(gene.pawn);
	//					});
	//					Find.WindowStack.Add(window);
	//				}
	//				Rect rect4 = new(40f, 0f, 200f, rect.height);
	//				Text.Anchor = TextAnchor.MiddleLeft;
	//				Widgets.Label(rect4, gene.LabelCap.Truncate(rect4.width * 1.8f));
	//				Text.Anchor = TextAnchor.UpperLeft;
	//				Rect rect5 = new(0f, 0f, 36f, 36f);
	//				XaG_UiUtility.XaG_DefIcon(rect5, gene.def, 1.2f);
	//				Widgets.EndGroup();
	//			}
	//			num2 += vector.y;
	//			num3++;
	//		}
	//		Widgets.EndScrollView();
	//	}

	//}

	public class Dialog_Generemover : GeneCreationDialogBase
	{

		public Gene gene;

		public List<GeneDef> selectedGenes = new();

		private bool? selectedCollapsed = false;

		public HashSet<GeneCategoryDef> matchingCategories = new();

		public Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

		private bool hoveredAnyGene;

		private GeneDef hoveredGene;

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		protected override List<GeneDef> SelectedGenes => selectedGenes;

		protected override string Header => gene.LabelCap;

		protected override string AcceptButtonLabel
		{
			get
			{
				return "Remove".Translate().CapitalizeFirst();
			}
		}

		public List<GeneDef> allGenes;

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
						if (allDef == gene.def)
                        {
							continue;
                        }
						cachedGeneDefsInOrder.Add(allDef);
					}
					cachedGeneDefsInOrder.SortBy((GeneDef x) => 0f - x.displayCategory.displayPriorityInXenotype, (GeneDef x) => x.displayCategory.label, (GeneDef x) => x.displayOrderInCategory);
				}
				return cachedGeneDefsInOrder;
			}
		}

		public List<Gene> genes;

		public Dialog_Generemover(Gene ignoredGene)
		{
			//xenotypeName = string.Empty;
			gene = ignoredGene;
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefsListForReading)
			{
				collapsedCategories.Add(allDef, value: false);
			}
			UpdateGenesInforamtion();
			OnGenesChanged();
		}

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
			GeneUIUtility.DrawGeneDef(geneRect: new(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y), gene: geneDef, geneType: GeneType.Xenogene, extraTooltip: () => GeneTip(geneDef), doBackground: false, clickable: false, overridden: overridden);
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

		private string GeneTip(GeneDef geneDef)
		{
			return (selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
		}

		protected override void OnGenesChanged()
		{
			selectedGenes.SortGeneDefs();
			base.OnGenesChanged();
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
			Rect rect12 = rect;
			rect12.yMin = rect12.yMax - ButSize.y;
			DoBottomButtons(rect12);
		}

		protected override bool CanAccept()
		{
			if (selectedGenes.NullOrEmpty())
			{
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				return false;
            }
			if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.pawn))
			{
				return false;
			}
			return true;
		}

		protected override void Accept()
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_GeneRemover_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), ClearGenes));
		}

		public void ClearGenes()
		{
			try
			{
				Gene_Shapeshifter gene_Shapeshifter = gene.pawn.genes.GetFirstGeneOfType<Gene_Shapeshifter>();
				foreach (Gene item in gene.pawn.genes.GenesListForReading)
				{
					if (selectedGenes.Contains(item.def))
					{
						item.pawn.genes.RemoveGene(item);
						gene_Shapeshifter?.TryOffsetResource(item);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed remove gene. Reason: " + arg);
			}
			if (gene is IGeneWithEffects effecter)
            {
				effecter.DoEffects();
			}
			ReimplanterUtility.PostImplantDebug(gene.pawn);
			ReimplanterUtility.TrySetSkinAndHairGenes(gene.pawn);
			UpdateGenesInforamtion();
			Close();
		}

		private void UpdateGenesInforamtion()
		{
			cachedGeneDefsInOrder = new();
			allGenes = new();
			foreach (GeneDef item in XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.GenesListForReading))
            {
				if (!allGenes.Contains(item))
				{
					allGenes.Add(item);
				}
			}
			UpdateSearchResults();
		}

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
