using System;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_Telepathy : Gene_Hivemind_Drone
	{

		//public override void PostAdd()
		//{
		//	base.PostAdd();
		//	HotPatchesUtility.Patch_IsGoodPositionForInteraction();
		//	Recache();
		//}

		//public override void PostRemove()
		//{
		//	base.PostRemove();
		//	Recache();
		//}

		//public override void Notify_OverriddenBy(Gene overriddenBy)
		//{
		//	base.Notify_OverriddenBy(overriddenBy);
		//	Recache();
		//}

		//public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		//{
		//	base.Notify_PawnDied(dinfo, culprit);
		//	Recache();
		//}

		//public override void Notify_Override()
		//{
		//	base.Notify_Override();
		//	Recache();
		//}

		//private static bool? isAnyHivemindTelepath;
		//public static bool HivemindTelepathy
		//{
		//	get
		//	{
		//		if (isAnyHivemindTelepath == null)
		//		{
		//			isAnyHivemindTelepath = HivemindUtility.HivemindPawns.Any(drone => drone?.genes?.GetFirstGeneOfType<Gene_Hivemind_Telepathy>() != null);
		//		}
		//		return isAnyHivemindTelepath.Value;
		//	}
		//}

		//private static void Recache()
		//{
		//	isAnyHivemindTelepath = null;
		//}

		public static int tickBeforeNextRandomHivemindInteraction = -1;

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(2500, delta))
			{
				return;
			}
			try
			{
				RandomInteraction();
			}
			catch (Exception arg)
			{
				Log.Error($"Failed hivemind interaction. Reason: {arg.Message}");
				tickBeforeNextRandomHivemindInteraction = 60000;
			}
		}

		public void RandomInteraction()
		{
			int ticksGame = Find.TickManager.TicksGame;
			if (ticksGame < tickBeforeNextRandomHivemindInteraction)
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (!HivemindUtility.InHivemind(pawn))
			{
				return;
			}
			foreach (Pawn drone in Hivemind)
			{
				if (GeneInteractionsUtility.TryInteractRandomly(drone, Hivemind.Where((hiver) => hiver != drone && hiver.interactions?.InteractedTooRecentlyToInteract() == false && hiver.Spawned).ToList(), psychicInteraction: false, ignoreTalking: true, closeTarget: false, out Pawn otherPawn))
				{
					tickBeforeNextRandomHivemindInteraction = ticksGame + Mathf.Clamp((int)((10000 / Hivemind.Count) / PsyFactor), 2500, 99999);
					//EffectsUtility.PulseEffect(drone);
					//EffectsUtility.PulseEffect(otherPawn);
					if (Rand.Chance(0.75f))
					{
						break;
					}
				}
			}
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	HotPatchesUtility.Patch_IsGoodPositionForInteraction();
		//}

	}

}
