// Verse.Gene_Healing
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class Gene_BodyPartsRestoration : Gene
	{
		private int ticksToHealBodyPart;

		// 15-30 days
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
					TryHealRandomPermanentWound(pawn);
				}
				ResetInterval();
			}
		}

		public static void TryHealRandomPermanentWound(Pawn pawn)
		{
			TaggedString taggedString = FixWorstHealthCondition(pawn);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			}
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

		public static TaggedString FixWorstHealthCondition(Pawn pawn)
		{
			BodyPartRecord bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);
			if (bodyPartRecord2 != null)
			{
				return Cure(bodyPartRecord2, pawn);
			}
			return null;
		}

		public static TaggedString Cure(BodyPartRecord part, Pawn pawn)
		{
			pawn.health.RestorePart(part);
			return "HealingRestoreBodyPart".Translate(pawn, part.Label);
		}

		public static BodyPartRecord FindBiggestMissingBodyPart(Pawn pawn, float minCoverage = 0f)
		{
			BodyPartRecord bodyPartRecord = null;
			foreach (Hediff_MissingPart missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
			{
				if (!(missingPartsCommonAncestor.Part.coverageAbsWithChildren < minCoverage) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(missingPartsCommonAncestor.Part) && (bodyPartRecord == null || missingPartsCommonAncestor.Part.coverageAbsWithChildren > bodyPartRecord.coverageAbsWithChildren))
				{
					bodyPartRecord = missingPartsCommonAncestor.Part;
				}
			}
			return bodyPartRecord;
		}
	}

	public class Gene_DustBodyPartsRestoration : Gene_DustDrain
	{
		private int ticksToHealBodyPart;

		// 5-15 days
		private static readonly IntRange HealingIntervalTicksRange = new(300000, 900000);

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
					Gene_BodyPartsRestoration.TryHealRandomPermanentWound(pawn);
				}
				ResetInterval();
			}
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
							Gene_BodyPartsRestoration.TryHealRandomPermanentWound(pawn);
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