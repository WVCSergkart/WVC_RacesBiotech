using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Thought_RecluseMemory : Thought_Memory
	{

		public override float MoodOffset()
		{
			return base.MoodOffset() * (1 + (StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.2f) - 0.4f);
		}

	}

	public class Thought_HumanCentricMemory : Thought_Memory
	{

		public override float MoodOffset()
		{
			return base.MoodOffset() * (1 + (StaticCollectionsClass.cachedNonHumansCount * 0.1f) - 0.1f);
		}

	}

	public class Thought_PackMentality : Thought_Memory
	{

		public override bool ShouldDiscard => !Gene_PackMentality.ThePack.Contains(pawn);

		public override float MoodOffset()
		{
			return base.MoodOffset() * Gene_PackMentality.ThePack.Count;
		}

	}

	public class Thought_HivemindSize : Thought_Memory
	{

		public override float MoodOffset()
		{
			return base.MoodOffset() * (HivemindUtility.HivemindPawns.Count - 1);
		}

	}

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

	public class Thought_Social_PackMentality : Thought_MemorySocial_NoFade
	{

		public override bool ShouldDiscard => !Gene_PackMentality.ThePack.Contains(pawn);

	}

}
