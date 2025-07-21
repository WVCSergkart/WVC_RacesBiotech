using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
    public class BloodeaterMode_Corpse : BloodeaterMode
	{

		public override bool GetFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
		{
			return TryHuntForFood(pawn, abilityDef, requestQueueing, queue);
		}

		public override bool GetFood_Caravan(Pawn pawn, Pawn victim, Caravan caravan)
		{
			if (pawn.TryGetNeedFood(out Need_Food food))
            {
				food.CurLevel = food.MaxLevel;
            }
			return true;
		}

		public static bool TryHuntForFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
		{
			if (!queue && Gene_Rechargeable.PawnHaveThisJob(pawn, MainDefOf.WVC_XaG_CastBloodfeedOnPawnMelee))
			{
				return false;
			}
			// =
			List<Thing> targets = pawn.Map.listerThings.AllThings.Where((thing) => thing is Corpse corpse && !corpse.IsUnnaturalCorpse()).ToList();
			// =
			foreach (Thing thing in targets)
			{
				if (thing is not Corpse corpse)
				{
					continue;
				}
				if (corpse.IsForbidden(pawn) || !pawn.CanReserveAndReach(corpse, PathEndMode.OnCell, pawn.NormalMaxDanger()))
				{
					continue;
				}
				if (!MiscUtility.TryGetAbilityJob(pawn, corpse, abilityDef, out Job job))
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
