using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class HealingUtility
	{

		public static List<Pawn> regenEyes = new();
		public static List<Pawn> nonRegenEyes = new();

		public static void UpdRegenCollection()
		{
			HealingUtility.regenEyes = new();
			HealingUtility.nonRegenEyes = new();
		}

		public static bool RegenEyes(Pawn pawn)
		{
			if (nonRegenEyes.Contains(pawn))
			{
				return false;
			}
			if (regenEyes.Contains(pawn))
			{
				return true;
			}
			if (ShouldRegenerateEyes(pawn))
			{
				regenEyes.Add(pawn);
				return true;
			}
			else
			{
				nonRegenEyes.Add(pawn);
			}
			return false;
		}

		private static bool ShouldRegenerateEyes(Pawn pawn)
		{
			//return !ModLister.IdeologyInstalled || !pawn.ideo.Ideo.IdeoApprovesOfBlindness();
			return pawn.ideo?.Ideo?.IdeoApprovesOfBlindness() != true;
		}

		// Regeneration
		public static bool Regeneration(Pawn pawn, float regeneration, int tick)
		{
			List<Hediff_Injury> tmpHediffInjuries = new();
			List<Hediff_MissingPart> tmpHediffMissing = new();
			regeneration /= 60000;
			if (tick > 0f)
			{
				regeneration *= tick;
			}
			bool woundRegen = false;
			if (regeneration > 0f)
			{
				pawn.health.hediffSet.GetHediffs(ref tmpHediffInjuries, (Hediff_Injury h) => h.def != HediffDefOf.Scarification || !WVC_Biotech.settings.totalHealingIgnoreScarification);
				foreach (Hediff_Injury tmpHediffInjury in tmpHediffInjuries)
				{
					float num5 = Mathf.Min(regeneration, tmpHediffInjury.Severity);
					regeneration -= num5;
					tmpHediffInjury.Heal(num5);
					pawn.health.hediffSet.Notify_Regenerated(num5);
					woundRegen = true;
					if (regeneration <= 0f)
					{
						break;
					}
				}
				if (regeneration > 0f)
                {
                    pawn.health.hediffSet.GetHediffs(ref tmpHediffMissing, (Hediff_MissingPart missingPart) => missingPart.Part.parent != null && !tmpHediffInjuries.Any((Hediff_Injury injury) => injury.Part == missingPart.Part.parent) && pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_MissingPart>(missingPart.Part.parent) == null && pawn.health.hediffSet.GetFirstHediffMatchingPart<Hediff_AddedPart>(missingPart.Part.parent) == null && RegenIfEyes(pawn, missingPart));
                    using List<Hediff_MissingPart>.Enumerator enumerator3 = tmpHediffMissing.GetEnumerator();
                    if (enumerator3.MoveNext())
                    {
                        HealingUtility.Regenerate(pawn, enumerator3.Current);
                        woundRegen = true;
                    }
                }
            }
			return woundRegen;
		}

        private static bool RegenIfEyes(Pawn pawn, Hediff_MissingPart missingPart)
        {
            return !missingPart.Part.def.tags.Contains(BodyPartTagDefOf.SightSource) || RegenEyes(pawn);
        }

        public static void Regenerate(Pawn pawn, Hediff hediff)
        {
            BodyPartRecord part = hediff.Part;
            pawn.health.RemoveHediff(hediff);
            Regenerate(pawn, part);
        }

        public static void Regenerate(Pawn pawn, BodyPartRecord part, int initialAgeTicks = 0)
        {
            Hediff hediff2 = pawn.health.AddHediff(HediffDefOf.Misc, part);
            float partHealth = pawn.health.hediffSet.GetPartHealth(part);
            hediff2.Severity = Mathf.Max(partHealth - 1f, partHealth * 0.9f);
			hediff2.ageTicks = initialAgeTicks;
			pawn.health.hediffSet.Notify_Regenerated(partHealth - hediff2.Severity);
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
			foreach (Hediff_MissingPart missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors().Where((part) => RegenIfEyes(pawn, part)))
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
				if (restoreWithFullHP || pawn.health.summaryHealth.SummaryHealthPercent < 0.4f)
				{
					return;
				}
				if (pawn.health.WouldDieAfterAddingHediff(HediffDefOf.Misc, part, 0.99f))
				{
					return;
				}
				Regenerate(pawn, part);
                //TakeDamage(pawn, part);
                for (int i = 0; i < part.parts.Count; i++)
				{
					if (pawn.health.summaryHealth.SummaryHealthPercent < 0.4f)
					{
						break;
					}
					SetPartWith1HP(pawn, part.parts[i]);
                }
            }
		}

        // Regrow
        public static void SetPartWith1HP(Pawn pawn, BodyPartRecord part)
        {
            if (part != null)
			{
				Regenerate(pawn, part, 3600000);
				//TakeDamage(pawn, part);
                for (int i = 0; i < part.parts.Count; i++)
                {
                    SetPartWith1HP(pawn, part.parts[i]);
                }
            }
        }

        //public static void TakeDamage(Pawn pawn, BodyPartRecord part, DamageDef damageDef = null)
        //{
        //	if (part.coverageAbs <= 0f)
        //	{
        //		return;
        //	}
        //	int num = (int)pawn.health.hediffSet.GetPartHealth(part) - 1;
        //	if (damageDef != null)
        //	{
        //		DamageInfo damageInfo = new(damageDef, num, 999f, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null, false, false);
        //		damageInfo.SetAllowDamagePropagation(false);
        //		pawn.TakeDamage(damageInfo);
        //	}
        //	else
        //	{
        //		Hediff_Injury hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(WVC_GenesDefOf.WVC_RegrowingPart, pawn);
        //		hediff_Injury.Part = part;
        //		hediff_Injury.sourceDef = null;
        //		hediff_Injury.sourceBodyPartGroup = null;
        //		hediff_Injury.sourceHediffDef = null;
        //		hediff_Injury.Severity = num;
        //		pawn.health.AddHediff(hediff_Injury);
        //	}
        //}

        public static void TakeDamage(Pawn pawn, BodyPartRecord part, DamageDef damageDef, int damageNum)
        {
            if (part.coverageAbs <= 0f)
            {
                return;
            }
            DamageInfo damageInfo = new(damageDef, damageNum, 999f, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null, false, false);
            damageInfo.SetAllowDamagePropagation(false);
            pawn.TakeDamage(damageInfo);
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
