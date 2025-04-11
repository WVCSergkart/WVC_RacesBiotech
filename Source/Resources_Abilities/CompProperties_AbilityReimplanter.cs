using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_AbilityReimplanter : CompProperties_AbilityEffect
	{

		public ThoughtDef afterResurrectionThoughtDef;

		public ThoughtDef resurrectorThoughtDef;
		public ThoughtDef resurrectedThoughtDef;

		public JobDef absorberJob;

		public List<GeneDef> geneDefs;

		public List<GeneDef> inheritableGenes;

		public bool reimplantEndogenes = true;
		public bool reimplantXenogenes = true;

		public XenotypeDef xenotypeDef;

		public CompProperties_AbilityReimplanter()
		{
			compClass = typeof(CompAbilityEffect_Reimplanter);
		}

	}

	public class CompAbilityEffect_Reimplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				if (ReimplanterUtility.TryReimplant(parent.pawn, pawn, Props.reimplantEndogenes, Props.reimplantXenogenes))
                {
					ReimplanterUtility.FleckAndLetter(parent.pawn, pawn);
                }
            }
		}

        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return ReimplanterUtility.ImplanterValidation(parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
        }

        public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		{
			if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
			{
				return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
			}
			return null;
		}

		public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		{
			Pawn pawn = target.Pawn;
			yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

	}
}
