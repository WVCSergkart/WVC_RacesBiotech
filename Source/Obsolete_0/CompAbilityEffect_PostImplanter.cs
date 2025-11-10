using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	[Obsolete]
	public class CompAbilityEffect_Reimplanter : CompAbilityEffect_NewImplanter
	{

		//public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		//public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//	base.Apply(target, dest);
		//	Pawn targetPawn = target.Pawn;
		//	if (targetPawn != null)
		//	{
		//		CompAbilityEffect_NewImplanter.ApplyOnPawn(targetPawn, parent.pawn, Props.reimplantEndogenes, Props.reimplantXenogenes);
		//	}
		//}

		//public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		//{
		//	return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages) && base.Valid(target, throwMessages);
		//}

		//public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		//{
		//	if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
		//	{
		//		return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
		//	}
		//	return null;
		//}

		//public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		//{
		//	Pawn pawn = target.Pawn;
		//	yield return MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		//}

	}

	[Obsolete]
	public class CompAbilityEffect_PostImplanter : CompAbilityEffect_NewImplanter
	{

		//private new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		//public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		//{
		//	base.Apply(target, dest);
		//	Pawn innerPawn = ((Corpse)target.Thing).InnerPawn;
		//	if (innerPawn != null)
		//	{
		//		CompAbilityEffect_NewImplanter.ApplyOnCorpse(innerPawn, parent.pawn, Props.reimplantEndogenes, Props.reimplantXenogenes);
		//	}
		//}

		//public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		//{
		//	if (target.HasThing && target.Thing is Corpse corpse)
		//	{
		//		return ReimplanterUtility.ValidCorpseForImplant(target, throwMessages, corpse) && base.Valid(target, throwMessages);
		//	}
		//	return false;
		//}

		//public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
		//{
		//	if (GeneUtility.PawnWouldDieFromReimplanting(parent.pawn))
		//	{
		//		return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(parent.pawn.Named("PAWN")), confirmAction, destructive: true);
		//	}
		//	if (target.Thing is Corpse corpse && !corpse.InnerPawn.guest.Recruitable && (corpse.InnerPawn.Faction == null || corpse.InnerPawn.Faction != Faction.OfPlayer))
		//	{
		//		return Dialog_MessageBox.CreateConfirmation("WVC_XaG_ReimplantResurrectionRecruiting_FailWarning".Translate(corpse.InnerPawn.Named("PAWN"), corpse.InnerPawn.Faction.NameColored.ToString()), confirmAction);
		//	}
		//	return null;
		//}

		//public override IEnumerable<Mote> CustomWarmupMotes(LocalTargetInfo target)
		//{
		//	yield return MoteMaker.MakeAttachedOverlay((Corpse)target.Thing, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		//}
	}

	[Obsolete]
	public class CompAbilityEffect_RiseFromTheDead : CompAbilityEffect_PostImplanter
	{

	}

}
