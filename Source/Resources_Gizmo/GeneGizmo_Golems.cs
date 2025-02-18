using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Golemlink : Gizmo
	{

		private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		private static readonly Color FilledBlockColor = ColorLibrary.Orange;

		private static readonly Color ExcessBlockColor = ColorLibrary.Red;

		private static readonly CachedTexture SummonSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoSummonSettings");

		private static readonly CachedTexture GolemSettingsIcon = new("WVC/UI/XaG_General/UI_Golemlink_GizmoGolemSettings");

		public Pawn mechanitor;

		public Gene_Golemlink gene;

		private int totalBandwidth;
		private int usedBandwidth;

		private int nextRecache = -1;

		private List<Pawn> allControlledGolems;

		public override bool Visible => true;

		public GeneGizmo_Golemlink(Gene_Golemlink geneMechlink)
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
			nextRecache--;
			if (nextRecache <= 0)
			{
				totalBandwidth = (int)MechanoidsUtility.TotalGolembond(mechanitor);
				usedBandwidth = (int)MechanoidsUtility.GetConsumedGolembond(mechanitor);
				allControlledGolems = MechanoidsUtility.GetAllControlledGolems(mechanitor);
				nextRecache = 120;
			}
			string text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
			TaggedString taggedString = "WVC_XaG_GolemBandwidth".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_GolemBandwidthGizmoTip".Translate() + "\n\n" +  "WVC_XaG_Gene_GolemlinkGizmoSpawnLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.summonMechanoids);
			if (usedBandwidth > 0)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_GolemBandwidthUsage".Translate() + ": ")) + usedBandwidth;
				IEnumerable<string> entries = from p in allControlledGolems
					where !p.IsGestating()
					group p by p.kindDef into p
					select (string)(p.Key.LabelCap + " x") + p.Count() + " (+" + p.Sum((Pawn mech) => mech.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost)) + ")";
				taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_GolemBandwidth".Translate());
			TooltipHandler.TipRegion(rect3, taggedString);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			int num = Mathf.Max(usedBandwidth, totalBandwidth);
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
					gene.summonMechanoids = !gene.summonMechanoids;
					if (gene.summonMechanoids)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			}
			TooltipHandler.TipRegion(rectSummonSettings, "WVC_XaG_Gene_GolemlinkGizmoSpawnDesc".Translate() + "\n\n" +  "WVC_XaG_Gene_GolemlinkGizmoSpawnLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.summonMechanoids));
			// Button
			Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
			Widgets.DrawTextureFitted(rectGolemsSettings, GolemSettingsIcon.Texture, 1f);
			if (Mouse.IsOver(rectGolemsSettings))
			{
				Widgets.DrawHighlight(rectGolemsSettings);
				if (Widgets.ButtonInvisible(rectGolemsSettings))
				{
					Find.WindowStack.Add(new Dialog_Golemlink(gene));
				}
			}
			TooltipHandler.TipRegion(rectGolemsSettings, "WVC_XaG_GeneGolemlinkSummonSettings_Desc".Translate());
			// Bonds
			int num2 = 2;
			int num3 = Mathf.FloorToInt(rect4.height / (float)num2);
			int num4 = Mathf.FloorToInt(rect4.width / (float)num3);
			int num5 = 0;
			while (num2 * num4 < num)
			{
				num2++;
				num3 = Mathf.FloorToInt(rect4.height / (float)num2);
				num4 = Mathf.FloorToInt(rect4.width / (float)num3);
				num5++;
				if (num5 >= 300)
				{
					break;
				}
			}
			int num6 = Mathf.FloorToInt(rect4.width / (float)num3);
			int num7 = num2;
			float num8 = (rect4.width - (float)(num6 * num3)) / 2f;
			int num9 = 0;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < num6; j++)
				{
					num9++;
					Rect rect5 = new Rect(rect4.x + (float)(j * num3) + num8, rect4.y + (float)(i * num3), num3, num3).ContractedBy(2f);
					if (num9 <= num)
					{
						if (num9 <= usedBandwidth)
						{
							Widgets.DrawRectFast(rect5, (num9 <= totalBandwidth) ? FilledBlockColor : ExcessBlockColor);
						}
						else
						{
							Widgets.DrawRectFast(rect5, EmptyBlockColor);
						}
					}
				}
			}
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth)
		{
			// return 136f;
			return 220f;
		}

	}

}
