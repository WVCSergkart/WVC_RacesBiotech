using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace WVC_XenotypesAndGenes
{

    public class GolemlinkMode
	{

		public Gene_Golemlink golemlink;
		public Gene_GolemlinkSubGene parent;

		//public virtual bool UseSummonMenu => true;

		public virtual List<GolemModeDef> AllowedGolemModes => ListsUtility.GetAllGolemModeDefs();
		public virtual int CountSpawn => WVC_Biotech.settings.golemlink_golemsToSpawnRange.RandomInRange;

		public virtual void SummonRandomMech(bool ignoreChunks = false)
        {
            SetupGolemList(golemlink.Spawner?.golemModeDefs);
            if (golemlink.golemsForSummon.NullOrEmpty())
            {
                Log.Error("Failed summon golems. golemsForSummon is null.");
                return;
            }
            string phase = "";
            try
            {
                phase = "start";
                int countSpawn = CountSpawn;
                //float possibleConsumption = 1;
                phase = "get total golembond";
                float currentLimit = MechanoidsUtility.TotalGolembond(golemlink.pawn);
                phase = "get consumed golembond";
                float currentConsumption = MechanoidsUtility.GetConsumedGolembond(golemlink.pawn);
                phase = "get all controlled golems";
                List<PawnKindDef> currentGolems = MechanoidsUtility.GetAllControlledGolems_PawnKinds(golemlink.pawn);
                if (currentGolems.NullOrEmpty())
                {
                    currentGolems = new();
                }
                bool summonInsteadAnimation = false;
                int countSummon = 0;
                //List<Thing> summonList = new();
                phase = "start spawn";
                for (int i = 0; i < countSpawn; i++)
                {
                    Thing chunk = Gene_Golemlink.GetBestStoneChunk(golemlink.pawn, false);
                    phase = "choose method";
                    if (ignoreChunks || chunk == null)
                    {
                        summonInsteadAnimation = true;
                        countSummon = countSpawn - i;
                        break;
                        //if (currentLimit < currentConsumption + possibleConsumption)
                        //{
                        //	break;
                        //}
                        //phase = "create golem";
                        //MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
                        //if (i == 0)
                        //{
                        //	Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
                        //}
                        //possibleConsumption++;
                    }
                    else if (Gene_Golemlink.TryGetBestGolemKindForSummon(currentLimit, currentConsumption, golemlink.golemsForSummon, currentGolems, out PawnKindDef newGolem, out float golemConsumtion))
                    {
                        phase = "try animate golem";
                        if (Gene_Golemlink.TryCreateGolemFromThing(chunk, newGolem, golemlink.pawn))
                        {
                            currentConsumption += golemConsumtion;
                            currentGolems.Add(newGolem);
                        }
                    }
                }
                if (!summonInsteadAnimation)
                {
                    return;
                }
                phase = "summon random golem";
                countSummon = Mathf.Clamp(countSummon, 0, (int)(currentLimit - currentConsumption));
                if (countSummon <= 0)
                {
                    return;
                }
                if (MechanoidsUtility.TrySummonMechanoids(golemlink.pawn, countSummon, golemlink.golemsForSummon, out List<Thing> summonList))
                {
                    Messages.Message("WVC_RB_Gene_Summoner".Translate(), new LookTargets(summonList), MessageTypeDefOf.PositiveEvent);
                }
            }
            catch (Exception arg)
            {
                golemlink.summonMechanoids = false;
                Log.Error($"Error while generating golems {this.ToStringSafe()} during phase {phase}: {arg}");
            }
        }

        public virtual void SetupGolemList(List<GolemModeDef> defaultGolems)
        {
            if (golemlink.golemsForSummon.NullOrEmpty())
            {
                golemlink.golemsForSummon = defaultGolems;
            }
            else
            {
                List<GolemModeDef> golemModes = AllowedGolemModes;
                foreach (GolemModeDef modeDef in golemlink.golemsForSummon.ToList())
                {
                    if (!golemModes.Contains(modeDef))
                    {
                        golemlink.golemsForSummon.Remove(modeDef);
                    }
                }
                if (golemlink.golemsForSummon.Empty())
                {
                    golemlink.golemsForSummon = defaultGolems;
                }
            }
        }

    }

	public class GolemlinkMode_DryadPetrifier : GolemlinkMode
	{

        public override List<GolemModeDef> AllowedGolemModes
        {
            get
            {
                return parent.AllowedGolemModes;
            }
        }

		private Gene_DryadQueen cachedDryadsQueenGene;
		public Gene_DryadQueen Gauranlen
		{
			get
			{
				if (cachedDryadsQueenGene == null || !cachedDryadsQueenGene.Active)
				{
					cachedDryadsQueenGene = golemlink.pawn?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedDryadsQueenGene;
			}
		}

		public override void SummonRandomMech(bool ignoreChunks = false)
        {
            SetupGolemList(AllowedGolemModes);
            float currentLimit = MechanoidsUtility.TotalGolembond(golemlink.pawn);
			float currentConsumption = MechanoidsUtility.GetConsumedGolembond(golemlink.pawn);
            int countSpawn = CountSpawn;
			int createdGolems = 0;
			foreach (Pawn dryad in Gauranlen.DryadsListForReading.ToList())
			{
				foreach (GolemModeDef modeDef in golemlink.golemsForSummon)
				{
					if (createdGolems >= countSpawn)
					{
						break;
					}
					if (dryad.kindDef != modeDef.dryadKindDef)
                    {
                        continue;
                    }
					if (currentLimit <= currentConsumption + modeDef.GolembondCost)
					{
						continue;
					}
                    if (TryCreateGolemFromThing(dryad, modeDef.pawnKindDef, golemlink.pawn))
                    {
						createdGolems++;
						break;
                    }
                }
            }
		}

		public static bool TryCreateGolemFromThing(Pawn dryad, PawnKindDef golemPawnkind, Pawn mechanitor)
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
