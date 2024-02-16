using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Telepathy : Gene
	{

		private int hashIntervalTick = 7200;

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(hashIntervalTick))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			if (!WVC_Biotech.settings.enableHarmonyTelepathyGene)
			{
				ThoughtUtility.TryInteractRandomly(pawn);
			}
			ResetInterval();
		}

		private void ResetInterval()
		{
			IntRange range = new(6600, 22000);
			hashIntervalTick = range.RandomInRange;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 0);
		}

	}

	public class Gene_Psyfeeder : Gene
	{

		// private int hashIntervalTick = 7200;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// ResetInterval();
		// }

		// public override void Tick()
		// {
			// base.Tick();
			// if (!pawn.IsHashIntervalTick(hashIntervalTick))
			// {
				// return;
			// }
			// if (!Active)
			// {
				// return;
			// }
			// if (!WVC_Biotech.settings.enableHarmonyTelepathyGene)
			// {
				// ThoughtUtility.TryInteractRandomly(pawn);
			// }
			// ResetInterval();
		// }

		// private void ResetInterval()
		// {
			// IntRange range = new(6600, 22000);
			// hashIntervalTick = range.RandomInRange;
		// }

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref hashIntervalTick, "hashIntervalTick", 0);
		// }

	}

}
