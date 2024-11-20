using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class HealingUtility
	{

		// Regeneration
		public static void Regeneration(Pawn pawn, float regeneration = -1, bool ignoreScarification = true, int tick = 10)
		{
			List<Hediff_Injury> tmpHediffInjuries = new();
			List<Hediff_MissingPart> tmpHediffMissing = new();
			regeneration *= 0.000166666665f;
			if (tick > 0f)
			{
				regeneration *= (tick / 10);
			}
			// Log.Error(regeneration.ToString());
			// Old 0.0001243781
			// New 0.5583333
			if (regeneration > 0f)
			{
				pawn.health.hediffSet.GetHediffs(ref tmpHediffInjuries, (Hediff_Injury h) => h.def != HediffDefOf.Scarification || !ignoreScarification);
				foreach (Hediff_Injury tmpHediffInjury in tmpHediffInjuries)
				{
					float num5 = Mathf.Min(regeneration, tmpHediffInjury.Severity);
					regeneration -= num5;
					tmpHediffInjury.Heal(num5);
					pawn.health.hediffSet.Notify_Regenerated(num5);
					if (regeneration <= 0f)
					{
						break;
					}
				}
				if (regeneration > 0f)
				{
					pawn.health.hediffSet.GetHediffs(ref tmpHediffMissing, (Hediff_MissingPart h) => h.Part.parent != null && !tmpHediffInjuries.Any((Hediff_Injury x) => x.Part == h.Part.parent) && pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_MissingPart>(h.Part.parent) == null && pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_AddedPart>(h.Part.parent) == null);
					using List<Hediff_MissingPart>.Enumerator enumerator3 = tmpHediffMissing.GetEnumerator();
					if (enumerator3.MoveNext())
					{
						Hediff_MissingPart current4 = enumerator3.Current;
						BodyPartRecord part = current4.Part;
						pawn.health.RemoveHediff(current4);
						Hediff hediff2 = pawn.health.AddHediff(HediffDefOf.Misc, part);
						float partHealth = pawn.health.hediffSet.GetPartHealth(part);
						hediff2.Severity = Mathf.Max(partHealth - 1f, partHealth * 0.9f);
						pawn.health.hediffSet.Notify_Regenerated(partHealth - hediff2.Severity);
					}
				}
			}
		}

		public static void Immunization(Pawn pawn, float immunization = -1, int tick = 200)
		{
			//List<HediffWithComps> tmpHediffInjuries = new();
			immunization *= 0.00333333341f;
			if (tick > 0f)
			{
				immunization *= (tick / 200);
			}
			// Log.Error(regeneration.ToString());
			// Old 0.0001243781
			// New 0.5583333
			if (immunization > 0f)
			{
				//pawn.health.hediffSet.GetHediffs(ref tmpHediffInjuries, (HediffWithComps h) => h.TryGetComp<HediffComp_Immunizable>() != null);
				//foreach (HediffWithComps tmpHediffInjury in tmpHediffInjuries)
				//{
				//	ImmunityRecord immunityRecord = pawn.health?.immunity?.GetImmunityRecord(tmpHediffInjury.def);
				//	if (immunityRecord == null)
				//	{
				//		continue;
				//	}
				//	float num5 = Mathf.Min(immunization, immunityRecord.immunity);
				//	immunization -= num5;
				//	immunityRecord.immunity += num5;
				//	if (immunization <= 0f)
				//	{
				//		break;
				//	}
				//}
				List<ImmunityRecord> immunityListForReading = pawn.health?.immunity?.ImmunityListForReading;
				foreach (ImmunityRecord immunityRecord in immunityListForReading)
				{
					if (immunityRecord.immunity >= 1f)
					{
						continue;
					}
					float num5 = Mathf.Min(immunization, immunityRecord.immunity);
                    immunization -= num5;
                    immunityRecord.immunity += num5;
                    if (immunization <= 0f)
                    {
                        break;
                    }
                }
            }
		}

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
			RestorePartWith1HP(pawn, part, WVC_Biotech.settings.restoreBodyPartsWithFullHP || restoreWithFullHP);
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
