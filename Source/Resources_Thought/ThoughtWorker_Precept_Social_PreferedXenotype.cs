using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	//public class ThoughtWorker_Precept_Social_PreferredXenotype : ThoughtWorker_Precept_Social
	//{

	//	public static Dictionary<Ideo, Pawn> notPreferredPawns = new();
	//	public static Dictionary<Ideo, Pawn> preferredPawns = new();

	//	public static bool IsPreferredXenotype(Pawn caller, Ideo ideo)
	//	{
	//		if (notPreferredPawns.TryGetValue(ideo, out _))
	//		{
	//			return false;
	//		}
	//		if (preferredPawns.TryGetValue(ideo, out _))
	//		{
	//			return true;
	//		}
	//		List<Gene> genesListForReading = caller.genes.GenesListForReading;
	//		foreach (XenotypeDef xenotypeDef in ideo.PreferredXenotypes)
	//		{
	//			if (!XaG_GeneUtility.GenesIsMatch(genesListForReading, xenotypeDef.genes, 0.6f))
	//			{
	//				notPreferredPawns.Add(ideo, caller);
	//			}
	//			else
	//			{
	//				preferredPawns.Add(ideo, caller);
	//			}
	//		}
	//		foreach (CustomXenotype xenotypeDef in ideo.PreferredCustomXenotypes)
	//		{
	//			if (!XaG_GeneUtility.GenesIsMatch(genesListForReading, xenotypeDef.genes, 0.6f))
	//			{
	//				notPreferredPawns.Add(ideo, caller);
	//			}
	//			else
	//			{
	//				preferredPawns.Add(ideo, caller);
	//			}
	//		}
	//		return true;
	//	}

	//	public static void UpdCollection()
	//	{
	//		preferredPawns = new();
	//		notPreferredPawns = new();
	//	}

	//	protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
	//	{
	//		if (otherPawn.genes == null)
	//		{
	//			return ThoughtState.Inactive;
	//		}
	//		if (!IsPreferredXenotype(p, p.Ideo))
	//		{
	//			return ThoughtState.Inactive;
	//		}
	//		if (IsPreferredXenotype(otherPawn, p.Ideo))
	//		{
	//			return ThoughtState.ActiveAtStage(0);
	//		}
	//		return ThoughtState.ActiveAtStage(1);
	//	}

	//}

	//public class ThoughtWorker_Precept_ColonyXenotypeMakeup : ThoughtWorker_Precept
	//{
	//	protected override ThoughtState ShouldHaveThought(Pawn p)
	//	{
	//		if (p.Faction == null)
	//		{
	//			return ThoughtState.Inactive;
	//		}
	//		if (!ThoughtWorker_Precept_Social_PreferredXenotype.IsPreferredXenotype(p, p.Ideo))
	//		{
	//			return ThoughtState.Inactive;
	//		}
	//		//List<Pawn> list = p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction);
	//		int num = 0;
	//		int num2 = 0;
	//		//bool flag = p.IsSlave || p.IsPrisoner;
	//		//foreach (Pawn item in list)
	//		//{
	//		//	bool flag2 = item.IsSlave || item.IsPrisoner;
	//		//	if (item.genes != null && flag == flag2)
	//		//	{
	//		//		num++;
	//		//		if (!p.Ideo.IsPreferredXenotype(item))
	//		//		{
	//		//			num2++;
	//		//		}
	//		//	}
	//		//}
	//		if (num2 == 0)
	//		{
	//			return ThoughtState.ActiveAtStage(0);
	//		}
	//		float num3 = (float)num2 / (float)num;
	//		if (num3 < 0.33f)
	//		{
	//			return ThoughtState.ActiveAtStage(1);
	//		}
	//		if (num3 < 0.66f)
	//		{
	//			return ThoughtState.ActiveAtStage(2);
	//		}
	//		return ThoughtState.ActiveAtStage(3);
	//	}

	//}

}
