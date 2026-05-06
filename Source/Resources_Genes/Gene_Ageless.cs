using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Ageless : XaG_Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			InitAgeless(this);
		}

		public static void InitAgeless(Gene gene)
		{
			List<Gene> endogenes = gene.pawn.genes.Endogenes;
			if (endogenes.Contains(gene))
			{
				AgelessUtility.InitialRejuvenation(gene.pawn);
			}
		}

	}

	public class Gene_FleshmassAging : XaG_Gene, IGeneNotifyLifeStageStarted
	{

		public void Notify_LifeStageStarted()
		{
			if (pawn.genes != null && !pawn.genes.HediffGiversCanGive(HediffDefOf.Carcinoma))
			{
				return;
			}
			if (pawn.IsMutant && !pawn.mutant.HediffGiversCanGive(HediffDefOf.Carcinoma))
			{
				return;
			}
			List<BodyPartDef> allowedBodyParts = pawn.health?.hediffSet?.GetNotMissingParts()?.Where((part) => part.def.canSuggestAmputation)?.ToList()?.ConvertToDefs();
			if (allowedBodyParts.NullOrEmpty())
			{
				allowedBodyParts = null;
			}
			HediffGiverUtility.TryApply(pawn, HediffDefOf.Carcinoma, allowedBodyParts, true);
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
			AgeReversal();
		}

		public void AgeReversal()
		{
			if (AgelessUtility.CanAgeReverse(pawn))
			{
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
