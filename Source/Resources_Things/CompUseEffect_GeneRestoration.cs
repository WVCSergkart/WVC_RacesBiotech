using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompUseEffect_GeneRestoration : CompUseEffect
	{
		public CompProperties_UseEffect_GeneRestoration Props => (CompProperties_UseEffect_GeneRestoration)props;

		public override void DoEffect(Pawn pawn)
		{
			// Humanity check
			// if (MechanoidizationUtility.PawnIsAndroid(pawn) || !pawn.RaceProps.Humanlike)
			// {
			// pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
			// Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
			// return;
			// }
			if (!SerumUtility.PawnIsHuman(pawn))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			// Main
			// XaG_GeneUtility.XenogermRestoration(pawn);
			HediffUtility.RemoveHediffsFromList(pawn, Props.hediffsToRemove);
		}

		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			failReason = null;
			if (!SerumUtility.PawnIsHuman(p))
			{
				failReason = "WVC_PawnIsAndroidCheck".Translate();
				return false;
			}
			return true;
		}

	}

	public class CompUseEffect_GeneticStabilizer : CompUseEffect_GeneRestoration
	{

		public override void DoEffect(Pawn pawn)
		{
			Gene_GeneticInstability geneticInstability = pawn?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>();
			if (!SerumUtility.PawnIsHuman(pawn) || geneticInstability == null)
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			geneticInstability.nextTick += 60000 * Props.daysDelay;
		}

		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			failReason = null;
			if (!SerumUtility.PawnIsHuman(p) || p?.genes?.GetFirstGeneOfType<Gene_GeneticInstability>() == null)
			{
				failReason = "WVC_PawnIsAndroidCheck".Translate();
				return false;
			}
			return true;
		}

	}

	public class CompUseEffect_GiveHediffIfHasGene : CompUseEffect
	{
		public CompProperties_UseEffect_GeneRestoration Props => (CompProperties_UseEffect_GeneRestoration)props;

		public override void DoEffect(Pawn pawn)
		{
			if (!SerumUtility.PawnIsHuman(pawn) || !XaG_GeneUtility.HasActiveGene(Props.geneDef, pawn))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
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

		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			failReason = null;
			if (!SerumUtility.PawnIsHuman(p) || !XaG_GeneUtility.HasActiveGene(Props.geneDef, p))
			{
				failReason = "WVC_PawnIsAndroidCheck".Translate();
				return false;
			}
			return true;
		}

	}

	public class CompUseEffect_GeneShapeshifterChanger : CompUseEffect
	{
		public CompProperties_UseEffect_GeneRestoration Props => (CompProperties_UseEffect_GeneRestoration)props;

		public override void DoEffect(Pawn pawn)
		{
			Gene_Shapeshifter shapeshifter = pawn?.genes?.GetFirstGeneOfType<Gene_Shapeshifter>();
			if (!SerumUtility.PawnIsHuman(pawn) || shapeshifter == null)
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_IncompatibilityComa);
				Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			if (Props.disableShapeshiftComaAfterUse)
			{
				shapeshifter.xenogermComaAfterShapeshift = false;
			}
		}

		public override bool CanBeUsedBy(Pawn p, out string failReason)
		{
			failReason = null;
			if (!SerumUtility.PawnIsHuman(p) || !p.IsShapeshifter())
			{
				failReason = "WVC_PawnIsAndroidCheck".Translate();
				return false;
			}
			return true;
		}

	}

}
