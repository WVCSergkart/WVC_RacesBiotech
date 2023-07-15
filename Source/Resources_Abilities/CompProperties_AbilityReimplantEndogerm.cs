using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class CompProperties_AbilityReimplantEndogerm : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityReimplantEndogerm()
		{
			compClass = typeof(CompAbilityEffect_ReimplantEndogerm);
		}
	}

	public class CompAbilityEffect_ReimplantEndogerm : CompAbilityEffect
	{
		// private static readonly CachedTexture ReimplantIcon = new CachedTexture("WVC/UI/Genes/Reimplanter");

#pragma warning disable IDE0051 // Remove unused private members
        private new CompProperties_AbilityReimplantEndogerm Props => (CompProperties_AbilityReimplantEndogerm)props;
#pragma warning restore IDE0051 // Remove unused private members

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
                MechanoidizationUtility.ReimplantEndogerm(parent.pawn, pawn);
				FleckMaker.AttachedOverlay(pawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
				if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
				}
			}
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return base.Valid(target, throwMessages);
			}
			if (pawn.IsQuestLodger())
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			// if (MechanoidizationUtility.PawnIsAndroid(pawn) || MechanoidizationUtility.PawnCannotUseSerums(pawn))
			// {
				// if (throwMessages)
				// {
					// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// }
				// return false;
			// }
			// if (GeneUtility.SameXenotype(pawn, parent.pawn))
			// {
				// if (throwMessages)
				// {
					// Messages.Message("MessageCannotUseOnSameXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// }
				// return false;
			// }
			// if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
			// {
				// if (throwMessages)
				// {
					// Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// }
				// return false;
			// }
			return base.Valid(target, throwMessages);
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

		// public override IEnumerable<Gizmo> CompGetGizmosExtra()
		// {
			// Pawn myPawn = parent.pawn;
			// if (parent.pawn?.genes == null)
			// {
				// yield break;
			// }
			// if (!parent.pawn.genes.HasGene(WVC_GenesDefOf.WVC_EndogermReimplanter))
			// {
				// yield break;
			// }
			// if (parent.pawn.IsPrisonerOfColony && parent.pawn.guest.PrisonerIsSecure)
			// {
				// yield break;
			// }
			// if (!parent.pawn.Downed)
			// {
				// yield break;
			// }
			// Command_Action command_Action = new Command_Action
			// {
				// defaultLabel = "ForceXenogermImplantation".Translate(),
				// defaultDesc = "ForceXenogermImplantationDesc".Translate(),
				// icon = ReimplantIcon.Texture,
				// action = delegate
				// {
					// List<FloatMenuOption> list = new List<FloatMenuOption>();
					// List<Pawn> list2 = myPawn.MapHeld.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
					// for (int i = 0; i < list2.Count; i++)
					// {
						// Pawn absorber = list2[i];
						// if (absorber.genes != null && absorber.IsColonistPlayerControlled && absorber.CanReach(myPawn, PathEndMode.ClosestTouch, Danger.Deadly))
						// {
							// if (!PawnIdeoCanAcceptReimplant(parent.pawn, absorber))
							// {
								// list.Add(new FloatMenuOption(absorber.LabelCap + ": " + "IdeoligionForbids".Translate(), null, absorber, Color.white));
							// }
							// else
							// {
								// list.Add(new FloatMenuOption(absorber.LabelShort, delegate
								// {
									// if (GeneUtility.PawnWouldDieFromReimplanting(myPawn))
									// {
										// Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromReimplanting".Translate(myPawn.Named("PAWN")), delegate
										// {
											// GeneUtility.GiveReimplantJob(absorber, myPawn);
										// }, destructive: true));
									// }
									// else
									// {
										// GeneUtility.GiveReimplantJob(absorber, myPawn);
									// }
								// }, absorber, Color.white));
							// }
						// }
					// }
					// if (list.Any())
					// {
						// Find.WindowStack.Add(new FloatMenu(list));
					// }
				// }
			// };
			// if (myPawn.IsQuestLodger())
			// {
				// command_Action.Disable("TemporaryFactionMember".Translate(myPawn.Named("PAWN")));
			// }
			// else if (myPawn.health.hediffSet.HasHediff(HediffDefOf.XenogermLossShock))
			// {
				// command_Action.Disable("XenogermLossShockPresent".Translate(myPawn.Named("PAWN")));
			// }
			// else if (myPawn.IsPrisonerOfColony && !myPawn.Downed)
			// {
				// command_Action.Disable("MessageTargetMustBeDownedToForceReimplant".Translate(myPawn.Named("PAWN")));
			// }
			// yield return command_Action;
		// }

		// public static bool PawnIdeoCanAcceptReimplant(Pawn implanter, Pawn implantee)
		// {
			// if (!ModsConfig.IdeologyActive)
			// {
				// return true;
			// }
			// if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.PropagateBloodfeederGene, implantee) && implanter.genes.Xenogenes.Any((Gene x) => x.def == GeneDefOf.Bloodfeeder))
			// {
				// return false;
			// }
			// if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.BecomeNonPreferredXenotype, implantee) && !implantee.Ideo.IsPreferredXenotype(implanter))
			// {
				// return false;
			// }
			// return true;
		// }
	}
}
