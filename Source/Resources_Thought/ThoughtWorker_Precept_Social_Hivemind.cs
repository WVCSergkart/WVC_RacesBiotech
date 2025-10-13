using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ThoughtWorker_Precept_Social_Hivemind : ThoughtWorker_Precept_Social
	{

		public static bool InHivemind(Pawn pawn)
        {
			return Gene_Hivemind.HivemindPawns.Contains(pawn);
		}

		protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn)
		{
			// I am hiver
			if (InHivemind(p))
			{
				// My friend non hiver
				if (!InHivemind(otherPawn))
				{
					return ThoughtState.ActiveAtStage(1);
				}
				// My friend is hiver too
				return ThoughtState.ActiveAtStage(2);
			}
			// I am not hiver, but my friend..
			if (InHivemind(otherPawn))
			{
				return ThoughtState.ActiveAtStage(0);
			}
			// No hivers
			return ThoughtState.Inactive;
		}

	}

    public class ThoughtWorker_Precept_Hivemind : ThoughtWorker_Precept
    {

        protected override ThoughtState ShouldHaveThought(Pawn p)
		{
			if (ThoughtWorker_Precept_Social_Hivemind.InHivemind(p))
			{
				return ThoughtState.ActiveDefault;
			}
			return ThoughtState.Inactive;
		}

    }

}
