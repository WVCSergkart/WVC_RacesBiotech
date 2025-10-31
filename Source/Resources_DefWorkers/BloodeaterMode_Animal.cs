using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class BloodeaterMode_Animal : BloodeaterMode
	{

		public override bool GetFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
		{
			return TryHuntForFood(pawn, abilityDef, requestQueueing, queue);
		}

		public override bool GetFood_Caravan(Pawn pawn, Pawn victim, Caravan caravan)
		{
			if (!CanBloodFeedNowWith(pawn, victim))
			{
				return false;
			}
			SanguophageUtility.DoBite(pawn, victim, 0.2f, 0.9f * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000), 0.4f, 1f, new(0, 0), null, null);
			return true;
		}

		public override bool CanBloodFeedNowWith(Pawn pawn, Pawn victim)
		{
			return victim.IsAnimal && victim.CanBleed() && victim.Faction == pawn.Faction && !victim.health.hediffSet.HasHediff(HediffDefOf.BloodLoss);
		}

		public static bool TryHuntForFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
		{
			if (!queue && Gene_Rechargeable.PawnHaveThisJob(pawn, MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee))
			{
				return false;
			}
			// =
			List<Pawn> targets = pawn.Map.mapPawns.AllPawnsSpawned.Where((target) => target.IsAnimal && target.Faction == pawn.Faction).ToList();
			// =
			foreach (Pawn animal in targets)
			{
				if (animal.health.hediffSet.HasHediff(HediffDefOf.BloodLoss))
				{
					continue;
				}
				if (animal.IsForbidden(pawn) || !pawn.CanReserveAndReach(animal, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				if (!MiscUtility.TryGetAbilityJob(pawn, animal, abilityDef, out Job job))
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
