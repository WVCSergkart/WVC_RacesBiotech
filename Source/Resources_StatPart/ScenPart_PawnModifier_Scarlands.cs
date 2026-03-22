// RimWorld.StatPart_Age
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	//public class ScenPart_ForcedMechanitor : ScenPart_PawnModifier
	//{

	//    protected override void ModifyNewPawn(Pawn p)
	//    {
	//        if (!p.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
	//        {
	//            p.health.AddHediff(HediffDefOf.MechlinkImplant, p.health.hediffSet.GetBrain());
	//        }
	//    }

	//}

	//public class ScenPart_PawnModifier_ShamblerWorld : ScenPart_PawnModifier
	//{

	//	protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
	//	{
	//		if (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayerSilentFail))
	//		{
	//			if (pawn.IsMutant || !pawn.IsHuman())
	//			{
	//				return;
	//			}
	//			pawn.equipment.DestroyAllEquipment();
	//			MutantDef mutantDef = Rand.Chance(0.1f) ? MutantDefOf.Ghoul : MutantDefOf.Shambler;
	//			MutantUtility.SetPawnAsMutantInstantly(pawn, mutantDef);
	//			//pawn.SetFaction(Faction.OfEntities);
	//			//pawn.lord 
	//		}
	//	}

	//}

	public class ScenPart_PawnModifier_Scarlands : ScenPart_PawnModifier
	{

		public List<XenotypeDef> xenotypeDefs;
		public BiomeDef biomeDef;

		public override void PostWorldGenerate()
		{
			SetResources();
			SetTraders();
			if (biomeDef == null)
			{
				return;
			}
			try
			{
				SetBiome();
			}
			catch (Exception arg)
			{
				Log.Error("Failed set biomes. Reason: " + arg);
			}
		}

		private void SetTraders()
		{
			try
			{
				foreach (TraderKindDef item in DefDatabase<TraderKindDef>.AllDefsListForReading)
				{
					if (item.tradeCurrency != TradeCurrency.Silver)
					{
						continue;
					}
					if (!item.orbital)
					{
						//item.commonality = 0.01f;
						//foreach (StockGenerator stock in item.stockGenerators.ToList())
						//{
						//	if (!item.orbital)
						//	{
						//		item.commonality = 0.01f;

						//	}
						//}
						item.stockGenerators = new();
						StockGenerator_SingleDef newStock = new();
						newStock.thingDef = ThingDefOf.Silver;
						newStock.countRange = new(300, 3000);
						StockGenerator_BuyAllSellable newStock2 = new();
						item.stockGenerators.Add(newStock);
						item.stockGenerators.Add(newStock2);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed set traders. Reason: " + arg);
			}
		}

		// per game
		public static bool setupResources = true;
		private void SetResources()
		{
			if (!setupResources)
			{
				return;
			}
			try
			{
				foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
				{
					if (thingDef.deepLumpSizeRange.TrueMax > 0f)
					{
						thingDef.deepCountPerCell /= 5;
						thingDef.deepCountPerPortion /= 4;
						thingDef.deepCommonality /= 3;
						thingDef.deepLumpSizeRange = new(Mathf.Clamp(thingDef.deepLumpSizeRange.TrueMin / 3, 1, thingDef.deepLumpSizeRange.TrueMax), thingDef.deepLumpSizeRange.TrueMax / 3);
					}
					if (thingDef.building?.isResourceRock == true)
					{
						//thingDef.building.mineableYield = Mathf.Clamp(thingDef.building.mineableYield / 5, 1, thingDef.building.mineableYield);
						//thingDef.building.mineableScatterCommonality /= 5;
						//thingDef.building.mineableScatterLumpSizeRange = new(Mathf.Clamp(thingDef.building.mineableScatterLumpSizeRange.TrueMin / 5, 1, thingDef.building.mineableScatterLumpSizeRange.TrueMax), thingDef.building.mineableScatterLumpSizeRange.TrueMax / 5);
						thingDef.building.mineableYield = Mathf.Clamp(thingDef.building.mineableYield / 10, 1, thingDef.building.mineableYield);
						thingDef.building.mineableScatterCommonality /= 10;
						thingDef.building.mineableScatterLumpSizeRange = new(Mathf.Clamp(thingDef.building.mineableScatterLumpSizeRange.TrueMin / 10, 1, thingDef.building.mineableScatterLumpSizeRange.TrueMax), thingDef.building.mineableScatterLumpSizeRange.TrueMax / 5);
					}
				}
				setupResources = false;
			}
			catch (Exception arg)
			{
				Log.Error("Failed set resources. Reason: " + arg);
			}
		}

		private void SetBiome()
		{
			List<SurfaceTile> tiles = Find.World.grid.Tiles.ToList();
			foreach (SurfaceTile surfaceTile in tiles)
			{
				if (surfaceTile.PrimaryBiome.isExtremeBiome || surfaceTile.PrimaryBiome == biomeDef || surfaceTile.PrimaryBiome.isBackgroundBiome || surfaceTile.PrimaryBiome.impassable || surfaceTile.PrimaryBiome.isWaterBiome)
				{
					continue;
				}
				surfaceTile.PrimaryBiome = biomeDef;
				foreach (TileMutatorDef tileMutatorDef in surfaceTile.Mutators.ToList())
				{
					try
					{
						//_ = tileMutatorDef.Worker.GetLabel(surfaceTile.tile);
						if (tileMutatorDef == TileMutatorDefOf.MixedBiome || !tileMutatorDef.Worker.IsValidTile(surfaceTile.tile, surfaceTile?.Layer))
						{
							surfaceTile.RemoveMutator(tileMutatorDef);
						}
					}
					catch
					{
						// Silent fail
						surfaceTile.RemoveMutator(tileMutatorDef);
					}
				}
			}
		}

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			try
			{
				if (xenotypeDefs == null || !pawn.IsHuman())
				{
					return;
				}
				XenotypeDef xenotypeDef = xenotypeDefs.RandomElement();
				if (pawn.genes.Xenotype == xenotypeDef)
				{
					return;
				}
				if (MiscUtility.GameNotStarted())
				{
					SetXenotype(pawn, xenotypeDef);
					return;
				}
				bool baseliner = pawn.genes.Xenotype == XenotypeDefOf.Baseliner;
				if ((pawn.IsQuestReward() || pawn.IsQuestLodger()) && !baseliner)
				{
					return;
				}
				if (xenotypeDef.inheritable && !AnyFactionContains(pawn.genes.Xenotype))
				{
					return;
				}
				SetXenotype(pawn, xenotypeDef);
			}
			catch (Exception arg)
			{
				Log.Error("Failed set xenotype for pawn: " + pawn.Name + ". Reason: " + arg);
			}
		}

		private static void SetXenotype(Pawn pawn, XenotypeDef xenotypeDef)
		{
			//if (baseliner || pawn.genes.Xenotype.inheritable || !xenotypeDef.inheritable)
			//{
			//	ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, xenotypeDef);
			//}
			//else
			//{
			//	XenotypeDef oldXenotype = pawn.genes.Xenotype;
			//	ReimplanterUtility.SetXenotype(pawn, xenotypeDef);
			//	ReimplanterUtility.SetXenotype_DoubleXenotype(pawn, oldXenotype);
			//}
			ReimplanterUtility.SetXenotype_Safe(pawn, new(xenotypeDef), doPostDebug: true);
			//if (pawn.genes.GenesListForReading.Any((gene) => gene.def.IsGeneDefOfType<Gene_Ageless>()))
			//{
			//	AgelessUtility.Rejuvenation(pawn);
			//}
			//if (xenotypeDef.Archite)
			//{
			//	HealingUtility.RemoveAllRemovableBadHediffs(pawn);
			//}
		}

		private static bool AnyFactionContains(XenotypeDef xenotypeDef)
		{
			foreach (Faction faction in Find.World.factionManager.AllFactionsVisible)
			{
				if (faction.def.xenotypeSet?.Contains(xenotypeDef) == true)
				{
					return true;
				}
			}
			return false;
		}

		//public override void PreConfigure()
		//{
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref biomeDef, "biomeDef");
			Scribe_Collections.Look(ref xenotypeDefs, "xenotypeDefs", LookMode.Def);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetPlants();
				SetResources();
			}
		}

		//private string cachedDesc = null;
		//public override string Summary(Scenario scen)
		//{
		//	if (cachedDesc == null)
		//	{
		//		StringBuilder stringBuilder = new();
		//		stringBuilder.AppendLine("WVC_XaG_ScenPart_Scarlands".Translate());
		//		//if (!xenotypeDefs.NullOrEmpty())
		//		//{
		//		//	stringBuilder.AppendLine();
		//		//	stringBuilder.AppendLine("WVC_AllowedXenotypes".Translate().CapitalizeFirst() + ":\n" + xenotypeDefs.Select((XenotypeDef x) => x.LabelCap.ToString()).ToLineList(" - "));
		//		//}
		//		cachedDesc = stringBuilder.ToString();
		//	}
		//	return cachedDesc;
		//}

		public override void PostGameStart()
		{
			SetPlants();
		}

		private void SetPlants()
		{
			try
			{
				List<ThingDef> thingDefs = biomeDef?.AllWildPlants;
				foreach (ThingDef thingDef in thingDefs)
				{
					if (thingDef.plant == null)
					{
						continue;
					}
					if (thingDef.plant.minGrowthTemperature > -55)
					{
						thingDef.plant.minGrowthTemperature = -55;
					}
					if (thingDef.plant.maxGrowthTemperature < 55)
					{
						thingDef.plant.maxGrowthTemperature = 55;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Warning("Failed patch plants. Reason: " + arg);
			}
		}

	}

}
