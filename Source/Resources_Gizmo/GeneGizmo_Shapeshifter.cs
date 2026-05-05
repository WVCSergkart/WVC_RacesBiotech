using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Shapeshifter : Gizmo
	{

		public Pawn pawn;

		public Gene_Shapeshifter gene;

		public override bool Visible => true;

		public virtual TaggedString GizmoTip => "WVC_XaG_ShapeshifterGizmoTip".Translate(gene.ShaperResource);

		protected virtual CachedTexture Shapeshifter_MenuIcon => XaG_UiUtility.Shapeshifter_MenuIcon;

		public GeneGizmo_Shapeshifter(Gene_Shapeshifter geneShapeshifter)
			: base()
		{
			gene = geneShapeshifter;
			pawn = gene?.pawn;
			Order = -95f;
		}

		protected float uncollapsedSize = 140;

		protected int nextRecache = 2;
		protected virtual void RecacheTick()
		{

		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			if (gene.gizmoCollapse)
			{
				Collapsed(topLeft, maxWidth);
			}
			else
			{
				Uncollapsed(topLeft, maxWidth);
			}
			return new GizmoResult(GizmoState.Clear);
		}

		public virtual void Uncollapsed(Vector2 topLeft, float maxWidth)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Button1c(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Button2c(rect5);
			// Button
			Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
			XaG_UiUtility.StyleButton_WithoutRect(rect6, pawn, gene, true);
			// Button
			//Rect rect7 = new(rect6.x + 44f, rect6.y, rect6.width, rect6.height);
			//ButtonGenesSettings(rect7);
		}

		private void Collapsed(Vector2 topLeft, float maxWidth)
		{
			Rect rect2 = LabelAndTip(topLeft, maxWidth);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Button1c(rect4);
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Button2c(rect5);
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
			Widgets.Label(rect3, gene.gizmoCollapse ? gene.def.LabelShortAdj.CapitalizeFirst() : gene.LabelCap);
			if (Mouse.IsOver(rect3))
			{
				TooltipHandler.TipRegion(rect3, gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + GizmoTip);
			}
			// Collapse button
			XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
			return rect2;
		}

		// Shapeshifter menu
		protected virtual void Button1c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, Shapeshifter_MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_Shapeshifter(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneShapeshifter_Desc".Translate(gene.Label));
		}

		// Shaper menu
		protected virtual void Button2c(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, XaG_UiUtility.Shapeshifter_BonusGenesIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_ShaperEditor(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_ShapeshifterGenesImplant_Desc".Translate(gene.ShaperResource));
		}

		//protected void ButtonGenesSettings(Rect rect4)
		//{
		//	Widgets.DrawTextureFitted(rect4, XaG_UiUtility.GenesSettingsIcon.Texture, 1f);
		//	if (Mouse.IsOver(rect4))
		//	{
		//		Widgets.DrawHighlight(rect4);
		//		if (Widgets.ButtonInvisible(rect4))
		//		{
		//			Find.WindowStack.Add(new Dialog_GenesSettings(pawn));
		//		}
		//	}
		//	TooltipHandler.TipRegion(rect4, "WVC_XaG_GenesSettingsDesc".Translate());
		//}

		public override float GetWidth(float maxWidth)
		{
			if (gene.gizmoCollapse)
			{
				return 96f;
			}
			return uncollapsedSize;
		}

	}

}
