using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Dialog_Golemlink : GeneCreationDialogBase
	{

		public Gene_Golemlink gene;

		public List<PawnKindDef> selectedGolems = new();

		protected HashSet<PawnKindDef> matchingGolems = new();

		private bool? selectedCollapsed = false;

		private bool? pawnKindsCollapsed = false;

		public static readonly Vector2 XenotypeSize = new(87f, 68f);

		public override Vector2 InitialSize => new(Mathf.Min(UI.screenWidth, 1036), UI.screenHeight - 4);

		public List<PawnKindDef> SelectedGolems => selectedGolems;

		protected override List<GeneDef> SelectedGenes => new();

		protected override string Header => gene.LabelCap;

		protected override string AcceptButtonLabel
		{
			get
			{
				return "Accept".Translate();
			}
		}


		public List<PawnKindDef> allPawnKinds;

		private List<PawnKindDef> cachedPawnKindDefsInOrder;

		public List<PawnKindDef> PawnKindsInOrder
		{
			get
			{
				if (cachedPawnKindDefsInOrder == null)
				{
					cachedPawnKindDefsInOrder = new();
					foreach (PawnKindDef allDef in allPawnKinds)
					{
						cachedPawnKindDefsInOrder.Add(allDef);
					}
					// cachedXenotypeDefsInOrder.SortBy((PawnKindDef x) => 0f - x.displayPriority - (x.inheritable ? 100000f : 0));
				}
				return cachedPawnKindDefsInOrder;
			}
		}


		public Dialog_Golemlink(Gene_Golemlink thisGene)
		{
			gene = thisGene;
			allPawnKinds = ListsUtility.GetAllGolemPawnkinds();
			// selectedXeno = allXenotypes.RandomElement();
			selectedGolems = new();
			if (!gene.golemsForSummon.NullOrEmpty())
			{
				selectedGolems.AddRange(gene.golemsForSummon);
			}
			else
			{
				selectedGolems.AddRange(allPawnKinds);
			}
			forcePause = true;
			closeOnAccept = false;
			absorbInputAroundWindow = true;
			alwaysUseFullBiostatsTableHeight = true;
			searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
		}

		protected override void DrawGenes(Rect rect)
		{
			GUI.BeginGroup(rect);
			float curY = 0f;
			DrawPawnKindSection(new Rect(0f, 0f, rect.width, selectedHeight), SelectedGolems, "WVC_XaG_DialogGolemlink_SelectedGolemnoids".Translate(), ref curY, ref selectedHeight, rect, ref selectedCollapsed, true);
			Widgets.Label(0f, ref curY, rect.width, "WVC_XaG_DialogGolemlink_Golemnoids".Translate().CapitalizeFirst());
			curY += 10f;
			float num2 = curY;
			Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
			Widgets.BeginScrollView(new Rect(0f, curY, rect.width, rect.height - curY), ref scrollPosition, rect2);
			Rect containingRect = rect2;
			containingRect.y = curY + scrollPosition.y;
			containingRect.height = rect.height;
			DrawPawnKindSection(rect, PawnKindsInOrder, null, ref curY, ref unselectedHeight, containingRect, ref pawnKindsCollapsed);
			if (Event.current.type == EventType.Layout)
			{
				scrollHeight = curY - num2;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
		}

		private void DrawPawnKindSection(Rect rect, List<PawnKindDef> pawnKinds, string label, ref float curY, ref float sectionHeight, Rect containingRect, ref bool? collapsed, bool ignoreFilter = false)
		{
			float curX = 4f;
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
				rect2.xMax -= Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f;
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
			if (!pawnKinds.Any())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
			}
			else
			{
				for (int i = 0; i < pawnKinds.Count; i++)
				{
					PawnKindDef pawnKindDef = pawnKinds[i];
					if (!ignoreFilter && quickSearchWidget.filter.Active && !matchingGolems.Contains(pawnKindDef))
					{
						continue;
					}
					if (curX + num2 > num3)
					{
						curX = 4f;
						curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					}
					curX = Mathf.Max(curX, b);
					if (DrawPawnKinds(pawnKindDef, ref curX, curY, num2, containingRect))
					{
						if (selectedGolems.Contains(pawnKindDef))
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
							selectedGolems.Remove(pawnKindDef);
						}
						else
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
							selectedGolems.Add(pawnKindDef);
						}
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


		private bool DrawPawnKinds(PawnKindDef pawnKindDef, ref float curX, float curY, float packWidth, Rect containingRect)
		{
			bool result = false;
			Rect rect = new(curX, curY, packWidth, XenotypeSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = selectedGolems.Contains(pawnKindDef);
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			DrawGolemstats(pawnKindDef.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost), ref curX, curY, 4f);
			Rect xenoRect = new(curX, curY + 4f, XenotypeSize.x, XenotypeSize.y);
			DrawXenotypeBasics(pawnKindDef, xenoRect, false);
			if (Mouse.IsOver(xenoRect))
			{
				string text = pawnKindDef.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + pawnKindDef.race.description;
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

		public static readonly CachedTexture GolembondTex = new("WVC/UI/XaG_General/GolembondTex_v0");

		public static readonly CachedTexture Background = new("WVC/UI/XaG_General/Ui_BackgroundGolems_v0");

		public static void DrawXenotypeBasics(PawnKindDef pawnKindDef, Rect geneRect, bool overridden)
		{
			GUI.BeginGroup(geneRect);
			Rect rect = geneRect.AtZero();
			float num = rect.width - Text.LineHeight;
			Rect rect2 = new(geneRect.width / 2f - num / 2f, 0f, num, num);
			Color iconColor = new(0.6f, 0.6f, 0.6f);
			if (overridden)
			{
				iconColor.a = 0.5f;
				GUI.color = ColoredText.SubtleGrayColor;
			}
			CachedTexture cachedTexture = Background;
			GUI.DrawTexture(rect2, cachedTexture.Texture);
			Widgets.DefIcon(rect2, pawnKindDef, null, 0.9f, null, drawPlaceholder: false, iconColor);
			Text.Font = GameFont.Tiny;
			float num2 = Text.CalcHeight(pawnKindDef.LabelCap, rect.width);
			Rect rect3 = new(0f, rect.yMax - num2, rect.width, num2);
			GUI.DrawTexture(new(rect3.x, rect3.yMax - num2, rect3.width, num2), TexUI.GrayTextBG);
			Text.Anchor = TextAnchor.LowerCenter;
			Widgets.Label(rect3, pawnKindDef.LabelCap);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
			GUI.EndGroup();
		}

		public static void DrawGolemstats(float golembond, ref float curX, float curY, float margin = 6f)
		{
			float num2 = 0f;
			float baseWidthOffset = 38f;
			float num3 = Text.LineHeightOf(GameFont.Small);
			Rect iconRect = new(curX, curY + margin + num2, num3, num3);
			DrawStat(iconRect, GolembondTex, golembond.ToString(), num3);
			Rect rect = new(curX, iconRect.y, baseWidthOffset, num3);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, WVC_GenesDefOf.WVC_GolemBondCost.label.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + WVC_GenesDefOf.WVC_GolemBondCost.description);
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
			gene.golemsForSummon = selectedGolems;
			Close(doCloseSound: true);
		}

		protected override void UpdateSearchResults()
		{
			quickSearchWidget.noResultsMatched = false;
			matchingGolems.Clear();
			if (!quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (PawnKindDef item in PawnKindsInOrder)
			{
				if (quickSearchWidget.filter.Matches(item.label))
				{
					matchingGolems.Add(item);
				}
			}
			quickSearchWidget.noResultsMatched = !matchingGolems.Any();
		}

	}

}