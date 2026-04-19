using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompAbilityEffect_Impregnate : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn targetPawn = target.Pawn;
			Pawn caster = parent.pawn;
			try
			{
				if (targetPawn.relations.OpinionOf(caster) < 100)
				{
					if (Props.opinionDef != null)
					{
						targetPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.opinionDef, caster);
					}
					if (Props.memoryDef != null)
					{
						targetPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Props.memoryDef, caster);
					}
				}
				GestationUtility.TryImpregnateOrUpdChildGenes(targetPawn, caster);
				Messages.Message("WVC_XaG_Impregnator_Succes".Translate(), targetPawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			catch (Exception arg)
			{
				Log.Error("Failed impregnate pawn:" + targetPawn.Name + " by pawn: " + caster.Name + ". Reason: " + arg.Message);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (!base.Valid(target, throwMessages))
			{
				return false;
			}
			if (target.Pawn != null && (!target.Pawn.CanBePregnant()))
			{
				if (throwMessages)
				{
					Messages.Message("WVC_XaG_AbilityGeneIsActive_PawnWrongGender".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, false) && !target.Pawn.IsSterile(true) && !parent.pawn.IsSterile(true);
		}

	}

}
