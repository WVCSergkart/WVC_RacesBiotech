using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_PermanentHediff : Gene
    {

        public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;
        public List<BodyPartDef> Bodyparts => def.GetModExtension<GeneExtension_Giver>().bodyparts;

        public override void PostAdd()
        {
            base.PostAdd();
            // ResetChance();
            // Log.Error("Ролим шанс. Шанс = " + chanceRange);
            // if (Active && chanceRange == 3)
            // {
            // }
            if (!pawn.health.hediffSet.HasHediff(HediffDefName))
            {
                // Gene_RandomHediffFromTime.HediffGiver(def.GetModExtension<GeneExtension_Giver>().bodyparts, HediffDefName, pawn);
                // int num = 0;
                // foreach (BodyPartDef bodypart in def.GetModExtension<GeneExtension_Giver>().bodyparts)
                // {
                // if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
                // {
                // pawn.health.AddHediff(HediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
                // num++;
                // }
                // }
                BodyPartGiver(Bodyparts, pawn, HediffDefName);
            }
        }

        public static void BodyPartGiver(List<BodyPartDef> bodyparts, Pawn pawn, HediffDef hediffDefName)
        {
            int num = 0;
            foreach (BodyPartDef bodypart in bodyparts)
            {
                if (!pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty() && num <= pawn.RaceProps.body.GetPartsWithDef(bodypart).Count)
                {
                    pawn.health.AddHediff(hediffDefName, pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray()[num]);
                    num++;
                }
            }
        }
    }

}
