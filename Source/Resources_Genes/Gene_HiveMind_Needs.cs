using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Hivemind_Needs : Gene_Hivemind_Drone
	{

		//public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

		private class NeedHolder
		{

			public NeedDef needDef;

			public float needValue;

		}

		private class NeedsHolder
		{

			public List<NeedHolder> holders = new();

			public bool TryGetNeed(NeedDef needDef, out NeedHolder needHolder)
			{
				needHolder = null;
				foreach (NeedHolder holder in holders)
				{
					if (holder.needDef == needDef)
					{
						needHolder = holder;
						return true;
					}
				}
				return false;
			}

			public void AddNeed(Need need)
			{
				NeedHolder needHolder = new();
				needHolder.needDef = need.def;
				needHolder.needValue = need.CurLevel;
				holders.Add(needHolder);
			}

		}

		public static int delayBeforeSync;
		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(2500, delta))
			{
				TryUpdNeeds();
			}
		}

		public void TryUpdNeeds()
		{
			int ticksGame = Find.TickManager.TicksGame;
			if (delayBeforeSync > ticksGame)
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
			string phase = "start";
			try
			{
				List<Pawn> bondedPawns = Hivemind;
				NeedsHolder sumSkillsExp = new();
				foreach (Pawn otherPawn in bondedPawns)
				{
					phase = "get needs";
					GetNeeds(otherPawn, sumSkillsExp);
				}
				foreach (Pawn otherPawn in bondedPawns)
				{
					phase = "set needs";
					SetNeeds(otherPawn, sumSkillsExp);
				}
				delayBeforeSync = ticksGame + Mathf.Clamp((int)(250 * bondedPawns.Count / PsyFactor), 2500, HivemindUtility.TickRefresh);
			}
			catch (Exception arg)
			{
				Log.Error("Failed sync hive needs. On phase: " + phase + ". Reason: " + arg);
				delayBeforeSync = 60000;
			}
		}

		private void GetNeeds(Pawn target, NeedsHolder needsHolder)
		{
			if (target.needs == null)
			{
				return;
			}
			foreach (Need need in target.needs.AllNeeds)
			{
				if (!Extension_Giver.needDefs.Contains(need.def))
				{
					continue;
				}
				if (needsHolder.TryGetNeed(need.def, out NeedHolder needHolder))
				{
					if (needHolder.needValue < need.CurLevel)
					{
						needHolder.needValue = need.CurLevel;
					}
				}
				else
				{
					needsHolder.AddNeed(need);
				}
			}
		}

		private void SetNeeds(Pawn target, NeedsHolder needsHolder)
		{
			//Log.Error("0");
			if (target.needs == null)
			{
				//Log.Error("Fail");
				return;
			}
			//Log.Error("1");
			foreach (Need need in target.needs.AllNeeds)
			{
				//Log.Error("2");
				//if (!Extension_Giver.needDefs.Contains(need.def))
				//{
				//	//Log.Error("WTF?");
				//	continue;
				//}
				//Log.Error("3");
				if (needsHolder.TryGetNeed(need.def, out NeedHolder needHolder))
				{
					need.CurLevel = needHolder.needValue;
				}
			}
		}

	}

	[Obsolete("Use Gene_Hivemind_Needs instead.")]
	public class Gene_HiveMind_Needs : Gene_Hivemind_Needs
	{

	}

}
