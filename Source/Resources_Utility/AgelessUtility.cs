using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class AgelessUtility
	{

		public static void SetAge(Pawn pawn, int age)
		{
			pawn.ageTracker.AgeBiologicalTicks = age;
			pawn.ageTracker.AgeChronologicalTicks = age;
		}

		public static void AddAge(Pawn pawn, int age)
		{
			pawn.ageTracker.AgeBiologicalTicks += age;
			pawn.ageTracker.AgeChronologicalTicks += age;
		}

		public static void AddAgeInYears(Pawn pawn, int ageInYears)
		{
			AddAge(pawn, ageInYears * 3600000);
		}

		public static void InitialRejuvenation(Pawn pawn)
		{
			if (MiscUtility.GameNotStarted())
			{
				AgelessUtility.Rejuvenation(pawn);
			}
		}

		public static void Rejuvenation(Pawn pawn)
		{
			if (CanAgeReverse(pawn, 18))
			{
				pawn.ageTracker.AgeBiologicalTicks = (3600000L * 18L) + 100000L;
			}
		}

		public static void ChronoCorrection(Pawn reincarnated, Pawn pawn)
		{
			if (reincarnated.ageTracker.AgeChronologicalTicks >= pawn.ageTracker.AgeChronologicalTicks)
			{
				if (reincarnated.ageTracker.AgeChronologicalTicks - (18L * 3600000L) < reincarnated.ageTracker.AgeBiologicalTicks)
				{
					reincarnated.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks;
				}
				else
				{
					reincarnated.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeChronologicalTicks - (18L * 3600000L);
				}
			}
		}

		public static bool CanAgeReverse(Pawn pawn, int reqYears = 18)
		{
			if (pawn.ageTracker.AgeBiologicalYears <= reqYears)
			{
				return false;
			}
			//long oneYear = 3600000L;
			//if ((oneYear * reqYears) + oneYear * 1.5f >= pawn.ageTracker.AgeBiologicalTicks)
			//{
			//    return false;
			//}
			return true;
		}

		public static void AgeReverse(Pawn pawn)
		{
			long oneYear = 3600000L;
			//int num = (int)(oneYear * pawn.ageTracker.AdultAgingMultiplier);
			//long val = (long)(oneYear * pawn.ageTracker.AdultMinAge);
			//pawn.ageTracker.AgeBiologicalTicks = Math.Max(val, pawn.ageTracker.AgeBiologicalTicks - num);
			pawn.ageTracker.AgeBiologicalTicks = (long)Mathf.Clamp(pawn.ageTracker.AgeBiologicalTicks - oneYear, oneYear * 18 + 2100000L, oneYear * 100);
			if (ModsConfig.IdeologyActive)
			{
				pawn.ageTracker.ResetAgeReversalDemand(Pawn_AgeTracker.AgeReversalReason.ViaTreatment);
			}
			string text = "WVC_ResurgentGeneAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
			}
		}

		public static bool TryAgeReverse(Pawn pawn)
		{
			if (CanAgeReverse(pawn))
			{
				AgeReverse(pawn);
				return true;
			}
			return false;
		}

	}
}
