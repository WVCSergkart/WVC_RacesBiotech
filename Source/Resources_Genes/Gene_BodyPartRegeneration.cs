// Verse.Gene_Healing
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_RestoreMissingBodyParts : Gene
	{
		private int ticksToHealBodyPart;

		private static readonly IntRange HealingIntervalTicksRange = new(900000, 1800000);

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksToHealBodyPart--;
			if (ticksToHealBodyPart <= 0)
			{
				if (Active)
				{
					Gene_BodyPartsRestoration.TryHealRandomPermanentWound(pawn, this, true);
				}
				ResetInterval();
			}
		}

		// public static void TryHealRandomPermanentWound(Pawn pawn)
		// {
			// TaggedString taggedString = HealthUtility.FixWorstHealthCondition(pawn);
			// if (PawnUtility.ShouldSendNotificationAbout(pawn))
			// {
				// Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			// }
		// }

		private void ResetInterval()
		{
			ticksToHealBodyPart = HealingIntervalTicksRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Restore lost limb",
					action = delegate
					{
						if (Active)
						{
							Gene_BodyPartsRestoration.TryHealRandomPermanentWound(pawn, this, true);
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToHealBodyPart, "ticksToHealBodyPart", 0);
		}
	}
}
