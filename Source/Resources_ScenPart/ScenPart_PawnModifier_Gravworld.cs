// RimWorld.StatPart_Age
using Microsoft.VisualBasic;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ScenPart_PawnModifier_Gravworld : ScenPart_PawnModifier_DynamicXenotypes
	{

		public override float DynamicChance => wvcXenotypesChance;

		public float defaultSubstructureSupport = 9999f;
		public float defaultBandwidth = 999f;
		public float wvcXenotypesChance = 0.02f;
		public List<PrefabDef> prefabDefs;

		//public override void PostGameStart()
		//{
		//	base.PostGameStart();
		//}

		//public override void PreMapGenerate()
		//{
		//	base.PreMapGenerate();
		//}

		public override void PostWorldGenerate()
		{
			base.PostWorldGenerate();
			SetGeneral();
		}

		public override void PostMapGenerate(Map map)
		{
			base.PostMapGenerate(map);
			SetCorpses(map);
		}

		private void SetCorpses(Map map)
		{
			if (!ModsConfig.AnomalyActive)
			{
				return;
			}
			try
			{
				if (map.Biome.inVacuum)
				{
					return;
				}
				IEnumerable<IntVec3> mapCelss = map.AllCells.Where(cell => cell.Standable(map) && cell.InBounds(map));
				int size = (int)(mapCelss.Count() * 0.0005f);
				for (int i = 0; i < size; i++)
				{
					IntVec3 cell = mapCelss.RandomElement();
					if (cell.IsValid)
					{
						PrefabUtility.SpawnPrefab(prefabDefs.RandomElement(), map, cell, Rot4.Random);
						SpawnCorpses(map, cell);
					}
				}
			}
			catch
			{
				Log.Warning("Failed scatter corpses and graves.");
			}

			static void SpawnCorpses(Map map, IntVec3 cell)
			{
				int randomInRange3 = new IntRange(1, 4).RandomInRange;
				for (int num2 = 0; num2 < randomInRange3; num2++)
				{
					if (CellFinder.TryFindRandomCellNear(cell, map, 4, (IntVec3 c) => c.Standable(map), out var result3))
					{
						Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Drifter, null, PawnGenerationContext.NonPlayer, map.Tile));
						pawn.health.SetDead();
						pawn.apparel.DestroyAll();
						pawn.equipment.DestroyAllEquipment();
						Find.WorldPawns.PassToWorld(pawn);
						Corpse corpse = pawn.MakeCorpse(null, null);
						corpse.Age = Mathf.RoundToInt(new IntRange(500, 2000).RandomInRange * 60000);
						corpse.GetComp<CompRottable>().RotProgress += corpse.Age;
						GenSpawn.Spawn(pawn.Corpse, result3, map);
						corpse.SetForbidden(true);
					}
				}
			}
		}

		public override void Notify_PawnDied(Corpse corpse)
		{
			//base.Notify_PawnDied(corpse);
			StaticCollectionsClass.GameComponent?.Backup_InitSummon();
		}

		private void SetGeneral()
		{
			string phase = "init";
			try
			{
				HivemindUtility.nonPlayerHivemindSize = 80;
				phase = "tweak substructure cost";
				ThingDefCountClass thingDefCountClass = new();
				thingDefCountClass.thingDef = ThingDefOf.Steel;
				thingDefCountClass.count = 5;
				TerrainDefOf.Substructure.costList = new() { thingDefCountClass };
				phase = "rename steel";
				ThingDefOf.Steel.label = "WVC_Gravsteel".Translate();
				ThingDefOf.Steel.description = "WVC_GravsteelDesc".Translate();
				//ThingDefOf.Steel.graphicData.texPath = "WVC/Things/XaG_Items/Gravsteel/Steel";
				//ThingDefOf.Steel.stuffProps.color = ThingDefOf.Vacstone.stuffProps.color;
				phase = "forceRockTypes for biomes";
				foreach (BiomeDef biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
				{
					if (biomeDef.inVacuum)
					{
						continue;
					}
					biomeDef.forceRockTypes = new() { ThingDefOf.Vacstone };
				}
				phase = "set resources";
				ScenPart_PawnModifier_Scarlands.SetResources();
				phase = "plants can grow in dark";
				foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
				{
					if (thingDef.plant == null)
					{
						continue;
					}
					thingDef.plant.growMinGlow = -0.01f;
					thingDef.plant.growOptimalGlow = 0f;
				}
				phase = "set SubstructureSupport values";
				StatDefOf.SubstructureSupport.minValue = defaultSubstructureSupport;
				StatDefOf.SubstructureSupport.maxValue = defaultSubstructureSupport;
				phase = "set bandwidth values";
				StatDefOf.MechBandwidth.minValue = defaultBandwidth;
				StatDefOf.MechBandwidth.maxValue = defaultBandwidth;
				if (ModsConfig.AnomalyActive)
				{
					phase = "set anomaly death world";
					AnomalyDefOf.ShamblerSwarm.baseChance = 15f;
					AnomalyDefOf.ShamblerSwarmAnimals.baseChance = 30f;
					AnomalyDefOf.SmallShamblerSwarm.baseChance = 20f;
					AnomalyDefOf.ShamblerAssault.baseChance = 5f;
					AnomalyDefOf.GhoulAttack.baseChance = 17f;
					phase = "death pall";
					IncidentDef deathPall = AnomalyDefOf.DeathPall;
					deathPall.baseChance = 100f;
					deathPall.earliestDay = 0;
					deathPall.durationDays = new FloatRange(deathPall.minRefireDays / 2, deathPall.minRefireDays - 1);
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Failed set gravship start. On phase: {phase}. Reason: {arg}");
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetGeneral();
			}
		}

	}

}
