using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_GeneRestoration : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffsToRemove);
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

	public class CompUseEffect_GeneticStabilizer : CompUseEffect_GeneRestoration
	{

		public override void DoEffect(Pawn pawn)
		{
			// Gene_GeneticInstability geneticInstability = pawn?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>();
			// if (!ReimplanterUtility.IsHuman(pawn) || geneticInstability == null)
			// {
				// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// return;
			// }
			pawn.genes.GetFirstGeneOfType<Gene_GeneticInstability>().nextTick += 60000 * Props.daysDelay;
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || p?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>() == null)
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

	public class CompUseEffect_GiveHediffIfHasGene : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			// if (!ReimplanterUtility.IsHuman(pawn) || !XaG_GeneUtility.HasActiveGene(Props.geneDef, pawn))
			// {
				// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// return;
			// }
			if (!pawn.health.hediffSet.HasHediff(Props.hediffDef))
			{
				pawn.health.AddHediff(Props.hediffDef);
			}
			else
			{
				Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
				HediffComp_Disappears hediffComp_Disappears = hediff.TryGetComp<HediffComp_Disappears>();
				if (hediffComp_Disappears != null)
				{
					hediffComp_Disappears.ticksToDisappear += hediffComp_Disappears.Props.disappearsAfterTicks.RandomInRange;
				}
			}
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || !XaG_GeneUtility.HasActiveGene(Props.geneDef, p))
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

	public class CompUseEffect_GeneShapeshifterChanger : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			Gene_Shapeshifter shapeshifter = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
			// if (!ReimplanterUtility.IsHuman(pawn) || shapeshifter == null)
			// {
				// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				// return;
			// }
			if (Props.disableShapeshiftComaAfterUse)
			{
				shapeshifter.xenogermComaAfterShapeshift = false;
			}
			if (Props.disableShapeshiftGenesRegrowAfterUse)
			{
				shapeshifter.genesRegrowAfterShapeshift = false;
			}
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || !p.IsShapeshifter())
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

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

	public class CompUseEffect_GeneMorpherForms : CompUseEffect
	{
		public CompProperties_UseEffect_XenogermSerum Props => (CompProperties_UseEffect_XenogermSerum)props;

		public override void DoEffect(Pawn pawn)
		{
			pawn?.genes?.GetFirstGeneOfType<Gene_Morpher>()?.AddLimit();
		}

		public override AcceptanceReport CanBeUsedBy(Pawn p)
		{
			if (!ReimplanterUtility.IsHuman(p) || !p.IsMorpher())
			{
				return "WVC_PawnIsAndroidCheck".Translate();
			}
			return true;
		}

	}

	public class CompUseEffect_GeneDuplicatorResetAbility : CompUseEffect
	{

		public override void DoEffect(Pawn pawn)
		{
			pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.ResetAbility();
		}

	}

}
