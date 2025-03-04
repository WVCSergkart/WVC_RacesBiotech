using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Dryads : Gizmo
	{
		public const int InRectPadding = 6;

		private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

		public Color filledBlockColor = ColorLibrary.LightOrange;
		public Color excessBlockColor = ColorLibrary.Red;

		private static readonly CachedTexture SummonSettingsIcon = new("WVC/UI/XaG_General/UI_GestateDryads");
		private static readonly CachedTexture SelectDryadsIcon = new("WVC/UI/XaG_General/UI_SelectDryads");

		public Pawn mechanitor;

		public Gene_DryadQueen gene;

		public float totalBandwidth = 6;

		public int usedBandwidth = 0;

		public List<Pawn> allDryads = new();

		private int nextRecache = -1;
		// public int recacheFrequency = 734;

		public override bool Visible => true;

		public GeneGizmo_Dryads(Gene_DryadQueen geneMechlink)
			: base()
		{
			gene = geneMechlink;
			mechanitor = gene?.pawn;
			Order = -90f;
			// allDryads = gene.DryadsListForReading;
			// totalBandwidth = mechanitor.GetStatValue(gene.Spawner.dryadsStatLimit, cacheStaleAfterTicks: 360000);
			// usedBandwidth = allDryads.Count;
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
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out _, out TaggedString taggedString, out Rect rect3);
            TooltipHandler.TipRegion(rect3, taggedString);
            Rect rect4 = new(rect2.x, rect2.y + 23f, 40f, 40f);
            Button1(rect4);
            // Button
            Rect rect5 = new(rect4.x + 44f, rect4.y, rect4.width, rect4.height);
            Button2(rect5);
        }

        private void Uncollapsed(Vector2 topLeft, float maxWidth)
        {
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out string text, out TaggedString taggedString, out Rect rect3);
            Text.Anchor = TextAnchor.UpperRight;
            Rect totalLabelRect = new(rect3.x - rect3.height, rect3.y, rect3.width, rect3.height);
            Widgets.Label(totalLabelRect, text);
            TooltipHandler.TipRegion(rect3, taggedString);
            Text.Anchor = TextAnchor.UpperLeft;
            int num = (int)Mathf.Max(usedBandwidth, totalBandwidth);
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
                            Widgets.DrawRectFast(rect5, (num9 <= totalBandwidth) ? filledBlockColor : excessBlockColor);
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
            taggedString = "WVC_XaG_BroodmindLimit".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_BroodmindLimitGizmoTip".Translate() + "\n\n" + "WVC_XaG_Gene_GauranlenConnection_SpawnOnOff".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.spawnDryads);
            ;
            if (usedBandwidth > 0)
            {
                taggedString += (string)("\n\n" + ("WVC_XaG_BroodmindUsage".Translate() + ": ")) + usedBandwidth;
                IEnumerable<string> entries = from p in allDryads
                                              where p.Map != null
                                              group p by p.kindDef into p
                                              select (string)(p.Key.LabelCap + " x") + p.Count();
                taggedString += "\n\n" + entries.ToLineList(" - ");
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, "WVC_XaG_BroodmindLimit".Translate());
            XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
        }

        private void RecacheTick()
        {
            nextRecache--;
            if (nextRecache < 0)
            {
                allDryads = gene.DryadsListForReading;
                totalBandwidth = mechanitor.GetStatValue(gene.Spawner.dryadsStatLimit, cacheStaleAfterTicks: 6000);
                usedBandwidth = allDryads.Count;
                nextRecache = 180;
            }
        }

        private void Button1(Rect rectSummonSettings)
        {
            Widgets.DrawTextureFitted(rectSummonSettings, SummonSettingsIcon.Texture, 1f);
            if (!gene.spawnDryads)
            {
                Widgets.DrawTextureFitted(rectSummonSettings, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
            }
            if (Mouse.IsOver(rectSummonSettings))
            {
                Widgets.DrawHighlight(rectSummonSettings);
                if (Widgets.ButtonInvisible(rectSummonSettings))
                {
                    gene.spawnDryads = !gene.spawnDryads;
                    XaG_UiUtility.FlickSound(gene.spawnDryads);
                }
            }
            TooltipHandler.TipRegion(rectSummonSettings, "WVC_XaG_Gene_GauranlenConnection_SpawnOnOffDesc".Translate() + "\n\n" + "WVC_XaG_Gene_GauranlenConnection_SpawnOnOff".Translate() + ": " + XaG_UiUtility.OnOrOff(gene.spawnDryads));
        }

        private void Button2(Rect rectGolemsSettings)
        {
            Widgets.DrawTextureFitted(rectGolemsSettings, SelectDryadsIcon.Texture, 1f);
            if (Mouse.IsOver(rectGolemsSettings))
            {
                Widgets.DrawHighlight(rectGolemsSettings);
                if (Widgets.ButtonInvisible(rectGolemsSettings))
                {
                    if (gene.DryadsListForReading.NullOrEmpty())
                    {
                        Messages.Message("WVC_XaG_DryadQueenSelectAllDryads_NonDryads".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else
                    {
                        Find.Selector.ClearSelection();
                        for (int i = 0; i < gene.DryadsListForReading.Count; i++)
                        {
                            Find.Selector.Select(gene.DryadsListForReading[i]);
                        }
                    }
                }
            }
            TooltipHandler.TipRegion(rectGolemsSettings, "WVC_XaG_DryadQueenSelectAllDryads_Desc".Translate());
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
