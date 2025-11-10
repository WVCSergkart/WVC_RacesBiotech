using System;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	// public class CompUseEffect_GeneShapeshifterModes : CompUseEffect
	// {
	// public CompProperties_UseEffect_GeneRestoration Props => (CompProperties_UseEffect_GeneRestoration)props;

	// public override void DoEffect(Pawn pawn)
	// {
	// Gene_Shapeshifter shapeshifter = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
	// if (!SerumUtility.IsHuman(pawn) || shapeshifter == null)
	// {
	// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
	// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
	// return;
	// }
	// if (Props.unlockModes.NullOrEmpty())
	// {
	// return;
	// }
	// foreach (ShapeshiftModeDef shapeshiftModeDef in Props.unlockModes)
	// {
	// shapeshifter.UnlockMode(shapeshiftModeDef);
	// }
	// }

	// public override AcceptanceReport CanBeUsedBy(Pawn p)
	// {
	// if (!SerumUtility.IsHuman(p) || !p.IsShapeshifter())
	// {
	// return "WVC_PawnIsAndroidCheck".Translate();
	// }
	// return true;
	// }

	// }

	[Obsolete]
	public class CompUseEffect_XenotypeNullifier : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			// if (SerumUtility.HumanityCheck(pawn))
			// {
			// return;
			// }
			// if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			// {
			// pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
			// return;
			// }
			if (Props.removeSkinColor)
			{
				ReimplanterUtility.SetXenotype(pawn, XenotypeDefOf.Baseliner);
			}
			else
			{
				DuplicateUtility.NullifyXenotype(pawn);
			}
			//pawn.health.AddHediff(HediffDefOf.XenogerminationComa);
			//GeneUtility.UpdateXenogermReplication(pawn);
			ReimplanterUtility.UpdateXenogermReplication_WithComa(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "WVC_LetterTextGenesImplanted".Translate(pawn.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION")), LetterDefOf.NeutralEvent, new LookTargets(pawn));
			}
			ReimplanterUtility.PostSerumUsedHook(pawn, false);
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			// if (p.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			// {
			// return "WVC_XaG_GeneShapeshifter_DisabledGenesRegrowing".Translate();
			// }
			return true;
		}

	}

}
