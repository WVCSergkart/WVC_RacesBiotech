using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	//[StaticConstructorOnStartup]
	public class GeneGizmo_ResourceResurgentCells : GeneGizmo_Resource
	{

		public List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new();

		protected override Color BarColor => new ColorInt(93, 101, 126).ToColor;

		protected override Color BarHighlightColor => new ColorInt(123, 131, 156).ToColor;

		protected override bool IsDraggable => false;

		protected override string BarLabel => $"{gene.ValueForDisplay}" + "%";

		public Gene_ResurgentTotalHealing totalHealingGene = null;
		public Gene_ResurgentAgeless ageReversionGene = null;
		public Gene_ResurgentClotting woundClottingGene = null;

		public GeneGizmo_ResourceResurgentCells(Gene_Resurgent gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barhighlightColor)
			: base(gene, drainGenes, barColor, barhighlightColor)
		{
			this.gene = gene;
			this.drainGenes = drainGenes;
			// BarColor = barColor;
			// BarHighlightColor = barHighlightColor;
			ageReversionGene = gene?.ResurgentAgeless;
			woundClottingGene = gene?.ResurgentClotting;
			totalHealingGene = gene?.ResurgentTotalHealing;
		}

		//public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		//{
		//	GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		//	float num = Mathf.Repeat(Time.time, 0.85f);
		//	if (num < 0.1f)
		//	{
		//		_ = num / 0.1f;
		//	}
		//	else if (num >= 0.25f)
		//	{
		//		_ = 1f - (num - 0.25f) / 0.6f;
		//	}
		//	return result;
		//}

		protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			if (gene?.pawn?.Faction == Faction.OfPlayer && gene is Gene_Resurgent cellsGene)
			{
				if (totalHealingGene != null)
				{
					GeneSwitcher(ref headerRect, ref mouseOverElement, ref cellsGene.totalHealingAllowed, "WVC_XaG_AutoTotalHealingDesc", totalHealingGene.def);
				}
				if (ageReversionGene != null)
				{
					GeneSwitcher(ref headerRect, ref mouseOverElement, ref cellsGene.ageReversionAllowed, "WVC_XaG_AutoAgeReversionDesc", ageReversionGene.def);
				}
				if (woundClottingGene != null)
				{
					GeneSwitcher(ref headerRect, ref mouseOverElement, ref cellsGene.woundClottingAllowed, "WVC_XaG_AutoWoundClottingDesc", woundClottingGene.def);
				}
			}
			base.DrawHeader(headerRect, ref mouseOverElement);
		}

		private void GeneSwitcher(ref Rect headerRect, ref bool mouseOverElement, ref bool geneAllowed, string autoGeneDesc, Def def)
		{
			headerRect.xMax -= 24f;
			Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
			Widgets.DefIcon(rect, def);
			GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), geneAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
			if (Widgets.ButtonInvisible(rect))
			{
				geneAllowed = !geneAllowed;
				XaG_UiUtility.FlickSound(geneAllowed);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				string onOff = (geneAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
				TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + autoGeneDesc.Translate(onOff.Named("ONOFF")), 1001);
				mouseOverElement = true;
			}
		}

		protected override string GetTooltip()
		{
			tmpDrainGenes.Clear();
			string text = $"{gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}: {gene.ValueForDisplay}" + "%";
			if (!drainGenes.NullOrEmpty())
			{
				float num = 0f;
				foreach (IGeneResourceDrain drainGene in drainGenes)
				{
					if (drainGene.CanOffset)
					{
						tmpDrainGenes.Add(new Pair<IGeneResourceDrain, float>(drainGene, drainGene.ResourceLossPerDay));
						num += drainGene.ResourceLossPerDay;
					}
				}
				if (num != 0f)
				{
					string text2 = ((num < 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate());
					text = text + "\n\n" + text2 + ": " + "WVC_XaG_PercentPerDay".Translate(Mathf.Abs(gene.PostProcessValue(num))).Resolve();
					foreach (Pair<IGeneResourceDrain, float> tmpDrainGene in tmpDrainGenes)
					{
						text = text + "\n  - " + tmpDrainGene.First.DisplayLabel.CapitalizeFirst() + ": " + "WVC_XaG_PercentPerDay".Translate(gene.PostProcessValue(0f - tmpDrainGene.Second).ToStringWithSign()).Resolve();
					}
				}
			}
			if (!gene.def.resourceDescription.NullOrEmpty())
			{
				text = text + "\n\n" + gene.def.resourceDescription.Formatted(gene.pawn.Named("PAWN")).Resolve();
			}
			return text;
		}
	}

}
