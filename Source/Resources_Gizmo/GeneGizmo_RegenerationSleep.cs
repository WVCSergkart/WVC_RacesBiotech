using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_RegenerationSleep : Gizmo
	{

		public Pawn pawn;

		public Gene gene;

		private static readonly CachedTexture AddIcon = new("WVC/UI/XaG_General/RegenComa_Add_v0");

		private static readonly CachedTexture RemoveIcon = new("WVC/UI/XaG_General/RegenComa_Remove_v0");

		public override bool Visible => true;

		public GeneGizmo_RegenerationSleep(Gene genegene)
			: base()
		{
			gene = genegene;
			pawn = gene?.pawn;
			Order = -50f;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			// Tip
			TaggedString taggedString = "WVC_Coma".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + "\n\n" + "WVC_XaG_RegenerationSleepGizmoTip".Translate();
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			// Label
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_Coma".Translate());
			//Text.Font = GameFont.Small;
			//Text.Anchor = TextAnchor.UpperRight;
			TooltipHandler.TipRegion(rect3, taggedString);
			// Button
			Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rect4, AddIcon.Texture, 1f);
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RegenerationComa);
					if (firstHediffOfDef != null)
					{
						HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
						if (hediffComp_Disappears != null)
						{
							hediffComp_Disappears.ticksToDisappear += 360000;
						}
					}
					else
					{
						pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.RegenerationComa, pawn));
					}
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_RegenerationSleepAddHediffTip".Translate());
			// Button
			Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
			Widgets.DrawTextureFitted(rect5, RemoveIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.RegenerationComa);
					if (firstHediffOfDef != null)
					{
						HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
						if (hediffComp_Disappears != null)
						{
							hediffComp_Disappears.ticksToDisappear = 1500;
						}
					}
				}
			}
			TooltipHandler.TipRegion(rect5, "WVC_XaG_RegenerationSleepRemoveHediffTip".Translate());
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			return 96f;
		}


	}

}
