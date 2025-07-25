﻿using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WVC_XenotypesAndGenes
{
    public class BloodeaterMode
	{

		public BloodeaterModeDef def;

		public virtual bool CanUseAbility(Pawn pawn)
        {
			if (pawn.abilities.GetAbility(def.abilityDef) == null)
			{
				return false;
			}
			return true;
		}

		public virtual bool GetFood(Pawn pawn, AbilityDef abilityDef, bool requestQueueing = true, bool queue = false)
        {
			return GeneResourceUtility.TryHuntForFood(pawn, requestQueueing, queue);
		}

		public virtual bool GetFood_Caravan(Pawn pawn, Pawn victim, Caravan caravan)
		{
			if (!GeneFeaturesUtility.CanBloodFeedNowWith(pawn, victim))
			{
				return false;
			}
			SanguophageUtility.DoBite(pawn, victim, 0.2f, 0.9f * pawn.GetStatValue(StatDefOf.HemogenGainFactor, cacheStaleAfterTicks: 360000), 0.4f, 1f, new(0, 0), ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
			return true;
		}

	}

}
