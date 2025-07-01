using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class GeneGizmo_Thralls : Gizmo
	{
		public const int InRectPadding = 6;

		//private static readonly Color EmptyBlockColor = new(0.3f, 0.3f, 0.3f, 1f);

        // private static readonly Color FilledBlockColor = ColorLibrary.Orange;

        // private static readonly Color ExcessBlockColor = ColorLibrary.Red;

        private static readonly CachedTexture XenoMenuIcon = new("WVC/UI/XaG_General/ThrallMaker_XenoMenu_Gizmo_v0");

        public bool cached = false;

		//public Color filledBlockColor = ColorLibrary.LightBlue;
		//public Color excessBlockColor = ColorLibrary.Red;

		public Pawn mechanitor;

		public Gene_ThrallMaker gene;
		//public GeneExtension_Giver extension;
		//public CompProperties_AbilityCellsfeederBite cellsfeederComponent;

		//private int resurgentPawnsCount;
		//private int thrallPawnsCount;

		//private int nextRecache = -1;
		// public int recacheFrequency = 734;

		//public float cellsPerDay;

		// public int golemIndex = -1;

		// public string tipSectionTitle = "WVC_XaG_GolemBandwidth";
		// public string tipSectionTip = "WVC_XaG_GolemBandwidthGizmoTip";

		// private List<Pawn> allThrallsInColony;
		//private List<Gene_GeneticThrall> geneThralls;

		public override bool Visible => true;

		public GeneGizmo_Thralls(Gene_ThrallMaker geneMechlink)
			: base()
		{
			gene = geneMechlink;
			mechanitor = gene?.pawn;
			//extension = gene?.def?.GetModExtension<GeneExtension_Giver>();
            Order = -90f;
        }

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Collapsed(topLeft, maxWidth);
            //if (gene.gizmoCollapse)
            //{
            //}
            //else
            //{
            //    Uncollapsed(topLeft, maxWidth);
            //}
            return new GizmoResult(GizmoState.Clear);
        }

        private void Collapsed(Vector2 topLeft, float maxWidth)
        {
            LabelAndDesc(topLeft, maxWidth, out Rect rect2, out Rect rect3);
            Buttons(rect2, rect3, out _);
        }

        //private void Uncollapsed(Vector2 topLeft, float maxWidth)
        //{
        //    LabelAndDesc(topLeft, maxWidth, out Rect rect2, out Rect rect3, out TaggedString taggedString);
        //    Buttons(rect2, rect3, out Rect rect4);
        //    TooltipHandler.TipRegion(rect4, taggedString);
        //    // Bonds
        //    int num = Mathf.Max(thrallPawnsCount, resurgentPawnsCount);
        //    int num2 = 2;
        //    int num3 = Mathf.FloorToInt(rect4.height / (float)num2);
        //    int num4 = Mathf.FloorToInt(rect4.width / (float)num3);
        //    int num5 = 0;
        //    while (num2 * num4 < num)
        //    {
        //        num2++;
        //        num3 = Mathf.FloorToInt(rect4.height / (float)num2);
        //        num4 = Mathf.FloorToInt(rect4.width / (float)num3);
        //        num5++;
        //        if (num5 >= 33)
        //        {
        //            break;
        //        }
        //    }
        //    int num6 = Mathf.FloorToInt(rect4.width / (float)num3);
        //    int num7 = num2;
        //    float num8 = (rect4.width - (float)(num6 * num3)) / 2f;
        //    int num9 = 0;
        //    for (int i = 0; i < num7; i++)
        //    {
        //        for (int j = 0; j < num6; j++)
        //        {
        //            num9++;
        //            Rect rect5 = new Rect(rect4.x + (float)(j * num3) + num8, rect4.y + (float)(i * num3), num3, num3).ContractedBy(2f);
        //            if (num9 <= num)
        //            {
        //                if (num9 <= thrallPawnsCount)
        //                {
        //                    Widgets.DrawRectFast(rect5, (num9 <= resurgentPawnsCount) ? filledBlockColor : excessBlockColor);
        //                }
        //                else
        //                {
        //                    Widgets.DrawRectFast(rect5, EmptyBlockColor);
        //                }
        //            }
        //        }
        //    }
        //}

        private void Buttons(Rect rect2, Rect rect3, out Rect rect4)
        {
            rect4 = new(rect2.x, rect3.yMax + 6f, rect2.width - 84f, rect2.height - rect3.height - 6f);
            // Button
            Rect rectSummonSettings = new(rect4.xMax, rect2.y + 23f, 40f, 40f);
            Button1(rectSummonSettings);
            // Button
            Rect rectGolemsSettings = new(rectSummonSettings.x + 44f, rectSummonSettings.y, rectSummonSettings.width, rectSummonSettings.height);
            Button2(rectGolemsSettings);
        }

        private void Button1(Rect rect4)
        {
            Widgets.DrawTextureFitted(rect4, XenoMenuIcon.Texture, 1f);
            if (Mouse.IsOver(rect4))
            {
                Widgets.DrawHighlight(rect4);
                if (Widgets.ButtonInvisible(rect4))
                {
                    gene.ThrallMakerDialog();
                }
            }
            TooltipHandler.TipRegion(rect4, "WVC_XaG_GeneThrallMaker_ButtonDesc".Translate());
        }

        private Ability ability;

        private void Button2(Rect rect5)
        {
            Widgets.DrawTextureFitted(rect5, XaG_UiUtility.GermlineImplanterIcon.Texture, 1f);
            if (ability == null)
            {
                try
                {
                    ability = gene.pawn.abilities.GetAbility(gene.def.abilities.FirstOrDefault());
                }
                catch
                {
                    gene.shouldDrawGizmo = false;
                    Log.Error("Failed get thrall maker ability. Gizmo disabled.");
                    return;
                }
            }
            if (ability.CooldownTicksRemaining > 0)
            {
                Widgets.DrawTextureFitted(rect5, XaG_UiUtility.NonAggressiveRedCancelIcon.Texture, 1f);
            }
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
                if (Widgets.ButtonInvisible(rect5))
                {
                    if (gene.thrallDef == null)
                    {
                        gene.ThrallMakerDialog();
                        Messages.Message("WVC_XaG_ThrallMakerNonThrallSelected".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else if (ability.CanCast)
                    {
                        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                        Find.Targeter.BeginTargeting(ability.verb);
                    }
                    else
                    {
                        SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    }
                }
            }
            TooltipHandler.TipRegion(rect5, ability.def.description + "\n\n" + "WVC_XaG_CooldownIn".Translate(ability.CooldownTicksRemaining.ToStringTicksToPeriod()));
        }

        private void LabelAndDesc(Vector2 topLeft, float maxWidth, out Rect rect2, out Rect rect3)
        {
            Rect rect = new(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            //nextRecache--;
            //if (nextRecache < 0)
            //{
            //    resurgentPawnsCount = GeneResourceUtility.GetThrallsLimit(mechanitor, cellsPerDay);
            //    geneThralls = GeneResourceUtility.GetAllThralls(mechanitor);
            //    thrallPawnsCount = geneThralls.Count;
            //    nextRecache = 300;
            //}
            //string text = thrallPawnsCount.ToString("F0") + "/" + resurgentPawnsCount.ToString("F0");
            //taggedString = "WVC_XaG_ThrallsBandwidthGizmoLabel".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" + "WVC_XaG_ThrallsBandwidthGizmoGizmoTip".Translate();
            //if (thrallPawnsCount > 0 && thrallPawnsCount < 11)
            //{
            //    taggedString += (string)("\n\n" + ("WVC_XaG_ThrallsBandwidthUsage".Translate() + ": ")) + thrallPawnsCount;
            //    IEnumerable<string> entries = from gene in geneThralls
            //                                  select (string)(gene.pawn.NameShortColored.ToString()) + " " + "WVC_XaG_ThrallsBandwidth_NextFeeding".Translate().Resolve() + ": " + gene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            //    taggedString += "\n\n" + entries.ToLineList(" - ");
            //}
            //else if (thrallPawnsCount > 10)
            //{
            //    taggedString += (string)("\n\n" + ("WVC_XaG_ThrallsBandwidthUsage".Translate() + ": ")) + thrallPawnsCount;
            //    IEnumerable<string> entries = from gene in geneThralls
            //                                  where gene.nextTick < (60000 * 3)
            //                                  select (string)(gene.pawn.NameShortColored.ToString()) + " " + "WVC_XaG_ThrallsBandwidth_NextFeeding".Translate().Resolve() + ": " + gene.nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            //    taggedString += "\n\n" + entries.ToLineList(" - ");
            //}
            Text.Font = GameFont.Small;
            rect3 = new(rect2.x, rect2.y, rect2.width, 20f);
            TooltipHandler.TipRegion(rect3, gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.description);
            //Text.Anchor = TextAnchor.UpperRight;
            //Widgets.Label(rect3, text);
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(rect3, "WVC_XaG_ThrallsBandwidthGizmoLabel".Translate());
            //XaG_UiUtility.GizmoButton(rect3, ref gene.gizmoCollapse);
        }

        public override float GetWidth(float maxWidth)
        {
            return 96f;
            //if (gene.gizmoCollapse)
            //{
            //}
            //return 220f;
            //return 136f;
		}

	}

}
