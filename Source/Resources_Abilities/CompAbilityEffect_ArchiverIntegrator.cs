using RimWorld;
using System;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompAbilityEffect_ArchiverDependant : CompAbilityEffect
    {

        public new CompProperties_AbilityChimera Props => (CompProperties_AbilityChimera)props;


        [Unsaved(false)]
        private Gene_Archiver cachedArchiverGene;
        public Gene_Archiver Archiver
        {
            get
            {
                if (cachedArchiverGene == null || !cachedArchiverGene.Active)
                {
                    cachedArchiverGene = parent?.pawn?.genes?.GetFirstGeneOfType<Gene_Archiver>();
                }
                return cachedArchiverGene;
            }
        }

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return Valid(target);
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (Archiver == null)
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_GeneArchiver_DeActive".Translate(), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }

    }

    public class CompAbilityEffect_ArchiverIntegrator : CompAbilityEffect_ArchiverDependant
    {

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn victim = target.Pawn;
            if (victim != null && Archiver != null)
            {
                DevourTarget(victim);
            }
        }

        private void DevourTarget(Pawn victim)
        {
            string phase = "start";
            try
            {
                Pawn caster = parent.pawn;
                phase = "drop apparel";
                victim.apparel?.DropAll(victim.Position, true, false);
                phase = "try archive";
                if (Archiver.TryArchiveSelectedPawn(victim, Archiver))
                {
                    phase = "meat boom";
                    MiscUtility.MeatSplatter(victim, FleshbeastUtility.MeatExplosionSize.Large, 7);
                    phase = "message";
                    Messages.Message("WVC_XaG_GeneArchiverIntegrator_Succes".Translate(victim.NameShortColored), victim, MessageTypeDefOf.NeutralEvent, historical: false);
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed archvie target: " + victim.Name + ". On phase: " + phase + ". Reason: " + arg);
            }
        }

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            Pawn pawn = target.Pawn;
            if (pawn == null)
            {
                return false;
            }
            if (parent.pawn.IsQuestLodger())
            {
                if (throwMessages)
                {
                    Messages.Message("WVC_XaG_PawnIsQuestLodgerMessage".Translate(parent.pawn), parent.pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
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
            if (pawn.relations.OpinionOf(parent.pawn) < 40 && !pawn.Downed && !pawn.Deathresting)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
            return base.Valid(target, throwMessages);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
        {
            return Dialog_MessageBox.CreateConfirmation("WVC_XaG_WarningPawnWillBeArchived".Translate(target.Pawn.Named("PAWN")), confirmAction, destructive: true);
        }

    }

}
