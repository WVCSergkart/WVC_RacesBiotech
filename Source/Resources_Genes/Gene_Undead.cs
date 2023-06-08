using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
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

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// It's not certain that this check will work, since in some cases vanilla simply ignores the state of the gene.
			if (!Active)
			{
				return;
			}
			int penalty = (int)(oneYear * penaltyYears);
			long limit = (long)(oneYear * minAge);
			float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			if ((currentAge - penalty) > limit)
			{
				ResurrectionUtility.Resurrect(pawn);
				pawn.ageTracker.AgeBiologicalTicks = Math.Max(limit, pawn.ageTracker.AgeBiologicalTicks - penalty);
				Gene_BackstoryChanger.BackstoryChanger(pawn, ChildBackstoryDef, AdultBackstoryDef);
				// int max = (int)((float)pawn.ageTracker.AgeBiologicalTicks / (float)(oneYear * 14));
				// For any case
				int skillsNumber = 0;
				foreach (SkillDef item in DefDatabase<SkillDef>.AllDefsListForReading)
				{
					skillsNumber++;
				}
				for (int i = 0; i < skillsNumber; i++)
				{
					if (pawn.skills.skills.Where((SkillRecord x) => !x.TotallyDisabled && x.XpTotalEarned > 0f).TryRandomElementByWeight((SkillRecord x) => (float)x.GetLevel() * 10f, out var result))
					{
						// float num = result.XpTotalEarned * XPLossPercentFromDeathrestRange.RandomInRange;
						float num = result.XpTotalEarned;
						// letterText += "\n\n" + "DeathrestLostSkill".Translate(pawn.Named("PAWN"), result.def.label.Named("SKILL"), ((int)num).Named("XPLOSS"));
						result.Learn(0f - num, direct: true);
						// gene_Deathless.lastSkillReductionTick = Find.TickManager.TicksGame;
					}
				}
				// Pretty dumb and lazy solution
				// LetterStack letterStack = Find.LetterStack;
				// List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick).ToList();
				// letterStack.RemoveLetter(dismissibleLetters.First());
				// if (ModsConfig.IdeologyActive)
				// {
					// letterStack.RemoveLetter(dismissibleLetters.First());
				// }
				Gene_PermanentHediff.BodyPartGiver(Bodyparts, pawn, HediffDefName);
				Find.LetterStack.ReceiveLetter("WVC_LetterLabelSecondChance_GeneUndead".Translate(), "WVC_LetterTextSecondChance_GeneUndead".Translate(pawn.Named("TARGET"), penaltyYears.Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
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

	}

}
