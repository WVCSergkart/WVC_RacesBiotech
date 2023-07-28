using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class UndeadUtility
    {

        public static void ResurrectWithPenalties(Pawn pawn, long limit, int penalty, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null, int penaltyYears = 5)
        {
            ResurrectionUtility.Resurrect(pawn);
            pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
            pawn.ageTracker.AgeBiologicalTicks = Math.Max(limit, pawn.ageTracker.AgeBiologicalTicks - penalty);
            // Gene_PermanentHediff.BodyPartGiver(Bodyparts, pawn, HediffDefName);
            Gene_BackstoryChanger.BackstoryChanger(pawn, childBackstoryDef, adultBackstoryDef);
            // int max = (int)((float)pawn.ageTracker.AgeBiologicalTicks / (float)(oneYear * 14));
            // For any case
            // int skillsNumber = 0;
            // foreach (SkillDef item in DefDatabase<SkillDef>.AllDefsListForReading)
            // {
            // skillsNumber++;
            // }
            // for (int i = 0; i < skillsNumber; i++)
            // {
            // if (pawn.skills.skills.Where((SkillRecord x) => !x.TotallyDisabled && x.XpTotalEarned > 0f).TryRandomElementByWeight((SkillRecord x) => (float)x.GetLevel() * 10f, out var result))
            // {
            // float num = result.XpTotalEarned;
            // result.Learn(0f - num, direct: true);
            // }
            // }
            // foreach (Trait item in pawn.story.traits.allTraits)
            // {
            // if (item.sourceGene == null)
            // {
            // pawn.story.traits.RemoveTrait(item);
            // }
            // }
            foreach (SkillRecord item in pawn.skills.skills)
            {
                if (!item.TotallyDisabled && item.XpTotalEarned > 0f)
                {
                    // float num = result.XpTotalEarned * XPLossPercentFromDeathrestRange.RandomInRange;
                    float num = item.XpTotalEarned;
                    // letterText += "\n\n" + "DeathrestLostSkill".Translate(pawn.Named("PAWN"), result.def.label.Named("SKILL"), ((int)num).Named("XPLOSS"));
                    item.Learn(0f - num, direct: true);
                    // gene_Deathless.lastSkillReductionTick = Find.TickManager.TicksGame;
                }
            }
            // pawn.relations.ClearAllNonBloodRelations();
            // Pretty dumb and lazy solution
            // LetterStack letterStack = Find.LetterStack;
            // List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick).ToList();
            // letterStack.RemoveLetter(dismissibleLetters.First());
            // if (ModsConfig.IdeologyActive)
            // {
            // letterStack.RemoveLetter(dismissibleLetters.First());
            // }
            SubXenotypeUtility.XenotypeShapeshifter(pawn);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndead".Translate(pawn.Named("TARGET"), penaltyYears.Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
            }
        }

        public static void Resurrect(Pawn pawn)
        {
            ResurrectionUtility.Resurrect(pawn);
            pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
            // SubXenotypeUtility.XenotypeShapeshifter(pawn);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndeadResurgent".Translate(pawn.Named("TARGET")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
            }
        }

        public static void OffsetResurgentCells(Pawn pawn, float offset)
        {
            if (!ModsConfig.BiotechActive)
            {
                return;
            }
            // if (offset > 0f && applyStatFactor)
            // {
            // offset *= pawn.GetStatValue(StatDefOf.HemogenGainFactor);
            // }
            // Gene_HemogenDrain gene_HemogenDrain = pawn.genes?.GetFirstGeneOfType<Gene_HemogenDrain>();
            // if (gene_HemogenDrain != null)
            // {
            // GeneResourceDrainUtility.OffsetResource(gene_HemogenDrain, offset);
            // return;
            // }
            Gene_ResurgentCells gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
            if (gene_Hemogen != null)
            {
                gene_Hemogen.Value += offset;
            }
        }

        // ResourceUtility
        public static void TickResourceDrain(IGeneResourceDrain drain)
        {
            if (drain.CanOffset && drain.Resource != null)
            {
                OffsetResource(drain, (0f - drain.ResourceLossPerDay) / 60000f);
            }
        }

        public static void OffsetResource(IGeneResourceDrain drain, float amnt)
        {
            float value = drain.Resource.Value;
            drain.Resource.Value += amnt;
            // PostResourceOffset(drain, value);
        }

        public static IEnumerable<Gizmo> GetResourceDrainGizmos(IGeneResourceDrain drain)
        {
            if (DebugSettings.ShowDevGizmos && drain.Resource != null)
            {
                Gene_Resource resource = drain.Resource;
                Command_Action command_Action = new()
                {
                    defaultLabel = "DEV: " + resource.ResourceLabel + " -10",
                    action = delegate
                    {
                        OffsetResource(drain, -0.1f);
                    }
                };
                yield return command_Action;
                Command_Action command_Action2 = new()
                {
                    defaultLabel = "DEV: " + resource.ResourceLabel + " +10",
                    action = delegate
                    {
                        OffsetResource(drain, 0.1f);
                    }
                };
                yield return command_Action2;
            }
        }

        // public static void PostResourceOffset(IGeneResourceDrain drain, float oldValue)
        // {
        // if (oldValue > 0f && drain.Resource.Value <= 0f)
        // {
        // Pawn pawn = drain.Pawn;
        // if (!pawn.health.hediffSet.HasHediff(HediffDefOf.ResurrectionSickness))
        // {
        // pawn.health.AddHediff(HediffDefOf.ResurrectionSickness);
        // }
        // }
        // }

    }
}
