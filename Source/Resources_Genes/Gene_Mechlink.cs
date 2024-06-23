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

	public class Gene_Mechlink : Gene, IGeneOverridden
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public int timeForNextSummon = -1;
		public bool summonMechanoids = false;

		public bool pawnHadMechlinkBefore = false;

		public override void PostAdd()
		{
			base.PostAdd();
			if (!WVC_Biotech.settings.link_addedMechlinkWithGene)
			{
				return;
			}
			if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			{
				pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			}
			else
			{
				pawnHadMechlinkBefore = true;
			}
			ResetSummonInterval();
		}

		public void ResetSummonInterval()
		{
			if (Spawner == null)
			{
				return;
			}
			timeForNextSummon = Spawner.spawnIntervalRange.RandomInRange;
		}

		public bool CanDoOrbitalSummon()
		{
			// if (!summonMechanoids)
			// {
				// return false;
			// }
			if (pawn.Faction != Faction.OfPlayer)
			{
				summonMechanoids = false;
				return false;
			}
			if (pawn.Map == null)
			{
				return false;
			}
			// if (!CommsConsoleUtility.PlayerHasPoweredCommsConsole(pawn.Map))
			// {
				// return false;
			// }
			if (!MechanitorUtility.IsMechanitor(pawn))
			{
				summonMechanoids = false;
				// Reset();
				return false;
			}
			return true;
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
			{
				HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
			}
		}

		public void Notify_Override()
		{
			if (WVC_Biotech.settings.link_removeMechlinkWithGene && WVC_Biotech.settings.link_addedMechlinkWithGene)
			{
				if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
				{
					pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
				}
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (WVC_Biotech.settings.link_removeMechlinkWithGene && !pawnHadMechlinkBefore)
			{
				HediffUtility.TryRemoveHediff(HediffDefOf.MechlinkImplant, pawn);
			}
		}

		// public override void Reset()
		// {
			// base.Reset();
			// if (!pawn.health.hediffSet.HasHediff(HediffDefOf.MechlinkImplant))
			// {
				// pawn.health.AddHediff(HediffDefOf.MechlinkImplant, pawn.health.hediffSet.GetBrain());
			// }
		// }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref timeForNextSummon, "timeForNextSummon", -1);
			Scribe_Values.Look(ref summonMechanoids, "summonMechanoids", false);
			Scribe_Values.Look(ref pawnHadMechlinkBefore, "pawnHadMechlinkBefore", false);
		}

	}

	// [Obsolete]
	// public class Gene_MechlinkWithGizmo : Gene_Mechlink
	// {

		// public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		// private Gizmo gizmo;

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (pawn?.Map == null)
			// {
				// yield break;
			// }
			// if (gizmo == null)
			// {
				// gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			// }
			// yield return gizmo;
		// }

	// }

	// [Obsolete]
	// public class Gene_Sporelink : Gene_MechlinkWithGizmo
	// {

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			// {
				// yield break;
			// }
			// foreach (Gizmo item in base.GetGizmos())
			// {
				// yield return item;
			// }
			// Command_Action command_Action = new()
			// {
				// defaultLabel = "WVC_XaG_Gene_Sporelink_ConnectWithResurgentTrees".Translate() + ": " + GeneUiUtility.OnOrOff(summonMechanoids),
				// defaultDesc = "WVC_XaG_Gene_Sporelink_ConnectWithResurgentTreesDesc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// summonMechanoids = !summonMechanoids;
					// if (summonMechanoids)
					// {
						// SoundDefOf.Tick_High.PlayOneShotOnCamera();
					// }
					// else
					// {
						// SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					// }
				// }
			// };
			// yield return command_Action;
		// }

	// }

	public class Gene_Golemlink : Gene_Mechlink, IGeneInspectInfo
	{

		public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

		public List<PawnKindDef> golemsForSummon = new();

		private Gizmo gizmo;

		public override void PostAdd()
		{
			base.PostAdd();
			golemsForSummon = Spawner?.mechTypes;
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
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
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
			if (pawn?.Map == null)
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
			// foreach (Gizmo item in base.GetGizmos())
			// {
				// yield return item;
			// }
			// Command_Action command_Action = new()
			// {
				// defaultLabel = "WVC_XaG_Gene_DustMechlink".Translate() + ": " + GeneUiUtility.OnOrOff(summonMechanoids),
				// defaultDesc = "WVC_XaG_Gene_DustMechlinkDesc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// summonMechanoids = !summonMechanoids;
					// if (summonMechanoids)
					// {
						// SoundDefOf.Tick_High.PlayOneShotOnCamera();
					// }
					// else
					// {
						// SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					// }
				// }
			// };
			// yield return command_Action;
		}

		private void SummonRandomMech(bool ignoreChunks = false)
		{
			if (golemsForSummon.NullOrEmpty())
			{
				golemsForSummon = Spawner?.mechTypes;
			}
			if (golemsForSummon.NullOrEmpty())
			{
				Log.Error("Failed summon golems. golemsForSummon is null.");
				return;
			}
			int phase = 0;
			try
			{
				phase = 1;
				int countSpawn = Spawner.summonRange.RandomInRange;
				float possibleConsumption = 1;
				// PawnKindDef newGolem = ;
				phase = 2;
				float currentLimit = MechanoidsUtility.TotalGolembond(pawn);
				phase = 3;
				float currentConsumption = MechanoidsUtility.GetConsumedGolembond(pawn);
				phase = 4;
				List<PawnKindDef> currentGolems = MechanoidsUtility.GetAllControlledGolems_PawnKinds(pawn);
				if (currentGolems.NullOrEmpty())
				{
					currentGolems = new();
				}
				phase = 5;
				List<Thing> chunks = XenotypeFilterUtility.GetAllStoneChunksOnMap(pawn.Map, pawn);
				for (int i = 0; i < countSpawn; i++)
				{
					phase = 6;
					if (ignoreChunks || chunks.NullOrEmpty())
					{
						phase = 7;
						if (currentLimit < currentConsumption + possibleConsumption)
						{
							break;
						}
						phase = 8;
						MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
						if (i == 0)
						{
							Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
						}
						possibleConsumption++;
					}
					else if (TryGetBestGolemKindForSummon(currentLimit, currentConsumption, golemsForSummon, currentGolems, out PawnKindDef newGolem, out float golemConsumtion))
					{
						phase = 9;
						Thing chunk = chunks.RandomElement();
						if (TryCreateGolemFromThing(chunk, newGolem))
						{
							phase = 10;
							chunks.Remove(chunk);
							phase = 11;
							currentConsumption += golemConsumtion;
							phase = 12;
							currentGolems.Add(newGolem);
						}
					}
				}
			}
			catch (Exception arg)
			{
				summonMechanoids = false;
				Log.Error($"Error while generating golems {this.ToStringSafe()} during phase {phase}: {arg}");
			}
		}

		public static bool TryGetBestGolemKindForSummon(float limit, float consumption, List<PawnKindDef> candidates, List<PawnKindDef> currentGolems, out PawnKindDef golem, out float golemConsumtion)
		{
			golem = null;
			golemConsumtion = 0f;
			foreach (PawnKindDef item in candidates)
			{
				if (currentGolems.Contains(item))
				{
					continue;
				}
				float golemBondReq = item.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost);
				if (limit >= consumption + golemBondReq)
				{
					golem = item;
					golemConsumtion = golemBondReq;
					break;
				}
			}
			if (golem == null && candidates.TryRandomElement((PawnKindDef mech) => limit >= consumption + mech.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost), out PawnKindDef newGolem))
			{
				golem = newGolem;
				golemConsumtion = newGolem.race.GetStatValueAbstract(WVC_GenesDefOf.WVC_GolemBondCost);
			}
			if (golem == null)
			{
				return false;
			}
			return true;
		}

		public bool TryCreateGolemFromThing(Thing chunk, PawnKindDef newGolem)
		{
			try
			{
				if (chunk != null && pawn.CanReserveAndReach(chunk, PathEndMode.OnCell, pawn.NormalMaxDanger()))
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
					pawn.relations.AddDirectRelation(PawnRelationDefOf.Overseer, summon);
					if (chunk.stackCount > 1)
					{
						chunk.stackCount--;
					}
					else
					{
						chunk.Kill();
					}
					Messages.Message("WVC_XaG_GolemCreatedFromRandomChunk_Message".Translate(summon.Name.ToString()), summon, MessageTypeDefOf.PositiveEvent);
					return true;
				}
			}
			catch
			{
				summonMechanoids = false;
				Log.Error("Failed generate golem.");
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref golemsForSummon, "golemsForSummon", LookMode.Def);
		}

	}

	[Obsolete]
	public class Gene_Stonelink : Gene_Golemlink
	{
	}

	public class Gene_Falselink : Gene_Mechlink, IGeneInspectInfo
	{

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

		private void SummonRandomMech()
		{
			int countSpawn = Spawner.summonRange.RandomInRange;
			for (int i = 0; i < countSpawn; i++)
			{
				MechanoidsUtility.MechSummonQuest(pawn, Spawner.summonQuest);
				if (i == 0)
				{
					Messages.Message("WVC_RB_Gene_Summoner".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!Active || Find.Selector.SelectedPawns.Count != 1 || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Summon mechs",
					action = delegate
					{
						SummonRandomMech();
						ResetSummonInterval();
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Get mechs list",
					action = delegate
					{
						List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef)).ToList();
						if (!pawnKindDefs.NullOrEmpty())
						{
							Log.Error("Mechanoids that can be summoned:" + "\n" + pawnKindDefs.Select((PawnKindDef x) => x.defName).ToLineList(" - "));
						}
						else
						{
							Log.Error("Mechanoids list is null");
						}
					}
				};
			}
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_Gene_DustMechlink".Translate() + ": " + GeneUiUtility.OnOrOff(summonMechanoids),
				defaultDesc = "WVC_XaG_Gene_DustMechlinkDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					summonMechanoids = !summonMechanoids;
					if (summonMechanoids)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			};
			yield return command_Action;
		}

		public string GetInspectInfo
		{
			get
			{
				if (summonMechanoids)
				{
					return "WVC_XaG_Gene_Blesslin_On_Info".Translate().Resolve() + ": " + timeForNextSummon.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}

}
