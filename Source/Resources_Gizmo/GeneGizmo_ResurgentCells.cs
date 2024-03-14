using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    [StaticConstructorOnStartup]
    public class GeneGizmo_ResourceResurgentCells : GeneGizmo_Resource
    {
        // private static readonly Texture2D HemogenCostTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

        // private const float TotalPulsateTime = 0.85f;

        public List<Pair<IGeneResourceDrain, float>> tmpDrainGenes = new();

        public GeneGizmo_ResourceResurgentCells(Gene_Resource gene, List<IGeneResourceDrain> drainGenes, Color barColor, Color barhighlightColor)
            : base(gene, drainGenes, barColor, barhighlightColor)
        {
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
            float num = Mathf.Repeat(Time.time, 0.85f);
            if (num < 0.1f)
            {
                _ = num / 0.1f;
            }
            else if (num >= 0.25f)
            {
                _ = 1f - (num - 0.25f) / 0.6f;
            }
            // if (((MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow)?.LastMouseoverGizmo is Command_Ability command_Ability && gene.Max != 0f)
            // {
            // foreach (CompAbilityEffect effectComp in command_Ability.Ability.EffectComps)
            // {
            // if (effectComp is CompAbilityEffect_HemogenCost compAbilityEffect_HemogenCost && compAbilityEffect_HemogenCost.Props.hemogenCost > float.Epsilon)
            // {
            // Rect rect = barRect.ContractedBy(3f);
            // float width = rect.width;
            // float num3 = gene.Value / gene.Max;
            // rect.xMax = rect.xMin + width * num3;
            // float num4 = Mathf.Min(compAbilityEffect_HemogenCost.Props.hemogenCost / gene.Max, 1f);
            // rect.xMin = Mathf.Max(rect.xMin, rect.xMax - width * num4);
            // GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
            // GenUI.DrawTextureWithMaterial(rect, HemogenCostTex, null);
            // GUI.color = Color.white;
            // return result;
            // }
            // }
            // return result;
            // }
            return result;
        }

        protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
        {
            if ((gene.pawn.IsColonistPlayerControlled || gene.pawn.IsPrisonerOfColony) && gene is Gene_ResurgentCells hemogenGene)
            {
                Gene_ResurgentTotalHealing totalHealingGene = hemogenGene.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentTotalHealing>();
                if (totalHealingGene != null)
                {
                    headerRect.xMax -= 24f;
                    Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
                    Widgets.DefIcon(rect, totalHealingGene.def);
                    GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.totalHealingAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
                    if (Widgets.ButtonInvisible(rect))
                    {
                        hemogenGene.totalHealingAllowed = !hemogenGene.totalHealingAllowed;
                        if (hemogenGene.totalHealingAllowed)
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        }
                        else
                        {
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                        }
                    }
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                        string onOff = (hemogenGene.totalHealingAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
                        TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoTotalHealingDesc".Translate(onOff.Named("ONOFF")), 1001);
                        mouseOverElement = true;
                    }
                    // DrawButton(labelRect, mouseOverAnyHighlightableElement, totalHealingGene.def, hemogenGene.totalHealingAllowed, "WVC_XaG_AutoTotalHealingDesc", 37f);
                }
                Gene_ResurgentAgeless ageReversionGene = hemogenGene.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentAgeless>();
                if (ageReversionGene != null)
                {
                    headerRect.xMax -= 24f;
                    Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
                    Widgets.DefIcon(rect, ageReversionGene.def);
                    GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.ageReversionAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
                    if (Widgets.ButtonInvisible(rect))
                    {
                        hemogenGene.ageReversionAllowed = !hemogenGene.ageReversionAllowed;
                        if (hemogenGene.ageReversionAllowed)
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        }
                        else
                        {
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                        }
                    }
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                        string onOff = (hemogenGene.ageReversionAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
                        TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoAgeReversionDesc".Translate(onOff.Named("ONOFF")), 1001);
                        mouseOverElement = true;
                    }
                    // DrawButton(labelRect, mouseOverAnyHighlightableElement, ageReversionGene.def, hemogenGene.ageReversionAllowed, "WVC_XaG_AutoAgeReversionDesc", 37f);
                }
                Gene_ResurgentClotting woundClottingGene = hemogenGene.pawn.genes?.GetFirstGeneOfType<Gene_ResurgentClotting>();
                if (woundClottingGene != null)
                {
                    headerRect.xMax -= 24f;
                    Rect rect = new(headerRect.xMax, headerRect.y, 24f, 24f);
                    Widgets.DefIcon(rect, woundClottingGene.def);
                    GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), hemogenGene.woundClottingAllowed ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
                    if (Widgets.ButtonInvisible(rect))
                    {
                        hemogenGene.woundClottingAllowed = !hemogenGene.woundClottingAllowed;
                        if (hemogenGene.woundClottingAllowed)
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        }
                        else
                        {
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                        }
                    }
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                        string onOff = (hemogenGene.woundClottingAllowed ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
                        TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + "WVC_XaG_AutoWoundClottingDesc".Translate(onOff.Named("ONOFF")), 1001);
                        mouseOverElement = true;
                    }
                    // DrawButton(labelRect, mouseOverAnyHighlightableElement, woundClottingGene.def, hemogenGene.woundClottingAllowed, "WVC_XaG_AutoWoundClottingDesc", 74f);
                }
            }
            base.DrawHeader(headerRect, ref mouseOverElement);
            // Button(labelRect, mouseOverAnyHighlightableElement, hemogenGene.ageReversionAllowed, 40f);
            // Button(labelRect, mouseOverAnyHighlightableElement, hemogenGene.totalHealingAllowed, 80f);
        }

        // public static void DrawButton(Rect labelRect, bool mouseOverAnyHighlightableElement, Def def, out bool buttonBool, string tooltipOfOn, float offset = 24f)
        // {
        // labelRect.xMax -= offset;
        // Rect rect = new(labelRect.xMax, labelRect.y, 24f, 24f);
        // Widgets.DefIcon(rect, def);
        // GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f), buttonBool ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
        // if (Widgets.ButtonInvisible(rect))
        // {
        // buttonBool = !buttonBool;
        // if (buttonBool)
        // {
        // SoundDefOf.Tick_High.PlayOneShotOnCamera();
        // }
        // else
        // {
        // SoundDefOf.Tick_Low.PlayOneShotOnCamera();
        // }
        // }
        // if (Mouse.IsOver(rect))
        // {
        // Widgets.DrawHighlight(rect);
        // string onOff = (buttonBool ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
        // TooltipHandler.TipRegion(rect, () => "WVC_XaG_AutoBaseDesc".Translate() + tooltipOfOn.Translate(onOff.Named("ONOFF")), 1001);
        // mouseOverAnyHighlightableElement = true;
        // }
        // }

        protected override string GetTooltip()
        {
            tmpDrainGenes.Clear();
            string text = $"{gene.ResourceLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor)}: {gene.ValueForDisplay} / {gene.MaxForDisplay}";
            // if (gene.pawn.IsColonistPlayerControlled || gene.pawn.IsPrisonerOfColony)
            // {
            // text = ((!(gene.targetValue <= 0f)) ? (text + (string)("ConsumeHemogenBelow".Translate() + ": ") + gene.PostProcessValue(gene.targetValue)) : (text + "NeverConsumeHemogen".Translate().ToString()));
            // }
            if (!drainGenes.NullOrEmpty())
            {
                float num = 0f;
                foreach (IGeneResourceDrain drainGene in drainGenes)
                {
                    if (drainGene.CanOffset)
                    {
                        tmpDrainGenes.Add(new Pair<IGeneResourceDrain, float>(drainGene, drainGene.ResourceLossPerDay));
                        num += drainGene.ResourceLossPerDay;
                    }
                }
                if (num != 0f)
                {
                    string text2 = ((num < 0f) ? "RegenerationRate".Translate() : "DrainRate".Translate());
                    text = text + "\n\n" + text2 + ": " + "PerDay".Translate(Mathf.Abs(gene.PostProcessValue(num))).Resolve();
                    foreach (Pair<IGeneResourceDrain, float> tmpDrainGene in tmpDrainGenes)
                    {
                        text = text + "\n  - " + tmpDrainGene.First.DisplayLabel.CapitalizeFirst() + ": " + "PerDay".Translate(gene.PostProcessValue(0f - tmpDrainGene.Second).ToStringWithSign()).Resolve();
                    }
                }
            }
            if (!gene.def.resourceDescription.NullOrEmpty())
            {
                text = text + "\n\n" + gene.def.resourceDescription.Formatted(gene.pawn.Named("PAWN")).Resolve();
            }
            return text;
        }
    }

}
