using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityVoidVoice : CompProperties_AbilityEffect
	{

		public InteractionDef interactionDef;

		public CompProperties_AbilityVoidVoice()
		{
			compClass = typeof(CompAbilityEffect_VoidVoice);
		}

	}

	public class CompAbilityEffect_VoidVoice : CompAbilityEffect
	{

		public new CompProperties_AbilityVoidVoice Props => (CompProperties_AbilityVoidVoice)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn targetPawn = target.Pawn;
			if (targetPawn != null)
			{
				ThoughtUtility.TryInteractWith(parent.pawn, targetPawn, Props.interactionDef, true);
			}
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return false;
			}
			if (!pawn.IsHuman())
			{
				if (throwMessages)
				{
					Messages.Message("WVC_PawnIsAndroidCheck".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return base.Valid(target, throwMessages);
		}

	}

}
