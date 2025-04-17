using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Dialog_EditShiftGenes : Window
    {

        public Gene_Shapeshifter gene;
        public List<GeneDef> pawnGenes;
        public bool inheritable;

        public Dialog_EditShiftGenes(Gene_Shapeshifter gene)
        {
            this.gene = gene;
            //this.callback = callback;
            pawnGenes = XaG_GeneUtility.ConvertGenesInGeneDefs(gene.pawn.genes.GenesListForReading);
            selectedGenes = new();
            allGenes = new();
            inheritable = !gene.pawn.genes.IsXenogene(gene);
            foreach (GeneDef item in DefDatabase<GeneDef>.AllDefsListForReading.Where((geneDef) => geneDef?.GetModExtension<GeneExtension_Undead>()?.reqGeneMat > 0).ToList())
            {
                GeneDefWithChance geneDefWithChance = new();
                geneDefWithChance.geneDef = item;
                geneDefWithChance.disabled = pawnGenes.Contains(item);
                // || XaG_GeneUtility.ConflictWith(item, pawnGenes)
                GeneExtension_Undead geneExtension_Undead = item.GetModExtension<GeneExtension_Undead>();
                if (geneExtension_Undead.overrideGeneCategory == null)
                {
                    geneDefWithChance.displayCategory = GeneCategoryDefOf.Miscellaneous;
                }
                else
                {
                    geneDefWithChance.displayCategory = geneExtension_Undead.overrideGeneCategory;
                }
                geneDefWithChance.cost = geneExtension_Undead.reqGeneMat;
                allGenes.Add(geneDefWithChance);
            }
            forcePause = true;
            absorbInputAroundWindow = true;
            foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
            {
                collapsedCategories.Add(allDef, value: false);
            }
            OnGenesChanged();
        }

        protected float scrollHeight;

        protected Vector2 scrollPosition;

        protected float selectedHeight;

        protected float unselectedHeight;

        public static readonly Vector2 GeneSize = new(87f, 68f);

        public List<GeneDefWithChance> allGenes = new();

        protected List<GeneDefWithChance> selectedGenes = new();

        protected bool? selectedCollapsed = false;

        protected HashSet<GeneCategoryDef> matchingCategories = new();

        protected Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

        protected bool hoveredAnyGene;

        protected GeneDef hoveredGene;

        public override Vector2 InitialSize => new(750, 800);
        public List<GeneDefWithChance> SelectedGenes => selectedGenes;
        protected static readonly Vector2 ButSize = new(150f, 38f);

        public int ReqGeneMat => selectedGenes.Sum(x => x.cost);

        public int AllGeneMat => gene.GeneticMaterial;

        public override void DoWindowContents(Rect rect)
        {
            Rect rect2 = rect;
            rect2.yMax -= ButSize.y + 4f;
            Rect rect3 = new(rect2.x, rect2.y, rect2.width, 35f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.Label(rect3, gene.LabelCap);
            Text.Font = GameFont.Small;
            float num3 = Text.LineHeight * 3f;
            rect2.y += 10;
            Rect geneRect = new(rect2.x, rect2.y, rect2.width, rect2.height - num3 - 8f - 50);
            DrawGenes(geneRect);
            float num4 = geneRect.yMax + 4f;
            Rect bioStatsRect = new(rect2.x + 4f, num4, rect.width - 4, num3);
            bioStatsRect.yMax = geneRect.yMax + num3 + 4f;
            Draw(bioStatsRect, AllGeneMat, ReqGeneMat);
            var bottomAreaRect = new Rect(bioStatsRect.x, bioStatsRect.yMax + 10, bioStatsRect.width, 75);
            DoBottomButtons(bottomAreaRect);
        }

        private static float cachedWidth;

        public static readonly CachedTexture ReqTex = new("WVC/UI/XaG_General/GenMatTex_Req");
        public static readonly CachedTexture HasTex = new("WVC/UI/XaG_General/GenMatTex_Has");

        private struct GeneMatStatData
        {
            public string labelKey;

            public string descKey;

            public Texture2D icon;

            public GeneMatStatData(string labelKey, string descKey, Texture2D icon)
            {
                this.labelKey = labelKey;
                this.descKey = descKey;
                this.icon = icon;
            }
        }

        private readonly GeneMatStatData[] NewStats = new GeneMatStatData[2]
        {
            new GeneMatStatData("WVC_XaG_GeneticMaterial_Shifter", "WVC_XaG_GeneticMaterial_ShifterDesc", HasTex.Texture),
            new GeneMatStatData("WVC_XaG_GeneticMaterial_Genes", "WVC_XaG_GeneticMaterial_GenesDesc", ReqTex.Texture),
        };

        protected Dictionary<string, string> truncateCache = new();
        private float MaxLabelWidth()
        {
            float num = 0f;
            int num2 = NewStats.Length;
            for (int i = 0; i < num2; i++)
            {
                num = Mathf.Max(num, Text.CalcSize(NewStats[i].labelKey.Translate()).x);
            }
            return num;
        }

        public void Draw(Rect rect, int sumCost, int totalHave)
        {
            int num = NewStats.Length;
            float num2 = MaxLabelWidth();
            float num3 = rect.height / (float)num;
            GUI.BeginGroup(rect);
            for (int i = 0; i < num; i++)
            {
                Rect position = new(0f, (float)i * num3 + (num3 - 22f) / 2f, 22f, 22f);
                Rect rect2 = new(position.xMax + 4f, (float)i * num3, num2, num3);
                Rect rect3 = new(0f, rect2.y, rect.width, rect2.height);
                if (i % 2 == 1)
                {
                    Widgets.DrawLightHighlight(rect3);
                }
                Widgets.DrawHighlightIfMouseover(rect3);
                rect3.xMax = rect2.xMax + 4f + 90f;
                TaggedString taggedString = NewStats[i].descKey.Translate();
                TooltipHandler.TipRegion(rect3, taggedString);
                GUI.DrawTexture(position, NewStats[i].icon);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect2, NewStats[i].labelKey.Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            float num4 = num2 + 4f + 22f + 4f;
            string text = sumCost.ToString();
            string text2 = totalHave.ToString();
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(num4, 0f, 90f, num3), text);
            Widgets.Label(new Rect(num4, num3, 90f, num3), text2);
            Text.Anchor = TextAnchor.MiddleLeft;
            float width = rect.width - num2 - 90f - 22f - 4f;
            Rect rect4 = new(num4 + 90f + 4f, num3, width, num3);
            if (rect4.width != cachedWidth)
            {
                cachedWidth = rect4.width;
                truncateCache.Clear();
            }
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }

        public void Accept()
        {
            foreach (GeneDefWithChance geneDefWithChance in selectedGenes)
            {
                if (geneDefWithChance.disabled)
                {
                    continue;
                }
                if (gene.TryConsumeResource(geneDefWithChance.cost))
                {
                    gene.TryForceGene(geneDefWithChance.geneDef, inheritable);
                }
            }
            gene.UpdateMetabolism();
            gene.DoEffects();
            Close();
        }

        private List<GeneDefWithChance> cachedGeneDefsInOrder;
        public List<GeneDefWithChance> GenesInOrder
        {
            get
            {
                if (cachedGeneDefsInOrder == null)
                {
                    cachedGeneDefsInOrder = new();
                    foreach (GeneDefWithChance allDef in allGenes)
                    {
                        cachedGeneDefsInOrder.Add(allDef);
                    }
                    cachedGeneDefsInOrder.SortBy((GeneDefWithChance x) => 0f - x.displayCategory.displayPriorityInXenotype, (GeneDefWithChance x) => x.displayCategory.label, (GeneDefWithChance x) => x.geneDef.displayOrderInCategory);
                }
                return cachedGeneDefsInOrder;
            }
        }

        public void DrawGenes(Rect rect)
        {
            hoveredAnyGene = false;
            GUI.BeginGroup(rect);
            float curY = 15f;
            //float num = curY;
            curY += 15f;
            float num2 = curY;
            Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
            var outerRect = new Rect(0f, curY, rect.width, rect.height - curY);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, rect2);
            bool? collapsed = null;
            DrawSection(rect, GenesInOrder, null, ref curY, ref unselectedHeight, adding: true, ref collapsed);
            var rect3 = new Rect(0f, 0f, rect.width, selectedHeight);
            DrawSection(rect3, selectedGenes, "SelectedGenes".Translate(), ref curY, ref selectedHeight, adding: false, ref selectedCollapsed);
            if (Event.current.type == EventType.Layout)
            {
                scrollHeight = curY - num2;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
            if (!hoveredAnyGene)
            {
                hoveredGene = null;
            }
        }

        private void DrawSection(Rect rect, List<GeneDefWithChance> genes, string label, ref float curY, ref float sectionHeight, bool adding, ref bool? collapsed)
        {
            float curX = 4f;
            Dialog_CreateChimera.DrawGenesSections_Label(ref rect, label, ref curY, adding, ref collapsed);
            if (collapsed == true)
            {
                if (Event.current.type == EventType.Layout)
                {
                    sectionHeight = 0f;
                }
                return;
            }
            Dialog_CreateChimera.DrawGenesSection_DrawRectFast(ref rect, ref curY, sectionHeight, adding, out float num, out bool flag, out float num2, out float num3, out float b, out Rect rect3);
            DrawGenesSection_Local(rect, genes, ref curY, ref sectionHeight, adding, ref curX, num, ref flag, num2, num3, b, rect3);
        }

        private void DrawGenesSection_Local(Rect rect, List<GeneDefWithChance> genes, ref float curY, ref float sectionHeight, bool adding, ref float curX, float num, ref bool flag, float num2, float num3, float b, Rect rect3)
        {
            if (!genes.Any())
            {
                XaG_UiUtility.MidleLabel_None(rect3);
            }
            else
            {
                GeneCategoryDef geneCategoryDef = null;
                int num5 = 0;
                for (int i = 0; i < genes.Count; i++)
                {
                    GeneCategoryDef currentGeneCategoryDef = genes[i].displayCategory;
                    bool flag2 = false;
                    if (curX + num2 > num3)
                    {
                        curX = 4f;
                        curY += GeneSize.y + 8f + 4f;
                        flag2 = true;
                    }
                    bool flag4 = collapsedCategories[currentGeneCategoryDef];
                    if (adding && geneCategoryDef != currentGeneCategoryDef)
                    {
                        if (!flag2 && flag)
                        {
                            curX = 4f;
                            curY += GeneSize.y + 8f + 4f;
                        }
                        geneCategoryDef = currentGeneCategoryDef;
                        Rect rect4 = new(curX, curY, rect.width - 8f, Text.LineHeight);

                        Rect position2 = new(rect4.x, rect4.y + (rect4.height - 18f) / 2f, 18f, 18f);
                        GUI.DrawTexture(position2, flag4 ? TexButton.Reveal : TexButton.Collapse);
                        if (Widgets.ButtonInvisible(rect4))
                        {
                            collapsedCategories[currentGeneCategoryDef] = !collapsedCategories[currentGeneCategoryDef];
                            if (collapsedCategories[currentGeneCategoryDef])
                            {
                                SoundDefOf.TabClose.PlayOneShotOnCamera();
                            }
                            else
                            {
                                SoundDefOf.TabOpen.PlayOneShotOnCamera();
                            }
                        }
                        if (num5 % 2 == 1)
                        {
                            Widgets.DrawLightHighlight(rect4);
                        }
                        if (Mouse.IsOver(rect4))
                        {
                            Widgets.DrawHighlight(rect4);
                        }
                        rect4.xMin += position2.width;

                        Widgets.Label(rect4, geneCategoryDef.LabelCap);
                        curY += rect4.height;
                        if (!flag4)
                        {
                            GUI.color = Color.grey;
                            Widgets.DrawLineHorizontal(curX, curY, rect.width - 8f);
                            GUI.color = Color.white;
                            curY += 10f;
                        }
                        num5++;
                    }
                    if (adding && flag4)
                    {
                        flag = false;
                        if (Event.current.type == EventType.Layout)
                        {
                            sectionHeight = curY - num;
                        }
                        continue;
                    }
                    curX = Mathf.Max(curX, b);
                    flag = true;
                    if (DrawGene(genes[i], !adding, ref curX, curY, num2, false))
                    {
                        if (selectedGenes.Contains(genes[i]))
                        {
                            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                            selectedGenes.Remove(genes[i]);
                        }
                        else
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera();
                            selectedGenes.Add(genes[i]);
                        }
                        OnGenesChanged();
                        break;
                    }
                }
            }
            if (!adding || flag)
            {
                curY += GeneSize.y + 12f;
            }
            if (Event.current.type == EventType.Layout)
            {
                sectionHeight = curY - num;
            }
        }

        protected List<GeneLeftChosenGroup> leftChosenGroups = new();

        private bool DrawGene(GeneDefWithChance geneWithChance, bool selectedSection, ref float curX, float curY, float packWidth, bool isMatch)
        {
            bool result = false;
            Rect rect = new(curX, curY, packWidth, GeneSize.y + 8f);
            bool selected = !selectedSection && selectedGenes.Contains(geneWithChance);
            bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneWithChance.geneDef)) || geneWithChance.disabled;
            Widgets.DrawOptionBackground(rect, selected);
            curX += 4f;
            DrawBiostats(geneWithChance.cost, ref curX, curY, 4f);
            Rect rect2 = new(curX, curY + 4f, GeneSize.x, GeneSize.y);
            if (isMatch)
            {
                Widgets.DrawStrongHighlight(rect2.ExpandedBy(6f));
            }
            GeneUIUtility.DrawGeneDef(geneWithChance.geneDef, rect2, (inheritable ? GeneType.Endogene : GeneType.Xenogene), () => GeneTip(geneWithChance, selectedSection), doBackground: false, clickable: false, overridden);
            curX += GeneSize.x + 4f;
            if (Mouse.IsOver(rect))
            {
                hoveredGene = geneWithChance.geneDef;
                hoveredAnyGene = true;
            }
            else if (hoveredGene != null && geneWithChance.geneDef.ConflictsWith(hoveredGene))
            {
                Widgets.DrawLightHighlight(rect);
            }
            if (Widgets.ButtonInvisible(rect))
            {
                result = true;
            }
            curX = Mathf.Max(curX, rect.xMax + 4f);
            return result;
        }

        public static void DrawBiostats(int geneMat, ref float curX, float curY, float margin = 6f)
        {
            float num2 = 0f;
            float num3 = Text.LineHeightOf(GameFont.Small);
            Rect iconRect = new(curX, curY + margin + num2, num3, num3);
            if (geneMat > 0)
            {
                XaG_UiUtility.DrawStat(iconRect, ReqTex, geneMat.ToString(), num3);
            }
            curX += 34f;
        }

        protected List<GeneDef> cachedOverriddenGenes = new();

        private string GeneTip(GeneDefWithChance geneWithChance, bool selectedSection)
        {
            string text = null;
            if (selectedSection)
            {
                if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneWithChance.geneDef))
                {
                    text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneWithChance.geneDef));
                }
                else if (cachedOverriddenGenes.Contains(geneWithChance.geneDef))
                {
                    text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneWithChance.geneDef)));
                }
            }
            if (selectedGenes.Contains(geneWithChance) && geneWithChance.geneDef.prerequisite != null && !Contains(selectedGenes, geneWithChance.geneDef.prerequisite))
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n\n";
                }
                text += ("MessageGeneMissingPrerequisite".Translate(geneWithChance.geneDef.label).CapitalizeFirst() + ": " + geneWithChance.geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
            }
            if (!text.NullOrEmpty())
            {
                text += "\n\n";
            }
            return text + (selectedGenes.Contains(geneWithChance) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
            static string GroupInfo(GeneLeftChosenGroup group)
            {
                if (group == null)
                {
                    return null;
                }
                return ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
            }
        }

        public void OnGenesChanged()
        {
            //selectedGenes.SortGeneDefs();
            leftChosenGroups.Clear();
            cachedOverriddenGenes.Clear();
            for (int k = 0; k < selectedGenes.Count; k++)
            {
                for (int l = k + 1; l < selectedGenes.Count; l++)
                {
                    if (!selectedGenes[k].geneDef.ConflictsWith(selectedGenes[l].geneDef))
                    {
                        continue;
                    }
                    int num = GenesInOrder.IndexOf(selectedGenes[k]);
                    int num2 = GenesInOrder.IndexOf(selectedGenes[l]);
                    GeneDefWithChance leftMost = ((num < num2) ? selectedGenes[k] : selectedGenes[l]);
                    GeneDefWithChance rightMost = ((num >= num2) ? selectedGenes[k] : selectedGenes[l]);
                    GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == leftMost.geneDef);
                    GeneLeftChosenGroup geneLeftChosenGroup2 = leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == rightMost.geneDef);
                    if (geneLeftChosenGroup == null)
                    {
                        geneLeftChosenGroup = new GeneLeftChosenGroup(leftMost.geneDef);
                        leftChosenGroups.Add(geneLeftChosenGroup);
                    }
                    if (geneLeftChosenGroup2 != null)
                    {
                        foreach (GeneDef overriddenGene in geneLeftChosenGroup2.overriddenGenes)
                        {
                            if (!geneLeftChosenGroup.overriddenGenes.Contains(overriddenGene))
                            {
                                geneLeftChosenGroup.overriddenGenes.Add(overriddenGene);
                            }
                            if (!cachedOverriddenGenes.Contains(overriddenGene))
                            {
                                cachedOverriddenGenes.Add(overriddenGene);
                            }
                        }
                        leftChosenGroups.Remove(geneLeftChosenGroup2);
                    }
                    if (!geneLeftChosenGroup.overriddenGenes.Contains(rightMost.geneDef))
                    {
                        geneLeftChosenGroup.overriddenGenes.Add(rightMost.geneDef);
                    }
                    if (!cachedOverriddenGenes.Contains(rightMost.geneDef))
                    {
                        cachedOverriddenGenes.Add(rightMost.geneDef);
                    }
                }
            }
            //foreach (GeneLeftChosenGroup leftChosenGroup in leftChosenGroups)
            //{
            //    leftChosenGroup.overriddenGenes.SortBy((GeneDef x) => selectedGenes.IndexOf(x));
            //}
            foreach (GeneDefWithChance geneDefChance in selectedGenes)
            {
                genesConflict = XaG_GeneUtility.ConflictWith(geneDefChance.geneDef, pawnGenes);
                if (genesConflict)
                {
                    break;
                }
            }
        }

        private bool genesConflict = false;
        public void DoBottomButtons(Rect rect)
        {
            var textInputRect = new Rect(rect.x, rect.y, 250, 32);
            var saveGenelineRect = new Rect(textInputRect.xMax + 10, textInputRect.y, 150, 32);
            var cannotReasonRect = new Rect(saveGenelineRect.xMax + 10, rect.y,
                rect.width - (textInputRect.width + saveGenelineRect.width + 20), 50);
            if (Widgets.ButtonText(saveGenelineRect, "WVC_XaG_ChimeraApply_Implant".Translate()) && CanAccept(true))
            {
                if (genesConflict)
                {
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_DialogEditShiftGenes_Warning".Translate() + "\n\n" + "WVC_XaG_DialogEditShiftGenes_ConflictWithPawnGenes".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), Accept));
                }
                else
                {
                    //Accept();
                    Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_DialogEditShiftGenes_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), Accept));
                }
                return;
            }
            if (leftChosenGroups.Any())
            {
                int num = leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
                GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups[0];
                string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1)) : string.Empty);
                DrawCannotReason(cannotReasonRect, text);
            }
            else if (SelectedGenes.Empty())
            {
                DrawCannotReason(cannotReasonRect, "WVC_XaG_GeneGeneticThief_NullGeneSet".Translate());
            }
            else if (AllGeneMat < ReqGeneMat)
            {
                DrawCannotReason(cannotReasonRect, "WVC_XaG_DialogEditShiftGenes_NeedMoreAmount".Translate());
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Tiny;
            var explanationRect = new Rect(rect.x, saveGenelineRect.yMax + 15, rect.width, 50);
            GUI.color = Color.grey;
            Widgets.Label(explanationRect, "WVC_XaG_DialogEditShiftGenes_Explanation".Translate());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
        }

        private static void DrawCannotReason(Rect rect, string text)
        {
            text = "WVC_XaG_DialogEditShiftGenes_CannotSave".Translate(text);
            GUI.color = ColorLibrary.RedReadable;
            var labelRect = new Rect(rect.x, rect.y, rect.width, rect.height);
            Widgets.Label(labelRect, text);
            GUI.color = Color.white;
        }

        public bool CanAccept(bool throwMessage = false)
        {
            if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.pawn, throwMessage))
            {
                return false;
            }
            if (SelectedGenes.Empty())
            {
                if (throwMessage)
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_NullGeneSet".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (SelectedGenes.Any((geneChance) => geneChance.disabled))
            {
                if (throwMessage)
                {
                    Messages.Message("WVC_XaG_DialogEditShiftGenes_PawnHasGene".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (leftChosenGroups.Any())
            {
                if (throwMessage)
                {
                    Messages.Message("WVC_XaG_GeneGeneticThief_ConflictingGenesMessage".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (AllGeneMat < ReqGeneMat)
            {
                if (throwMessage)
                {
                    Messages.Message("WVC_XaG_DialogEditShiftGenes_NeedMoreAmount".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            List<GeneDefWithChance> selectedGenes = SelectedGenes;
            foreach (GeneDefWithChance selectedGene in SelectedGenes)
            {
                if (selectedGene.geneDef.prerequisite != null && !Contains(selectedGenes, selectedGene.geneDef.prerequisite) && !pawnGenes.Contains(selectedGene.geneDef.prerequisite))
                {
                    if (throwMessage)
                    {
                        Messages.Message("MessageGeneMissingPrerequisite".Translate(selectedGene.geneDef.label).CapitalizeFirst() + ": " + selectedGene.geneDef.prerequisite.LabelCap, null, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
            }
            return true;
        }

        private static bool Contains(List<GeneDefWithChance> list, GeneDef geneDef)
        {
            foreach (GeneDefWithChance selectedGene in list)
            {
                if (selectedGene.geneDef == geneDef)
                {
                    return true;
                }
            }
            return true;
        }

    }

}
