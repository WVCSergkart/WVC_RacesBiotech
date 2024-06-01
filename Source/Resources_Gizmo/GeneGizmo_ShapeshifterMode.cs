using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_ShapeshifterMode : Gizmo
	{

		public Pawn pawn;

		public Gene_Shapeshifter gene;

		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Shapeshifter_GizmoMain");

		private static readonly CachedTexture styleIcon = new("WVC/UI/XaG_General/Shapeshifter_GizmoStyle");

		public override bool Visible => true;

		public GeneGizmo_ShapeshifterMode(Gene_Shapeshifter geneShapeshifter)
			: base()
		{
			gene = geneShapeshifter;
			pawn = gene?.pawn;
			Order = -500f;
			// if (!ModLister.CheckIdeology("Styling station"))
			// {
				// styleIcon = new("WVC/UI/XaG_General/UI_DisabledWhite");
			// }
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_ShapeshifterGizmo".Translate());
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rect4, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Find.WindowStack.Add(new Dialog_Shapeshifter(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneShapeshifter_Desc".Translate());
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Widgets.DrawTextureFitted(rect5, styleIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					if (ModLister.IdeologyInstalled)
					{
						Find.WindowStack.Add(new Dialog_StylingShift(pawn, gene));
					}
					else
					{
						Messages.Message("WVC_XaG_ReqIdeology".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
					}
				}
			}
			TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneShapeshifterStyles_Desc".Translate());
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			// return 136f;
			return 96f;
		}

	}

}
