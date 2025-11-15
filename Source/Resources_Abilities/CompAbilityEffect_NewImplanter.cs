using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class CompAbilityEffect_NewImplanter : CompAbilityEffect
	{

		public new CompProperties_AbilityReimplanter Props => (CompProperties_AbilityReimplanter)props;

		public virtual bool IgnoreIdeo => false;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			string phase = "";
			try
			{
				phase = "start";
				base.Apply(target, dest);
				Pawn caster = parent.pawn;
				phase = "try corpse";
				if (target.Thing is Corpse corpse && corpse.InnerPawn != null)
				{
					ApplyOnCorpse(corpse.InnerPawn, caster, Props.reimplantEndogenes, Props.reimplantXenogenes);
					return;
				}
				phase = "try target";
				Pawn targetPawn = target.Pawn;
				if (targetPawn != null)
				{
					ApplyOnPawn(targetPawn, caster, Props.reimplantEndogenes, Props.reimplantXenogenes);
					return;
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed use implanter. On phase " + phase + ". Reason: " + arg.Message);
			}
		}

		public static void ApplyOnCorpse(Pawn innerPawn, Pawn caster, bool implantEndogenes = true, bool implantXenogenes = true)
		{
			GeneResourceUtility.ResurrectWithSickness(innerPawn);
			Messages.Message("MessagePawnResurrected".Translate(innerPawn), innerPawn, MessageTypeDefOf.PositiveEvent);
			MoteMaker.MakeAttachedOverlay(innerPawn, ThingDefOf.Mote_ResurrectFlash, Vector3.zero);
			//if (ReimplanterUtility.TryReimplant(caster, innerPawn, implantEndogenes, implantXenogenes))
			//{
			//	CompAbilityEffect_Reimplanter.Notify_Reimplanted(innerPawn, caster);
			//	ReimplanterUtility.FleckAndLetter(caster, innerPawn);
			//}
			ApplyOnPawn(innerPawn, caster, implantEndogenes, implantXenogenes);
		}

		public static void ApplyOnPawn(Pawn pawn, Pawn caster, bool implantEndogenes = true, bool implantXenogenes = true)
		{
			if (ReimplanterUtility.TryReimplant(caster, pawn, implantEndogenes, implantXenogenes))
			{
				Notify_Reimplanted(pawn, caster);
				ReimplanterUtility.FleckAndLetter(caster, pawn);
			}
		}

		public static void Notify_Reimplanted(Pawn target, Pawn caster)
		{
			try
			{
				foreach (Gene gene in caster.genes.GenesListForReading)
				{
					if (gene is Gene_ImplanterDependant postgene && gene.Active)
					{
						postgene.Notify_PostReimplanted(target);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed Notify_Reimplanted (sub-genes trigger). Reason: " + arg.Message);
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			if (!base.Valid(target, throwMessages))
			{
				return false;
			}
			if (target.HasThing && target.Thing is Corpse corpse)
			{
				return ReimplanterUtility.ValidCorpseForImplant(target, throwMessages, corpse);
			}
			return ReimplanterUtility.ImplanterValidation(parent.def, parent.pawn, target, throwMessages, !IgnoreIdeo);
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
			yield return MoteMaker.MakeAttachedOverlay(target.Thing, ThingDefOf.Mote_XenogermImplantation, new Vector3(0f, 0f, 0.3f));
		}

	}

}
