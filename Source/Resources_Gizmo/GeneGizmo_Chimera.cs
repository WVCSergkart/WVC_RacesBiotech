using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Chimera : Gizmo
	{

		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Chimera_GizmoMenu");

		public Pawn pawn;

		public Gene_Chimera gene;

		public override bool Visible => true;

		public GeneGizmo_Chimera(Gene_Chimera geneChimera)
			: base()
		{
			gene = geneChimera;
			pawn = gene?.pawn;
			Order = -94f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (gene.gizmoCollapse)
            {
                Basic(topLeft, maxWidth, out Rect rect2, out Rect rect3);
                // Button
                Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
                EditorButton(rect4);
            }
            else
            {
                Basic(topLeft, maxWidth, out Rect rect2, out Rect rect3);
                Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 44f, rect2.height - rect3.height - 6f);
                EditorButton(rect4);
                XaG_UiUtility.StyleButton_WithoutRect(new(rect4.x + 88f, rect4.y, 41f, 37f), pawn, gene, false);
            }
            return new GizmoResult(GizmoState.Clear);
        }

        private void Basic(Vector2 topLeft, float maxWidth, out Rect rect2, out Rect rect3)
        {
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            TaggedString taggedString = "WVC_XaG_Gene_Chimera_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_Gene_Chimera_GizmoTip".Translate(gene.CollectedGenes.Count, gene.EatedGenes.Count, gene.DestroyedGenes.Count, gene.XenogenesLimit);
            TooltipHandler.TipRegion(rect, taggedString);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, "WVC_XaG_GeneChimeraGizmoLabel".Translate().CapitalizeFirst());
            // Collapse button
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
        }

        private void EditorButton(Rect rect4)
        {
            Widgets.DrawTextureFitted(rect4, MenuIcon.Texture, 1f);
            if (Mouse.IsOver(rect4))
            {
                Widgets.DrawHighlight(rect4);
                if (Widgets.ButtonInvisible(rect4))
                {
                    if (gene.CanBeUsed)
                    {
                        Find.WindowStack.Add(new Dialog_CreateChimera(gene));
                    }
                    else
                    {
                        Messages.Message("WVC_XaG_Gene_Chimera_LimitToLow".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                }
            }
            TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneGeneticThief_Desc".Translate());
        }

        public override float GetWidth(float maxWidth)
		{
			if (gene.gizmoCollapse)
			{
				return 96f;
			}
			return 140f;
		}

	}

}
