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

}
