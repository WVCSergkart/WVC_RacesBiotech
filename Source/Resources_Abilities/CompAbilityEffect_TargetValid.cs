using RimWorld;
using System.Collections.Generic;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityTargetValidation : CompProperties_AbilityEffect
	{

		public bool humanityCheck = false;

		public bool serumsCheck = false;

		// It is enabled by default, as it plays the role of an anti-bug rather than balancing.
		public bool reimplanterDelayCheck = true;

		public CompProperties_AbilityTargetValidation()
		{
			compClass = typeof(CompAbilityEffect_TargetValidation);
		}
	}

	public class CompAbilityEffect_TargetValidation : CompAbilityEffect
	{
		public new CompProperties_AbilityTargetValidation Props => (CompProperties_AbilityTargetValidation)props;

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (Props.humanityCheck && !SerumUtility.PawnIsHuman(pawn))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (Props.serumsCheck && !SerumUtility.PawnCanUseSerums(pawn))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (Props.reimplanterDelayCheck && MechanoidizationUtility.DelayedReimplanterIsActive(pawn))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_AbilityValidation_ReimplantationCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}
	}

}
