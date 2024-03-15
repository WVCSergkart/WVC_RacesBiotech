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

		public Gene_ResurgentTotalHealing totalHealingGene;
		public Gene_ResurgentAgeless ageReversionGene;
		public Gene_ResurgentClotting woundClottingGene;

		public GeneGizmo_ResourceResurgentCells(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barhighlightColor)
			: base(gene, drainGenes, barColor, barhighlightColor)
		{
			ageReversionGene = gene?.pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentAgeless>();
			woundClottingGene = gene?.pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentClotting>();
			totalHealingGene = gene?.pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentTotalHealing>();
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
			float num = Mathf.Repeat(Time.time, 0.85f);
			if (num < 0.1f)
			{
				_ = num / 0.1f;
			}
			else if (num >= 0.25f)
			{
				_ = 1f - (num - 0.25f) / 0.6f;
			}
			return result;
		}

		protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
		{
			if (gene?.pawn?.Faction == Faction.OfPlayer && gene is Gene_ResurgentCells hemogenGene)
			{
				if (totalHealingGene != null)
				{
					headerRect.xMax -= 24f;
					Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
					Widgets.DefIcon(rect, totalHealingGene.def);
					GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.totalHealingAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
					if (Widgets.ButtonInvisible(rect))
					{
						hemogenGene.totalHealingAllowed = !hemogenGene.totalHealingAllowed;
						if (hemogenGene.totalHealingAllowed)
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
						}
						else
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						}
					}
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						string onOff = (hemogenGene.totalHealingAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
						TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoTotalHealingDesc".Translate(onOff.Named("ONOFF")), 1001);
						mouseOverElement = true;
					}
				}
				if (ageReversionGene != null)
				{
					headerRect.xMax -= 24f;
					Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
					Widgets.DefIcon(rect, ageReversionGene.def);
					GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.ageReversionAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
					if (Widgets.ButtonInvisible(rect))
					{
						hemogenGene.ageReversionAllowed = !hemogenGene.ageReversionAllowed;
						if (hemogenGene.ageReversionAllowed)
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
						}
						else
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						}
					}
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						string onOff = (hemogenGene.ageReversionAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
						TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoAgeReversionDesc".Translate(onOff.Named("ONOFF")), 1001);
						mouseOverElement = true;
					}
				}
				if (woundClottingGene != null)
				{
					headerRect.xMax -= 24f;
					Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
					Widgets.DefIcon(rect, woundClottingGene.def);
					GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.woundClottingAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
					if (Widgets.ButtonInvisible(rect))
					{
						hemogenGene.woundClottingAllowed = !hemogenGene.woundClottingAllowed;
						if (hemogenGene.woundClottingAllowed)
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
						}
						else
						{
							SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						}
					}
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						string onOff = (hemogenGene.woundClottingAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
						TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoWoundClottingDesc".Translate(onOff.Named("ONOFF")), 1001);
						mouseOverElement = true;
					}
				}
			}
			base.DrawHeader(headerRect, ref mouseOverElement);
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
