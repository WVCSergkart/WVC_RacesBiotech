// RimWorld.CompAbilityEffect_GiveHediff
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompAbilityEffect_ImplantImplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Thing implant = target.Thing;
			if (HediffUtility.TryInstallPart(parent.pawn, implant.def))
            {
				MiscUtility.ReduceStack(implant);
                Messages.Message("WVC_XaG_GeneImplantImplanter_Succes".Translate(), parent.pawn, MessageTypeDefOf.NeutralEvent, historical: false);
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Thing?.def?.IsImplantable(parent.pawn, out _) == true)
            {
				return true;
			}
			Messages.Message("WVC_XaG_GeneralWrongTarget".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}

	}

}
