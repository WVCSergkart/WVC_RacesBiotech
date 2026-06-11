using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Sacrificer : Gizmo
	{

		public Pawn pawn;
		public Gene_Sacrificer gene;

		public override bool Visible => true;

		private static readonly CachedTexture EditorIcon = new("WVC/UI/XaG_General/UI_XenogenesEditorFleshmass");
		private static readonly CachedTexture RewardIcon = new("WVC/UI/XaG_General/UI_SacrificerSkullIcon");

		public virtual TaggedString GizmoTip => "WVC_XaG_SacrificerGizmoTip".Translate(gene.GenelineGenes.Count, gene.ComplexityLimit, gene.ArchiteLimit, gene.ReqMetRange.TrueMin, gene.ReqMetRange.TrueMax, gene.RewardName);

		public GeneGizmo_Sacrificer(Gene_Sacrificer gene)
			: base()
		{
			this.gene = gene;
			pawn = gene?.pawn;
			Order = -95f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Button1c(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Button2c(rect5);
			return new GizmoResult(GizmoState.Clear);
		}

		protected Rect LabelAndTip(Vector2 topLeft, float maxWidth)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Tip
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, gene.def.LabelShortAdj.CapitalizeFirst());
			if (Mouse.IsOver(rect3))
			{
				TooltipHandler.TipRegion(rect3, gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + GizmoTip);
			}
			return rect2;
		}

		// Editor
		protected virtual void Button1c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, EditorIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_XenogenesEditor(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_SacrificerEditorTip".Translate(gene.Label));
		}

		// Reward
		protected virtual void Button2c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, RewardIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					List<FloatMenuOption> list = new();
					list.Add(new FloatMenuOption("Biotech".Translate().CapitalizeFirst(), delegate
					{
						gene.SetRewardMode("biotech");
					}, orderInPriority: 1));
					if (ModsConfig.AnomalyActive)
					{
						list.Add(new FloatMenuOption("Anomaly".Translate().CapitalizeFirst(), delegate
						{
							gene.SetRewardMode("anomaly");
						}, orderInPriority: 2));
					}
					if (ModsConfig.OdysseyActive)
					{
						list.Add(new FloatMenuOption("Odyssey".Translate().CapitalizeFirst(), delegate
						{
							gene.SetRewardMode("odyssey");
						}, orderInPriority: 3));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_SacrificerRewardTip".Translate());
		}

		public override float GetWidth(float maxWidth)
		{
			return 96f;
		}

	}

}
