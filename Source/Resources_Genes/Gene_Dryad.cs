using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_DryadQueen : Gene, IGeneInspectInfo, IGeneOverridden
	{

		public GeneExtension_Spawner Spawner => def?.GetModExtension<GeneExtension_Spawner>();

		public GeneExtension_Giver Props => def.GetModExtension<GeneExtension_Giver>();

		private List<Pawn> dryads = new();

		// private PawnKindDef choosedDryadCast = null;

		public List<Pawn> AllDryads => dryads.ToList();

		public int nextTick = 60000;

		public bool spawnDryads = true;

		public override void PostAdd()
		{
			base.PostAdd();
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, Props.bodyparts);
		}

		public override void Tick()
		{
			base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			if (ModsConfig.IdeologyActive)
			{
				SpawnDryad();
			}
			ResetInterval();
		}

		private void SpawnDryad()
		{
			foreach (Pawn item in AllDryads)
			{
				if (item.Dead)
				{
					dryads.Remove(item);
				}
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				spawnDryads = false;
			}
			if (Spawner?.defaultDryadPawnKindDef == null || dryads.Count >= pawn.GetStatValue(Spawner.dryadsStatLimit) || pawn.Map == null || !spawnDryads)
			{
				return;
			}
			Pawn dryad = GenerateNewDryad(Spawner.defaultDryadPawnKindDef, Spawner.defaultDryadThingDef);
			if (dryad == null)
			{
				return;
			}
			GenSpawn.Spawn(dryad, pawn.Position, pawn.Map).Rotation = Rot4.South;
			EffecterDefOf.DryadSpawn.Spawn(pawn.Position, pawn.Map).Cleanup();
			SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
			// Post Spawn Sickness
			if (Spawner.postGestationSickness != null)
			{
				Hediff hediff = HediffMaker.MakeHediff(Spawner.postGestationSickness, pawn);
				pawn.health.AddHediff(hediff);
			}
		}

		public Pawn GenerateNewDryad(PawnKindDef dryadCaste, ThingDef dryadThing)
		{
			if (dryadThing == null)
			{
				return null;
			}
			Pawn dryad = PawnGenerator.GeneratePawn(new PawnGenerationRequest(dryadCaste, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, Gender.Male, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn));
			dryad.def = dryadThing;
			dryad.InitializeComps();
			ResetDryad(dryad);
			CompGauranlenDryad newPawnComp = dryad.TryGetComp<CompGauranlenDryad>();
			// if (newPawnComp == null)
			// {
			// }
			newPawnComp.SetMaster(pawn);
			dryad.connections?.ConnectTo(pawn);
			dryads.Add(dryad);
			return dryad;
		}

		private void ResetDryad(Pawn dryad)
		{
			if (dryad.Faction != pawn?.Faction)
			{
				dryad.SetFaction(pawn?.Faction);
			}
			if (dryad.training == null)
			{
				return;
			}
			foreach (TrainableDef allDef in DefDatabase<TrainableDef>.AllDefs)
			{
				if (dryad.training.CanAssignToTrain(allDef).Accepted)
				{
					dryad.training.SetWantedRecursive(allDef, checkOn: true);
					dryad.training.Train(allDef, pawn, complete: true);
					if (allDef == TrainableDefOf.Release)
					{
						dryad.playerSettings.followDrafted = true;
					}
				}
			}
		}

		public void RemoveDryad(Pawn oldDryad)
		{
			dryads.Remove(oldDryad);
		}

		public void Notify_DryadKilled(Pawn oldDryad, bool gainMemory = false)
		{
			if (pawn.Dead)
			{
				return;
			}
			if (gainMemory && Spawner.dryadDiedMemoryDef != null)
			{
				pawn.needs?.mood?.thoughts?.memories.TryGainMemory(Spawner.dryadDiedMemoryDef);
			}
			RemoveDryad(oldDryad);
		}

		private void ResetInterval()
		{
			nextTick = Spawner.spawnIntervalRange.RandomInRange;
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			// if (!ModsConfig.IdeologyActive)
			// {
				// yield break;
			// }
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SpawnDryad",
					action = delegate
					{
						SpawnDryad();
						ResetInterval();
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
			Command_Action command_Action = new()
			{
				defaultLabel = "WVC_XaG_Gene_GauranlenConnection_SpawnOnOff".Translate() + ": " + GeneUiUtility.OnOrOff(spawnDryads),
				defaultDesc = "WVC_XaG_Gene_GauranlenConnection_SpawnOnOffDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					spawnDryads = !spawnDryads;
					if (spawnDryads)
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

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			base.Notify_PawnDied(dinfo, culprit);
			KillConnectedDryads();
		}

		public override void PostRemove()
		{
			base.PostRemove();
			KillConnectedDryads();
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, Props.bodyparts);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			KillConnectedDryads();
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, Props.bodyparts);
		}

		public void Notify_Override()
		{
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, Props.bodyparts);
		}

		public void KillConnectedDryads()
		{
			for (int i = 0; i < dryads.Count; i++)
			{
				dryads[i].connections?.Notify_ConnectedThingDestroyed(pawn);
				dryads[i].forceNoDeathNotification = true;
				dryads[i].Kill(null, null);
				dryads[i].forceNoDeathNotification = false;
			}
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			StatDef stat = Spawner.dryadsStatLimit;
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat.LabelCap, pawn.GetStatValue(stat).ToString(), stat.description, 200);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextDryad", 0);
			Scribe_Values.Look(ref spawnDryads, "spawnDryads", true);
			Scribe_Collections.Look(ref dryads, "connectedDryads", LookMode.Reference);
		}

		public string GetInspectInfo
		{
			get
			{
				if (spawnDryads && dryads.Count < pawn.GetStatValue(Spawner.dryadsStatLimit, cacheStaleAfterTicks: 30000))
				{
					return "WVC_XaG_Gene_GauranlenConnection_NextDryad_Info".Translate().Resolve() + ": " + nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}


}
