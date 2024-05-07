using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Thought_MemorySocial_WithStat : Thought_MemorySocial
	{

		[Unsaved(false)]
		private float statValue = 0;

		public float Stat
		{
			get
			{
				if (def.effectMultiplyingStat == null)
				{
					return 1f;
				}
				if (statValue == 0f)
				{
					statValue = pawn.GetStatValue(def.effectMultiplyingStat, cacheStaleAfterTicks: 360000);
				}
				return statValue;
			}
		}

		public override bool ShouldDiscard
		{
			get
			{
				if (otherPawn != null && (opinionOffset * Stat) != 0f)
				{
					return base.ShouldDiscard;
				}
				return true;
			}
		}

		public override float OpinionOffset()
		{
			return base.OpinionOffset() * Stat;
		}

		public override bool TryMergeWithExistingMemory(out bool showBubble)
		{
			showBubble = false;
			List<Thought_Memory> memories = pawn.needs.mood.thoughts.memories.Memories;
			for (int i = 0; i < memories.Count; i++)
			{
				if (memories[i].def == def && memories[i] is Thought_MemorySocial_WithStat thought)
				{
					if (thought.OtherPawn() == otherPawn)
					{
						thought.opinionOffset += opinionOffset;
						return true;
					}
				}
			}
			return false;
		}

	}

}
