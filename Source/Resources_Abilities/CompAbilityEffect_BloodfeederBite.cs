using RimWorld;
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
				if (innerPawn.IsAndroid() || target.Thing.IsUnnaturalCorpse())
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
				if (!innerPawn.CanBleed() || innerPawn.health.hediffSet.TryGetHediff(HediffDefOf.BloodLoss, out Hediff hediff) && (hediff.Severity >= hediff.def.maxSeverity || hediff.Severity >= hediff.def.lethalSeverity))
				{
					if (throwMessages)
					{
						Messages.Message("WVC_XaG_CropsefeederNoBloodMessage".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
            }
			return base.Valid(target, throwMessages);
		}

	}

	public class CompAbilityEffect_AnimalfeederBite : CompAbilityEffect_BloodfeederBite
	{
		public new CompProperties_AbilityBloodfeederBite Props => (CompProperties_AbilityBloodfeederBite)props;

		//public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//	base.Apply(target, dest);
		//	Pawn animal = target.Pawn;
		//	if (animal == null)
		//	{
		//		return;
		//	}
		//	SanguophageUtility.DoBite(parent.pawn, animal, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, 0, Props.bloodFilthToSpawnRange, null, null);
		//}

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return Valid(target);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn animal = target.Pawn;
			if (animal == null)
            {
				return false;
			}
			if (!animal.IsAnimal)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_AnimalfeederBite_NonAnimal".Translate(), animal, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!animal.RaceProps.IsFlesh)
			{
				if (throwMessages)
				{
					Messages.Message("WVC_AnimalfeederBite_NonFlesh".Translate(), animal, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (animal.Faction != parent.pawn.Faction)
			{
				if (animal.Faction.HostileTo(parent.pawn.Faction))
				{
					if (!animal.Downed)
					{
						if (throwMessages)
						{
							Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), animal, MessageTypeDefOf.RejectInput, historical: false);
						}
						return false;
					}
				}
				else if (animal.IsQuestLodger())
				{
					if (throwMessages)
					{
						Messages.Message("MessageCannotUseOnOtherFactions".Translate(parent.def.Named("ABILITY")), animal, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			if (animal.InMentalState)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), animal, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!animal.CanBleed())
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotUseOnNonBleeder".Translate(parent.def.Named("ABILITY")), animal, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return true;
		}

	}

}
