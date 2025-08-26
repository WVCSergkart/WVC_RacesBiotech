using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class Gene_FleshmassNucleus : Gene, IGeneInspectInfo
	{

		//public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

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
			}
            if (pawn.IsHashIntervalTick(2571, delta))
            {
				HealingUtility.Regeneration(pawn, Regeneration, 2571);
            }
        }

		public void TryGiveMutation()
		{
			if (HediffUtility.TryGetBestMutation(pawn, out HediffDef mutation) && HediffUtility.TryGiveFleshmassMutation(pawn, mutation))
			{
				cachedRegen = null;
				if (!PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					return;
				}
				Messages.Message("WVC_XaG_HasReceivedA".Translate(pawn.NameShortColored, mutation.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			else if (TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
			{
				hediffWithComps_FleshmassHeart.LevelUp();
				cachedRegen = null;
				if (!PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					return;
				}
				Messages.Message("WVC_XaG_MutationProgressing".Translate(hediffWithComps_FleshmassHeart.def.LabelCap), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			else
			{
				//hideInspectInfo = true;
				TrySpawnMeat(pawn);
			}
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

	}

	[Obsolete]
	public class Gene_FleshmassBuilder : Gene_DeadlifeBuilder
    {

    }

    public class Gene_FleshmassBrain : Gene_AutoResearch
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(3333, delta))
			{
				DoResearch(3333);
			}
		}

	}

	public class Gene_FleshmassArmor : Gene
	{

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(16333, delta))
			{
				DoApparelDamage(16333);
			}
		}

		public void DoApparelDamage(int tick)
		{
			List<Apparel> apparels = pawn.apparel.WornApparel.Where((apparel) => apparel.def.apparel.countsAsClothingForNudity && apparel.def.useHitPoints).ToList();
			bool spaceFlag = pawn.InSpace();
			foreach (Apparel apparel in apparels)
			{
                apparel.HitPoints = Mathf.Clamp(apparel.HitPoints - (tick / apparels.Count / 1500 / (spaceFlag ? 10 : 1)), 1, apparel.MaxHitPoints);
			}
		}

	}

	public class Gene_DeadlifeBuilder : Gene, IGeneOverridden, IGeneNotifyGenesChanged
	{

		public int nextTick = 6000;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 3000;
			Notify_GenesChanged(null);
		}

		public override void TickInterval(int delta)
		{
			nextTick -= delta;
			if (nextTick > 0f)
			{
				return;
			}
			Construct(60);
		}

		//private static int? cachedBuildEfficiency;
		//public static int GetBuildEfficiency
		//{
		//	get
		//	{
		//		if (!cachedBuildEfficiency.HasValue)
		//		{
		//			Recache();
		//		}
		//		return cachedBuildEfficiency.Value;

		//	}
		//}

		//private static Pawn cachedCaster;
		//public static Pawn CurrentCaster
		//{
		//	get
		//	{
		//		if (!cachedBuildEfficiency.HasValue)
		//		{
		//			Recache();
		//		}
		//		return cachedCaster;

		//	}
		//}

		//private static void Recache()
		//{
		//	int buildEfficiency = 0;
		//	foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.ToList())
		//	{
		//		if (pawn.genes?.GetFirstGeneOfType<Gene_FleshmassBuilder>() != null)
		//		{
		//			if (pawn.Map != null)
		//			{
		//				cachedCaster = pawn;
		//			}
		//			buildEfficiency++;
		//		}
		//	}
		//	cachedBuildEfficiency = buildEfficiency;
		//}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: StartDeadlifeBuild",
					action = delegate
					{
						nextTick = 0;
					}
				};
			}
		}

		private static List<Gene_DeadlifeBuilder> cachedBuilderGenes;
		public static List<Gene_DeadlifeBuilder> Builders
		{
			get
			{
				if (cachedBuilderGenes == null)
				{
					List<Gene_DeadlifeBuilder> list = new();
					foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
                    {
						Gene_DeadlifeBuilder builder = pawn.genes?.GetFirstGeneOfType<Gene_DeadlifeBuilder>();
						if (builder != null)
                        {
							list.Add(builder);
                        }
                    }
                    cachedBuilderGenes = list;
				}
				return cachedBuilderGenes;
			}
		}

		public void Construct(int tick)
		{
			//if (!pawn.Faction.IsPlayer || CurrentCaster != pawn)
			//{
			//	nextTick = 60000;
			//	return;
			//}
			//if (pawn.Map == null)
			//{
			//	nextTick = 60000;
			//	Recache();
			//	return;
			//}
            if (XaG_GeneUtility.FactionMap(pawn))
            {
                nextTick = 60000;
                return;
            }
			bool foundSelf = false;
			foreach (Gene_DeadlifeBuilder builder in Builders)
            {
                if (builder != this)
                {
                    builder.nextTick = 7000;
                }
				else
                {
					foundSelf = true;
                }
            }
			if (!foundSelf)
            {
				Notify_GenesChanged(null);
			}
            GasUtility.AddDeadifeGas(pawn.PositionHeld, pawn.MapHeld, pawn.Faction, 30);
			int cycleTry = 0;
			bool pause = true;
			string phase = "";
			try
			{
				float num = pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * tick;
				foreach (Thing thing in pawn.Map.listerBuildings.allBuildingsColonist.ToList())
				{
					phase = "search buildings";
					if (thing is Frame frame && frame.IsCompleted())
					{
						//phase = "try build frame: " + thing.def.defName;
						cycleTry++;
						phase = "spawn gas and do effects";
						thing.Map.effecterMaintainer.AddEffecterToMaintain(frame.ConstructionEffect.Spawn(thing.Position, thing.Map), thing.Position, tick);
						GasUtility.AddDeadifeGas(thing.PositionHeld, thing.MapHeld, pawn.Faction, 30);
						phase = "learn skill";
						if (frame.resourceContainer.Count > 0 && pawn.skills != null)
						{
							foreach (Gene_DeadlifeBuilder builder in Builders)
							{
								builder.SkillLearn(tick, 0.05f);
							}
						}
						phase = "get work speed";
						if (frame.Stuff != null)
						{
							num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
						}
						phase = "do work";
						float workToBuild = frame.WorkToBuild;
						frame.workDone += num;
						if (frame.workDone >= workToBuild)
						{
							phase = "CompleteConstruction";
							frame.CompleteConstruction(pawn);
						}
						pause = false;
					}
					else if (thing.def.useHitPoints && thing is Building building && building.HitPoints < building.MaxHitPoints)
					{
						//phase = "try repair thing: " + thing.def.defName;
						cycleTry++;
						phase = "spawn gas and do effects";
						//IntVec3 positionHeld = thing.PositionHeld;
						//if (thing.def.passability == Traversability.Impassable)
						//{
						//	CellRect rect = thing.OccupiedRect();
						//	positionHeld = rect.ClosestCellTo(positionHeld);
						//}
                        GasUtility.AddDeadifeGas(thing.PositionHeld, thing.MapHeld, pawn.Faction, 30);
						thing.Map.effecterMaintainer.AddEffecterToMaintain(building.def.repairEffect.Spawn(thing.Position, thing.Map), thing.Position, tick);
						phase = "learn skill";
						if (pawn.skills != null)
						{
							foreach (Gene_DeadlifeBuilder builder in Builders)
							{
								builder.SkillLearn(tick, 0.01f);
							}
						}
                        phase = "regen hp";
						building.HitPoints += (int)(2 * num);
						building.HitPoints = Mathf.Min(building.HitPoints, building.MaxHitPoints);
						phase = "Notify_BuildingRepaired";
						pawn.Map.listerBuildingsRepairable.Notify_BuildingRepaired(building);
						pause = false;
					}
					if (cycleTry >= (3 * Builders.Count))
					{
						break;
					}
				}
			}
			catch (Exception arg)
			{
				nextTick = 60000;
				Log.Error("Failed do any build job on phase: " + phase + ". Reason: " + arg);
			}
			if (pause)
			{
				nextTick = tick * (int)(300 + (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 0 ? StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.6f : 1));
			}
			else
            {
				nextTick = tick;
            }
		}

        public void SkillLearn(int tick, float rate)
        {
            pawn.skills.Learn(SkillDefOf.Construction, rate * tick);
        }

        public override void PostRemove()
        {
            base.PostRemove();
			Notify_GenesChanged(null);
		}

        public void Notify_GenesChanged(Gene changedGene)
		{
			cachedBuilderGenes = null;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			Notify_GenesChanged(null);
		}

		public void Notify_Override()
		{
			Notify_GenesChanged(null);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
		}

    }

	public class Gene_SelfDevourStomach : Gene
	{

		private int nextTick = 7539;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 7539;
		}

		public override void TickInterval(int delta)
        {
            if (!GeneResourceUtility.CanTick(ref nextTick, 8678, delta))
            {
                return;
            }
            if (pawn.Faction != Faction.OfPlayer)
            {
                //GeneResourceUtility.OffsetNeedFood(pawn, 1f, true);
                return;
            }
            if (!pawn.TryGetNeedFood(out Need_Food food))
            {
                return;
            }
            if (food.CurLevelPercentage > 0.05f)
            {
                return;
            }
            DevourPart();
		}

        private void DevourPart()
        {
            GeneResourceUtility.OffsetNeedFood(pawn, GetNutritionFromPawn(pawn, true, def.label), false);
			HediffUtility.MutationMeatSplatter(pawn);
		}

        public static float GetNutritionFromPawn(Pawn pawn, bool applyDigestion, string geneDef)
		{
			//(from x in pawn.health.hediffSet.GetNotMissingParts()
			// where !x.def.conceptual && x != pawn.RaceProps.body.corePart && x.def.canSuggestAmputation && !pawn.health.hediffSet.HasDirectlyAddedPartFor(x)
			// select x).TryRandomElement(out var result);
			float bodyPartNutrition = 0f;
			if (pawn.health.hediffSet.GetNotMissingParts().Where((part) => !part.def.conceptual && part != pawn.RaceProps.body.corePart && part.def.canSuggestAmputation && !pawn.health.hediffSet.HasDirectlyAddedPartFor(part)).TryRandomElement(out BodyPartRecord bodyPart))
			{
                float currentCorpseNutrition = (pawn.GetStatValue(StatDefOf.MeatAmount) * 0.1f * (pawn.GetStatValue(StatDefOf.RawNutritionFactor) + pawn.GetStatValue(StatDefOf.MaxNutrition))) / pawn.health.summaryHealth.SummaryHealthPercent;
                bodyPartNutrition = FoodUtility.GetBodyPartNutrition(currentCorpseNutrition, pawn, bodyPart);
				if (applyDigestion)
				{
					Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, pawn, bodyPart);
					hediff_MissingPart.IsFresh = true;
					hediff_MissingPart.lastInjury = HediffDefOf.Digested;
					pawn.health.AddHediff(hediff_MissingPart);
					Messages.Message("WVC_XaG_DigestedBy".Translate(bodyPart.LabelCap, geneDef), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
			}
			return bodyPartNutrition;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SelfDevour",
					action = delegate
					{
						DevourPart();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
		}

	}

    public class Gene_FleshmassImmunity : Gene_AddOrRemoveHediff
	{

		private List<HediffDef> immunizedHediffs = new();
		public List<HediffDef> ImmunizedHediffs => immunizedHediffs;

		public void ImmunizeHediff(HediffDef hediffDef)
        {
			if (immunizedHediffs == null)
            {
				immunizedHediffs = new();
			}
			if (!immunizedHediffs.Contains(hediffDef))
            {
				immunizedHediffs.Add(hediffDef);
				foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
					if (hediff is Hediff_FleshmassImmunity fleshmass)
                    {
						fleshmass.Reset();
                    }
                }
			}
		}

		private int nextTick = 6000;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 6000;
		}

		public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
			if (GeneResourceUtility.CanTick(ref nextTick, 66966, delta))
            {
                TryImmunizeNewHediffs();
            }
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: TryImmunizeNewHediffs",
					action = delegate
					{
						TryImmunizeNewHediffs();
					}
				};
			}
		}

		public void TryImmunizeNewHediffs()
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                HediffComp_Immunizable hediffComp_Immunizable = hediff.TryGetComp<HediffComp_Immunizable>();
                if (hediffComp_Immunizable != null)
                {
                    ImmunizeHediff(hediff.def);
                }
            }
        }

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", -1);
			Scribe_Collections.Look(ref immunizedHediffs, "immunizedHediffs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.LoadingVars && immunizedHediffs != null && immunizedHediffs.RemoveAll((HediffDef x) => x == null) > 0)
			{
				Log.Warning("Removed null hediffDef(s)");
			}
		}

	}

	public class Gene_FleshmassReproduction : Gene, IGenePregnantHuman, IGeneOverridden
	{

		public override bool Active
		{
			get
			{
				if (permanentlyDisabled)
				{
					return false;
				}
				return pawn?.gender == Gender.Female && base.Active;
			}
		}

		public bool Notify_CustomPregnancy(Hediff_Pregnant pregnancy)
		{
			return false;
		}

		public void Notify_PregnancyStarted(Hediff_Pregnant pregnancy)
		{
			Gene_Parthenogenesis.AddParentGenes(pawn, pregnancy);
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(59900, delta))
			{
				StartPregnancyRandom();
			}
		}

		public void StartPregnancyRandom()
		{
			if (Rand.Chance(0.025f * pawn.GetStatValue(StatDefOf.Fertility)))
			{
				MiscUtility.Impregnate(pawn);
			}
		}

		public void Notify_TentacleRemoved()
		{
			permanentlyDisabled = true;
		}

		private bool permanentlyDisabled = false;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref permanentlyDisabled, "permanentlyDisabled", defaultValue: false);
		}

		public static bool TryAddLoveEnchancer(Pawn pawn)
		{
			if (!ModsConfig.RoyaltyActive)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.LoveEnhancer))
			{
				return false;
			}
			if (TryMakeLoveHediff(pawn, out Hediff_Fleshmass_LoveEnhancer hediff))
			{
				pawn.health.AddHediff(hediff, pawn.health.hediffSet.TryGetBodyPartRecord(BodyPartDefOf.Torso, out BodyPartRecord result) && result != null ? result : null);
				return true;
			}
			return false;
		}

		public static bool TryRemoveLoveEnchancer(Pawn pawn)
		{
			if (!ModsConfig.RoyaltyActive)
			{
				return false;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.LoveEnhancer))
			{
				return false;
			}
			bool flag = false;
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList())
			{
				if (hediff is Hediff_Fleshmass_LoveEnhancer)
				{
					pawn.health.RemoveHediff(hediff);
					flag = true;
				}
			}
			return flag;
		}

		private static bool TryMakeLoveHediff(Pawn pawn, out Hediff_Fleshmass_LoveEnhancer hediff)
		{
			hediff = null;
			if (pawn == null)
			{
				return false;
			}
			hediff = (Hediff_Fleshmass_LoveEnhancer)Activator.CreateInstance(typeof(Hediff_Fleshmass_LoveEnhancer));
			hediff.def = HediffDefOf.LoveEnhancer;
			hediff.pawn = pawn;
			hediff.Part = pawn.health.hediffSet.TryGetBodyPartRecord(BodyPartDefOf.Torso, out BodyPartRecord result) && result != null ? result : null;
			hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
			hediff.PostMake();
			return true;
		}

		public override void PostAdd()
		{
			if (Active)
			{
				base.PostAdd();
				TryAddLoveEnchancer(pawn);
			}
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			TryRemoveLoveEnchancer(pawn);
		}

		public void Notify_Override()
		{
			TryAddLoveEnchancer(pawn);
		}

		public override void PostRemove()
		{
			base.PostRemove();
			TryRemoveLoveEnchancer(pawn);
		}

	}

}
