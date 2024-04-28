using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilitySafeBloodfeed : CompProperties_AbilityEffect
	{

		public CompProperties_AbilitySafeBloodfeed()
		{
			compClass = typeof(CompAbilityEffect_SafeBloodfeed);
		}

	}

	public class CompAbilityEffect_SafeBloodfeed : CompAbilityEffect
	{

		public new CompProperties_AbilitySafeBloodfeed Props => (CompProperties_AbilitySafeBloodfeed)props;

		public CompAbilityEffect_BloodfeederBite Bloodfeeder => parent.CompOfType<CompAbilityEffect_BloodfeederBite>();

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (Bloodfeeder == null)
			{
				return;
			}
			Pawn pawn = (Pawn)target;
			if (pawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff bloodloss))
			{
				float severity = bloodloss.Severity;
				if (severity - Bloodfeeder.Props.targetBloodLoss <= bloodloss.def.lethalSeverity)
				{
					bloodloss.Severity = Bloodfeeder.Props.targetBloodLoss + 0.01f;
				}
			}
		}

	}

}
