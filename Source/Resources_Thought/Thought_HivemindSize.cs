using RimWorld;

namespace WVC_XenotypesAndGenes
{
	public class Thought_HivemindSize : Thought_Memory
	{

		public override float MoodOffset()
		{
			return base.MoodOffset() * (HivemindUtility.HivemindPawns.Count - 1);
		}

	}

}
