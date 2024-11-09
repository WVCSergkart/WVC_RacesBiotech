using RimWorld;
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
				ThoughtUtility.TryInteractRandomly(pawn, true, true, false, out _);
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

		private Gene_Hemogen cachedHemogenGene = null;

		public Gene_Hemogen Gene_Hemogen
		{
			get
			{
				if (cachedHemogenGene == null || !cachedHemogenGene.Active)
				{
					cachedHemogenGene = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
				}
				return cachedHemogenGene;
			}
		}

		public override void PostAdd()
		{
			base.PostAdd();
		}

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(8000))
			{
				return;
			}
			if (!Active)
			{
				return;
			}
			GeneFeaturesUtility.TryPsyFeedRandomly(pawn, Gene_Hemogen);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryPsyFeed",
					action = delegate
					{
						GeneFeaturesUtility.TryPsyFeedRandomly(pawn, Gene_Hemogen);
					}
				};
			}
		}

	}

}
