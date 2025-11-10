using RimWorld;
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
