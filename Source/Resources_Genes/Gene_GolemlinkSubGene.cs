using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_GolemlinkSubGene : Gene
	{

		private GeneExtension_Spawner cachedGeneExtension_Spawner;
		public GeneExtension_Spawner Spawner
		{
			get
			{
				if (cachedGeneExtension_Spawner == null)
				{
					cachedGeneExtension_Spawner = def.GetModExtension<GeneExtension_Spawner>();
				}
				return cachedGeneExtension_Spawner;
			}
		}

		[Unsaved(false)]
		private Gene_Golemlink cachedGolemlinkGene;
		public Gene_Golemlink Golemlink
		{
			get
			{
				if (cachedGolemlinkGene == null || !cachedGolemlinkGene.Active)
				{
					cachedGolemlinkGene = pawn?.genes?.GetFirstGeneOfType<Gene_Golemlink>();
				}
				return cachedGolemlinkGene;
			}
		}

		//public virtual Type CustomWorker => null;
		public virtual List<GolemModeDef> AllowedGolemModes => null;

		public override void TickInterval(int delta)
		{

		}

		public virtual void SubGolemMaker(out int golemsSpawned)
        {
			golemsSpawned = 0;
		}

	}

	public class Gene_Golemlink_DryadPetrifier : Gene_GolemlinkSubGene
	{

		private Gene_DryadQueen cachedDryadsQueenGene;
		public Gene_DryadQueen Gauranlen
		{
			get
			{
				if (cachedDryadsQueenGene == null || !cachedDryadsQueenGene.Active)
				{
					cachedDryadsQueenGene = pawn?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedDryadsQueenGene;
			}
		}

		public override List<GolemModeDef> AllowedGolemModes => Spawner.golemModeDefs;

		//public override void TickInterval(int delta)
		//{
		//	if
		//}

		public override void SubGolemMaker(out int golemsSpawned)
		{
			golemsSpawned = 0;
			PetrifieDryads(ref golemsSpawned);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: PetrifieDryad",
					action = delegate
					{
						int golems = 0;
						PetrifieDryads(ref golems);
					}
				};
			}
		}

		public void PetrifieDryads(ref int golemsCount)
		{
			//SetupGolemList(AllowedGolemModes);
			float currentLimit = MechanoidsUtility.TotalGolembond(pawn);
			float currentConsumption = MechanoidsUtility.GetConsumedGolembond(pawn);
			//int countSpawn = Golemlink.CountSpawn;
			//int createdGolems = 0;
			foreach (Pawn dryad in Gauranlen.DryadsListForReading.ToList())
			{
				foreach (GolemModeDef modeDef in Golemlink.golemsForSummon)
				{
					if (dryad.kindDef != modeDef.dryadKindDef)
					{
						continue;
					}
					if (currentLimit <= currentConsumption + modeDef.GolembondCost)
					{
						continue;
					}
					if (TryCreateGolemFromThing(dryad, modeDef.pawnKindDef, pawn))
					{
						golemsCount++;
						return;
					}
				}
			}
		}

		private bool TryCreateGolemFromThing(Pawn dryad, PawnKindDef golemPawnkind, Pawn mechanitor)
		{
			try
			{
				if (dryad != null)
				{
					PawnGenerationRequest request = new(golemPawnkind, mechanitor.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
					Pawn newGolem = PawnGenerator.GeneratePawn(request);
					newGolem.Position = dryad.PositionHeld;
					newGolem.SpawnSetup(dryad.MapHeld, respawningAfterLoad: false);
					newGolem.stances?.stunner?.StunFor(60, null, addBattleLog: false);
					CompSpawnOnDeath_GetColor compGolem = newGolem.TryGetComp<CompSpawnOnDeath_GetColor>();
					if (compGolem != null)
					{
						ThingDef def = DeepDrillUtility.RockForTerrain(dryad.MapHeld.terrainGrid.BaseTerrainAt(dryad.PositionHeld)) ?? Gene_Golemlink.GetBestStoneChunk(dryad, false)?.def;
						compGolem.SetStoneChunk(def);
						//compGolem.SetStoneChunk(DeepDrillUtility.RockForTerrain(dryad.MapHeld.terrainGrid.BaseTerrainAt(dryad.PositionHeld)));
					}
					MechanoidsUtility.SetOverseer(mechanitor, newGolem);
					//pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, summon);
					newGolem.ageTracker.AgeBiologicalTicks = dryad.ageTracker.AgeBiologicalTicks;
					newGolem.ageTracker.AgeChronologicalTicks = dryad.ageTracker.AgeChronologicalTicks;
					if (!dryad.Name.Numerical)
					{
						newGolem.Name = dryad.Name;
					}
					newGolem.Rotation = dryad.Rotation;
					MiscUtility.DoShapeshiftEffects_OnPawn(dryad);
					dryad.Destroy();
					Messages.Message("WVC_XaG_GolemCreatedFromRandomDryad_Message".Translate(newGolem.Name.ToString()), newGolem, MessageTypeDefOf.PositiveEvent);
					return true;
				}
			}
			catch
			{
				Log.Error("Failed create golem from " + dryad.def.defName);
			}
			return false;
		}

	}

}
