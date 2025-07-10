// RimWorld.CompAbilityEffect_GiveHediff
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
    public class CompAbilityEffect_BodyPartsRipper_Corpse : CompAbilityEffect
	{

		public new CompProperties_AbilityGiveHediff Props => (CompProperties_AbilityGiveHediff)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
            if (target.Thing is Corpse corpse)
			{
                List<BodyPartRecord> casterBodyPartRecords = parent.pawn.health.hediffSet.GetNotMissingParts().ToList();
                List<BodyPartDef> casterParts = casterBodyPartRecords.ConvertToDefs();
                List<BodyPartRecord> injuredTargetParts = corpse.InnerPawn.health.hediffSet.GetInjuredParts();
                List<BodyPartRecord> parts = corpse.InnerPawn.health.hediffSet.GetNotMissingParts().Where((part) => !casterParts.Contains(part.def) && !injuredTargetParts.Contains(part)).ToList();
				if (parts.NullOrEmpty())
                {
					return;
                }
                foreach (BodyPartRecord part in parts)
                {
                    foreach (BodyPartRecord casterPart in casterBodyPartRecords)
                    {
                        if (casterPart.def != part.def)
                        {
                            continue;
                        }
                        HealingUtility.RestorePartWith1HP(parent.pawn, casterPart, true);
                        corpse.InnerPawn.health.AddHediff(HediffDefOf.MissingBodyPart, part);
                    }
                }
				//Find.WindowStack.Add(new Dialog_BodyPartsHarvest(parent.pawn, corpse.InnerPawn, parts));
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target.Thing is not Corpse corpse)
            {
                return false;
            }
            return Valid(parent, parent.pawn, corpse.InnerPawn, false);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (throwMessages)
            {
                if (target.Thing is not Corpse corpse || corpse.InnerPawn.RaceProps.body != parent.pawn.RaceProps.body)
                {
                    SoundDefOf.ClickReject.PlayOneShotOnCamera();
                    return false;
                }
                if (!Valid(parent, parent.pawn, corpse.InnerPawn, throwMessages))
                {
                    return false;
                }
                return base.Valid(target, throwMessages);
            }
            return true;
        }

        public static bool Valid(Ability ability, Pawn caster, Pawn victim, bool throwMessages = false, bool checkFaction = false)
        {
            if (victim == null)
            {
                return false;
            }
            if (!victim.IsHuman())
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_PawnIsAndroidCheck".Translate(), victim, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            if (checkFaction)
            {
                if (victim.IsQuestLodger())
                {
                    if (throwMessages)
                    {
                        Messages.Message("WVC_XaG_PawnIsQuestLodgerMessage".Translate(victim.Name.ToStringShort), victim, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
                if (victim.HostileTo(caster) && !victim.Downed)
                {
                    if (throwMessages)
                    {
                        Messages.Message("MessageCantUseOnResistingPerson".Translate(ability.def.Named("ABILITY")), victim, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
            }
            return true;
        }

    }

}
