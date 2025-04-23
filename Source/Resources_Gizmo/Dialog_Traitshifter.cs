using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
 //   public class Dialog_Traitshifter_Simple : Window
	//{

	//	public List<TraitDef> allTraits;
	//	public List<TraitDef> pawnTraits;
	//	public List<TraitDef> lockedTraits;
	//	public Pawn pawn;

	//	public Dialog_Traitshifter_Simple(Pawn pawn)
 //       {
 //           this.pawn = pawn;
 //           UpdTraits(pawn);
 //           forcePause = true;
 //           doCloseButton = true;
 //       }

 //       private void UpdTraits(Pawn pawn)
 //       {
 //           this.allTraits = DefDatabase<TraitDef>.AllDefsListForReading.Where((def) => def.IsVanillaDef() || def.GetGenderSpecificCommonality(pawn.gender) > 0f).ToList();
 //           this.pawnTraits = new();
 //           lockedTraits = new();
 //           foreach (Trait item in pawn.story.traits.allTraits)
 //           {
 //               if (item.sourceGene != null || item.ScenForced)
 //               {
 //                   lockedTraits.Add(item.def);
 //               }
 //               pawnTraits.Add(item.def);
 //           }
 //       }

 //       protected Vector2 scrollPosition;
	//	protected float bottomAreaHeight;

	//	public override void DoWindowContents(Rect inRect)
	//	{
	//		Vector2 vector = new(inRect.width - 16f, 40f);
	//		float y = vector.y;
	//		float height = (float)allTraits.Count * y;
	//		Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
	//		float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
	//		Rect outRect = inRect.TopPartPixels(num);
	//		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
	//		float num2 = 0f;
	//		int num3 = 0;
	//		foreach (TraitDef trait in allTraits)
	//		{
	//			if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
 //               {
 //                   Rect rect = new(0f, num2, vector.x, vector.y);
 //                   TooltipHandler.TipRegion(rect, trait.degreeDatas.Count > 1 ? "WVC_XaG_Traitshifter_Degree".Translate() : trait.degreeDatas.FirstOrDefault().description.Formatted(pawn.Named("PAWN")));
 //                   if (num3 % 2 == 0)
 //                   {
 //                       Widgets.DrawAltRect(rect);
 //                   }
 //                   Widgets.BeginGroup(rect);
 //                   GUI.color = Color.white;
 //                   Text.Font = GameFont.Small;
 //                   Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
 //                   if (lockedTraits.Contains(trait))
 //                   {
 //                       Widgets.Label(rect3, "Locked".Translate().CapitalizeFirst());
 //                   }
 //                   else if (Widgets.ButtonText(rect3, !pawnTraits.Contains(trait) ? "Add".Translate().CapitalizeFirst() : "Remove".Translate().CapitalizeFirst()))
 //                   {
 //                       Apply(trait);
 //                   }
 //                   Rect rect4 = new(0f, 0f, rect.width - rect3.width, rect.height);
 //                   Text.Anchor = TextAnchor.MiddleLeft;
 //                   Widgets.Label(rect4, GetLabelCap(trait).Truncate(rect4.width));
 //                   Widgets.EndGroup();
 //               }
 //               num2 += vector.y;
	//			num3++;
	//		}
	//		Widgets.EndScrollView();
	//	}

 //       private string GetLabelCap(TraitDef trait)
 //       {
	//		string label = "";
	//		foreach (TraitDegreeData traitDegree in trait.degreeDatas)
 //           {
	//			if (!label.NullOrEmpty())
 //               {
	//				label += " / ";
	//			}
	//			label += traitDegree.GetLabelCapFor(pawn);
	//		}
 //           return label;
 //       }

 //       private void Apply(TraitDef trait)
	//	{
	//		Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Traitshifter_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
	//		{
 //               TraitSet traitSet = pawn.story.traits;
	//			if (pawnTraits.Contains(trait))
	//			{
	//				Trait targetTrait = traitSet.allTraits.First((tr) => tr.def == trait);
	//				if (targetTrait.Suppressed)
 //                   {
 //                       targetTrait.suppressedByGene = null;
	//					targetTrait.suppressedByTrait = false;
 //                   }
 //                   traitSet.RemoveTrait(targetTrait, true);
	//			}
	//			else if (traitSet.allTraits.Count < 3)
	//			{
	//				if (trait.degreeDatas.Count > 1)
	//				{
 //                       Find.WindowStack.Add(new Dialog_Traitshifter_Degree(trait, this));
	//				}
	//				else
	//				{
 //                       AddTrait(trait, 0);
	//				}
	//			}
	//			else
 //               {
 //                   SoundDefOf.ClickReject.PlayOneShotOnCamera();
	//			}
	//			traitSet.RecalculateSuppression();
 //               UpdTraits(pawn);
	//		});
	//		Find.WindowStack.Add(window);
	//	}

	//	public void AddTrait(TraitDef trait, int degree)
	//	{
	//		pawn.story.traits.GainTrait(new Trait(trait, degree), true);
	//	}

	//}

	//public class Dialog_Traitshifter_Degree : Window
	//{

	//	public TraitDef traitDef;
	//	public Dialog_Traitshifter_Simple dialog_Traitshifter;

	//	public Dialog_Traitshifter_Degree(TraitDef traitDef, Dialog_Traitshifter_Simple dialog_Traitshifter)
	//	{
	//		this.dialog_Traitshifter = dialog_Traitshifter;
	//		this.traitDef = traitDef;
	//		forcePause = true;
	//		doCloseButton = true;
	//	}

	//	protected Vector2 scrollPosition;
	//	protected float bottomAreaHeight;

	//	public override void DoWindowContents(Rect inRect)
	//	{
	//		Vector2 vector = new(inRect.width - 16f, 40f);
	//		float y = vector.y;
	//		float height = (float)traitDef.degreeDatas.Count * y;
	//		Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
	//		float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
	//		Rect outRect = inRect.TopPartPixels(num);
	//		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
	//		float num2 = 0f;
	//		int num3 = 0;
	//		foreach (TraitDegreeData traitDegreeData in traitDef.degreeDatas)
	//		{
	//			if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
	//			{
	//				Rect rect = new(0f, num2, vector.x, vector.y);
	//				TooltipHandler.TipRegion(rect, traitDef.description.Formatted(dialog_Traitshifter.pawn.Named("PAWN")));
	//				if (num3 % 2 == 0)
	//				{
	//					Widgets.DrawAltRect(rect);
	//				}
	//				Widgets.BeginGroup(rect);
	//				GUI.color = Color.white;
	//				Text.Font = GameFont.Small;
	//				Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
	//				if (Widgets.ButtonText(rect3, "Add".Translate().CapitalizeFirst()))
	//				{
	//					dialog_Traitshifter.AddTrait(traitDef, traitDegreeData.degree);
	//					Close();
	//				}
	//				Rect rect4 = new(0f, 0f, 200f, rect.height);
	//				Text.Anchor = TextAnchor.MiddleLeft;
	//				Widgets.Label(rect4, traitDegreeData.GetLabelCapFor(dialog_Traitshifter.pawn).Truncate(rect4.width * 1.8f));
	//				Text.Anchor = TextAnchor.UpperLeft;
	//				Widgets.EndGroup();
	//			}
	//			num2 += vector.y;
	//			num3++;
	//		}
	//		Widgets.EndScrollView();
	//	}

 //   }

    public class Dialog_Traitshifter : Window
    {

        public List<TraitDefHolder> allTraits;
        public List<TraitDefHolder> pawnTraits;
        //public List<TraitDefHolder> lockedDegreeTraits;
        public List<TraitDef> lockedTraits;
        public Pawn pawn;
        public Gene_Traitshifter gene;
        public List<TraitDefHolder> conflictWithLockedTraits;

        public Dialog_Traitshifter(Gene_Traitshifter gene)
        {
            forcePause = true;
            absorbInputAroundWindow = true;
            doCloseX = true;
            this.gene = gene;
            this.pawn = gene.pawn;
            UpdTraits(pawn);
            OnTraitsChanged();
        }

        private void UpdTraits(Pawn pawn)
        {
            this.allTraits = new();
            lockedTraits = new();
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (trait.sourceGene != null || trait.ScenForced || trait.def.GetGenderSpecificCommonality(pawn.gender) <= 0f)
                {
                    lockedTraits.Add(trait.def);
                }
            }
            foreach (TraitDef traitDef in DefDatabase<TraitDef>.AllDefsListForReading.Where((def) => def.GetGenderSpecificCommonality(pawn.gender) > 0f).ToList())
            {
                foreach (TraitDegreeData degreeData in traitDef.degreeDatas)
                {
                    TraitDefHolder newHolder = new();
                    newHolder.traitDef = traitDef;
                    newHolder.traitDegreeData = degreeData;
                    newHolder.targetPawn = pawn;
                    newHolder.traitDegreeIndex = degreeData.degree;
                    if (lockedTraits.Contains(traitDef))
                    {
                        newHolder.locked = true;
                    }
                    this.allTraits.Add(newHolder);
                }
            }
            this.pawnTraits = new();
            selectedTraitHolders = new();
            foreach (TraitDefHolder holder in allTraits)
            {
                foreach (Trait trait in pawn.story.traits.allTraits)
                {
                    if (!holder.IsSame(trait))
                    {
                        continue;
                    }
                    pawnTraits.Add(holder);
                    selectedTraitHolders.Add(holder);
                }
            }
            conflictWithLockedTraits = new();
            foreach (TraitDefHolder holder in allTraits)
            {
                foreach (TraitDef traitDef in lockedTraits)
                {
                    if (holder.IsSame(traitDef) || holder.ConflictsWith(traitDef))
                    {
                        conflictWithLockedTraits.Add(holder);
                    }
                }
            }
        }

        private List<TraitDefHolder> cachedTraitsDegree;
        public List<TraitDefHolder> TraitsInOrder
        {
            get
            {
                if (cachedTraitsDegree == null)
                {
                    cachedTraitsDegree = new();
                    foreach (TraitDefHolder allDef in allTraits)
                    {
                        //if (lockedTraits.Contains(allDef.traitDef))
                        //{
                        //    allDef.locked = true;
                        //}
                        cachedTraitsDegree.Add(allDef);
                    }
                }
                return cachedTraitsDegree;
            }
        }

        protected float scrollHeight;

        protected Vector2 scrollPosition;

        protected float selectedHeight;

        protected float unselectedHeight;

        //public static readonly Vector2 GeneSize = new(87f, 68f);

        //public List<GeneDefWithChance> allGenes = new();

        //protected List<GeneDefWithChance> selectedGenes = new();

        protected bool? selectedCollapsed = false;

        //protected HashSet<GeneCategoryDef> matchingCategories = new();

        //protected Dictionary<GeneCategoryDef, bool> collapsedCategories = new();

        protected bool hoveredAnyTrait;

        protected TraitDefHolder hoveredTraitDegree;

        public List<TraitDefHolder> selectedTraitHolders = new();

        public override Vector2 InitialSize => new(750, 800);
        //public List<GeneDefWithChance> SelectedGenes => selectedGenes;
        protected static readonly Vector2 ButSize = new(150f, 38f);

        public int MaxTraits => (int)WVC_Biotech.settings.traitshifter_MaxTraits;

        private int? cachedCount;
        public int CurrentTraits
        {
            get
            {
                if (!cachedCount.HasValue)
                {
                    cachedCount = selectedTraitHolders.Where((holder) => !holder.locked).ToList().Count;
                }
                return cachedCount.Value;
            }
        }

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
            Draw(bioStatsRect, MaxTraits, CurrentTraits);
            Rect bottomAreaRect = new(bioStatsRect.x, bioStatsRect.yMax + 10, bioStatsRect.width, 75);
            DoBottomButtons(bottomAreaRect);
        }

        private static float cachedWidth;

        public static readonly CachedTexture ReqTex = new("WVC/UI/XaG_General/GenMatTex_Req");
        public static readonly CachedTexture HasTex = new("WVC/UI/XaG_General/GenMatTex_Has");

        private readonly Dialog_EditShiftGenes.GeneMatStatData[] NewStats = new Dialog_EditShiftGenes.GeneMatStatData[2]
        {
            new Dialog_EditShiftGenes.GeneMatStatData("WVC_XaG_Traitshifter_MaxTraits", "WVC_XaG_Traitshifter_MaxTraitsDesc", HasTex.Texture),
            new Dialog_EditShiftGenes.GeneMatStatData("WVC_XaG_Traitshifter_SelectedTraits", "WVC_XaG_Traitshifter_SelectedTraitsDesc", ReqTex.Texture),
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
            try
            {
                TraitSet traitSet = pawn.story.traits;
                //foreach (TraitDegreeData trait in lockedDegreeTraits)
                //{
                //    if (selectedDegrees.Contains(trait))
                //    {
                //        selectedDegrees.Remove(trait);
                //    }
                //}
                //foreach (Trait trait in traitSet.allTraits.ToList())
                //{
                //    if (trait.sourceGene == null && !selectedDegrees.Contains(trait.CurrentData) && allTraits.Contains(trait.def))
                //    {
                //        trait.suppressedByGene = null;
                //        trait.suppressedByTrait = false;
                //        traitSet.RemoveTrait(trait, true);
                //    }
                //}
                //foreach (TraitDegreeData degree in selectedDegrees)
                //{
                //    Trait newTrait = new(allTraits.First((trait) => trait.degreeDatas.Contains(degree)), degree.degree);
                //    traitSet.GainTrait(newTrait, true);
                //}
                foreach (Trait trait in traitSet.allTraits.ToList())
                {
                    if (!lockedTraits.Contains(trait.def))
                    {
                        trait.suppressedByGene = null;
                        trait.suppressedByTrait = false;
                        traitSet.RemoveTrait(trait, true);
                    }
                }
                foreach (TraitDefHolder holder in selectedTraitHolders)
                {
                    if (!lockedTraits.Contains(holder.traitDef) && holder.CanAdd() && holder.traitDegreeIndex.HasValue)
                    {
                        traitSet.GainTrait(new (holder.traitDef, holder.traitDegreeIndex.Value), true);
                    }
                }
                traitSet.RecalculateSuppression();
                gene.DoEffects();
            }
            catch (Exception arg)
            {
                Log.Error("Failed update traits for: " + pawn.NameFullColored + ". Reason: " + arg);
            }
            Close();
        }

        public void DrawGenes(Rect rect)
        {
            hoveredAnyTrait = false;
            GUI.BeginGroup(rect);
            float curY = 15f;
            //float num = curY;
            curY += 15f;
            float num2 = curY;
            Rect rect2 = new(0f, curY, rect.width - 16f, scrollHeight);
            var outerRect = new Rect(0f, curY, rect.width, rect.height - curY);
            Widgets.BeginScrollView(outerRect, ref scrollPosition, rect2);
            bool? collapsed = null;
            DrawSection(rect, TraitsInOrder, null, ref curY, ref unselectedHeight, ref collapsed, false);
            var rect3 = new Rect(0f, 0f, rect.width, selectedHeight);
            DrawSection(rect3, selectedTraitHolders, "Traits".Translate(), ref curY, ref selectedHeight, ref selectedCollapsed, true);
            if (Event.current.type == EventType.Layout)
            {
                scrollHeight = curY - num2;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
            if (!hoveredAnyTrait)
            {
                hoveredTraitDegree = null;
            }
        }

        private void DrawSection(Rect rect, List<TraitDefHolder> traitDegrees, string label, ref float curY, ref float sectionHeight, ref bool? collapsed, bool adding)
        {
            float curX = 4f;
            if (!label.NullOrEmpty())
            {
                Rect rect2 = new(0f, curY, rect.width, Text.LineHeight);
                //rect2.xMax -= Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f;
                if (collapsed.HasValue)
                {
                    Rect position = new(rect2.x, rect2.y + (rect2.height - 18f) / 2f, 18f, 18f);
                    GUI.DrawTexture(position, collapsed.Value ? TexButton.Reveal : TexButton.Collapse);
                    if (Widgets.ButtonInvisible(rect2))
                    {
                        collapsed = !collapsed;
                        if (collapsed.Value)
                        {
                            SoundDefOf.TabClose.PlayOneShotOnCamera();
                        }
                        else
                        {
                            SoundDefOf.TabOpen.PlayOneShotOnCamera();
                        }
                    }
                    if (Mouse.IsOver(rect2))
                    {
                        Widgets.DrawHighlight(rect2);
                    }
                    rect2.xMin += position.width;
                }
                Widgets.Label(rect2, label);
                curY += Text.LineHeight + 3f;
            }
            if (collapsed == true)
            {
                if (Event.current.type == EventType.Layout)
                {
                    sectionHeight = 0f;
                }
                return;
            }
            float num = curY;
            Rect rect3 = new(0f, curY, rect.width, sectionHeight);
            if (adding)
            {
                Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
            }
            curY += 4f;
            if (!traitDegrees.Any())
            {
                XaG_UiUtility.MiddleLabel_None(rect3);
            }
            else
            {
                for (int i = 0; i < traitDegrees.Count; i++)
                {
                    TraitDefHolder traitDegree = traitDegrees[i];
                    string labelCap = traitDegree.LabelCap;
                    float num2 = 12f + Text.CalcSize(labelCap).x;
                    //float num2 = 32f;
                    float num3 = rect.width - 16f;
                    //float num4 = num2 + 4f;
                    //float b = (num3 - num4 * Mathf.Floor(num3 / num4)) / 2f;
                    if (curX + num2 > num3)
                    {
                        curX = 4f;
                        curY += 26;
                    }
                    curX = Mathf.Max(curX, 4);
                    if (DrawTraitDegree(labelCap, traitDegree, ref curX, curY, num2))
                    {
                        if (!lockedTraits.Contains(traitDegree.traitDef))
                        {
                            if (selectedTraitHolders.Contains(traitDegree))
                            {
                                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                                selectedTraitHolders.Remove(traitDegree);
                            }
                            else
                            {
                                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                                selectedTraitHolders.Add(traitDegree);
                            }
                        }
                        else
                        {
                            SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        }
                        OnTraitsChanged();
                        break;
                    }
                }
            }
            curY += 34f;
            if (Event.current.type == EventType.Layout)
            {
                sectionHeight = curY - num;
            }
        }


        private bool DrawTraitDegree(string labelCap, TraitDefHolder holder, ref float curX, float curY, float packWidth)
        {
            bool result = false;
            Rect rect = new(curX, curY, packWidth, 22);
            bool selected = selectedTraitHolders.Contains(holder);
            //Widgets.DrawOptionBackground(rect, selected);
            Color color3 = GUI.color;
            if (selected)
            {
                GUI.color = ColorLibrary.DarkOrange;
                Widgets.DrawBox(rect.ExpandedBy(1f), 1);
            }
            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUI.DrawTexture(rect, BaseContent.WhiteTex);
            GUI.color = color3;
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
            }
            curX += 5f;
            //Rect xenoRect = new(curX, curY + 4f, Text.CalcSize(labelCap).x + 8f, 32);
            bool locked = holder.locked;
            bool overridden = !locked && (conflictingTraits.Contains(holder) || conflictWithLockedTraits.Contains(holder) || (!selected && conflictingWithSelectedTrait.Contains(holder)));
            DrawLabel(labelCap, rect, overridden, locked);
            if (Mouse.IsOver(rect))
            {
                string text = holder.Description;
                if (locked)
                {
                    text += "\n\n" + "WVC_XaG_Traitshifter_LockedTrait".Translate().Colorize(ColoredText.SubtleGrayColor);
                }
                else
                {
                    text += "\n\n" + (selected ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
                }
                TooltipHandler.TipRegion(rect, text);
            }
            curX += Text.CalcSize(labelCap).x;
            if (Widgets.ButtonInvisible(rect))
            {
                result = true;
            }
            curX = Mathf.Max(curX, rect.xMax + 4f);
            return result;
        }

        public void DrawLabel(string labelCap, Rect traitRect, bool overridden, bool locked)
        {
            GUI.BeginGroup(traitRect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect rect = traitRect.AtZero();
            Text.Font = GameFont.Small;
            float num2 = Text.CalcHeight(labelCap, rect.width);
            Rect rect3 = new(0f, rect.y, rect.width, num2);
            //GUI.DrawTexture(new(rect3.x, rect3.y, Text.CalcSize(labelCap).x + 4f, num2), TexUI.GrayTextBG);
            if (overridden)
            {
                GUI.color = Color.grey;
            }
            else if (locked)
            {
                GUI.color = ColoredText.GeneColor;
            }
            Widgets.Label(new Rect(rect3.x + 5f, rect3.y, rect3.width - 10f, rect3.height), labelCap);
            //Widgets.Label(rect3, labelCap);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }

        //private struct TraitsRectSection
        //{
        //    public Rect rect;

        //    public Action<Rect> drawer;

        //    //public float calculatedSize;
        //}

        //public void DrawTraits(Rect rect)
        //{
        //    Widgets.BeginGroup(rect);
        //    List<TraitsRectSection> list = new();
        //    float stackHeight = 0f;
        //    float num2 = 0;
        //    Rect rect2 = GenUI.DrawElementStack(new Rect(0f, 0f, rect.width - 5f, rect.height), 22f, TraitsInOrder, delegate
        //    {
        //    }, (TraitDegreeData trait) => Text.CalcSize(trait.LabelCap).x + 10f, 4f, 5f, allowOrderOptimization: false);
        //    num2 += rect2.height;
        //    stackHeight = rect2.height;
        //    list.Add(new TraitsRectSection
        //    {
        //        rect = new Rect(0f, 0f, rect.width, num2),
        //        drawer = delegate (Rect sectionRect)
        //        {
        //            float currentY3 = sectionRect.y;
        //            Widgets.Label(new Rect(sectionRect.x, currentY3, 200f, 30f), "Traits".Translate().AsTipTitle());
        //            currentY3 += 24f;
        //            GenUI.DrawElementStack(new Rect(sectionRect.x, currentY3, rect.width - 5f, stackHeight), 22f, TraitsInOrder, delegate (Rect r, TraitDegreeData trait)
        //            {
        //                Color color3 = GUI.color;
        //                GUI.color = new Color(1f, 1f, 1f, 0.1f);
        //                GUI.DrawTexture(r, BaseContent.WhiteTex);
        //                GUI.color = color3;
        //                if (Mouse.IsOver(r))
        //                {
        //                    Widgets.DrawHighlight(r);
        //                }
        //                //if (trait.Suppressed)
        //                //{
        //                //    GUI.color = ColoredText.SubtleGrayColor;
        //                //}
        //                //else if (trait.sourceGene != null)
        //                //{
        //                //    GUI.color = ColoredText.GeneColor;
        //                //}
        //                Widgets.Label(new Rect(r.x + 5f, r.y, r.width - 10f, r.height), trait.LabelCap);
        //                GUI.color = Color.white;
        //                if (Mouse.IsOver(r))
        //                {
        //                    TraitDegreeData trLocal = trait;
        //                    TooltipHandler.TipRegion(tip: new TipSignal(() => trLocal.description.Formatted(pawn.Named("PAWN")), (int)currentY3 * 37), rect: r);
        //                }
        //            }, (TraitDegreeData trait) => Text.CalcSize(trait.LabelCap).x + 10f, 4f, 5f, allowOrderOptimization: false);
        //        }
        //    });
        //}

        //private string GeneTip(TraitDegreeData trait)
        //{
        //    return (selectedDegrees.Contains(trait) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
        //}

        public List<TraitDefHolder> conflictingTraits = new();
        public List<TraitDefHolder> conflictingWithSelectedTrait = new();

        public void OnTraitsChanged()
        {
            cachedCount = null;
            conflictingTraits = new();
            conflictingWithSelectedTrait = new();
            //List<TraitDef> traitDefs = new();
            //foreach (TraitDegreeData degreeData in selectedDegrees)
            //{
            //    traitDefs.Add(allTraits.First((trait) => trait.degreeDatas.Contains(degreeData)));
            //}
            //foreach (TraitDef traitDef in traitDefs)
            //{
            //    if (traitDefs.FindAll((def) => def == traitDef).Count > 1)
            //    {
            //        foreach (TraitDegreeData selectedData in traitDef.degreeDatas)
            //        {
            //            conflictingTraits.Add(selectedData);
            //        }
            //    }
            //    foreach (TraitDef traitDef2 in traitDefs)
            //    {
            //        if (traitDef == traitDef2)
            //        {
            //            continue;
            //        }
            //        if (traitDef.ConflictsWith(traitDef2))
            //        {
            //            foreach (TraitDegreeData selectedData in traitDef.degreeDatas)
            //            {
            //                conflictingTraits.Add(selectedData);
            //            }
            //        }
            //    }
            //    foreach (TraitDef lockedTrait in lockedTraits)
            //    {
            //        if (lockedTrait.ConflictsWith(traitDef))
            //        {
            //            foreach (TraitDegreeData selectedData in traitDef.degreeDatas)
            //            {
            //                conflictingTraits.Add(selectedData);
            //            }
            //        }
            //    }
            //}
            //foreach (TraitDegreeData lockedTrait in lockedDegreeTraits)
            //{
            //    if (selectedDegrees.Contains(lockedTrait))
            //    {
            //        conflictingTraits.Add(lockedTrait);
            //    }
            //}
            foreach (TraitDefHolder holder in selectedTraitHolders)
            {
                if (selectedTraitHolders.FindAll((def) => def.traitDef == holder.traitDef).Count > 1)
                {
                    conflictingTraits.Add(holder);
                }
                foreach (TraitDefHolder holder2 in selectedTraitHolders)
                {
                    if (!holder.IsSame(holder2) && holder.ConflictsWith(holder2))
                    {
                        conflictingTraits.Add(holder);
                        conflictingTraits.Add(holder2);
                    }
                }
                foreach (TraitDefHolder holder2 in pawnTraits)
                {
                    if (!holder.IsSame(holder2) && holder.ConflictsWith(holder2))
                    {
                        conflictingTraits.Add(holder);
                        conflictingTraits.Add(holder2);
                    }
                }
            }
            foreach (TraitDefHolder holder in allTraits)
            {
                foreach (TraitDefHolder holder2 in selectedTraitHolders)
                {
                    if (holder.ConflictsWith(holder2))
                    {
                        conflictingWithSelectedTrait.Add(holder);
                    }
                }
            }
        }

        public void DoBottomButtons(Rect rect)
        {
            var textInputRect = new Rect(rect.x, rect.y, 250, 32);
            var saveGenelineRect = new Rect(textInputRect.xMax + 10, textInputRect.y, 150, 32);
            var cannotReasonRect = new Rect(saveGenelineRect.xMax + 10, rect.y,
                rect.width - (textInputRect.width + saveGenelineRect.width + 20), 50);
            if (Widgets.ButtonText(saveGenelineRect, "WVC_XaG_Traitshifter_Apply".Translate()) && CanAccept(true))
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WVC_XaG_Traitshifter_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), Accept));
                return;
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Tiny;
            var explanationRect = new Rect(rect.x, saveGenelineRect.yMax + 15, rect.width, 50);
            GUI.color = Color.grey;
            Widgets.Label(explanationRect, "WVC_XaG_Traitshifter_Desc".Translate());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
        }

        //private static void DrawCannotReason(Rect rect, string text)
        //{
        //    text = "WVC_XaG_DialogEditShiftGenes_CannotSave".Translate(text);
        //    GUI.color = ColorLibrary.RedReadable;
        //    var labelRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        //    Widgets.Label(labelRect, text);
        //    GUI.color = Color.white;
        //}

        public bool CanAccept(bool throwMessage = false)
        {
            if (!GeneResourceUtility.CanDo_ShifterGeneticStuff(gene.pawn, throwMessage))
            {
                return false;
            }
            if (CurrentTraits > MaxTraits)
            {
                Messages.Message("WVC_XaG_Traitshifter_ToManyTraits".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            if (!conflictingTraits.Empty())
            {
                Messages.Message("WVC_XaG_Traitshifter_ConflictingTraits".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                return false;
            }
            return true;
        }

    }

}
