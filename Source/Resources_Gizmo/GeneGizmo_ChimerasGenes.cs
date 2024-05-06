using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_ChimerasGenes : Gizmo
	{
		// public const int InRectPadding = 6;

		// private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		public Color filledBlockColor = ColorLibrary.LightBlue;
		public Color excessBlockColor = ColorLibrary.LightGreen;

		public Pawn mechanitor;

		public Gene_Chimera gene;

		public int totalBandwidth = 0;

		public int usedBandwidth = 0;

		// public int allGenesCount = 0;

		// public List<Pawn> allDryads = new();

		// private int nextRecache = -1;
		// public int recacheFrequency = 734;

		public override bool Visible => Find.Selector.SelectedPawns.Count == 1;

		public GeneGizmo_ChimerasGenes(Gene_Chimera geneMechlink)
			: base()
		{
			gene = geneMechlink;
			mechanitor = gene?.pawn;
			// allDryads = gene.AllDryads;
			// usedBandwidth = allDryads.Count;
			Order = -90f;
			totalBandwidth = gene.AllGenes.Count;
			usedBandwidth = gene.EatedGenes.Count;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			if (mechanitor.IsHashIntervalTick(60))
			{
				totalBandwidth = gene.AllGenes.Count;
				// totalBandwidth = gene.StolenGenes.Count;
				usedBandwidth = gene.EatedGenes.Count;
			}
			// if (Find.TickManager.TicksGame > nextRecache)
			// {
				// nextRecache = Find.TickManager.TicksGame + recacheFrequency;
			// }
			string text = totalBandwidth.ToString("F0");
			TaggedString taggedString = "WVC_XaG_Gene_Chimera_GizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_Gene_Chimera_GizmoTip".Translate();
			if (usedBandwidth > 0)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_Gene_Chimera_GizmoEatedLabel".Translate() + ": ")) + usedBandwidth;
				// IEnumerable<string> entries = from p in allDryads
					// where p.Map != null
					// group p by p.kindDef into p
					// select (string)(p.Key.LabelCap + " x") + p.Count();
				// taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			TooltipHandler.TipRegion(rect, taggedString);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "Genes".Translate().CapitalizeFirst());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			int num = (int)Mathf.Max(usedBandwidth, totalBandwidth);
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
							Widgets.DrawRectFast(rect5, excessBlockColor);
						}
						else
						{
							Widgets.DrawRectFast(rect5, filledBlockColor);
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
