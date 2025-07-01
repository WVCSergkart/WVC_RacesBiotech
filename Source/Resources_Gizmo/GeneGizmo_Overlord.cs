using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Overlord : Gizmo
	{

		//private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		//private static readonly Color FilledBlockColor = ColorLibrary.Orange;

		//private static readonly Color ExcessBlockColor = ColorLibrary.Red;

		private static readonly CachedTexture SummonSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoSummonSettings");

		private static readonly CachedTexture GolemSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoGolemSettings");

		public Pawn mechanitor;

		public Gene_Overlord gene;

		private int nextRecache = -1;

		private List<Pawn> allControlledGolems;

		public override bool Visible => true;

		public GeneGizmo_Overlord(Gene_Overlord geneMechlink)
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

        private void RecacheTick()
        {
            nextRecache--;
            if (nextRecache <= 0)
            {
                allControlledGolems = gene.UndeadsListForReading;
                nextRecache = 120;
            }
        }

        private void Button1(Rect rect4)
        {
            Widgets.DrawTextureFitted(rect4, SummonSettingsIcon.Texture, 1f);
            if (Mouse.IsOver(rect4))
            {
                Widgets.DrawHighlight(rect4);
                if (Widgets.ButtonInvisible(rect4))
                {
                    //gene.summonMechanoids = !gene.summonMechanoids;
                    //XaG_UiUtility.FlickSound(gene.summonMechanoids);
                }
            }
            TooltipHandler.TipRegion(rect4, "WVC_XaG_OverlordModeLabel".Translate() + "\n\n" + "WVC_XaG_OverlordModeDesc".Translate());
        }

        private void Button2(Rect rect5)
        {
            Widgets.DrawTextureFitted(rect5, GolemSettingsIcon.Texture, 1f);
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
                if (Widgets.ButtonInvisible(rect5))
                {
                    //Find.WindowStack.Add(new Dialog_Golemlink(gene));
                }
            }
            TooltipHandler.TipRegion(rect5, "WVC_XaG_OverlordButton2".Translate());
        }

        private void Uncollapsed(Vector2 topLeft, float maxWidth)
        {
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out TaggedString taggedString, out Rect rect3);
            Text.Anchor = TextAnchor.UpperRight;
            Rect totalLabelRect = new(rect3.x - rect3.height, rect3.y, rect3.width, rect3.height);
            Widgets.Label(totalLabelRect, gene.LabelCap + ": " + allControlledGolems.Count);
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
            TooltipHandler.TipRegion(rect4, taggedString);
            // Button
            Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
            Button1(rectSummonSettings);
            // Button
            Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
            Button2(rectGolemsSettings);
        }

        private void LabelAndDesc(Vector2 topLeft, float maxWidth, out Rect rect2, out TaggedString taggedString, out Rect rect3)
        {
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            RecacheTick();
            taggedString = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + ": " + allControlledGolems.Count + "\n\n" + "WVC_XaG_OverlordGizmoTip".Translate();
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, gene.LabelCap);
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
            TooltipHandler.TipRegion(rect3, taggedString);
        }

        public override float GetWidth(float maxWidth)
		{
            if (gene.gizmoCollapse)
            {
                return 96f;
            }
			// return 136f;
			return 220f;
		}

	}

}
