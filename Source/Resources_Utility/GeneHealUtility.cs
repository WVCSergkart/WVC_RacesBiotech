using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class HealingUtility
	{

		// General
		public static void TryHealRandomPermanentWound(Pawn pawn, Gene gene, bool healWound = false, bool restoreWithFullHP = false)
		{
			TaggedString taggedString = FixWorstHealthCondition(pawn, gene, healWound, restoreWithFullHP);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message(taggedString, pawn, MessageTypeDefOf.PositiveEvent);
			}
		}

		// Base
		public static TaggedString FixWorstHealthCondition(Pawn pawn, Gene gene, bool healWound = false, bool restoreWithFullHP = false)
		{
			BodyPartRecord bodyPartRecord2 = FindBiggestMissingBodyPart(pawn);
			if (bodyPartRecord2 != null)
			{
				return Cure(bodyPartRecord2, pawn, restoreWithFullHP);
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
		public static TaggedString Cure(BodyPartRecord part, Pawn pawn, bool restoreWithFullHP = false)
		{
			// pawn.health.RestorePart(part);
			RestorePartWith1HP(pawn, part, !WVC_Biotech.settings.restoreBodyPartsWithFullHP ? restoreWithFullHP : true);
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
		public static bool TryHealRandomPermanentWound(Pawn pawn, string cause)
		{
			if (pawn.health.hediffSet.hediffs.Where((Hediff hd) => (hd.IsPermanent() || hd.def.chronic) && hd.def != HediffDefOf.Scarification).TryRandomElement(out var result))
			{
				HealthUtility.Cure(result);
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("MessagePermanentWoundHealed".Translate(cause, pawn.LabelShort, result.Label, pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
				}
				return true;
			}
			return false;
		}

		// RestorePart
		public static void RestorePartWith1HP(Pawn pawn, BodyPartRecord part, bool restoreWithFullHP = false)
		{
			if (part != null)
			{
				pawn.health.RestorePart(part);
				if (restoreWithFullHP)
				{
					return;
				}
				if (pawn.health.WouldDieAfterAddingHediff(WVC_GenesDefOf.WVC_RegrowingPart, part, 0.01f))
				{
					return;
				}
				// WVC_GenesDefOf.WVC_RegrowingPart
				// DamageDefOf.Cut
				TakeDamage(pawn, part);
				for (int i = 0; i < part.parts.Count; i++)
				{
					SetPartWith1HP(pawn, part.parts[i]);
				}
			}
		}

		// Regrow
		public static void SetPartWith1HP(Pawn pawn, BodyPartRecord part)
		{
			if (part != null)
			{
				TakeDamage(pawn, part);
				for (int i = 0; i < part.parts.Count; i++)
				{
					SetPartWith1HP(pawn, part.parts[i]);
				}
			}
		}

		public static void TakeDamage(Pawn pawn, BodyPartRecord part, DamageDef damageDef = null)
		{
			if (part.coverageAbs <= 0f)
			{
				return;
			}
			int num = (int)pawn.health.hediffSet.GetPartHealth(part) - 1;
			if (damageDef != null)
			{
				// Too buggy
				DamageInfo damageInfo = new(damageDef, num, 999f, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null, false, false);
				damageInfo.SetAllowDamagePropagation(false);
				pawn.TakeDamage(damageInfo);
			}
			else
			{
				// Less buggy
				Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(WVC_GenesDefOf.WVC_RegrowingPart, pawn);
				hediff_Injury.Part = part;
				hediff_Injury.sourceDef = null;
				hediff_Injury.sourceBodyPartGroup = null;
				hediff_Injury.sourceHediffDef = null;
				hediff_Injury.Severity = num;
				pawn.health.AddHediff(hediff_Injury);
			}
		}

		// Special
		// public static void RegrowAllBodyParts(Pawn pawn)
		// {
			// List<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts().ToList();
			// List<BodyPartRecord> missingBodyParts = new();
			// foreach (Hediff_MissingPart missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
			// {
				// BodyPartRecord bodyPart = missingPartsCommonAncestor.Part;
				// if (pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(bodyPart))
				// {
					// continue;
				// }
				// missingBodyParts.Add(bodyPart);
			// }
			// foreach (BodyPartRecord part in missingBodyParts)
			// {
				// RestorePartWith1HP(pawn, part);
			// }
			// foreach (BodyPartRecord part in bodyParts)
			// {
				// SetPartWith1HP(pawn, part);
			// }
		// }

	}
}
