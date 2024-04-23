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

	public class Gene_GauranlenConnection : Gene, IGeneInspectInfo
	{

		public GeneExtension_Spawner Props => def?.GetModExtension<GeneExtension_Spawner>();

		private List<Pawn> dryads = new();

		// private PawnKindDef choosedDryadCast = null;

		public List<Pawn> AllDryads => dryads.ToList();

		public int nextTick = 60000;

		public bool spawnDryads = true;

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// choosedDryadCast = Props?.defaultDryadPawnKindDef;
		// }

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
			if (Props?.defaultDryadPawnKindDef == null || dryads.Count >= pawn.GetStatValue(Props.dryadsStatLimit) || pawn.Map == null || !spawnDryads)
			{
				return;
			}
			Pawn dryad = GenerateNewDryad(Props.defaultDryadPawnKindDef);
			// CompGauranlenDryad newPawnComp = dryad.TryGetComp<CompGauranlenDryad>();
			// newPawnComp.SetMaster(pawn);
			GenSpawn.Spawn(dryad, pawn.Position, pawn.Map).Rotation = Rot4.South;
			EffecterDefOf.DryadSpawn.Spawn(pawn.Position, pawn.Map).Cleanup();
			SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
			// Post Spawn Sickness
			if (Props.postGestationSickness != null)
			{
				Hediff hediff = HediffMaker.MakeHediff(Props.postGestationSickness, pawn);
				pawn.health.AddHediff(hediff);
			}
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

		public void RemoveDryad(Pawn oldDryad)
		{
			dryads.Remove(oldDryad);
		}

		private void ResetInterval()
		{
			nextTick = Props.spawnIntervalRange.RandomInRange;
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || pawn.Faction != Faction.OfPlayer)
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
			// yield return new Command_Action
			// {
				// defaultLabel = def.LabelCap,
				// defaultDesc = "ChangeModeDesc".Translate(),
				// icon = currentMode?.pawnKindDef != null ? Widgets.GetIconFor(currentMode.pawnKindDef.race) : ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// Find.WindowStack.Add(new Dialog_ChangeDryadCaste(this));
				// }
			// };
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
			StatDef stat = Props.dryadsStatLimit;
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
				if (spawnDryads && dryads.Count < pawn.GetStatValue(Props.dryadsStatLimit))
				{
					return "WVC_XaG_Gene_GauranlenConnection_NextDryad_Info".Translate().Resolve() + ": " + nextTick.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
				}
				return null;
			}
		}

	}


}
