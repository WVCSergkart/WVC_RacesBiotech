using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Photosynthesis : Gene_FoodEfficiency
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		private float? cachedNutritionPerTick;

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(541))
			{
				return;
			}
			if (pawn.Map == null)
			{
				InCaravan();
				return;
			}
			if (!pawn.Position.InSunlight(pawn.Map))
			{
				return;
			}
			if (pawn.apparel.AnyClothing)
			{
				return;
			}
			ReplenishHunger();
		}

		private void InCaravan()
		{
			Caravan caravan = pawn.GetCaravan();
			if (caravan?.NightResting != false)
			{
				return;
			}
			ReplenishHunger();
		}

		public void ReplenishHunger()
		{
			if (!cachedNutritionPerTick.HasValue)
			{
				cachedNutritionPerTick = Giver.passivelyReplenishedNutrition + (pawn.needs?.food != null ? pawn.needs.food.FoodFallPerTick : 0f);
			}
			UndeadUtility.OffsetNeedFood(pawn, cachedNutritionPerTick.Value);
		}

	}

	public class Gene_DeadlyUVSensitivity : Gene
	{

		public GeneExtension_Opinion Opinion => def?.GetModExtension<GeneExtension_Opinion>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(1523))
			{
				return;
			}
			if (pawn.Map == null)
			{
				return;
			}
			if (!pawn.Position.InSunlight(pawn.Map))
			{
				return;
			}
			if (!ThoughtWorker_Precept_GroinChestHairOrFaceUncovered.HasUncoveredGroinChestHairOrFace(pawn))
			{
				return;
			}
			InSunlight();
		}

		private void InSunlight()
		{
			SoundDefOf.Interact_BeatFire.PlayOneShot(pawn);
			// BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new(pawn, RulePackDefOf.DamageEvent_Fire);
			// Find.BattleLog.Add(battleLogEntry_DamageTaken);
			// pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, new FloatRange(2.5f, 5.5f).RandomInRange)).AssociateWithLog(battleLogEntry_DamageTaken);
			pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, new FloatRange(2.5f, 5.5f).RandomInRange, hitPart: pawn?.health?.hediffSet?.GetRandomNotMissingPart(DamageDefOf.Burn, depth: BodyPartDepth.Outside)));
			if (Opinion?.MeAboutThoughtDef != null)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Opinion.MeAboutThoughtDef);
			}
		}

	}

}
