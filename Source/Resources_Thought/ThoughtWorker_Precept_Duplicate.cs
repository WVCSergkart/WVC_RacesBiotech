using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class ThoughtWorker_Precept_Social_MyDuplicate : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!ModsConfig.IdeologyActive)
			{
				return ThoughtState.Inactive;
			}
			if (!otherPawn.IsDuplicate)
            {
				return ThoughtState.Inactive;
            }
			//Log.Error("GetSourceCyclic");
			return p.GetSourceCyclic() == otherPawn.GetSourceCyclic();
		}

	}

	public class ThoughtWorker_Precept_Social_MySource : ThoughtWorker_Precept_Social
	{

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			if (!ModsConfig.IdeologyActive)
			{
				return ThoughtState.Inactive;
			}
			if (!p.IsDuplicate)
			{
				return ThoughtState.Inactive;
			}
			//Log.Error("GetSourceCyclic");
			return p.GetSourceCyclic() == otherPawn;
		}

	}

}
