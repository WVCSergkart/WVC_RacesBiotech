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

		private static readonly CachedTexture StyleIcon = new("WVC/UI/XaG_General/Shapeshifter_GizmoStyle");

		// private static readonly CachedTexture ModeSettingsIcon = new("WVC/UI/XaG_General/ShapeshifterModeSettings");

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
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_ShapeshifterGizmo".Translate());
			// Text.Font = GameFont.Small;
			// Text.Anchor = TextAnchor.UpperRight;
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
			Widgets.DrawTextureFitted(rect5, StyleIcon.Texture, 1f);
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
			// Button
			// Rect rect6 = new(rect5.x + 44f, rect5.y, rect5.width, rect5.height);
			// Widgets.DrawTextureFitted(rect6, gene.ShiftMode.uiIcon, 1f);
			// if (Mouse.IsOver(rect6))
			// {
				// Widgets.DrawHighlight(rect6);
				// if (Widgets.ButtonInvisible(rect6))
				// {
					// List<FloatMenuOption> list = new();
					// foreach (ShapeshiftModeDef shapeshiftModeDef in DefDatabase<ShapeshiftModeDef>.AllDefsListForReading.OrderBy((ShapeshiftModeDef d) => d.uiOrder))
					// {
						// if (!shapeshiftModeDef.unlockedByDefault && !gene.UnlockedModes.Contains(shapeshiftModeDef))
						// {
							// continue;
						// }
						// FloatMenuOption floatMenuOption = new(shapeshiftModeDef.LabelCap, delegate
						// {
							// gene.SetMode(shapeshiftModeDef);
						// }, shapeshiftModeDef.uiIcon, Color.white);
						// floatMenuOption.tooltip = new TipSignal(shapeshiftModeDef.description, shapeshiftModeDef.index ^ 0xDFE8661);
						// list.Add(floatMenuOption);
					// }
					// if (list.Any())
					// {
						// Find.WindowStack.Add(new FloatMenu(list));
					// }
				// }
			// }
			// TooltipHandler.TipRegion(rect6, gene.ShiftMode.description);
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
