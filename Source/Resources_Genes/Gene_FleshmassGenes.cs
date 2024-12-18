﻿using RimWorld;
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

	public class Gene_FleshmassNucleus : Gene
	{

		public GeneExtension_Undead Undead => def?.GetModExtension<GeneExtension_Undead>();

		private int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			nextTick = 150000;
		}

		public override void Tick()
		{
			// base.Tick();
			if (pawn.IsHashIntervalTick(600))
			{
				HealingUtility.Regeneration(pawn, Undead.regeneration, WVC_Biotech.settings.totalHealingIgnoreScarification, 600);
			}
			//new IntRange(60000, 150000).RandomInRange
			if (GeneResourceUtility.CanTick(ref nextTick, 150000))
			{
				TryGiveMutation();
			}
		}

		private void TryGiveMutation()
		{
			if (Gene_MorphMutations.TryGetBestMutation(pawn, out HediffDef mutation))
			{
				if (HediffUtility.TryGiveFleshmassMutation(pawn, mutation))
				{
					return;
				}
				if (TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart))
				{
					hediffWithComps_FleshmassHeart.LevelUp();
				}
				else
				{
					TrySpawnMeat(pawn);
				}
			}
			//Evolve();
		}

		//private void Evolve()
		//{
		//	if (pawn.Faction != Faction.OfPlayer)
		//	{
		//		return;
		//	}
		//	if (!Rand.Chance(0.2f))
		//	{
		//		return;
		//	}
		//	if (StaticCollectionsClass.cachedColonistsCount > 1)
		//	{
		//		return;
		//	}
		//	if (DefDatabase<GeneDef>.AllDefsListForReading.Where((geneDef) => geneDef != def && geneDef.GetModExtension<GeneExtension_General>()?.isFleshmass == true).TryRandomElement(out GeneDef geneDef))
		//	{
		//		XaG_GeneUtility.AddGeneToChimera(pawn, geneDef);
		//	}
		//}

		private bool TryGetWeakerPawnMutation(out HediffAddedPart_FleshmassNucleus hediffWithComps_FleshmassHeart)
		{
			hediffWithComps_FleshmassHeart = null;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs.Where((Hediff hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff && massHediff.CurrentLevel < Undead.maxMutationLevel).OrderBy((hediff) => hediff is HediffAddedPart_FleshmassNucleus massHediff ? massHediff.CurrentLevel : 0f).ToList();
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
			if (ModsConfig.AnomalyActive && pawn.Corpse?.Map != null)
			{
				if (GenDrop.TryDropSpawn(PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.FleshmassNucleus, Faction.OfEntities)), pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld, ThingPlaceMode.Near, out var resultingThing))
				{
					CompActivity activity = resultingThing.TryGetComp<CompActivity>();
					if (activity != null)
					{
						activity.AdjustActivity(1f);
					}
					//if (!pawn.Corpse.Destroyed)
					//{
					//	EffecterDefOf.MeatExplosion.Spawn(pawn.Corpse.PositionHeld, pawn.Corpse.MapHeld).Cleanup();
					//	pawn.Corpse.Destroy();
					//}
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
					defaultLabel = "DEV: FleshmassEvolve",
					action = delegate
					{
						XaG_GeneUtility.ImplantChimeraDef(pawn, def);
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextTick", 0);
		}

	}

	public class Gene_FleshmassBrain : Gene
	{

		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(3333))
			{
				DoResearch(3333);
			}
		}

		private float? cachedResearchSpeed;

		public void DoResearch(int tick)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (!cachedResearchSpeed.HasValue)
			{
				cachedResearchSpeed = pawn.GetStatValue(StatDefOf.ResearchSpeed);
			}
			ResearchManager researchManager = Find.ResearchManager;
			ResearchProjectDef currentProject = researchManager.GetProject();
			if (currentProject != null)
			{
				pawn.skills.Learn(SkillDefOf.Intellectual, 0.04f * tick);
				float researchAmount = (cachedResearchSpeed.Value * tick) / (StaticCollectionsClass.cachedColonistsCount > 0 ? StaticCollectionsClass.cachedColonistsCount : 1f);
				researchManager.ResearchPerformed(researchAmount, pawn);
			}
		}

	}

	public class Gene_FleshmassArmor : Gene
	{

		public override void Tick()
		{
			if (pawn.IsHashIntervalTick(16333))
			{
				DoApparelDamage(16333);
			}
		}

		public void DoApparelDamage(int tick)
		{
			List<Apparel> apparels = pawn.apparel.WornApparel.Where((apparel) => apparel.def.apparel.countsAsClothingForNudity && apparel.def.useHitPoints).ToList();
			foreach (Apparel apparel in apparels)
			{
				apparel.HitPoints -= Mathf.Clamp(tick / apparels.Count / 1500, 1, apparel.MaxHitPoints);
			}
		}

	}

	public class Gene_FleshmassBuilder : Gene
	{

		public int nextTick = 6000;

		public override void Tick()
		{
			nextTick--;
			if (nextTick > 0f)
			{
				return;
			}
			Construct(30);
		}

		public void Construct(int tick)
		{
			if (XaG_GeneUtility.FactionMap(pawn))
			{
				nextTick = 60000;
				return;
			}
			GasUtility.AddDeadifeGas(pawn.PositionHeld, pawn.MapHeld, pawn.Faction, 30);
			int cycleTry = 0;
			bool pause = true;
			try
			{
				foreach (Thing thing in pawn.Map.listerBuildings.allBuildingsColonist.ToList())
				{
					if (thing is Frame frame && frame.IsCompleted())
					{
						cycleTry++;
						thing.Map.effecterMaintainer.AddEffecterToMaintain(frame.ConstructionEffect.Spawn(thing.Position, thing.Map), thing.Position, 30);
						//SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(thing.Position, thing.Map));
						//GasUtility.AddGas(thing.PositionHeld, thing.MapHeld, GasType.DeadlifeDust, 30);
						GasUtility.AddDeadifeGas(thing.PositionHeld, thing.MapHeld, pawn.Faction, 30);
						//FleckMaker.Static(thing.Position, pawn.Map, FleckDefOf.HealingCross, 1f);
						if (frame.resourceContainer.Count > 0 && pawn.skills != null)
						{
							pawn.skills.Learn(SkillDefOf.Construction, 0.05f * tick);
						}
						float num = pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * tick;
						if (frame.Stuff != null)
						{
							num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
						}
						float workToBuild = frame.WorkToBuild;
						frame.workDone += num;
						if (frame.workDone >= workToBuild)
						{
							frame.CompleteConstruction(pawn);
						}
						pause = false;
					}
					else if (thing is Building building && building.HitPoints < building.MaxHitPoints)
					{
						cycleTry++;
						//GasUtility.AddGas(thing.PositionHeld, thing.MapHeld, GasType.DeadlifeDust, radius: 1f);
						GasUtility.AddDeadifeGas(thing.PositionHeld, thing.MapHeld, pawn.Faction, 30);
						thing.Map.effecterMaintainer.AddEffecterToMaintain(building.def.repairEffect.Spawn(thing.Position, thing.Map), thing.Position, 30);
						//FleckMaker.Static(thing.Position, pawn.Map, FleckDefOf.HealingCross, 1f);
						//SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(thing.Position, thing.Map));
						if (pawn.skills != null)
						{
							pawn.skills.Learn(SkillDefOf.Construction, 0.01f * tick);
						}
						float num = pawn.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f * tick / 80;
						building.HitPoints = Mathf.Clamp(building.HitPoints + (int)num, building.HitPoints + 1, building.MaxHitPoints);
						pawn.Map.listerBuildingsRepairable.Notify_BuildingRepaired(building);
						pause = false;
					}
					if (cycleTry >= 3)
					{
						break;
					}
				}
			}
			catch (Exception arg)
			{
				//nextTick = 180000;
				Log.Error("Failed do any build job. Reason: " + arg);
			}
			if (pause)
			{
				nextTick = 6000 * (StaticCollectionsClass.cachedColonistsCount > 0 ? StaticCollectionsClass.cachedColonistsCount : 1);
			}
			else
            {
				nextTick = 30 * (StaticCollectionsClass.cachedColonistsCount > 0 ? StaticCollectionsClass.cachedColonistsCount : 1);
            }
		}

	}

}
