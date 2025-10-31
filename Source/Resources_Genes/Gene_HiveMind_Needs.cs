using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_HiveMind_Needs : Gene_Hivemind
	{

		public GeneExtension_Giver Giver => def.GetModExtension<GeneExtension_Giver>();

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

		public override void UpdGeneSync()
		{
			base.UpdGeneSync();
			List<Pawn> bondedPawns = Hivemind;
			string phase = "start";
			try
			{
				NeedsHolder sumSkillsExp = new();
				foreach (Pawn otherPawn in bondedPawns)
				{
					//Gene_HiveMind hiveMind = otherPawn.genes.GetFirstGeneOfType<Gene_HiveMind>();
					//if (hiveMind == null)
					//{
					//    continue;
					//}
					phase = "get needs";
					GetNeeds(otherPawn, sumSkillsExp);
				}
				foreach (Pawn otherPawn in bondedPawns)
				{
					phase = "set needs";
					SetNeeds(otherPawn, sumSkillsExp);
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed sync hive needs. On phase: " + phase + ". Reason: " + arg);
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
				if (!Giver.needDefs.Contains(need.def))
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
				if (!Giver.needDefs.Contains(need.def))
				{
					//Log.Error("WTF?");
					continue;
				}
				//Log.Error("3");
				if (needsHolder.TryGetNeed(need.def, out NeedHolder needHolder))
				{
					need.CurLevel = needHolder.needValue;
				}
			}
		}

	}

}
