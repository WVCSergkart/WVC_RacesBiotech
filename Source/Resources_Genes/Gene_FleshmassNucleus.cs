using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_FleshmassNucleus : XaG_Gene, IGeneInspectInfo
	{

		//public GeneExtension_Giver Props => def?.GetModExtension<GeneExtension_Giver>();

		private static StatDef cachedNucleusStatDef;
		public StatDef NucleusStatDef
		{
			get
			{
				if (cachedNucleusStatDef == null)
				{
					cachedNucleusStatDef = def?.GetModExtension<GeneExtension_Giver>()?.statDef;
				}
				return cachedNucleusStatDef;
			}
		}
		public float CooldownFactor => pawn.GetStatValue(NucleusStatDef);
		private void ApplyStatFactor()
		{
			if (NucleusStatDef != null)
			{
				nextTick = Mathf.Clamp((int)(nextTick * CooldownFactor), 60000, 3600000);
			}
		}

		private int nextTick = 60000;

		public override string LabelCap => base.LabelCap + " (" + "WVC_HealingPerDay".Translate(Regeneration) + ")";

		public override void PostAdd()
		{
			base.PostAdd();
			if (MiscUtility.GameNotStarted())
			{
				if (pawn.ageTracker.AgeBiologicalYearsFloat > 16)
				{
					SimpleCurve ageCurve = new()
					{
						new CurvePoint(18f, 0f),
						new CurvePoint(27f, 1f),
						new CurvePoint(86f, 2f),
						new CurvePoint(126f, 3f),
						new CurvePoint(654f, 4f),
						new CurvePoint(1000f, 5f)
					};
					float mutationsCount = (float)Math.Round(ageCurve.Evaluate(pawn.ageTracker.AgeChronologicalYearsFloat), 0);
					HediffUtility.SetMutations(pawn, mutationsCount);
				}
				nextTick = new IntRange(100000, 300000).RandomInRange;
			}
			else if (pawn.Faction != Faction.OfPlayer && !pawn.SpawnedOrAnyParentSpawned)
			{
				nextTick = new IntRange(150000, 300000).RandomInRange;
			}
			else
			{
				nextTick = 300000;
			}
			ApplyStatFactor();
		}


		//private int? cachedRegen;
		//public int RegenRate
		//{
		//	get
		//	{
		//		if (!cachedRegen.HasValue)
		//		{
		//			int regen = 0;
		//			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
		//			{
		//				if (hediff is HediffAddedPart_FleshmassNucleus fleshHediff)
		//				{
		//					regen += fleshHediff.CurrentLevel;
		//				}
		//			}
		//			cachedRegen = regen;
		//		}
		//		return cachedRegen.Value;
		//	}
		//}

		private float? cachedRegen;
		public float Regeneration
		{
			get
			{
				if (!cachedRegen.HasValue)
				{
					float regen = 0f;
					foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
					{
						if (hediff is HediffAddedPart_FleshmassNucleus fleshmassHediff)
						{
							regen += (fleshmassHediff.CurrentLevel * 2f);
						}
					}
					cachedRegen = regen;
				}
				return cachedRegen.Value;
			}
		}

		public override void TickInterval(int delta)
		{
			if (GeneResourceUtility.CanTick(ref nextTick, 300000, delta))
			{
				TryGiveMutation();
				ApplyStatFactor();
			}
			if (pawn.IsHashIntervalTick(2571, delta))
			{
				HealingUtility.Regeneration(pawn, Regeneration, 2571);
			}
		}

		public void TryGiveMutation()
		{
			//float shapeshifterResourceOffset = 0;
			if (Rand.Chance(0.9f) && HediffUtility.TryGetBestMutation(pawn, out HediffDef mutation) && HediffUtility.TryGiveFleshmassMutation(pawn, mutation))
			{
				cachedRegen = null;
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("WVC_XaG_HasReceivedA".Translate(pawn.NameShortColored, mutation.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				//shapeshifterResourceOffset = 12;
			}
			else if (TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
			{
				hediffWithComps_FleshmassHeart.LevelUp();
				cachedRegen = null;
				if (PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("WVC_XaG_MutationProgressing".Translate(hediffWithComps_FleshmassHeart.def.LabelCap), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				//shapeshifterResourceOffset = 9;
			}
			else
			{
				//hideInspectInfo = true;
				TrySpawnMeat(pawn);
				//shapeshifterResourceOffset = 19;
			}
			// Direct offset without cache
			//Gene_Shapeshifter.OffsetResource(pawn, shapeshifterResourceOffset);
		}

		private bool TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart)
		{
			hediffWithComps_FleshmassHeart = null;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where((Hediff hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff && massHediff.CurrentLevel < Fleshmass_MaxMutationsLevel).OrderBy((hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff ? massHediff.CurrentLevel : 0f).ToList();
			foreach (Hediff hediff in hediffs)
			{
				if (hediff is HediffAddedPart_FleshmassNucleus massHediff)
				{
					hediffWithComps_FleshmassHeart = massHediff;
					break;
				}
			}
			return hediffWithComps_FleshmassHeart != null;
		}

		public static float Fleshmass_MaxMutationsLevel => WVC_Biotech.settings.fleshmass_MaxMutationsLevel;

		public static void TrySpawnMeat(Pawn pawn)
		{
			int num = Mathf.CeilToInt(20 * pawn.BodySize * pawn.GetStatValue(StatDefOf.MaxNutrition));
			int randomInRange = new IntRange(3, 6).RandomInRange;
			for (int i = 0; i < randomInRange; i++)
			{
				if (CellFinder.TryRandomClosewalkCellNear(pawn.PositionHeld, pawn.MapHeld, 3, out var result))
				{
					Thing thing = ThingMaker.MakeThing(ThingDefOf.Meat_Twisted);
					thing.stackCount = Rand.RangeInclusive(4, num);
					GenDrop.TryDropSpawn(thing, result, pawn.MapHeld, ThingPlaceMode.Near, out var _);
				}
			}
			HediffUtility.MutationMeatSplatter(pawn);
			//EffecterDefOf.MeatExplosion.Spawn(pawn.PositionHeld, pawn.MapHeld).Cleanup();
		}

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (!MiscUtility.TryGetAndDestroyCorpse_WithPosition(pawn, out Map mapHeld, out IntVec3 positionHeld))
			{
				return;
			}
			if (!ModsConfig.AnomalyActive || mapHeld == null)
			{
				return;
			}
			DropPostDeathSpawn(mapHeld, positionHeld);
		}

		public virtual void DropPostDeathSpawn(Map mapHeld, IntVec3 positionHeld)
		{
			if (GenDrop.TryDropSpawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.FleshmassNucleus, Faction.OfEntities)), positionHeld, mapHeld, ThingPlaceMode.Near, out var resultingThing))
			{
				CompActivity activity = resultingThing.TryGetComp<CompActivity>();
				if (activity != null)
				{
					activity.AdjustActivity(1f);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: FleshmassMutation",
					action = delegate
					{
						TryGiveMutation();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: FleshmassLevelUp",
					action = delegate
					{
						if (TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
						{
							hediffWithComps_FleshmassHeart.LevelUp();
						}
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
			//Scribe_Values.Look(ref hideInspectInfo, "hideInspectInfo", false);
		}

		//public bool hideInspectInfo = false;

		public string GetInspectInfo
		{
			get
			{
				return "WVC_XaG_FleshmassNucleusOvergrow".Translate().Resolve() + ": " + nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			StatDef stat = NucleusStatDef;
			if (stat != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat.LabelCap, pawn.GetStatValue(stat).ToStringByStyle(stat.toStringStyle), stat.description, stat.displayPriorityInCategory);
			}
		}

	}

	public class Gene_FleshmassHeart : Gene_FleshmassNucleus
	{

		public override void DropPostDeathSpawn(Map mapHeld, IntVec3 positionHeld)
		{
			PawnKindDef fingerspike = PawnKindDefOf.Fingerspike;
			if (pawn.ageTracker.Adult)
			{
				fingerspike = PawnKindDefOf.Toughspike;
			}
			if (GenDrop.TryDropSpawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(fingerspike, Faction.OfEntities)), positionHeld, mapHeld, ThingPlaceMode.Near, out _))
			{
				FleshbeastUtility.MeatSplatter(3, positionHeld, mapHeld, FleshbeastUtility.MeatExplosionSize.Large);
			}
		}

	}

}
