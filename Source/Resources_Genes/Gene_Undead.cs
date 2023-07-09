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

		public bool PawnCanResurrect => CanResurrect();

		public override void Notify_PawnDied()
		{
			base.Notify_PawnDied();
			// It's not certain that Active check will work, since in some cases vanilla simply ignores the state of the gene.
			// With the help of HasGene, we will try to prevent a possible bug associated with incorrect resurrection and subsequent shit
			if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless))
			{
				return;
			}
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if ((gene_Resurgent.Value - def.resourceLossPerDay) >= 0f)
				{
					gene_Resurgent.Value -= def.resourceLossPerDay;
					UndeadUtility.Resurrect(pawn);
				}
				return;
			}
			int penalty = (int)(oneYear * penaltyYears);
			long limit = (long)(oneYear * minAge);
			float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			if ((currentAge - penalty) > limit)
			{
				UndeadUtility.ResurrectWithPenalties(pawn, limit, penalty, ChildBackstoryDef, AdultBackstoryDef, penaltyYears);
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

		//For ShouldSendNotificationAbout check
		public bool CanResurrect()
		{
			if (!Active || Overridden || pawn.genes.HasGene(GeneDefOf.Deathless))
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

		public bool EnoughResurgentCells()
		{
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if ((gene_Resurgent.Value - def.resourceLossPerDay) >= 0f)
				{
					return true;
				}
			}
			return false;
		}

		public bool CorrectAge()
		{
			int penalty = (int)(oneYear * penaltyYears);
			long limit = (long)(oneYear * minAge);
			float currentAge = pawn.ageTracker.AgeBiologicalTicks;
			if ((currentAge - penalty) > limit)
			{
				return true;
			}
			return false;
		}

	}

}
