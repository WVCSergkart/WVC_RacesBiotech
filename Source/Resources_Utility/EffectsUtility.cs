using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	public static class EffectsUtility
	{

		public static void DoShapeshiftEffects_OnPawn(Pawn pawn)
		{
			if (ModsConfig.AnomalyActive)
			{
				EffectsUtility.MeatSplatter(pawn, FleshbeastUtility.MeatExplosionSize.Small);
			}
			MainDefOf.WVC_ShapeshiftBurst.SpawnAttached(pawn, pawn.Map).Trigger(pawn, null);
		}

		public static void DoSkipEffects(IntVec3 spawnCell, Map map)
		{
			map.effecterMaintainer.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(spawnCell, map), spawnCell, 60);
			SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(spawnCell, map));
		}

		public static void FleckAndLetter(Pawn caster, Pawn victim)
		{
			FleckMaker.AttachedOverlay(victim, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
			if (PawnUtility.ShouldSendNotificationAbout(caster) || PawnUtility.ShouldSendNotificationAbout(victim))
			{
				int max = HediffDefOf.XenogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				int max2 = HediffDefOf.XenogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
				Find.LetterStack.ReceiveLetter("LetterLabelGenesImplanted".Translate(), "LetterTextGenesImplanted".Translate(caster.Named("CASTER"), victim.Named("TARGET"), max.ToStringTicksToPeriod().Named("COMADURATION"), max2.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(caster, victim));
			}
		}

		public static void MeatSplatter(Pawn pawn, FleshbeastUtility.MeatExplosionSize size, int bloodDropSize = 3)
		{
			for (int i = 0; i < bloodDropSize; i++)
			{
				pawn.health.DropBloodFilth();
			}
			FleshbeastUtility.MeatSplatter(3, pawn.PositionHeld, pawn.MapHeld, size);
		}

		public static void PulseEffect(Thing parent)
		{
			//Find.TickManager.slower.SignalForceNormalSpeedShort();
			//SoundDefOf.PsychicPulseGlobal.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
			FleckMaker.AttachedOverlay(parent, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), Vector3.zero);
		}

		public static bool SkipBodyFromMap(Pawn pawn)
		{
			if (MiscUtility.TryGetAndDestroyCorpse_WithPosition(pawn, out Map mapHeld, out IntVec3 positionHeld))
			{
				if (mapHeld == null)
				{
					return false;
				}
				EffectsUtility.DoSkipEffects(positionHeld, mapHeld);
			}
			return true;
		}

		// ============================= GENE THRALL =============================

		//public static bool CanCellsFeedNowWith(Pawn biter, Pawn victim)
		//{
		//	if (MiscUtility.BasicTargetValidation(biter, victim))
		//	{
		//		Gene_Resurgent cells = victim.genes?.GetFirstGeneOfType<Gene_Resurgent>();
		//		if (cells == null)
		//		{
		//			return false;
		//		}
		//		if (cells.ValuePercent < 0.20f)
		//		{
		//			return false;
		//		}
		//		return true;
		//	}
		//	return false;
		//}

		//public static void DoCellsBite(Pawn biter, Pawn victim, float daysGain, float cellsConsumeFactor)
		//{
		//	float cells = daysGain * cellsConsumeFactor;
		//	int ticks = (int)(daysGain * (victim.BodySize * 60000));
		//	GeneResourceUtility.OffsetInstabilityTick(biter, ticks);
		//	GeneResourceUtility.OffsetResurgentCells(victim, 0f - (cells * 0.01f));
		//}

		public static bool TrySpawnBloodFilth(Pawn victim, IntRange bloodFilthToSpawnRange)
		{
			if (victim?.Map == null)
			{
				return false;
			}
			int randomInRange = bloodFilthToSpawnRange.RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				victim?.health?.DropBloodFilth();
			}
			return true;
		}

	}

}