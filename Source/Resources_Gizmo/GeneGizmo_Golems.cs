using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

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
			LabelAndDesc(topLeft, maxWidth, out Rect rect2, out _, out _, out _);
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
				totalBandwidth = (int)MechanoidsUtility.TotalGolembond(mechanitor);
				usedBandwidth = (int)MechanoidsUtility.GetConsumedGolembond(mechanitor);
				allControlledGolems = MechanoidsUtility.GetAllControlledGolems(mechanitor);
				nextRecache = 120;
			}
		}

		private void Button1(Rect rect4)
		{
			Widgets.DrawTextureFitted(rect4, SummonSettingsIcon.Texture, 1f);
			if (!gene.summonMechanoids)
			{
				Widgets.DrawTextureFitted(rect4, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
			}
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				if (Widgets.ButtonInvisible(rect4))
				{
					gene.summonMechanoids = !gene.summonMechanoids;
					XaG_UiUtility.FlickSound(gene.summonMechanoids);
				}
			}
			TooltipHandler.TipRegion(rect4, "WVC_XaG_Gene_GolemlinkGizmoSpawnDesc".Translate() + "\n\n" + "WVC_XaG_Gene_GolemlinkGizmoSpawnLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.summonMechanoids));
		}

		private void Button2(Rect rect5)
		{
			Widgets.DrawTextureFitted(rect5, GolemSettingsIcon.Texture, 1f);
			if (Mouse.IsOver(rect5))
			{
				Widgets.DrawHighlight(rect5);
				if (Widgets.ButtonInvisible(rect5))
				{
					Find.WindowStack.Add(new Dialog_Golemlink(gene));
				}
			}
			TooltipHandler.TipRegion(rect5, "WVC_XaG_GeneGolemlinkSummonSettings_Desc".Translate());
		}

		private void Uncollapsed(Vector2 topLeft, float maxWidth)
		{
			LabelAndDesc(topLeft, maxWidth, out Rect rect2, out string text, out TaggedString taggedString, out Rect rect3);
			Text.Anchor = TextAnchor.UpperRight;
			Rect totalLabelRect = new(rect3.x - rect3.height, rect3.y, rect3.width, rect3.height);
			Widgets.Label(totalLabelRect, text);
			Text.Anchor = TextAnchor.UpperLeft;
			int num = Mathf.Max(usedBandwidth, totalBandwidth);
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
			TooltipHandler.TipRegion(rect4, taggedString);
			// Button
			Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
			Button1(rectSummonSettings);
			// Button
			Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
			Button2(rectGolemsSettings);
			// Bonds
			int num2 = 2;
			int num3 = Mathf.FloorToInt(rect4.height / num2);
			int num4 = Mathf.FloorToInt(rect4.width / num3);
			int num5 = 0;
			while (num2 * num4 < num)
			{
				num2++;
				num3 = Mathf.FloorToInt(rect4.height / num2);
				num4 = Mathf.FloorToInt(rect4.width / num3);
				num5++;
				if (num5 >= 300)
				{
					break;
				}
			}
			int num6 = Mathf.FloorToInt(rect4.width / num3);
			int num7 = num2;
			float num8 = (rect4.width - num6 * num3) / 2f;
			int num9 = 0;
			for (int i = 0; i < num7; i++)
			{
				for (int j = 0; j < num6; j++)
				{
					num9++;
					Rect rect5 = new Rect(rect4.x + j * num3 + num8, rect4.y + i * num3, num3, num3).ContractedBy(2f);
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
		}

		private void LabelAndDesc(Vector2 topLeft, float maxWidth, out Rect rect2, out string text, out TaggedString taggedString, out Rect rect3)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			RecacheTick();
			text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
			taggedString = "WVC_XaG_GolemBandwidth".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_GolemBandwidthGizmoTip".Translate() + "\n\n" + "WVC_XaG_Gene_GolemlinkGizmoSpawnLabel".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.summonMechanoids);
			if (usedBandwidth > 0)
			{
				taggedString += "\n\n" + ("WVC_XaG_GolemBandwidthUsage".Translate() + ": ") + usedBandwidth;
				IEnumerable<string> entries = from p in allControlledGolems
											  where !p.IsGestating()
											  group p by p.kindDef into p
											  select p.Key.LabelCap + " x" + p.Count() + " (+" + p.Sum((Pawn mech) => mech.GetStatValue(MainDefOf.WVC_GolemBondCost)) + ")";
				taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_GolemBandwidth".Translate());
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
