using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_Cellur : HediffCompProperties
	{

		public bool forceNoDeathNotification = true;

		public HediffCompProperties_Cellur()
		{
			compClass = typeof(HediffCompCellur);
		}
	}

	public class HediffCompCellur : HediffComp
	{

		public HediffCompProperties_Cellur Props => (HediffCompProperties_Cellur)props;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			Pawn.forceNoDeathNotification = Props.forceNoDeathNotification;
		}

		public override void CompPostPostRemoved()
		{
			Pawn.forceNoDeathNotification = false;
		}

		public override void Notify_Spawned()
		{
			Pawn.forceNoDeathNotification = Props.forceNoDeathNotification;
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (Pawn.Faction != Faction.OfPlayer && !WVC_Biotech.settings.canNonPlayerPawnResurrect || !Pawn.Dead)
			{
				return;
			}
			ResurrectionParams resurrectionParams = new();
			resurrectionParams.restoreMissingParts = false;
			resurrectionParams.noLord = true;
			resurrectionParams.removeDiedThoughts = true;
			resurrectionParams.canPickUpOpportunisticWeapons = true;
			resurrectionParams.gettingScarsChance = 0f;
			resurrectionParams.canKidnap = false;
			resurrectionParams.canSteal = false;
			resurrectionParams.breachers = false;
			resurrectionParams.sappers = false;
			ResurrectionUtility.TryResurrect(Pawn, resurrectionParams);
			Pawn.health.forceDowned = true;
		}

	}

}
