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

		//private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		//private static readonly Color FilledBlockColor = ColorLibrary.Orange;

		//private static readonly Color ExcessBlockColor = ColorLibrary.Red;

		private static readonly CachedTexture SummonSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoSummonSettings");

		private static readonly CachedTexture GolemSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoGolemSettings");

		public Pawn mechanitor;

		public Gene_Voidlink gene;

		//private int totalBandwidth;
		//private int usedBandwidth;

		//private int nextRecache = -1;

		//private List<Pawn> allControlledGolems;

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
			//nextRecache--;
			//if (nextRecache <= 0)
			//{
			//	totalBandwidth = (int)MechanoidsUtility.TotalGolembond(mechanitor);
			//	usedBandwidth = (int)MechanoidsUtility.GetConsumedGolembond(mechanitor);
			//	allControlledGolems = MechanoidsUtility.GetAllControlledGolems(mechanitor);
			//	nextRecache = 120;
			//}
			TaggedString taggedString = "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "WVC_XaG_Gene_Voidlink_GizmoTip".Translate();
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_Gene_Voidlink_GizmoLabel".Translate());
			TooltipHandler.TipRegion(rect3, taggedString);
			//Text.Font = GameFont.Small;
			//Text.Anchor = TextAnchor.UpperRight;
			//Widgets.Label(rect3, text);
			//Text.Anchor = TextAnchor.UpperLeft;
			//int num = Mathf.Max(usedBandwidth, totalBandwidth);
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
			TooltipHandler.TipRegion(rect4, taggedString);
			// Button
			Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
			Widgets.DrawTextureFitted(rectSummonSettings, SummonSettingsIcon.Texture, 1f);
			if (Mouse.IsOver(rectSummonSettings))
			{
				Widgets.DrawHighlight(rectSummonSettings);
				if (Widgets.ButtonInvisible(rectSummonSettings))
				{
					Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Gene_VoidlinkKillAllVoidMechsWarning".Translate(), delegate
					{
						gene.KillMechs();
					});
					Find.WindowStack.Add(window);
				}
			}
			TooltipHandler.TipRegion(rectSummonSettings, "WVC_XaG_Gene_VoidlinkKillAllVoidMechs".Translate());
			// Button
			Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
			Widgets.DrawTextureFitted(rectGolemsSettings, GolemSettingsIcon.Texture, 1f);
			if (Mouse.IsOver(rectGolemsSettings))
			{
				Widgets.DrawHighlight(rectGolemsSettings);
				if (Widgets.ButtonInvisible(rectGolemsSettings))
				{
					//Find.WindowStack.Add(new Dialog_Golemlink(gene));
				}
			}
			TooltipHandler.TipRegion(rectGolemsSettings, "WVC_XaG_Gene_VoidlinkSummonSettings_Desc".Translate());
			// Bonds
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			// return 136f;
			return 220f;
		}

	}

}
