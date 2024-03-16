using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Golems : Gizmo
	{
		public const int InRectPadding = 6;

		private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		// private static readonly Color FilledBlockColor = ColorLibrary.Orange;

		// private static readonly Color ExcessBlockColor = ColorLibrary.Red;

		public bool cached = false;

		public Color filledBlockColor = ColorLibrary.Orange;
		public Color excessBlockColor = ColorLibrary.Red;

		public Pawn mechanitor;

		public Gene_MechlinkWithGizmo gene;
		public GeneExtension_Giver extension;

		private int totalBandwidth;
		private int usedBandwidth;

		private int nextRecache = -1;
		public int recacheFrequency = 734;

		public int golemIndex = -1;

		public string tipSectionTitle = "WVC_XaG_GolemBandwidth";
		public string tipSectionTip = "WVC_XaG_GolemBandwidthGizmoTip";

		private List<Pawn> allControlledGolems;

		public override bool Visible => Find.Selector.SelectedPawns.Count == 1;

		public GeneGizmo_Golems(Gene_MechlinkWithGizmo geneMechlink)
			: base()
		{
			gene = geneMechlink;
			mechanitor = gene?.pawn;
			extension = gene?.def?.GetModExtension<GeneExtension_Giver>();
			if (extension != null)
			{
				Order = extension.gizmoOrder;
				filledBlockColor = extension.filledBlockColor;
				excessBlockColor = extension.excessBlockColor;
				recacheFrequency = extension.recacheFrequency;
				tipSectionTitle = extension.tipSectionTitle;
				tipSectionTip = extension.tipSectionTip;
				golemIndex = extension.golemistTypeIndex;
			}
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			if (Find.TickManager.TicksGame > nextRecache)
			{
				totalBandwidth = (int)MechanoidsUtility.TotalGolembond(mechanitor);
				usedBandwidth = (int)MechanoidsUtility.GetConsumedGolembond(mechanitor);
				allControlledGolems = MechanoidsUtility.GetAllControlledGolemsOfIndex(mechanitor, golemIndex);
				nextRecache = Find.TickManager.TicksGame + recacheFrequency;
			}
			string text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
			TaggedString taggedString = tipSectionTitle.Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + tipSectionTip.Translate();
			if (usedBandwidth > 0)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_GolemBandwidthUsage".Translate() + ": ")) + usedBandwidth;
				IEnumerable<string> entries = from p in allControlledGolems
					where !p.IsGestating()
					group p by p.kindDef into p
					select (string)(p.Key.LabelCap + " x") + p.Count() + " (+" + p.Sum((Pawn mech) => mech.GetStatValue(WVC_GenesDefOf.WVC_GolemBondCost)) + ")";
				taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			TooltipHandler.TipRegion(rect, taggedString);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_GolemBandwidth".Translate());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			int num = Mathf.Max(usedBandwidth, totalBandwidth);
			Rect rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
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
				if (num5 >= 1000)
				{
					Log.Error("Failed to fit bandwidth cells into gizmo rect.");
					return new GizmoResult(GizmoState.Clear);
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
							Widgets.DrawRectFast(rect5, (num9 <= totalBandwidth) ? filledBlockColor : excessBlockColor);
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
			return 136f;
		}

	}

}
