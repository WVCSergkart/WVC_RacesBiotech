using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompAbilityEffect_PostImplanter : CompAbilityEffect
	{

		private new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
            if (innerPawn == null)
            {
                return;
            }
            GeneResourceUtility.ResurrectWithSickness(innerPawn);
            //if (ModLister.IdeologyInstalled)
            //{
            //	Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.WVC_ReimplanterResurrection, parent.pawn.Named(HistoryEventArgsNames.Doer)));
            //}
            Messages.Message("MessagePawnResurrected".Translate(innerPawn), innerPawn, MessageTypeDefOf.PositiveEvent);
            MoteMaker.MakeAttachedOverlay(innerPawn, ThingDefOf.Mote_ResurrectFlash, Vector3.zero);
            if (ReimplanterUtility.TryReimplant(parent.pawn, innerPawn, Props.reimplantEndogenes, Props.reimplantXenogenes))
			{
				Notify_Reimplanted(innerPawn);
				ReimplanterUtility.FleckAndLetter(parent.pawn, innerPawn);
            }
        }

        public virtual void Notify_Reimplanted(Pawn innerPawn)
        {
            foreach (Gene gene in parent.pawn.genes.GenesListForReading)
            {
                if (gene is Gene_ImplanterDependant postgene && gene.Active)
                {
                    postgene.Notify_PostReimplanted(innerPawn);
                }
            }
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
				if (!innerPawn.IsHuman() || innerPawn.IsMutant || target.Thing.IsUnnaturalCorpse())
				{
					if (throwMessages)
					{
						Messages.Message("WVC_PawnIsAndroidCheck".Translate(), innerPawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					return false;
				}
			}
			return base.Valid(target, throwMessages);
		}

		public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
			}
			if (target.Thing is Corpse corpse && !corpse.InnerPawn.guest.Recruitable && (corpse.InnerPawn.Faction == null || corpse.InnerPawn.Faction != Faction.OfPlayer))
			{
				return Dialog_MessageBox.CreateConfirmation("WVC_XaG_ReimplantResurrectionRecruiting_FailWarning".Translate(corpse.InnerPawn.Named("PAWN"), corpse.InnerPawn.Faction.NameColored.ToString()), confirmAction);
			}
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			yield return MoteMaker.MakeAttachedOverlay((Corpse)target.Thing, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}
	}

	[Obsolete]
	public class CompAbilityEffect_RiseFromTheDead : CompAbilityEffect_PostImplanter
	{

    }

}
