using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Ageless : Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			List<Gene> endogenes = pawn.genes.Endogenes;
			if (endogenes.Contains(this))
			{
				// if (pawn.ageTracker.AgeBiologicalTicks >= 68400000f)
				// {
				// pawn.ageTracker.AgeBiologicalTicks = 65800000;
				// }
				AgelessUtility.Rejuvenation(pawn);
			}
			// List<Gene> endogenes = pawn.genes.Endogenes;
			// for (int i = 0; i < endogenes.Count; i++)
			// {
			// if (endogenes[i].def.defName == def.defName)
			// {
			// }
			// }
		}

		// public override void Tick()
		// {
		// base.Tick();
		// if (!pawn.IsHashIntervalTick(60000))
		// {
		// return;
		// }
		// if (Active && pawn.ageTracker.AgeBiologicalTicks >= 75600000f)
		// {
		// int num = (int)(3600000f * pawn.ageTracker.AdultAgingMultiplier);
		// long val = (long)(3600000f * pawn.ageTracker.AdultMinAge);
		// pawn.ageTracker.AgeBiologicalTicks = Math.Max(val, pawn.ageTracker.AgeBiologicalTicks - num);
		// pawn.ageTracker.ResetAgeReversalDemand(Pawn_AgeTracker.AgeReversalReason.ViaTreatment);
		// int num2 = (int)(pawn.ageTracker.AgeReversalDemandedDeadlineTicks / 60000);
		// string text = "BiosculpterAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
		// Ideo ideo = pawn.Ideo;
		// if (ideo != null && ideo.HasPrecept(PreceptDefOf.AgeReversal_Demanded))
		// {
		// text += " " + "AgeReversalExpectationDeadline".Translate(pawn.Named("PAWN"), num2.Named("DEADLINE"));
		// }
		// Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
		// }
		// }
	}

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
				// if ((oneYear * humanAdultAge) <= pawn.ageTracker.AgeBiologicalTicks)
				// {
				// pawn.ageTracker.AgeBiologicalTicks = (oneYear * humanAdultAge) + 100000;
				// }
				AgelessUtility.Rejuvenation(pawn);
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
			if (AgelessUtility.CanAgeReverse(pawn))
			{
				Gene_Dust gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Dust>();
				if (gene_Resurgent != null)
				{
					AgelessUtility.AgeReverse(pawn);
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

	public class Gene_ResurgentAgeless : Gene_ResurgentDependent
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
			// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			if (cachedResurgentGene != null)
			{
				// if ((oneYear * humanAdultAge) <= pawn.ageTracker.AgeBiologicalTicks)
				// {
				// pawn.ageTracker.AgeBiologicalTicks = (oneYear * humanAdultAge) + 100000;
				// }
				AgelessUtility.Rejuvenation(pawn);
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
			if (AgelessUtility.CanAgeReverse(pawn))
			{
				// Gene_ResurgentCells gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				if (cachedResurgentGene != null)
				{
					if (cachedResurgentGene.ageReversionAllowed)
					{
						if ((cachedResurgentGene.Value - def.resourceLossPerDay) >= 0f)
						{
							cachedResurgentGene.Value -= def.resourceLossPerDay;
							AgelessUtility.AgeReverse(pawn);
						}
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
