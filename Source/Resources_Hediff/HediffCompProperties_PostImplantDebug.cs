using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_PostImplantDebug : HediffCompProperties
	{

		public HediffCompProperties_PostImplantDebug()
		{
			compClass = typeof(HediffComp_PostImplantDebug);
		}

	}

	public class HediffComp_PostImplantDebug : HediffComp
	{

		public override void CompPostPostRemoved()
		{
			ReimplanterUtility.PostImplantDebug(Pawn);
		}

	}

	public class HediffCompProperties_XenogermReplicating : HediffCompProperties
	{

		public HediffCompProperties_XenogermReplicating()
		{
			compClass = typeof(HediffComp_XenogermReplicating);
		}

	}

	public class HediffComp_XenogermReplicating : HediffComp
	{

		public bool updated = false;

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//if (Pawn.IsHashIntervalTick(55555, delta))
			//{
			//	if (Rand.Chance(0.1f) && !Pawn.health.hediffSet.AnyHediffMakesSickThought)
			//	{
			//		ReimplanterUtility.XenogermReplicating_WithCustomDuration(Pawn, new IntRange(-60000 * 30, -60000 * 15), parent);
			//	}
			//}
			if (updated)
			{
				return;
			}
			updated = true;
			//SetDuration();
		}

		//private void SetDuration()
		//{
		//	if (MiscUtility.GameNotStarted())
		//	{
		//		return;
		//	}
		//	//if (Pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermLossShock))
		//	//{
		//	//	return;
		//	//}
		//	CompHumanlike comp = Pawn?.HumanComponent();
		//	if (comp == null)
		//	{
		//		return;
		//	}
		//	//Log.Error(Pawn.Name + " " + comp.newGenesCount);
		//	int newTick = CompHumanlike.newGenesCount * 60000;
		//	//Log.Error(Pawn.Name + " " + newTick.ToString());
		//	float? cleanness = Pawn.GetRoom()?.GetStat(RoomStatDefOf.InfectionChanceFactor);
		//	if (cleanness != null)
		//	{
		//		newTick = (int)(newTick * Mathf.Clamp(cleanness.Value, 0.8f, 1.5f));
		//	}
		//	else
		//	{
		//		newTick = (int)(newTick * 1.2f);
		//	}
		//	//Log.Error(Pawn.Name + " " + newTick.ToString());
		//	//ReimplanterUtility.XenogermReplicating_WithCustomDuration(Pawn, new(newTick, newTick), parent);
		//	Hediff firstHediffOfDef = Pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.XenogermReplicating);
		//	if (firstHediffOfDef != null)
		//	{
		//		HediffComp_Disappears hediffComp_Disappears = firstHediffOfDef.TryGetComp<HediffComp_Disappears>();
		//		if (hediffComp_Disappears != null)
		//		{
		//			hediffComp_Disappears.ticksToDisappear = newTick;
		//		}
		//	}
		//	//comp.ResetLastNewGenes();
		//}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look(ref updated, "isXenogermReplicatingUpdated", false);
		}

	}

}
