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

        private void Collapsed(Vector2 topLeft, float maxWidth)
        {
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out _, out _);
            Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
            Button1(rect4);
            // Button
            Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
            Button2(rect5);
        }

        private void LabelAndDesc(Vector2 topLeft, float maxWidth, out Rect rect2, out TaggedString taggedString, out Rect rect3)
        {
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            taggedString = "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + "WVC_XaG_Gene_Voidlink_GizmoResourceLabel".Translate(gene.CurrentResource * 100, gene.MaxResource * 100, gene.ResourcePerDay) + "\n\n" + "WVC_XaG_Gene_Voidlink_GizmoTip".Translate(gene.MaxMechs, gene.AllMechsCount, gene.SphereChance * 100);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate());
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
            //Widgets.Label(rect3, gene.ResourcePerDay);
            TooltipHandler.TipRegion(rect3, taggedString);
        }

        private void Button1(Rect rect4)
        {
            Widgets.DrawTextureFitted(rect4, KillMechsIcon.Texture, 1f);
            if (Mouse.IsOver(rect4))
            {
                Widgets.DrawHighlight(rect4);
                if (Widgets.ButtonInvisible(rect4))
                {
                    Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Gene_VoidlinkKillAllVoidMechsWarning".Translate(), delegate
                    {
                        gene.KillMechs(true);
                    });
                    Find.WindowStack.Add(window);
                }
            }
            TooltipHandler.TipRegion(rect4, "WVC_XaG_Gene_VoidlinkKillAllVoidMechs".Translate());
        }

        private void Button2(Rect rect5)
        {
            Widgets.DrawTextureFitted(rect5, SummonIcon.Texture, 1f);
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
                if (Widgets.ButtonInvisible(rect5))
                {
                    Find.WindowStack.Add(new Dialog_Voidlink(gene));
                }
            }
            TooltipHandler.TipRegion(rect5, "WVC_XaG_Gene_VoidlinkSummonSettings_Desc".Translate());
        }

        private void Uncollapsed(Vector2 topLeft, float maxWidth)
        {
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out TaggedString taggedString, out Rect rect3);
            Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
            TooltipHandler.TipRegion(rect4, taggedString);
            // Button
            Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
            Button1(rectSummonSettings);
            // Button
            Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
            Button2(rectGolemsSettings);
            // Bonds
            Rect barRect = new(rect4.x, rect4.y + 3f, rect4.width - 4f, rect4.height - 9f);
            Widgets.FillableBar(barRect, gene.ResourcePercent, BarTex, EmptyBarTex, doBorder: true);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(barRect, gene.ResourceForDisplay.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override float GetWidth(float maxWidth)
        {
            if (gene.gizmoCollapse)
            {
                return 96f;
            }
            return 220f;
		}

	}

}
