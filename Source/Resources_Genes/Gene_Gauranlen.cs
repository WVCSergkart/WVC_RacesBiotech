using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_GauranlenConnection : Gene
	{

		public GeneExtension_Spawner Props => def?.GetModExtension<GeneExtension_Spawner>();

		private List<Pawn> dryads = new();

		// private PawnKindDef choosedDryadCast = null;

		public PawnKindDef DryadKind => currentMode?.pawnKindDef ?? Props?.defaultDryadPawnKindDef;

		public GauranlenGeneModeDef currentMode = null;

		public int nextTick = 60000;

		public override void PostAdd()
		{
			base.PostAdd();
			// choosedDryadCast = Props?.defaultDryadPawnKindDef;
		}

		public override void Tick()
		{
			base.Tick();
			nextTick--;
			if (nextTick > 0)
			{
				return;
			}
			if (!ModsConfig.IdeologyActive)
			{
				return;
			}
			SpawnDryad();
			ResetInterval();
		}

		private void SpawnDryad()
		{
			foreach (Pawn item in dryads.ToList())
			{
				if (item.Dead)
				{
					dryads.Remove(item);
				}
			}
			if (Props?.defaultDryadPawnKindDef == null || dryads.Count >= Props.connectedDryadsLimit || pawn.Map == null)
			{
				return;
			}
			Pawn dryad = GenerateNewDryad(Props.defaultDryadPawnKindDef);
			GenSpawn.Spawn(dryad, pawn.Position, pawn.Map).Rotation = Rot4.South;
			EffecterDefOf.DryadSpawn.Spawn(pawn.Position, pawn.Map).Cleanup();
			SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
		}

		public Pawn GenerateNewDryad(PawnKindDef dryadCaste)
		{
			Pawn dryad = PawnGenerator.GeneratePawn(new PawnGenerationRequest(dryadCaste, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, Gender.Male, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn));
			ResetDryad(dryad);
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

		private void ResetInterval()
		{
			nextTick = Props.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
			{
				yield break;
			}
			if (!ModsConfig.IdeologyActive)
			{
				yield break;
			}
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
			yield return new Command_Action
			{
				defaultLabel = def.LabelCap,
				defaultDesc = "ChangeModeDesc".Translate(),
				icon = currentMode?.pawnKindDef != null ? Widgets.GetIconFor(currentMode.pawnKindDef.race) : ContentFinder<Texture2D>.Get(def.iconPath),
				action = delegate
				{
					Find.WindowStack.Add(new Dialog_ChangeDryadCaste(this));
				}
			};
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

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref dryads, "connectedDryads", LookMode.Reference);
		}

	}


}
