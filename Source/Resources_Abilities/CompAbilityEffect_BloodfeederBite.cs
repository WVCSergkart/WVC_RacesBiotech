using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompAbilityEffect_HemofeederBite : CompAbilityEffect_BloodfeederBite
    {
        public new CompProperties_AbilityBloodfeederBite Props => (CompProperties_AbilityBloodfeederBite)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            //Gene_Hemogen hemogen = target.Pawn.genes.GetFirstGeneOfType<Gene_Hemogen>();
            //if (hemogen != null)
            //{
            //    hemogen.Value -= Props.hemogenGain;
            //}
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Gene_Hemogen hemogen = target.Pawn?.genes?.GetFirstGeneOfType<Gene_Hemogen>();
            if (hemogen == null || hemogen.Value < Props.hemogenGain)
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_HemofeederErrorMessage".Translate(), target.Pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }

	}

	public class CompAbilityEffect_CorpsefeederBite : CompAbilityEffect
	{
		public new CompProperties_AbilityBloodfeederBite Props => (CompProperties_AbilityBloodfeederBite)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
			if (innerPawn == null)
			{
				return;
			}
			SanguophageUtility.DoBite(parent.pawn, innerPawn, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, 0, Props.bloodFilthToSpawnRange, null, null);
		}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				if (corpse.GetRotStage() == RotStage.Dessicated)
				{
					if (throwMessages)
					{
						Messages.Message("MessageCannotResurrectDessicatedCorpse".Translate(), corpse, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				Pawn innerPawn = corpse.InnerPawn;
				if (innerPawn.IsAndroid() || (innerPawn.IsMutant && !innerPawn.mutant.Def.canBleed) || target.Thing.IsUnnaturalCorpse())
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				//if (innerPawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff hediff) && hediff != null && hediff.def.maxSeverity > hediff.Severity + Props.targetBloodLoss)
				//{
				//	if (throwMessages)
				//	{
				//		Messages.Message("WVC_XaG_CropsefeederNoBloodMessage".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
				//	}
				//	return false;
				//}
			}
			return base.Valid(target, throwMessages);
		}

	}

}
