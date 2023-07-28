using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Ageless : Gene
    {

        public override void PostAdd()
        {
            base.PostAdd();
            List<Gene> endogenes = pawn.genes.Endogenes;
            if (endogenes.Contains(this))
            {
                // if (pawn.ageTracker.AgeBiologicalTicks >= 68400000f)
                // {
                // pawn.ageTracker.AgeBiologicalTicks = 65800000;
                // }
                AgelessUtility.Rejuvenation(pawn);
            }
            // List<Gene> endogenes = pawn.genes.Endogenes;
            // for (int i = 0; i < endogenes.Count; i++)
            // {
            // if (endogenes[i].def.defName == def.defName)
            // {
            // }
            // }
        }

        // public override void Tick()
        // {
        // base.Tick();
        // if (!pawn.IsHashIntervalTick(60000))
        // {
        // return;
        // }
        // if (Active && pawn.ageTracker.AgeBiologicalTicks >= 75600000f)
        // {
        // int num = (int)(3600000f * pawn.ageTracker.AdultAgingMultiplier);
        // long val = (long)(3600000f * pawn.ageTracker.AdultMinAge);
        // pawn.ageTracker.AgeBiologicalTicks = Math.Max(val, pawn.ageTracker.AgeBiologicalTicks - num);
        // pawn.ageTracker.ResetAgeReversalDemand(Pawn_AgeTracker.AgeReversalReason.ViaTreatment);
        // int num2 = (int)(pawn.ageTracker.AgeReversalDemandedDeadlineTicks / 60000);
        // string text = "BiosculpterAgeReversalCompletedMessage".Translate(pawn.Named("PAWN"));
        // Ideo ideo = pawn.Ideo;
        // if (ideo != null && ideo.HasPrecept(PreceptDefOf.AgeReversal_Demanded))
        // {
        // text += " " + "AgeReversalExpectationDeadline".Translate(pawn.Named("PAWN"), num2.Named("DEADLINE"));
        // }
        // Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
        // }
        // }
    }

}
