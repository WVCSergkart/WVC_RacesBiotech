using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class Gene_Hivemind_SharedFood : Gene_Hivemind
    {

        public override void SyncHive()
        {
            try
            {
                List<Pawn> hivemind = HivemindUtility.HivemindPawns;
                float totalNutriotion = 0;
                foreach (Pawn pawn in hivemind)
                {
                    if (!pawn.TryGetNeedFood(out Need_Food need_Food))
                    {
                        continue;
                    }
                    totalNutriotion += need_Food.CurLevel;
                }
                float averageNutrition = totalNutriotion / hivemind.Count;
                foreach (Pawn pawn in hivemind)
                {
                    if (!pawn.TryGetNeedFood(out Need_Food need_Food))
                    {
                        continue;
                    }
                    need_Food.CurLevel = averageNutrition;
                }
            }
            catch (Exception arg)
            {
                Log.Error("Failed share food for hivemind. Reason: " + arg.Message);
            }
        }

    }

}
