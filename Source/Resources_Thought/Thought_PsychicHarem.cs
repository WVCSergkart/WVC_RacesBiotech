using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Thought_PsychicHarem : Thought_Memory
	{

		public int badMoodTicks = -1;

		public override bool ShouldDiscard => !Gene_PsychicHarem.Harem.Contains(pawn);

		public override float MoodOffset()
		{
			if (badMoodTicks > Find.TickManager.TicksGame)
			{
				return -10;
			}
			int count = Gene_PsychicHarem.Harem.Count;
			return count > 1 ? 10 + count : 0;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref badMoodTicks, "lastBadMoodTick", -1);
		}

	}

	public class Thought_Social_PsychicHarem : Thought_MemorySocial_NoFade
	{

		public override bool ShouldDiscard => !Gene_PsychicHarem.Harem.Contains(pawn) || !Gene_PsychicHarem.Harem.Contains(otherPawn);

		public override float OpinionOffset()
		{
			return 999f;
		}

	}

}
