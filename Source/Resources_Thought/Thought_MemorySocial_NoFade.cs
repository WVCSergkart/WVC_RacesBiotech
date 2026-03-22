using RimWorld;

namespace WVC_XenotypesAndGenes
{
	//public class Thought_HivemindSharedThoghts : Thought_Memory
	//{

	//	public override bool ShouldDiscard => !HivemindUtility.InHivemind(pawn);

	//	public override float MoodOffset()
	//	{
	//		return Gene_Hivemind_Thoughts.HivemindMood;
	//	}

	//}
	public class Thought_MemorySocial_NoFade : Thought_MemorySocial
	{

		public override float OpinionOffset()
		{
			if (RimWorld.ThoughtUtility.ThoughtNullified(pawn, def))
			{
				return 0f;
			}
			if (ShouldDiscard)
			{
				return 0f;
			}
			return opinionOffset;
		}

	}

}
