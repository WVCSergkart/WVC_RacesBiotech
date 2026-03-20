using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Dialog_StylingEyesGene : Dialog_StylingGene
	{

		public Gene_Eyes gene_Eyes;
		public StyleGeneDef initialHoloface;
		public Color initialColor;

		public Dialog_StylingEyesGene(Pawn pawn, Gene gene, bool unlockTattoos) : base(pawn, gene, unlockTattoos)
		{
			if (gene is Gene_Eyes fungi)
			{
				gene_Eyes = fungi;
				initialHoloface = fungi.CurrentTextID;
				initialColor = fungi.CurrentColor;
			}
		}

		public override List<Color> AllHairColors
		{
			get
			{
				if (allHairColors == null)
				{
					allHairColors = new List<Color>();
					foreach (GeneralHolder colorHolder in gene_Eyes.ColorHolder)
					{
						if (allHairColors.Contains(colorHolder.color))
						{
							continue;
						}
						allHairColors.Add(colorHolder.color);
					}
					foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefsListForReading)
					{
						if (allHairColors.Contains(allDef.color))
						{
							continue;
						}
						allHairColors.Add(allDef.color);
					}
					allHairColors.SortByColor((Color x) => x);
				}
				return allHairColors;
			}
		}

		public override void DrawTabs(Rect rect)
		{
			tabs.Clear();
			tabs.Add(new TabRecord("WVC_EyesColor".Translate().CapitalizeFirst(), delegate
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
					DrawHairColors(new Rect(rect.x, rect.y, rect.width, colorsHeight));
					break;
			}
		}

		public override void DrawHairColors(Rect rect)
		{
			float y = rect.y;
			Widgets.ColorSelector(new Rect(rect.x, y, rect.width, colorsHeight), ref gene_Eyes.color, AllHairColors, out colorsHeight);
			colorsHeight += Text.LineHeight * 2f;
		}

		public override void Reset(bool resetColors = true)
		{
			gene_Eyes.color = initialColor;
			gene_Eyes.CurrentTextID = initialHoloface;
			//Close();
		}

		public override void Accept()
		{
			//gene_FungoidHair.color = desiredHairColor;
			Close();
		}

	}

}
