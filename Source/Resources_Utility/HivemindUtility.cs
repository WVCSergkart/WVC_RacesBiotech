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
                    cachedPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.Where((target) => CanBeInHivemind(target)).ToList();
                }
                return cachedPawns;
            }
        }

        public static bool CanBeInHivemind(Pawn target)
        {
            if (!target.IsPsychicSensitive())
            {
                return false;
            }
            if (target.genes != null && target.genes.GenesListForReading.Any((gene) => gene is IGeneHivemind && gene.Active))
            {
                return true;
            }
            return SubCanBeInHivemind(target);
        }

        /// <summary>
        /// Modders hook. Use for custom hivemind rules. For example hediffs or for merging with others mod hiveminds.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>

        #pragma warning disable IDE0079 // Remove unnecessary suppression
        #pragma warning disable IDE0060 // Remove unused parameter
        public static bool SubCanBeInHivemind(Pawn target)
        {
            return false;
        }
        #pragma warning restore IDE0060 // Remove unused parameter
        #pragma warning restore IDE0079 // Remove unnecessary suppression

        /// <summary>
        /// General reset collection. Used for all hivemind genes.
        /// Modders hook.
        /// </summary>
        public static void ResetCollection()
        {
            cachedPawns = null;
            cachedRefreshRate = null;
            Gene_Chimera_HiveGeneline.cachedGenelineGenes = null;
            Gene_Hivemind_Regeneration.cachedRegenRate = null;
            // HediffWithComps_ChimeraLimitFromHiveMind.curStage = null; // Reset by chimera gene
            HediffWithComps_Hivemind_Beauty.Recache();
            HediffWithComps_Hivemind_Learning.Recache();
        }

        private static int cachedTickIndex = 1;
        public static int NextTickIndex
        {
            get
            {
                return cachedTickIndex;
            }
            set
            {
                cachedTickIndex = value;
                if (cachedTickIndex > 10)
                {
                    cachedTickIndex = 1;
                }
            }
        }

        /// <summary>
        /// Cycles the tick update from min to max.
        /// Distributes gene ticks so they overlap less often, which slightly improves performance.
        /// </summary>
        public static void ResetTick(ref int nextTick)
        {
            IntRange intRange = new((int)(HivemindUtility.TickRefresh * 0.8f), (int)(HivemindUtility.TickRefresh * 2f));
            if (HivemindUtility.NextTickIndex < 1)
            {
                nextTick = intRange.RandomInRange;
                //Log.Error("Index 0. New tick: " + nextTick);
            }
            else
            {
                nextTick = Mathf.Clamp(((intRange.TrueMax - intRange.TrueMin) / 10) * HivemindUtility.NextTickIndex + intRange.TrueMin, intRange.TrueMin, intRange.TrueMax);
                //Log.Error("New tick: " + nextTick);
            }
            HivemindUtility.NextTickIndex++;
        }

        /// <summary>
        /// The tick frequency decreases as the hivemind grows.
        /// This keeps the impact on performance to a minimum and is an important part of the hivemind mechanics.
        /// Synchronization latency increases, requiring more synchronization drones.
        /// </summary>
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
        /// Only affects gene effects and the sync- trigger.
        /// Pawns that fail these conditions can still be part of the hivemind.
        /// </summary>
        public static bool SuitableForHivemind(Pawn pawn)
        {
            // Hivemind is colonists only party
            if (!pawn.IsColonist)
            {
                return false;
            }
            // Basic vanilla check with cache
            if (!pawn.IsPsychicSensitive())
            {
                return false;
            }
            //Log.Error("SuitableForHivemind");
            // Deaf hivemind drones
            if (pawn.GetStatValue(StatDefOf.PsychicSensitivity) < 0.2f)
            {
                return false;
            }
            return true;
        }

        public static bool InHivemind(this Pawn pawn)
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
