using RimWorld;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{
	internal static class EffectsUtility
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

		public static void MeatSplatter(Pawn pawn, FleshbeastUtility.MeatExplosionSize size, int bloodDropSize = 3)
		{
			for (int i = 0; i < bloodDropSize; i++)
			{
				pawn.health.DropBloodFilth();
			}
			FleshbeastUtility.MeatSplatter(3, pawn.PositionHeld, pawn.MapHeld, size);
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

	}

}