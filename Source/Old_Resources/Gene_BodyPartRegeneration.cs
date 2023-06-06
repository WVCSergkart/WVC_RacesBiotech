// Verse.Gene_Healing
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC
{
	public class Gene_RestoreMissingBodyParts : Gene
	{
		private int ticksToHealBodyPart;

		private static readonly IntRange HealingIntervalTicksRange = new IntRange(900000, 1800000);

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
					TryHealRandomPermanentWound(pawn);
				}
				ResetInterval();
			}
		}

		public static void TryHealRandomPermanentWound(Pawn pawn)
		{
			// if (pawn.health.hediffSet.hediffs.Where((Hediff hd) => hd.IsPermanent() || hd.def.chronic || hd.def.defName.Contains("MissingBodyPart")).TryRandomElement(out var result))
			// if (pawn.health.hediffSet.hediffs.Where((Hediff hd) => hd.def.defName.Contains("MissingBodyPart")).TryRandomElement(out var result))
			// {
				// HealthUtility.Cure(result);
			// }
			TaggedString taggedString = HealthUtility.FixWorstHealthCondition(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			}
			// HealthUtility.FixWorstHealthCondition(pawn);
			// if (PawnUtility.ShouldSendNotificationAbout(pawn))
			// {
				// Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, result.Label, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
			// }
		}

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
							TryHealRandomPermanentWound(pawn);
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
