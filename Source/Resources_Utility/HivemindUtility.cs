using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public static class HivemindUtility
    {

        private static List<Pawn> cachedPawns;
        public static List<Pawn> HivemindPawns
        {
            get
            {
                if (cachedPawns == null)
                {
                    cachedPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Where((target) => target.IsPsychicSensitive() && target.genes != null && target.genes.GenesListForReading.Any((gene) => gene is IGeneHivemind)).ToList();
                }
                return cachedPawns;
            }
        }


        /// <summary>
        /// General reset collection. Used for all hivemind genes.
        /// Modders hook.
        /// </summary>
        public static void ResetCollection()
        {
            cachedPawns = null;
            cachedRefreshRate = null;
            Gene_Chimera_HiveGeneline.cachedGenelineGenes = null;
            // HediffWithComps_ChimeraLimitFromHiveMind.curStage = null; // Reset by chimera gene
            HediffWithComps_Hivemind_Beauty.Recache();
            HediffWithComps_Hivemind_Learning.Recache();
        }

        private static int? cachedRefreshRate;
        public static int TickRefresh
        {
            get
            {
                if (!cachedRefreshRate.HasValue)
                {
                    cachedRefreshRate = (int)(11992 * ((HivemindPawns.Count > 1 ? HivemindPawns.Count : 5) * 0.4f));
                }
                return cachedRefreshRate.Value;
            }
        }

        /// <summary>
        /// Modders hook.
        /// </summary>
        public static bool SuitableForHivemind(Pawn pawn)
        {
            if (!pawn.IsColonist)
            {
                return false;
            }
            if (!pawn.IsPsychicSensitive())
            {
                return false;
            }
            return true;
        }

        public static bool InHivemind(Pawn pawn)
		{
            // Cause hivemind initialized after game start
            if (MiscUtility.GameNotStarted())
            {
                return true;
            }
			return HivemindUtility.HivemindPawns.Contains(pawn);
		}

	}
}
