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
                Messages.Message("WVC_PawnIsAndroidCheck".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
                return;
            }
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
