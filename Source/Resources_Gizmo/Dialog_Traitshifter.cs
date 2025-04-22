using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Dialog_Traitshifter : Window
	{

		public List<TraitDef> allTraits;
		public List<TraitDef> pawnTraits;
		public List<TraitDef> lockedTraits;
		public Pawn pawn;

		public Dialog_Traitshifter(Pawn pawn)
        {
            this.pawn = pawn;
            UpdTraits(pawn);
            forcePause = true;
            doCloseButton = true;
        }

        private void UpdTraits(Pawn pawn)
        {
            this.allTraits = DefDatabase<TraitDef>.AllDefsListForReading.Where((def) => def.IsVanillaDef() || def.GetGenderSpecificCommonality(pawn.gender) > 0f).ToList();
            this.pawnTraits = new();
            lockedTraits = new();
            foreach (Trait item in pawn.story.traits.allTraits)
            {
                if (item.sourceGene != null || item.ScenForced)
                {
                    lockedTraits.Add(item.def);
                }
                pawnTraits.Add(item.def);
            }
        }

        protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)allTraits.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (TraitDef trait in allTraits)
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
                {
                    Rect rect = new(0f, num2, vector.x, vector.y);
                    TooltipHandler.TipRegion(rect, trait.degreeDatas.Count > 1 ? "WVC_XaG_Traitshifter_Degree".Translate() : trait.degreeDatas.FirstOrDefault().description.Formatted(pawn.Named("PAWN")));
                    if (num3 % 2 == 0)
                    {
                        Widgets.DrawAltRect(rect);
                    }
                    Widgets.BeginGroup(rect);
                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                    Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
                    if (lockedTraits.Contains(trait))
                    {
                        Widgets.Label(rect3, "Locked".Translate().CapitalizeFirst());
                    }
                    else if (Widgets.ButtonText(rect3, !pawnTraits.Contains(trait) ? "Add".Translate().CapitalizeFirst() : "Remove".Translate().CapitalizeFirst()))
                    {
                        Apply(trait);
                    }
                    Rect rect4 = new(0f, 0f, rect.width - rect3.width, rect.height);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(rect4, GetLabelCap(trait).Truncate(rect4.width));
                    Text.Anchor = TextAnchor.UpperLeft;
                    //Rect rect5 = new(0f, 0f, 36f, 36f);
                    //XaG_UiUtility.XaG_DefIcon(rect5, trait, 1.2f);
                    Widgets.EndGroup();
                }
                num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

        private string GetLabelCap(TraitDef trait)
        {
			string label = "";
			foreach (TraitDegreeData traitDegree in trait.degreeDatas)
            {
				if (!label.NullOrEmpty())
                {
					label += " / ";
				}
				label += traitDegree.GetLabelCapFor(pawn);
			}
            return label;
        }

        private void Apply(TraitDef trait)
		{
			Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Traitshifter_Warning".Translate() + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
			{
                TraitSet traitSet = pawn.story.traits;
				if (pawnTraits.Contains(trait))
				{
					Trait targetTrait = traitSet.allTraits.First((tr) => tr.def == trait);
					if (targetTrait.Suppressed)
                    {
                        targetTrait.suppressedByGene = null;
						targetTrait.suppressedByTrait = false;
                    }
                    traitSet.RemoveTrait(targetTrait, true);
				}
				else if (traitSet.allTraits.Count < 3)
				{
					if (trait.degreeDatas.Count > 1)
					{
                        Find.WindowStack.Add(new Dialog_Traitshifter_Degree(trait, this));
					}
					else
					{
                        AddTrait(trait, 0);
					}
				}
				else
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
				}
				traitSet.RecalculateSuppression();
                UpdTraits(pawn);
			});
			Find.WindowStack.Add(window);
		}

		public void AddTrait(TraitDef trait, int degree)
		{
			pawn.story.traits.GainTrait(new Trait(trait, degree), true);
		}

	}

	public class Dialog_Traitshifter_Degree : Window
	{

		public TraitDef traitDef;
		public Dialog_Traitshifter dialog_Traitshifter;

		public Dialog_Traitshifter_Degree(TraitDef traitDef, Dialog_Traitshifter dialog_Traitshifter)
		{
			this.dialog_Traitshifter = dialog_Traitshifter;
			this.traitDef = traitDef;
			forcePause = true;
			doCloseButton = true;
		}

		protected Vector2 scrollPosition;
		protected float bottomAreaHeight;

		public override void DoWindowContents(Rect inRect)
		{
			Vector2 vector = new(inRect.width - 16f, 40f);
			float y = vector.y;
			float height = (float)traitDef.degreeDatas.Count * y;
			Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
			float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
			Rect outRect = inRect.TopPartPixels(num);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num2 = 0f;
			int num3 = 0;
			foreach (TraitDegreeData traitDegreeData in traitDef.degreeDatas)
			{
				if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
				{
					Rect rect = new(0f, num2, vector.x, vector.y);
					TooltipHandler.TipRegion(rect, traitDef.description.Formatted(dialog_Traitshifter.pawn.Named("PAWN")));
					if (num3 % 2 == 0)
					{
						Widgets.DrawAltRect(rect);
					}
					Widgets.BeginGroup(rect);
					GUI.color = Color.white;
					Text.Font = GameFont.Small;
					Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
					if (Widgets.ButtonText(rect3, "Add".Translate().CapitalizeFirst()))
					{
						dialog_Traitshifter.AddTrait(traitDef, traitDegreeData.degree);
						Close();
					}
					Rect rect4 = new(0f, 0f, 200f, rect.height);
					Text.Anchor = TextAnchor.MiddleLeft;
					Widgets.Label(rect4, traitDegreeData.GetLabelCapFor(dialog_Traitshifter.pawn).Truncate(rect4.width * 1.8f));
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.EndGroup();
				}
				num2 += vector.y;
				num3++;
			}
			Widgets.EndScrollView();
		}

	}

}
