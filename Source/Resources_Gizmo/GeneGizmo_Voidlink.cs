using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Voidlink : Gizmo
	{

		private static readonly CachedTexture KillMechsIcon = new("WVC/UI/XaG_General/UI_KillMechs_Gizmos_v0");

		private static readonly CachedTexture SummonIcon = new("WVC/UI/XaG_General/UI_VoidlinkSummon_Gizmos_v0");

		public Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(93, 101, 126).ToColor);
		public Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

		public Pawn mechanitor;

		public Gene_Voidlink gene;

		public override bool Visible => true;

		public GeneGizmo_Voidlink(Gene_Voidlink geneMechlink)
			: base()
		{
			gene = geneMechlink;
			mechanitor = gene?.pawn;
			Order = -90;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			TaggedString taggedString = "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_Gene_Voidlink_GizmoTip".Translate(gene.MaxMechs);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate());
			TooltipHandler.TipRegion(rect3, taggedString);
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
			TooltipHandler.TipRegion(rect4, taggedString);
			// Button
			Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rectSummonSettings, KillMechsIcon.Texture, 1f);
			if (Mouse.IsOver(rectSummonSettings))
			{
				Widgets.DrawHighlight(rectSummonSettings);
				if (Widgets.ButtonInvisible(rectSummonSettings))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Gene_VoidlinkKillAllVoidMechsWarning".Translate(), delegate
					{
						gene.KillMechs(true);
					});
					Find.WindowStack.Add(window);
				}
			}
			TooltipHandler.TipRegion(rectSummonSettings, "WVC_XaG_Gene_VoidlinkKillAllVoidMechs".Translate());
			// Button
			Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
			Widgets.DrawTextureFitted(rectGolemsSettings, SummonIcon.Texture, 1f);
			if (Mouse.IsOver(rectGolemsSettings))
			{
				Widgets.DrawHighlight(rectGolemsSettings);
				if (Widgets.ButtonInvisible(rectGolemsSettings))
				{
					Find.WindowStack.Add(new Dialog_Voidlink(gene));
				}
			}
			TooltipHandler.TipRegion(rectGolemsSettings, "WVC_XaG_Gene_VoidlinkSummonSettings_Desc".Translate());
			// Bonds
			Rect barRect = new(rect4.x, rect4.y + 3f, rect4.width - 4f, rect4.height - 9f);
			Widgets.FillableBar(barRect, gene.ResourcePercent, BarTex, EmptyBarTex, doBorder: true);
			Text.Anchor = TextAnchor.MiddleCenter;
			Widgets.Label(barRect, gene.ResourceForDisplay.ToString());
			Text.Anchor = TextAnchor.UpperLeft;
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			return 220f;
		}

	}

}
