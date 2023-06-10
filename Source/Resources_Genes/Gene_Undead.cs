using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
// using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Undead : Gene
	{
		public int penaltyYears = 5;
		public int minAge = 14;

		private readonly float oneYear = 3600000f;

		public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>().childBackstoryDef;
		public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>().adultBackstoryDef;

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		// public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// It's not certain that Active check will work, since in some cases vanilla simply ignores the state of the gene.
			// With the help of HasGene, we will try to prevent a possible bug associated with incorrect resurrection and subsequent shit
			if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless))
			{
				return;
			}
			int penalty = (int)(oneYear * penaltyYears);
			long limit = (long)(oneYear * minAge);
			float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			if ((currentAge - penalty) > limit)
			{
				Resurrect(pawn, limit, penalty, ChildBackstoryDef, AdultBackstoryDef, penaltyYears);
			}
			// pawn.ageTracker.ResetAgeReversalDemand(Pawn_AgeTracker.AgeReversalReason.ViaTreatment);
			// int num2 = (int)(pawn.ageTracker.AgeReversalDemandedDeadlineTicks / 60000);
			// string text = "BiosculpterAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
			// Ideo ideo = pawn.Ideo;
			// if (ideo != null && ideo.HasPrecept(PreceptDefOf.AgeReversal_Demanded))
			// {
				// text += " " + "AgeReversalExpectationDeadline".Translate(pawn.Named("PAWN"), num2.Named("DEADLINE"));
			// }
			// Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
		}

		public static void Resurrect(Pawn pawn, long limit, int penalty, BackstoryDef childBackstoryDef = null, BackstoryDef adultBackstoryDef = null, int penaltyYears = 5)
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
			pawn.relations.ClearAllNonBloodRelations();
			// Pretty dumb and lazy solution
			// LetterStack letterStack = Find.LetterStack;
			// List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick).ToList();
			// letterStack.RemoveLetter(dismissibleLetters.First());
			// if (ModsConfig.IdeologyActive)
			// {
				// letterStack.RemoveLetter(dismissibleLetters.First());
			// }
			Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndead".Translate(pawn.Named("TARGET"), penaltyYears.Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
		}

	}

}
