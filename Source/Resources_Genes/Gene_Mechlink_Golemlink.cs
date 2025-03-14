using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Golemlink : Gene_Mechlink, IGeneInspectInfo
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public List<GolemModeDef> golemsForSummon = new();

		private Gizmo gizmo;

		public override void PostAdd()
		{
			base.PostAdd();
			golemsForSummon = Spawner?.golemModeDefs;
			ResetSummonInterval();
		}

		public void ResetSummonInterval()
		{
			if (Spawner == null)
			{
				return;
			}
			timeForNextSummon = WVC_Biotech.settings.golemlink_spawnIntervalRange.RandomInRange;
		}

		public override void Tick()
		{
			base.Tick();
			if (!summonMechanoids)
			{
				return;
			}
			timeForNextSummon--;
			if (timeForNextSummon > 0)
			{
				return;
			}
			if (CanDoOrbitalSummon())
			{
				SummonRandomMech();
			}
			ResetSummonInterval();
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMapMechanitor(pawn, this))
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Animate golems",
					action = delegate
					{
						SummonRandomMech();
						ResetSummonInterval();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Summon golems",
					action = delegate
					{
						SummonRandomMech(true);
						ResetSummonInterval();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: SetCooldown 1 hour",
					action = delegate
					{
						timeForNextSummon = 1500;
					}
				};
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		private void SummonRandomMech(bool ignoreChunks = false)
		{
			if (golemsForSummon.NullOrEmpty())
			{
				golemsForSummon = Spawner?.golemModeDefs;
			}
			if (golemsForSummon.NullOrEmpty())
			{
				Log.Error("Failed summon golems. golemsForSummon is null.");
				return;
			}
			string phase = "";
			try
			{
				phase = "start";
				int countSpawn = WVC_Biotech.settings.golemlink_golemsToSpawnRange.RandomInRange;
				float possibleConsumption = 1;
				phase = "get total golembond";
				float currentLimit = MechanoidsUtility.TotalGolembond(pawn);
				phase = "get consumed golembond";
				float currentConsumption = MechanoidsUtility.GetConsumedGolembond(pawn);
				phase = "get all controlled golems";
				List<PawnKindDef> currentGolems = MechanoidsUtility.GetAllControlledGolems_PawnKinds(pawn);
				if (currentGolems.NullOrEmpty())
				{
					currentGolems = new();
				}
				//List<Thing> summonList = new();
				phase = "start spawn";
				for (int i = 0; i < countSpawn; i++)
				{
					Thing chunk = GetBestStoneChunk(pawn, false);
					phase = "choose method";
					if (ignoreChunks || chunk == null)
					{
						phase = "summon random golem";
						if (currentLimit < currentConsumption + possibleConsumption)
						{
							break;
						}
						phase = "create golem";
						MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
						if (i == 0)
						{
							Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
						}
						possibleConsumption++;
					}
					else if (TryGetBestGolemKindForSummon(currentLimit, currentConsumption, golemsForSummon, currentGolems, out PawnKindDef newGolem, out float golemConsumtion))
					{
						phase = "try animate golem";
						if (TryCreateGolemFromThing(chunk, newGolem, pawn))
						{
							currentConsumption += golemConsumtion;
							currentGolems.Add(newGolem);
						}
					}
				}
				//phase = "try summon golems";
				//if (!golemsForSummon.NullOrEmpty())
				//{
				//	MiscUtility.SummonDropPod(pawn.Map, summonList);
				//}
			}
			catch (Exception arg)
			{
				summonMechanoids = false;
				Log.Error($"Error while generating golems {this.ToStringSafe()} during phase {phase}: {arg}");
			}
		}

		public static Thing GetBestStoneChunk(Pawn pawn, bool forced)
		{
			Danger danger = (forced ? Danger.Deadly : Danger.Some);
			return (Thing)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Chunk), PathEndMode.InteractionCell, TraverseParms.For(pawn, danger), 9999f, delegate (Thing t)
			{
				Thing chunk = (Thing)t;
				if (!pawn.CanReach(t, PathEndMode.InteractionCell, danger) || chunk.def.thingCategories.NullOrEmpty() || !chunk.def.thingCategories.Contains(ThingCategoryDefOf.StoneChunks))
				{
					return false;
				}
				return !t.IsForbidden(pawn) && pawn.CanReserve(t, 1, -1, null, forced);
			});
		}

		public static bool TryGetBestGolemKindForSummon(float limit, float consumption, List<GolemModeDef> candidates, List<PawnKindDef> currentGolems, out PawnKindDef golem, out float golemConsumtion)
		{
			golem = null;
			golemConsumtion = 0f;
			foreach (GolemModeDef item in candidates)
			{
				if (currentGolems.Contains(item.pawnKindDef))
				{
					continue;
				}
				float golemBondReq = item.GolembondCost;
				if (limit >= consumption + golemBondReq)
				{
					golem = item.pawnKindDef;
					golemConsumtion = golemBondReq;
					break;
				}
			}
			if (golem == null && candidates.TryRandomElement((GolemModeDef mech) => limit >= consumption + mech.GolembondCost, out GolemModeDef newGolem))
			{
				golem = newGolem.pawnKindDef;
				golemConsumtion = newGolem.GolembondCost;
			}
			if (golem == null)
			{
				return false;
			}
			return true;
		}

		public static bool TryCreateGolemFromThing(Thing chunk, PawnKindDef newGolem, Pawn pawn)
		{
			try
			{
				if (chunk != null)
				{
					PawnGenerationRequest request = new(newGolem, pawn.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
					Pawn summon = PawnGenerator.GeneratePawn(request);
					summon.Position = chunk.Position;
					summon.SpawnSetup(chunk.Map, respawningAfterLoad: false);
					CompSpawnOnDeath_GetColor compGolem = summon.TryGetComp<CompSpawnOnDeath_GetColor>();
					if (compGolem != null)
					{
						compGolem.SetStoneChunk(chunk.def);
					}
					MechanoidsUtility.SetOverseer(pawn, summon);
					//pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, summon);
					chunk.ReduceStack();
					Messages.Message("WVC_XaG_GolemCreatedFromRandomChunk_Message".Translate(summon.Name.ToString()), summon, MessageTypeDefOf.PositiveEvent);
					return true;
				}
			}
			catch
			{
				Log.Error("Failed create golem from " + chunk.def.defName);
			}
			return false;
		}

		public string GetInspectInfo
		{
			get
			{
				if (summonMechanoids)
				{
					return "WVC_XaG_Gene_Golemlink_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref golemsForSummon, "golemsForSummon", LookMode.Def);
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
		}

	}

}
