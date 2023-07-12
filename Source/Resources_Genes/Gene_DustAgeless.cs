using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DustAgeless : Gene_DustDrain
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
			Gene_Dust gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
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
		}

		public void AgeReversal()
		{
			if ((oneYear * humanAdultAge) + oneYear * 1.5f >= pawn.ageTracker.AgeBiologicalTicks)
			{
				return;
			}
			Gene_Dust gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
			if (gene_Resurgent != null)
			{
				int num = (int)(oneYear * pawn.ageTracker.AdultAgingMultiplier);
				long val = (long)(oneYear * pawn.ageTracker.AdultMinAge);
				pawn.ageTracker.AgeBiologicalTicks = Math.Max(val, pawn.ageTracker.AgeBiologicalTicks - num);
				string text = "WVC_ResurgentGeneAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
				if (pawn.IsColonist)
				{
					Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
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
			IntRange intRange = new(300000, 900000);
			ticksToAgeReversal = intRange.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToAgeReversal, "ticksToAgeReversal", 0);
		}
	}

}
