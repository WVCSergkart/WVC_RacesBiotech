using RimWorld;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public static class HealingUtility
	{

		// General
		public static void TryHealRandomPermanentWound(Pawn pawn, Gene gene, bool healWound = false)
		{
			TaggedString taggedString = FixWorstHealthCondition(pawn, gene, healWound);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			}
		}

		// Base
		public static TaggedString FixWorstHealthCondition(Pawn pawn, Gene gene, bool healWound = false)
		{
			BodyPartRecord bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);
			if (bodyPartRecord2 != null)
			{
				return Cure(bodyPartRecord2, pawn);
			}
			else if (healWound)
			{
				if (WVC_Biotech.settings.totalHealingIgnoreScarification)
				{
					TryHealRandomPermanentWound(pawn, gene.LabelCap);
				}
				else
				{
					HediffComp_HealPermanentWounds.TryHealRandomPermanentWound(pawn, gene.LabelCap);
				}
			}
			return null;
		}

		// Body parts
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

		// Scars
		public static void TryHealRandomPermanentWound(Pawn pawn, string cause)
		{
			if (pawn.health.hediffSet.hediffs.Where((Hediff hd) => (hd.IsPermanent() || hd.def.chronic) && hd.def != HediffDefOf.Scarification).TryRandomElement(out var result))
			{
				HealthUtility.Cure(result);
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, result.Label, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}


	}
}
