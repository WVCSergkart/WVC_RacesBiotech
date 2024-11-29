using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Thralls : Gizmo
	{
		public const int InRectPadding = 6;

		private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		// private static readonly Color FilledBlockColor = ColorLibrary.Orange;

		// private static readonly Color ExcessBlockColor = ColorLibrary.Red;

		public bool cached = false;

		public Color filledBlockColor = ColorLibrary.LightBlue;
		public Color excessBlockColor = ColorLibrary.Red;

		public Pawn mechanitor;

		public Gene_ThrallMaker gene;
		public GeneExtension_Giver extension;
		//public CompProperties_AbilityCellsfeederBite cellsfeederComponent;

		private int resurgentPawnsCount;
		private int thrallPawnsCount;

		private int nextRecache = -1;
		// public int recacheFrequency = 734;

		public float cellsPerDay;

		// public int golemIndex = -1;

		// public string tipSectionTitle = "WVC_XaG_GolemBandwidth";
		// public string tipSectionTip = "WVC_XaG_GolemBandwidthGizmoTip";

		// private List<Pawn> allThrallsInColony;
		private List<Gene_GeneticThrall> geneThralls;

		public override bool Visible => true;

		public GeneGizmo_Thralls(Gene_ThrallMaker geneMechlink)
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
				//cellsfeederComponent = Gene_GeneticThrall.GetAbilityCompProperties_CellsFeeder(WVC_GenesDefOf.WVC_XaG_Cellsfeed);
				cellsPerDay = 1f;
			}
			resurgentPawnsCount = GeneResourceUtility.GetThrallsLimit(mechanitor, cellsPerDay);
			geneThralls = GeneResourceUtility.GetAllThralls(mechanitor);
			thrallPawnsCount = geneThralls.Count;
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect rect2 = rect.ContractedBy(6f);
			Widgets.DrawWindowBackground(rect);
			nextRecache--;
			if (nextRecache < 0)
			{
				int newResurgentPawnsCount = GeneResourceUtility.GetThrallsLimit(mechanitor, cellsPerDay);
				if (newResurgentPawnsCount != resurgentPawnsCount)
				{
					resurgentPawnsCount = newResurgentPawnsCount;
					gene.Notify_GenesChanged(null);
				}
				geneThralls = GeneResourceUtility.GetAllThralls(mechanitor);
				thrallPawnsCount = geneThralls.Count;
				nextRecache = 300;
			}
			string text = thrallPawnsCount.ToString("F0") + " / " + resurgentPawnsCount.ToString("F0");
			TaggedString taggedString = "WVC_XaG_ThrallsBandwidthGizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_ThrallsBandwidthGizmoGizmoTip".Translate();
			if (thrallPawnsCount > 0 && thrallPawnsCount < 11)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_ThrallsBandwidthUsage".Translate() + ": ")) + thrallPawnsCount;
				IEnumerable<string> entries = from gene in geneThralls
					where !gene.pawn.Dead
					// group p by p.kindDef into p
					select (string)(gene.pawn.NameShortColored.ToString()) + " " + "WVC_XaG_ThrallsBandwidth_NextFeeding".Translate().Resolve() + ": " + gene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			else if (thrallPawnsCount > 10)
			{
				taggedString += (string)("\n\n" + ("WVC_XaG_ThrallsBandwidthUsage".Translate() + ": ")) + thrallPawnsCount;
				IEnumerable<string> entries = from gene in geneThralls
					where !gene.pawn.Dead && gene.nextTick < (60000 * 3)
					// group p by p.kindDef into p
					select (string)(gene.pawn.NameShortColored.ToString()) + " " + "WVC_XaG_ThrallsBandwidth_NextFeeding".Translate().Resolve() + ": " + gene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				taggedString += "\n\n" + entries.ToLineList(" - ");
			}
			TooltipHandler.TipRegion(rect, taggedString);
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
			Widgets.Label(rect3, "WVC_XaG_ThrallsBandwidthGizmoLabel".Translate());
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperRight;
			Widgets.Label(rect3, text);
			Text.Anchor = TextAnchor.UpperLeft;
			int num = Mathf.Max(thrallPawnsCount, resurgentPawnsCount);
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
						if (num9 <= thrallPawnsCount)
						{
							Widgets.DrawRectFast(rect5, (num9 <= resurgentPawnsCount) ? filledBlockColor : excessBlockColor);
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
