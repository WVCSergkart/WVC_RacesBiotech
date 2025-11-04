using System;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_HiveMind_Opinion : Gene_Hivemind_Drone
	{

		private GeneExtension_Opinion cachedExtension;
		public GeneExtension_Opinion Opinion
		{
			get
			{
				if (cachedExtension == null)
				{
					cachedExtension = def?.GetModExtension<GeneExtension_Opinion>();
				}
				return cachedExtension;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59998, delta))
			{
				return;
			}
			if (!HivemindUtility.SuitableForHivemind(pawn) || !HivemindUtility.InHivemind(pawn))
			{
				return;
			}
			SyncHive();
		}

		public virtual void SyncHive()
		{
			List<Pawn> bondedPawns = Hivemind;
			//string phase = "start";
			try
			{
				foreach (Pawn otherPawn in bondedPawns)
				{
					if (otherPawn == pawn)
					{
						continue;
					}
					otherPawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, pawn);
					pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.thoughtDef, otherPawn);
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed sync hive opinions. Reason: " + arg);
			}
		}

	}

	public class Gene_Hivemind_Mood : Gene_HiveMind_Opinion
	{

		public override void SyncHive()
		{
			pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(Opinion.thoughtDef);
		}

	}

}
