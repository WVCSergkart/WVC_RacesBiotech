using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    //public class Gene_Hivemind_EfficiencyNode : Gene_Hivemind
    //{

    //    public static int? cachedEfficiencyOffset;
    //    public static int EfficiencyOffset
    //    {
    //        get
    //        {
    //            if (!cachedEfficiencyOffset.HasValue)
    //            {
    //                cachedEfficiencyOffset = 0;
    //            }
    //            return cachedEfficiencyOffset.Value;
    //        }
    //    }

    //    private static void SetEfficiency()
    //    {
    //        MiscUtility.UpdateStaticCollection();
    //        float efficiency = 0;
    //        List<Pawn> hivemindPawns = HivemindUtility.HivemindPawns;
    //        foreach (Pawn hiver in hivemindPawns)
    //        {
    //            efficiency += (hiver.GetStatValue(StatDefOf.PsychicSensitivity) - 1f) * 100f;
    //            if (StaticCollectionsClass.cachedPlayerPawnsCount == hivemindPawns.Count)
    //            {
    //                efficiency += 10000;
    //            }
    //            else
    //            {
    //                efficiency += (hivemindPawns.Count - StaticCollectionsClass.cachedPlayerPawnsCount) * 100f;
    //            }
    //        }
    //        Gene_Hivemind_EfficiencyNode.cachedEfficiencyOffset = (int)(efficiency * -1f);
    //    }

    //    public override void UpdGeneSync()
    //    {
    //        SetEfficiency();
    //    }

    //}

}
