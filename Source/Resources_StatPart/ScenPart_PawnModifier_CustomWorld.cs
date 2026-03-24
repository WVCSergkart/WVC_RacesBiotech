// RimWorld.StatPart_Age
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
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
	//public class ScenPart_PawnModifier_VoidRules : ScenPart_PawnModifier_XenotypesAndGenes
	//{

	//	public GeneDef voidRules_GeneDef; 
	//	private int nextTick = 6000;
	//	public override void Tick()
	//	{
	//		if (nextTick > 0)
	//		{
	//			nextTick--;
	//			return;
	//		}
	//		nextTick = 60000;
	//		if (pawns.NullOrEmpty())
	//		{
	//			return;
	//		}
	//		bool gameOver = true;
	//		List<Pawn> colonists = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive.Where((p) => p.Faction == Faction.OfPlayer).ToList();
	//		foreach (Pawn pawn in pawns.ToList())
	//		{
	//			if (!colonists.Contains(pawn))
	//			{
	//				pawns.Remove(pawn);
	//				continue;
	//			}
	//			Hediff_Pregnant hediff_Pregnant = pawn
	//			if (pawn.health.hediffSet.HasPregnancyHediff())
	//			{
	//				gameOver = false;
	//				break;
	//			}
	//		}
	//		if (gameOver)
	//		{
	//			foreach (Pawn pawn in pawns.ToList())
	//			{
	//				if (!pawn.Dead)
	//				{
	//					pawn.Kill(null);

	//				}
	//			}
	//		}
	//	}

	//	public List<Pawn> pawns;
	//	protected override void ModifyNewPawn(Pawn p)
	//	{
	//		if (pawns == null)
	//		{
	//			pawns = new();
	//		}
	//		pawns.Add(p);
	//		base.ModifyNewPawn(p);
	//	}

	//	public override void ExposeData()
	//	{
	//		base.ExposeData();
	//		Scribe_Values.Look(ref nextTick, "nextTick", 120);
	//		Scribe_Collections.Look(ref pawns, "specialPawns", LookMode.Reference);
	//	}

	//}
	public class ScenPart_PawnModifier_CustomWorld : ScenPart_PawnModifier
	{

		public List<XenotypeDef> xenotypeDefs;
		public BiomeDef biomeDef;


		public List<XenotypeHolder> cachedHolder;
		public virtual List<XenotypeHolder> Xenotypes
		{
			get
			{
				if (cachedHolder == null)
				{
					cachedHolder = xenotypeDefs.ConvertToHolder();
				}
				return cachedHolder;
			}
		}

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

		public static bool TrySetupCustomWorldXenotype(Pawn pawn, List<XenotypeHolder> xenotypeDefs)
		{
			try
			{
				if (xenotypeDefs == null || !pawn.IsHuman())
				{
					return false;
				}
				XenotypeHolder xenotype = xenotypeDefs.RandomElement();
				if (!xenotype.CustomXenotype && pawn.genes.Xenotype == xenotype.xenotypeDef)
				{
					return false;
				}
				if (MiscUtility.GameNotStarted())
				{
					SetXenotype(pawn, xenotype);
					return false;
				}
				bool baseliner = pawn.genes.Xenotype == XenotypeDefOf.Baseliner;
				if ((pawn.IsQuestReward() || pawn.IsQuestLodger()) && !baseliner)
				{
					return false;
				}
				if (xenotype.inheritable && !AnyFactionContains(pawn.genes.Xenotype))
				{
					return false;
				}
				SetXenotype(pawn, xenotype);
			}
			catch (Exception arg)
			{
				Log.Error("Failed set xenotype for pawn: " + pawn.Name + ". Reason: " + arg);
				return false;
			}
			return true;
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

		private static void SetXenotype(Pawn pawn, XenotypeHolder xenotypeDef)
		{
			ReimplanterUtility.SetXenotype_Safe(pawn, xenotypeDef, doPostDebug: true);
		}

	}

}
