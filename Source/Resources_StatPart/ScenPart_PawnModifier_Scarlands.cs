// RimWorld.StatPart_Age
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
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
