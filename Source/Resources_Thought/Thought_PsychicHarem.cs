using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Thought_PsychicHarem : Thought_Memory
	{

		public int badMoodTicks = -1;

		public override bool ShouldDiscard => !Gene_PsychicHarem.InHarem(pawn);

		public override float MoodOffset()
		{
			if (badMoodTicks > Find.TickManager.TicksGame)
			{
				return -Mood;
			}
			return Mood;
		}

		public static float? cachedMoodOffset;
		public static float Mood
		{
			get
			{
				if (cachedMoodOffset == null)
				{
					float totalMood = 0f;
					int countedPawns = 0;
					foreach (Pawn pawn in Gene_PsychicHarem.Harem)
					{
						if (pawn.needs?.mood == null)
						{
							continue;
						}
						countedPawns++;
						totalMood += pawn.needs.mood.CurLevel;
					}
					totalMood = (totalMood * 5) / countedPawns;
					totalMood += 5f;
					cachedMoodOffset = totalMood;
				}
				return cachedMoodOffset.Value;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref badMoodTicks, "lastBadMoodTick", -1);
		}

	}

	public class Thought_Social_PsychicHarem : Thought_MemorySocial_NoFade
	{

		public override bool ShouldDiscard => !Gene_PsychicHarem.InHarem(pawn) || !Gene_PsychicHarem.InHarem(otherPawn);

		public override float OpinionOffset()
		{
			return 999f;
		}

	}

}
