using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Dialog_StylingExtra : Dialog_StylingGene
	{

		public bool unlockAll;

		private List<IGeneCustomGraphic> cachedGraphicGenes;
		public List<IGeneCustomGraphic> GraphicGenes
		{
			get
			{
				if (cachedGraphicGenes == null)
				{
					List<IGeneCustomGraphic> list = new();
					foreach (Gene item in pawn.genes.GenesListForReading)
					{
						if (item is IGeneCustomGraphic geneCustom && item.Active)
						{
							list.Add(geneCustom);
						}
					}
					//cachedGraphicGenes = pawn.genes.GenesListForReading.Where((g) => g is IGeneCustomGraphic && g.Active).Select((g) => g as IGeneCustomGraphic).ToList();
					cachedGraphicGenes = list;
				}
				return cachedGraphicGenes;
			}
		}

		private bool returnable;
		public override string CloseButtonText
		{
			get
			{
				if (returnable)
				{
					return "Back".Translate();
				}
				return "CloseButton".Translate();
			}
		}

		public List<ResetCache> savedBackup;
		public class ResetCache
		{
			public IGeneCustomGraphic graphicGene;
			public StyleGeneDef initialGeneGraphic;
			public Color initialColor;
		}

		public Dialog_StylingExtra(Pawn pawn, Gene gene, bool unlockTattoos, bool unlockAll, bool newTab) : base(pawn, gene, unlockTattoos)
		{
			//graphicGene = gene as IGeneCustomGraphic;,
			//initialHoloface = graphicGene.CurrentTextID;
			//initialColor = graphicGene.CurrentColor;
			this.returnable = newTab;
			this.unlockAll = unlockAll;
			if (gene is IGeneCustomGraphic geneCustom)
			{
				SetCurrentGene(geneCustom);
			}
			else if (unlockAll && !GraphicGenes.Empty())
			{
				SetCurrentGene(GraphicGenes.First());
			}
			savedBackup = new();
			foreach (IGeneCustomGraphic item in GraphicGenes)
			{
				ResetCache backup = new()
				{
					graphicGene = item,
					initialGeneGraphic = item.StyleGeneDef,
					initialColor = item.CurrentColor
				};
				savedBackup.Add(backup);
			}
		}

		public IGeneCustomGraphic graphicGene;
		public StyleGeneDef initialGeneGraphic;
		public Color initialColor;

		public override void DrawTabs(Rect rect)
		{
			tabs.Clear();
			foreach (IGeneCustomGraphic igene in GraphicGenes)
			{
				if (!unlockAll && igene is Gene genegene && genegene != gene)
				{
					continue;
				}
				tabs.Add(new TabRecord(igene.Label.CapitalizeFirst(), delegate
				{
					SetCurrentGene(igene);
				}, graphicGene == igene));
			}
			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(rect, tabs);
			rect = rect.ContractedBy(18f);
			if (graphicGene == null)
			{
				return;
			}
			if (graphicGene.StyleId > 0)
			{
				rect.yMax -= colorsHeight;
				DrawStylingItemType(rect, ref hairScrollPosition, delegate (Rect r, StyleGeneDef h)
				{
					GUI.color = graphicGene.CurrentColor;
					Widgets.DefIcon(r, h, null, 1.25f);
					GUI.color = Color.white;
				}, delegate (StyleGeneDef h)
				{
					graphicGene.StyleGeneDef = h;
				}, (StyleItemDef h) => graphicGene.StyleGeneDef == h, (StyleItemDef h) => initialGeneGraphic == h, null, doColors: !AllHairColors.Empty(), currentStyleId: graphicGene.StyleId);
			}
			else
			{
				rect.yMax -= colorsHeight;
				DrawHairColors(new Rect(rect.x, rect.y, rect.width, colorsHeight));
			}
		}

		private void SetCurrentGene(IGeneCustomGraphic gene)
		{
			graphicGene = gene;
			if (gene != null)
			{
				initialGeneGraphic = gene.StyleGeneDef;
				initialColor = gene.CurrentColor;
				allHairColors = gene.AllColors;
			}
		}

		public override void DrawHairColors(Rect rect)
		{
			float y = rect.y;
			Color someColor = graphicGene.CurrentColor;
			Widgets.ColorSelector(new Rect(rect.x, y, rect.width, colorsHeight), ref someColor, AllHairColors, out colorsHeight);
			graphicGene.CurrentColor = someColor;
			pawn?.Drawer?.renderer?.SetAllGraphicsDirty();
			colorsHeight += Text.LineHeight * 2f;
		}

		public override void Reset(bool resetColors = true)
		{
			if (savedBackup != null)
			{
				foreach (ResetCache item in savedBackup)
				{
					foreach (IGeneCustomGraphic gene in GraphicGenes)
					{
						if (gene == item.graphicGene)
						{
							gene.CurrentColor = item.initialColor;
							gene.StyleGeneDef = item.initialGeneGraphic;
						}
					}
				}
			}
			else if (graphicGene != null)
			{
				graphicGene.CurrentColor = initialColor;
				graphicGene.StyleGeneDef = initialGeneGraphic;
			}
			//Close();
			pawn.Drawer.renderer.SetAllGraphicsDirty();
		}

		private bool AnyChanges()
		{
			foreach (ResetCache item in savedBackup)
			{
				foreach (IGeneCustomGraphic gene in GraphicGenes)
				{
					if (gene == item.graphicGene && (gene.CurrentColor != item.initialColor || gene.StyleGeneDef != item.initialGeneGraphic))
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Accept()
		{
			//gene_FungoidHair.color = desiredHairColor;
			if (gene is IGeneWithEffects effecter && AnyChanges())
			{
				effecter.DoEffects();
			}
			pawn.Drawer.renderer.SetAllGraphicsDirty();
			Close();
		}

	}

}
