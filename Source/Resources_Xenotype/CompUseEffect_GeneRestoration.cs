using System.Linq;
using System.Collections.Generic;
using System.IO;
using RimWorld;
using Verse;
using Verse.AI;
using WVC;
using WVC_XenotypesAndGenes;

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
			SerumUtility.HumanityCheck(pawn);
			// Main
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating));
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogerminationComa))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogerminationComa));
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermLossShock))
			{
				pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermLossShock));
			}
		}

	}

}
