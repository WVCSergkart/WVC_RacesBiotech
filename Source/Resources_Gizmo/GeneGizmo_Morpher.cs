using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Morpher : Gizmo
	{

		public Pawn pawn;

		public Gene_Morpher gene;

		private static readonly CachedTexture JobIcon = new("WVC/UI/XaG_General/MorpherFindArchites_Gizmo_v0");
		private static readonly CachedTexture MenuIcon = new("WVC/UI/XaG_General/Morpher_Gizmo_v0");

		// private static readonly CachedTexture InheritableGenesIcon = new("WVC/UI/XaG_General/UI_ShapeshifterMode_Duplicate");

		public override bool Visible => true;

		public GeneGizmo_Morpher(Gene_Morpher geneShapeshifter)
			: base()
		{
			gene = geneShapeshifter;
			pawn = gene?.pawn;
			Order = -95f;
			// if (!ModLister.CheckIdeology("Styling station"))
			// {
				// styleIcon = new("WVC/UI/XaG_General/UI_DisabledWhite");
			// }
			// if (gene.ShiftMode == null)
			// {
				// gene.Reset();
			// }
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Tip
			// string text = gene.heritableGenesSlots.ToString("F0");
			// +text
			// TaggedString taggedString = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + ": " + "\n\n";
			TaggedString taggedString = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + ": " + "\n\n" + "WVC_XaG_MorpherGizmoTip".Translate() + "\n\n" + "WVC_XaG_MorpherGizmoLimitTip".Translate(gene.FormsCount, gene.CurrentLimit);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			// Widgets.Label(rect3, "WVC_XaG_ShapeshifterGizmo".Translate());
			Widgets.Label(rect3, gene.def.LabelCap);
			//Text.Font = GameFont.Small;
			//Text.Anchor = TextAnchor.UpperRight;
			TooltipHandler.TipRegion(rect3, taggedString);
			// Widgets.Label(rect3, text);
			// Text.Anchor = TextAnchor.UpperLeft;
			// Text.Font = GameFont.Small;
			// Text.Anchor = TextAnchor.UpperRight;
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rect4, JobIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					gene.AddLimit();
					// Find.WindowStack.Add(new Dialog_Shapeshifter(gene));
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneMorpherArchitesLevel_Desc".Translate() + "\n\n" + "WVC_XaG_MorpherGizmoLimitTip".Translate(gene.FormsCount, gene.CurrentLimit));
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Widgets.DrawTextureFitted(rect5, MenuIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					if (!gene.GetGeneSets().NullOrEmpty())
					{
						Find.WindowStack.Add(new Dialog_Morpher(gene));
					}
					else
					{
						Messages.Message("WVC_XaG_ReqAnyGeneSetHolders".Translate().CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
					}
				}
			}
			TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneMorpherMenu_Desc".Translate());
			// Button
			// Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
			// Widgets.DrawTextureFitted(rect6, InheritableGenesIcon.Texture, 1f);
			// if (Mouse.IsOver(rect6))
			// {
				// Widgets.DrawHighlight(rect6);
				// if (Widgets.ButtonInvisible(rect6))
				// {
					// Find.WindowStack.Add(new Dialog_ShapeshifterHeritableGenes(gene));
				// }
			// }
			// TooltipHandler.TipRegion(rect6, "WVC_XaG_GeneShapeshifterHeritableGenes_Desc".Translate());
			// UpperButton
			// Rect rectModeButton = new(rect.x + rect.width - 52f - 6f, rect.y + 6f, 26f, 26f);
			// Widgets.DrawTextureFitted(rectModeButton, MenuIcon.Texture, 1f);
			// if (Mouse.IsOver(rectModeButton))
			// {
				// Widgets.DrawHighlight(rectModeButton);
				// if (Widgets.ButtonInvisible(rectModeButton))
				// {
				// }
			// }
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			// return 136f;
			return 96f;
			// return 140f;
		}

		// public static IEnumerable<FloatMenuOption> GetWorkModeOptions(MechanitorControlGroup controlGroup)
		// {
			// foreach (MechWorkModeDef wm in DefDatabase<MechWorkModeDef>.AllDefsListForReading.OrderBy((MechWorkModeDef d) => d.uiOrder))
			// {
				// FloatMenuOption floatMenuOption = new FloatMenuOption(wm.LabelCap, delegate
				// {
					// controlGroup.SetWorkMode(wm);
				// }, wm.uiIcon, Color.white);
				// floatMenuOption.tooltip = new TipSignal(wm.description, wm.index ^ 0xDFE8661);
				// yield return floatMenuOption;
			// }
		// }

	}

}
