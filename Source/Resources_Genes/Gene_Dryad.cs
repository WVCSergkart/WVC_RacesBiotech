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

		// public List<Pawn> AllDryads => dryads.ToList();
		public List<Pawn> DryadsListForReading
		{
			get
			{
				List<Pawn> dryadsForReading = new();
				foreach (Pawn dryad in dryads)
				{
					if (dryad == null || dryad.Dead || dryad.Destroyed)
					{
						continue;
					}
					dryadsForReading.Add(dryad);
				}
				return dryadsForReading;
			}
		}

		public int nextTick = 60000;

		private bool spawnDryads = false;

        public bool SpawnDryads
        {
            get
            {
                return spawnDryads;
            }
			set
            {
				spawnDryads = value;
				//ResetInterval();
			}
        }

        public override void PostAdd()
		{
			base.PostAdd();
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, null);
			ResetInterval();
		}

		public override void TickInterval(int delta)
		{
			//base.Tick();
			if (!spawnDryads)
			{
				return;
			}
			nextTick -= delta;
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
			if (!WVC_Biotech.settings.enable_dryadQueenMechanicGenerator)
            {
				spawnDryads = false;
				return;
            }
            string phase = "start";
            try
			{
				if (dryads == null)
				{
					dryads = new();
				}
				phase = "upd dryads list";
				foreach (Pawn item in dryads.ToList())
				{
					if (item == null || item.Dead || item.Destroyed)
					{
						// dryads.Remove(item);
						RemoveDryad(item);
					}
				}
				if (pawn.Faction != Faction.OfPlayer)
				{
					spawnDryads = false;
				}
				phase = "get count";
				int litterSize = ((pawn.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.litterSizeCurve)));
				if (litterSize < 1)
				{
					litterSize = 1;
				}
				phase = "spawn dryad sequence";
				for (int i = 0; i < litterSize; i++)
				{
					if (pawn.Map == null || Spawner?.defaultDryadPawnKindDef == null || dryads.Count >= pawn.GetStatValue(Spawner.dryadsStatLimit))
					{
						return;
					}
					phase = "generate dryad";
					Pawn dryad = GenerateNewDryad(Spawner.defaultDryadPawnKindDef);
					if (dryad == null)
					{
						return;
					}
					phase = "spawn dryad";
					GenSpawn.Spawn(dryad, pawn.Position, pawn.Map).Rotation = Rot4.South;
					EffecterDefOf.DryadSpawn.Spawn(pawn.Position, pawn.Map).Cleanup();
					SoundDefOf.Pawn_Dryad_Spawn.PlayOneShot(SoundInfo.InMap(dryad));
				}
				// Post Spawn Sickness
				phase = "upd queen hediff";
				if (Spawner.postGestationSickness != null)
				{
					Hediff hediff = HediffMaker.MakeHediff(Spawner.postGestationSickness, pawn);
					pawn.health.AddHediff(hediff);
				}
				UpdHediff();
			}
			catch (Exception arg)
            {
				spawnDryads = false;
				nextTick = 2000;
				Log.Error("Failed spawn dryad. On phase: " + phase + ". Reason: " + arg);
			}
		}

		public void UpdHediff()
		{
			if (pawn.health.hediffSet.TryGetHediff(out HediffWithComps_DryadQueen hediff))
            {
				hediff.Recache();
			}
			else
			{
				HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, null);
			}
			//HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
			//HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, null);
		}

		public Pawn GenerateNewDryad(PawnKindDef dryadCaste)
        {
            // if (dryadThing == null)
            // {
            // return null;
            // }
            Pawn dryad = PawnGenerator.GeneratePawn(new PawnGenerationRequest(dryadCaste, null, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, Gender.Male, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn));
            // dryad.def = dryadThing;
            // dryad.InitializeComps();
            ResetDryad(dryad);
            CompGestatedDryad newPawnComp = dryad.TryGetComp<CompGestatedDryad>();
            if (newPawnComp == null)
            {
                return null;
            }
            newPawnComp.SetMaster(pawn);
            dryad.connections?.ConnectTo(pawn);
            AddDryad(dryad);
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is IGeneDryadQueen geneDryadQueen && gene.Active)
                {
                    geneDryadQueen.Notify_DryadSpawned(dryad);
                }
			}
            //if (dryad?.needs?.rest != null)
            //{
            //    dryad.needs.rest.CurLevel = dryad.needs.rest.MaxLevel;
            //}
            HediffUtility.TryAddOrRemoveHediff(Spawner.initialHediffDef, dryad, this, null);
			Gene_Subhuman.ClearOrSetPawnAsMutantInstantly(dryad, Props.mutantDef);
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

		public void AddDryad(Pawn newDryad)
		{
			if (dryads == null)
			{
				dryads = new();
			}
			if (!dryads.Contains(newDryad))
			{
				dryads.Add(newDryad);
			}
		}

		public void RemoveDryad(Pawn oldDryad)
		{
			if (dryads == null)
			{
				dryads = new();
			}
			if (dryads.Contains(oldDryad))
			{
				dryads.Remove(oldDryad);
			}
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
			UpdHediff();
		}

		private void ResetInterval()
		{
			//nextTick = Spawner.spawnIntervalRange.RandomInRange;
			CompProperties_TreeConnection compProperties_TreeConnection = MainDefOf.Plant_TreeGauranlen?.GetCompProperties<CompProperties_TreeConnection>();
            float basicDays = (compProperties_TreeConnection != null ? compProperties_TreeConnection.spawnDays : 8);
			//float humanGestationDays = pawn.RaceProps.gestationPeriodDays / 2;
			//float ticks = (basicDays / humanGestationDays);
			nextTick = (int)(basicDays * 60000);
		}

		private Gizmo gizmo;

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (!WVC_Biotech.settings.enable_dryadQueenMechanicGenerator)
			{
				yield break;
			}
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
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
				yield return new Command_Action
				{
					defaultLabel = "DEV: GetDryadsList",
					action = delegate
					{
						// List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef randomXenotypeDef) => MechanoidsUtility.MechanoidIsPlayerMechanoid(randomXenotypeDef)).ToList();
						if (!dryads.NullOrEmpty())
						{
							Log.Error("All dryads:" + "\n" + dryads.Select((Pawn x) => x.Name.ToString()).ToLineList(" - "));
						}
						else
						{
							Log.Error("Dryads list is null");
						}
					}
				};
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
			// yield return new Command_Action
			// {
				// defaultLabel = "WVC_XaG_Gene_GauranlenConnection_SpawnOnOff".Translate() + ": " + GeneUiUtility.OnOrOff(spawnDryads),
				// defaultDesc = "WVC_XaG_Gene_GauranlenConnection_SpawnOnOffDesc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(def.iconPath),
				// action = delegate
				// {
					// spawnDryads = !spawnDryads;
					// if (spawnDryads)
					// {
						// SoundDefOf.Tick_High.PlayOneShotOnCamera();
					// }
					// else
					// {
						// SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					// }
				// }
			// };
			// yield return new Command_Action
			// {
				// defaultLabel = "WVC_XaG_DryadQueenSelectAllDryads".Translate(),
				// defaultDesc = "WVC_XaG_DryadQueenSelectAllDryads_Desc".Translate(),
				// icon = ContentFinder<Texture2D>.Get("WVC/UI/XaG_General/UI_SelectDryads"),
				// Order = -87f,
				// action = delegate
				// {
					// Find.Selector.ClearSelection();
					// for (int i = 0; i < dryads.Count; i++)
					// {
						// Find.Selector.Select(dryads[i]);
					// }
				// }
			// };
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
			HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
		}

		public void Notify_OverriddenBy(Gene overriddenBy)
		{
			KillConnectedDryads();
			HediffUtility.TryRemoveHediff(Props.hediffDefName, pawn);
		}

		public void Notify_Override()
		{
			HediffUtility.TryAddOrRemoveHediff(Props.hediffDefName, pawn, this, null);
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
			//UpdHediff();
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			StatDef stat = Spawner.dryadsStatLimit;
			yield return new StatDrawEntry(StatCategoryDefOf.Genetics, stat.LabelCap, pawn.GetStatValue(stat).ToString(), stat.description, 200);
		}

		public bool gizmoCollapse = WVC_Biotech.settings.geneGizmosDefaultCollapse;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextTick, "nextDryad", 0);
			Scribe_Values.Look(ref spawnDryads, "spawnDryads", false);
			Scribe_Values.Look(ref gizmoCollapse, "gizmoCollapse", WVC_Biotech.settings.geneGizmosDefaultCollapse);
			Scribe_Collections.Look(ref dryads, "connectedDryads", LookMode.Reference);
			// if (!dryads.NullOrEmpty())
			// {
				// foreach (Pawn dryad in dryads.ToList())
				// {
					// if (dryad == null)
					// {
						// RemoveDryad(dryad);
					// }
				// }
			// }
			if (dryads != null && dryads.RemoveAll((Pawn x) => x == null || x.Destroyed || x.Dead) > 0)
			{
				Log.Warning("Removed null dryad(s) from gene: " + def.defName);
			}
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
