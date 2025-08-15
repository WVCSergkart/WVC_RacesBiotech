using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    public class BloodeaterMode_Hemogen : BloodeaterMode
	{

		public override bool GetFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
		{
			return TryHuntForFood(pawn, abilityDef, requestQueueing, queue, true);
		}

        public override bool CanBloodFeedNowWith(Pawn pawn, Pawn victim)
        {
            return GeneFeaturesUtility.CanBloodFeedNowWith(pawn, victim, true);
        }

        public override bool GetFood_Caravan(Pawn pawn, Pawn victim, Caravan caravan)
		{
			if (!CanBloodFeedNowWith(pawn, victim))
			{
				return false;
			}
			SanguophageUtility.DoBite(pawn, victim, 0.2f, 0.9f * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000), 0.4f, 1f, new(0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
			return true;
		}

		public static bool TryHuntForFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false, bool reqHemogen = false)
		{
			if (!queue && Gene_Rechargeable.PawnHaveThisJob(pawn, MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee))
			{
				return false;
			}
			// =
			List<Pawn> targets = MiscUtility.GetAllPlayerControlledMapPawns_ForBloodfeed(pawn);
			// =
			foreach (Pawn colonist in targets)
			{
				if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, colonist, reqHemogen))
				{
					continue;
				}
				if (colonist.IsForbidden(pawn) || !pawn.CanReserveAndReach(colonist, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				//if (colonist.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
				//{
				//	continue;
				//}
				if (!MiscUtility.TryGetAbilityJob(pawn, colonist, abilityDef, out Job job))
				{
					continue;
				}
				job.def = MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee;
				pawn.TryTakeOrderedJob(job, JobTag.SatisfyingNeeds, requestQueueing);
				return true;
			}
			return false;
		}

	}

}
