using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{
	public class MentalBreakWorker_Duplicate : MentalBreakWorker
	{
		public override bool BreakCanOccur(Pawn pawn)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			if (pawn.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.CanDuplicate() != true)
            {
                return false;
            }
            return base.BreakCanOccur(pawn);
		}

		public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
		{
			TrySendLetter(pawn, "WVC_XaG_LetterDuplicatorMentalBreak", reason);
			pawn.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.TryDuplicate();
			pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.Catharsis);
			return true;
		}
	}

	//public class MentalState_Duplicator : MentalState
	//{

	//	public override void PostStart(string reason)
	//	{
	//		base.PostStart(reason);
	//		pawn.genes?.GetFirstGeneOfType<Gene_Duplicator>()?.TryDuplicate();
	//		RecoverFromState();
	//	}

	//}

	//public class MentalStateWorker_Duplicator : MentalStateWorker
	//{

	//	public override bool StateCanOccur(Pawn pawn)
	//	{
	//		if (pawn.genes?.GetFirstGeneOfType<Gene_Duplicator>() == null)
	//		{
	//			return false;
	//		}
	//		return base.StateCanOccur(pawn);
	//	}
	//}

}
