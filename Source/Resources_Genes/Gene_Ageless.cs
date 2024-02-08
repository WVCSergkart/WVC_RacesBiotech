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
				AgelessUtility.Rejuvenation(pawn);
			}
		}

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
