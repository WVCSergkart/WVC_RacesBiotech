using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentAgeless : Gene
	{
		public readonly long oneYear = 3600000L;

		public readonly long humanAdultAge = 18L;

		private int ticksToAgeReversal;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!Active)
			{
				return;
			}
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if ((oneYear * humanAdultAge) <= pawn.ageTracker.AgeBiologicalTicks)
				{
					pawn.ageTracker.AgeBiologicalTicks = (oneYear * humanAdultAge) + 100000;
				}
			}
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToAgeReversal--;
			if (ticksToAgeReversal > 0)
			{
				return;
			}
			ResetInterval();
			if (!pawn.IsColonist || !pawn.IsPrisonerOfColony || !Active)
			{
				return;
			}
			AgeReversal();
			// if (pawn.ageTracker.AgeBiologicalTicks >= 65800000f)
			// {
			// }
		}

		public void AgeReversal()
		{
			if ((oneYear * humanAdultAge) + oneYear * 1.5f >= pawn.ageTracker.AgeBiologicalTicks)
			{
				return;
			}
			Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (gene_Resurgent != null)
			{
				if ((gene_Resurgent.Value - def.resourceLossPerDay) >= 0f)
				{
					gene_Resurgent.Value -= def.resourceLossPerDay;
					int num = (int)(oneYear * pawn.ageTracker.AdultAgingMultiplier);
					long val = (long)(oneYear * pawn.ageTracker.AdultMinAge);
					pawn.ageTracker.AgeBiologicalTicks = Math.Max(val, pawn.ageTracker.AgeBiologicalTicks - num);
					// pawn.ageTracker.ResetAgeReversalDemand(Pawn_AgeTracker.AgeReversalReason.ViaTreatment);
					// int num2 = (int)(pawn.ageTracker.AgeReversalDemandedDeadlineTicks / 60000);
					string text = "WVC_ResurgentGeneAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
					// Ideo ideo = pawn.Ideo;
					// if (ideo != null && ideo.HasPrecept(PreceptDefOf.AgeReversal_Demanded))
					// {
						// text += " " + "AgeReversalExpectationDeadline".Translate(pawn.Named("PAWN"), num2.Named("DEADLINE"));
					// }
					if (pawn.IsColonist)
					{
						Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
					}
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Revers age",
					action = delegate
					{
						if (Active)
						{
							AgeReversal();
						}
						ResetInterval();
					}
				};
			}
		}

		private void ResetInterval()
		{
			IntRange intRange = new(60000, 120000);
			ticksToAgeReversal = intRange.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToAgeReversal, "ticksToAgeReversal", 0);
		}
	}

}
