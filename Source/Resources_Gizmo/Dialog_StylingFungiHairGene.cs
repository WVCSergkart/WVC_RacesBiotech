using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Dialog_StylingFungiHairGene : Dialog_StylingGene
	{

		public Gene_FungoidHair gene_FungoidHair;
		public StyleGeneDef initialFungiHair;
		public Color initialColor;

		public override List<Color> AllHairColors
		{
			get
			{
				if (allHairColors == null)
				{
					allHairColors = new List<Color>();
					foreach (GeneralHolder colorHolder in gene_FungoidHair.ColorHolder)
					{
						if (allHairColors.Contains(colorHolder.color))
						{
							continue;
						}
						allHairColors.Add(colorHolder.color);
					}
					foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefsListForReading)
					{
						Color color = allDef.color;
						if (allDef.displayInStylingStationUI && !allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
						{
							allHairColors.Add(color);
						}
					}
					foreach (GeneDef allDef in DefDatabase<GeneDef>.AllDefsListForReading)
					{
						if (!allDef.hairColorOverride.HasValue)
						{
							continue;
						}
						Color color = allDef.hairColorOverride.Value;
						if (!allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
						{
							allHairColors.Add(color);
						}
					}
					allHairColors.SortByColor((Color x) => x);
				}
				return allHairColors;
			}
		}

		public Dialog_StylingFungiHairGene(Pawn pawn, Gene gene, bool unlockTattoos) : base(pawn, gene, unlockTattoos)
		{
			if (gene is Gene_FungoidHair fungi)
			{
				gene_FungoidHair = fungi;
				initialFungiHair = fungi.CurrentTextID;
				initialColor = fungi.CurrentColor;
			}
		}

		public override void DrawTabs(Rect rect)
		{
			tabs.Clear();
			tabs.Add(new TabRecord("WVC_FungalHair".Translate().CapitalizeFirst(), delegate
			{
				curTab = StylingTab.Hair;
			}, curTab == StylingTab.Hair));
			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(rect, tabs);
			rect = rect.ContractedBy(18f);
			switch (curTab)
			{
				case StylingTab.Hair:
					rect.yMax -= colorsHeight;
					DrawStylingItemType(rect, ref hairScrollPosition, delegate (Rect r, FungiHairDef h)
					{
						GUI.color = gene_FungoidHair.color;
						Widgets.DefIcon(r, h, null, 1.25f);
						GUI.color = Color.white;
					}, delegate (FungiHairDef h)
					{
						gene_FungoidHair.CurrentTextID = h;
					}, (StyleItemDef h) => gene_FungoidHair.CurrentTextID == h, (StyleItemDef h) => initialFungiHair == h, null, doColors: true);
					break;
			}
		}

		public override void DrawHairColors(Rect rect)
		{
			float y = rect.y;
			Widgets.ColorSelector(new Rect(rect.x, y, rect.width, colorsHeight), ref gene_FungoidHair.color, AllHairColors, out colorsHeight);
			colorsHeight += Text.LineHeight * 2f;
		}

		public override void Reset(bool resetColors = true)
		{
			gene_FungoidHair.color = initialColor;
			gene_FungoidHair.CurrentTextID = initialFungiHair;
			//Close();
		}

		public override void Accept()
		{
			//gene_FungoidHair.color = desiredHairColor;
			Close();
		}

	}

}
