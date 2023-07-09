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
		private readonly int penaltyYears = 5;

		private readonly float oneYear = 3600000f;

		private int Penalty => (int)(oneYear * penaltyYears);
		private long Limit => (long)(oneYear * MinAge);
		private float CurrentAge => pawn.ageTracker.AgeBiologicalTicks;
		private float MinAge => pawn.ageTracker.AdultMinAge;

		public BackstoryDef ChildBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.childBackstoryDef;
		public BackstoryDef AdultBackstoryDef => def.GetModExtension<GeneExtension_Giver>()?.adultBackstoryDef;

		// public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
		// public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

		public Gene_ResurgentCells Gene_ResurgentCells => pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();

		public bool PawnCanResurrect => CanResurrect();

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// It's not certain that Active check will work, since in some cases vanilla simply ignores the state of the gene.
			// With the help of HasGene, we will try to prevent a possible bug associated with incorrect resurrection and subsequent shit
			// if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless))
			// {
				// return;
			// }
			// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (!CanResurrect())
			{
				return;
			}
			if (EnoughResurgentCells())
			{
				Gene_ResurgentCells.Value -= def.resourceLossPerDay;
				UndeadUtility.Resurrect(pawn);
				// return;
			}
			else if (CorrectAge())
			{
				UndeadUtility.ResurrectWithPenalties(pawn, Limit, Penalty, ChildBackstoryDef, AdultBackstoryDef, penaltyYears);
			}
			// int penalty = (int)(oneYear * penaltyYears);
			// long limit = (long)(oneYear * minAge);
			// float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			// if ((currentAge - penalty) > limit)
			// {
			// }
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

		//For ShouldSendNotificationAbout check
		private bool CanResurrect()
		{
			if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless) || (!pawn.IsColonist && !WVC_Biotech.settings.canNonPlayerPawnResurrect))
			{
				return false;
			}
			if (EnoughResurgentCells())
			{
				return true;
			}
			else if (CorrectAge())
			{
				return true;
			}
			return false;
		}

		private bool EnoughResurgentCells()
		{
			// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (Gene_ResurgentCells != null)
			{
				if ((Gene_ResurgentCells.Value - def.resourceLossPerDay) >= 0f)
				{
					return true;
				}
			}
			return false;
		}

		private bool CorrectAge()
		{
			// int penalty = (int)(oneYear * penaltyYears);
			// long limit = (long)(oneYear * minAge);
			// float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			if ((CurrentAge - Penalty) > Limit)
			{
				return true;
			}
			return false;
		}

	}

}
