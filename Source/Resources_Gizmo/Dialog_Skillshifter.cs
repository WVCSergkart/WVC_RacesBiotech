using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class Dialog_Skillshifter : Window
    {

        public List<SkillRecord> skills;
        public int leftSkillPoints = 0;
        public int maxSkillPoints = 0;
        public Pawn pawn;
        public Gene_Skillshifter gene;

        public Dialog_Skillshifter(Gene_Skillshifter gene)
        {
            this.gene = gene;
            this.pawn = gene.pawn;
            //maxSkillPoints = pawn.skills.skills.Sum((x) => x.levelInt);
            maxSkillPoints = 0;
            foreach (SkillRecord skillRecord in skills)
            {
                int skillLevel = skillRecord.levelInt;
                while (skillLevel > 0)
                {
                    maxSkillPoints += skillLevel;
                    skillLevel--;
                }
            }
            UpdSkills(pawn);
            forcePause = true;
            doCloseButton = false;
        }

        private void UpdSkills(Pawn pawn)
        {
            this.skills = pawn.skills.skills;
        }

        protected Vector2 scrollPosition;
        protected float bottomAreaHeight;

        public override void DoWindowContents(Rect inRect)
        {
            Vector2 vector = new(inRect.width - 16f, 40f);
            float y = vector.y;
            float height = (float)skills.Count * y;
            Rect viewRect = new(0f, 0f, inRect.width - 16f, height);
            float num = inRect.height - Window.CloseButSize.y - bottomAreaHeight - 18f;
            Rect outRect = inRect.TopPartPixels(num);
            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            float num2 = 0f;
            int num3 = 0;
            foreach (SkillRecord skillRecord in skills.ToList())
            {
                if (num2 + vector.y >= scrollPosition.y && num2 <= scrollPosition.y + outRect.height)
                {
                    Rect rect = new(0f, num2, vector.x, vector.y);
                    TooltipHandler.TipRegion(rect, skillRecord.def.description);
                    if (num3 % 2 == 0)
                    {
                        Widgets.DrawAltRect(rect);
                    }
                    Widgets.BeginGroup(rect);
                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                    Rect rect3 = new(rect.width - 100f, (rect.height - 36f) / 2f, 100f, 36f);
                    Rect buttonPlus = new(rect3.x, rect3.y, rect3.width / 2, rect3.height);
                    if (Widgets.ButtonText(buttonPlus, "+"))
                    {
                        if (skillRecord.levelInt < 20 && leftSkillPoints >= skillRecord.levelInt + 1)
                        {
                            skillRecord.levelInt++;
                            leftSkillPoints -= skillRecord.levelInt;
                            UpdSkills(pawn);
                            anyChanges = true;
                        }
                        else
                        {
                            SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        }
                    }
                    Rect buttonMinus = new(rect3.x + (rect3.width / 2), rect3.y, rect3.width / 2, rect3.height);
                    if (Widgets.ButtonText(buttonMinus, "-"))
                    {
                        if (skillRecord.levelInt > 0)
                        {
                            leftSkillPoints += skillRecord.levelInt;
                            skillRecord.levelInt--;
                            UpdSkills(pawn);
                            anyChanges = true;
                        }
                        else
                        {
                            SoundDefOf.ClickReject.PlayOneShotOnCamera();
                        }
                    }
                    Rect rect4 = new(4f, 0f, rect.width - rect3.width, rect.height);
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Widgets.Label(rect4, skillRecord.def.LabelCap.Truncate(rect4.width) + " " + skillRecord.levelInt);
                    Text.Anchor = TextAnchor.UpperLeft;
                    Widgets.EndGroup();
                }
                num2 += vector.y;
                num3++;
            }
            Widgets.EndScrollView();
            SkillPointsLeft(inRect);
        }

        public bool anyChanges = false;

        private void SkillPointsLeft(Rect inRect)
        {
            //zeroRect = windowRect.AtZero();
            Rect buttonCloseRect = new(inRect.width / 2f - CloseButSize.x / 2f, inRect.height - 40f, CloseButSize.x, CloseButSize.y);
            string label = "WVC_XaG_Skillshifter_SkillPointsLeft".Translate(leftSkillPoints, maxSkillPoints);
            Rect pointsRect = new(buttonCloseRect.x - Text.CalcSize(label).x - 18f, buttonCloseRect.y + (Text.CalcSize(label).y / 2), Text.CalcSize(label).x, Text.CalcSize(label).y);
            Widgets.Label(pointsRect, label);
            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(buttonCloseRect, CloseButtonText))
            {
                if (leftSkillPoints > 0)
                {
                    Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_Skillshifter_Warning".Translate(leftSkillPoints) + "\n\n" + "WouldYouLikeToContinue".Translate(), delegate
                    {
                        Close();
                    });
                    Find.WindowStack.Add(window);
                }
                else
                {
                    Close();
                }
            }
        }

        private void Close()
        {
            if (anyChanges)
            {
                gene.DoEffects();
            }
            base.Close();
        }

    }

}
