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

	public class ScenPart_PawnModifier_Scarlands : ScenPart_PawnModifier_CustomWorld
	{

		//public override void PreConfigure()
		//{
		//	SetFactions();
		//}

		//private void SetFactions()
		//{
		//	foreach (FactionDef factionDef in DefDatabase<FactionDef>.AllDefsListForReading)
		//	{
		//		if (!factionDef.displayInFactionSelection || factionDef.hidden)
		//		{
		//			continue;
		//		}
		//		factionDef.requiredCountAtGameStart = 0;
		//	}
		//}

		public override void PostWorldGenerate()
		{
			SetResources();
			SetTraders();
			base.PostWorldGenerate();
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
						item.label = "goods collector";
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
			}
			catch (Exception arg)
			{
				Log.Error("Failed set resources. Reason: " + arg);
			}
			setupResources = false;
		}

		protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
		{
			TrySetupCustomWorldXenotype(pawn, Xenotypes);
		}

		//public override void PreConfigure()
		//{
		//}

		public override void ExposeData()
		{
			base.ExposeData();
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				SetResources();
				SetTraders();
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

	}

}
