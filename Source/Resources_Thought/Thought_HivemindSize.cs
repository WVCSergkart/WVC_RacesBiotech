using RimWorld;

namespace WVC_XenotypesAndGenes
{
	public class Thought_HivemindSize : Thought_Memory
	{

		public override float MoodOffset()
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return base.BaseMoodOffset * HivemindUtility.nonPlayerHivemindSize;
			}
			return base.BaseMoodOffset * (HivemindUtility.HivemindPawns.Count - 1);
		}

	}

}
