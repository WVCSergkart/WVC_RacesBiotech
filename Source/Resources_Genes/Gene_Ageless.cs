using RimWorld;
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
				//AgelessUtility.Rejuvenation(pawn);
				AgelessUtility.InitialRejuvenation(pawn);
			}
		}

	}

	public class Gene_DustAgeless : Gene_FoodEfficiency
	{
		public readonly long oneYear = 3600000L;

		public readonly long humanAdultAge = 18L;

		private int ticksToAgeReversal;

		public override void PostAdd()
		{
			base.PostAdd();
			//if (!Active)
			//{
			//	return;
			//}
			//Gene_Dustogenic gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Dustogenic>();
			//if (gene_Resurgent != null)
			//{
			//	AgelessUtility.Rejuvenation(pawn);
			//}
			AgelessUtility.InitialRejuvenation(pawn);
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.delta();
			ticksToAgeReversal -= delta;
			if (ticksToAgeReversal > 0)
			{
				return;
			}
			ResetInterval();
			//if (pawn.Faction != Faction.OfPlayer)
			//{
			//	return;
			//}
			AgeReversal();
		}

		public void AgeReversal()
		{
			if (AgelessUtility.CanAgeReverse(pawn))
			{
				//Gene_Dustogenic gene_Resurgent = pawn.genes?.GetFirstGeneOfType<Gene_Dustogenic>();
				//if (gene_Resurgent != null)
				//{
				//}
				AgelessUtility.AgeReverse(pawn);
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
			//if (!Active)
			//{
			//	return;
			//}
			//if (Resurgent != null)
			//{
			//	AgelessUtility.Rejuvenation(pawn);
			//}
			AgelessUtility.InitialRejuvenation(pawn);
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			ticksToAgeReversal -= delta;
			if (ticksToAgeReversal > 0)
			{
				return;
			}
			ResetInterval();
			//if (!Active || (!pawn.IsColonist && pawn.Map == null))
			//{
			//	return;
			//}
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
				if (Resurgent != null)
				{
					if (Resurgent.ageReversionAllowed)
					{
						if ((Resurgent.Value - def.resourceLossPerDay) >= 0f)
						{
							Resurgent.Value -= def.resourceLossPerDay;
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
