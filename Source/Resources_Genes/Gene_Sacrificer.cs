using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using static UnityEngine.GraphicsBuffer;
using static Verse.HediffCompProperties_RandomizeSeverityPhases;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Sacrificer : Gene_XenogenesEditor, IGeneShapeshifter, IGeneFloatMenuOptions
	{

		public XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;
		private static bool? sacrificerIsActive;

		public static void ResetCache()
		{
			sacrificerIsActive = null;
			devActive = false;
			//metUpdated = false;
		}

		private static bool devActive;

		public static bool IsActive
		{
			get
			{
				if (devActive)
				{
					return true;
				}
				if (sacrificerIsActive == null)
				{
					if (ModsUtility.GameNotStarted())
					{
						return false;
					}
					sacrificerIsActive = MiscUtility.MonolithAndCerebrexCoreCheck();
				}
				return sacrificerIsActive.Value;
			}
		}

		public override bool Active
		{
			get
			{
				if (IsActive)
				{
					return base.Active;
				}
				return false;
			}
		}

		public static SacrificerModeDef forcedSacrificerModeDef;
		public static bool ForcedMode => forcedSacrificerModeDef != null;

		private static List<SacrificerModeDef> cachedModeDefs;
		public static List<SacrificerModeDef> ModeDefs
		{
			get
			{
				if (cachedModeDefs == null)
				{
					cachedModeDefs = DefDatabase<SacrificerModeDef>.AllDefsListForReading;
				}
				return cachedModeDefs;
			}
		}

		private SacrificerModeDef sacrificerModeDef;
		public SacrificerModeDef SacrificerModeDef
		{
			get
			{
				if (forcedSacrificerModeDef != null)
				{
					return forcedSacrificerModeDef;
				}
				if (sacrificerModeDef == null)
				{
					if (ModeDefs.TryRandomElement(mode => mode.initial, out SacrificerModeDef result))
					{
						sacrificerModeDef = result;
					}
					else
					{
						sacrificerModeDef = ModeDefs.RandomElement();
					}
				}
				return sacrificerModeDef;
			}
			set
			{
				sacrificerModeDef = value;
			}
		}

		public override List<GeneDef> GenelineGenes => GameComponent.UnlockedGeneDefs;

		//private string rewardMode = "biotech";
		//public void SetRewardMode(string modeName)
		//{
		//	rewardMode = modeName;
		//}

		//private bool needMore = false;
		//private int delay = 180;

		//private static bool metUpdated = false;

		private static bool update = true;
		public int DefMet
		{
			get
			{
				return GameComponent.sacrificer_defMet;
			}
			set
			{
				GameComponent.sacrificer_defMet = value;
			}
		}

		public void SetMet(int newMet)
		{
			DefMet = newMet;
			update = true;
		}

		public override void TickInterval(int delta)
		{
			if (update)
			{
				def.biostatMet = DefMet;
				update = false;
			}
			//if (needMore)
			//{
			//	delay -= delta;
			//	if (delay <= 0)
			//	{
			//		Sacrifice();
			//		delay = new IntRange(3, 7).RandomInRange * 60;
			//	}
			//}
			//if (!metUpdated)
			//{
			//	metUpdated = true;
			//	def.biostatMet = AllSacrifices;
			//}
			if (pawn.IsHashIntervalTick(12050, delta))
			{
				Sacrifice();
			}
		}

		private Gizmo gizmo;
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: SacrificerDevActive " + XaG_UiUtility.OnOrOff(devActive),
					action = delegate
					{
						devActive = !devActive;
					}
				};
				yield return new Command_Action
				{
					defaultLabel = "DEV: Sacrifice",
					action = delegate
					{
						Sacrifice();
					}
				};
			}
			if (XaG_GeneUtility.SkipIf_SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		private void Sacrifice()
		{
			if (pawn.HomeFaction != Faction.OfPlayer)
			{
				//needMore = false;
				return;
			}
			if (!SacrificerModeDef.Worker.CanFireNow(pawn))
			{
				return;
			}
			IEnumerable<Pawn> allPawns = pawn.MapHeld.mapPawns.AllPawns.Where(sacrifice => sacrifice != pawn);
			foreach (Pawn target in allPawns)
			{
				if (!SacrificerModeDef.Worker.IsAllowed(target))
				{
					continue;
				}
				TrySkipTarget(target);
				break;
			}
			//needMore = allPawns.Any(pawn => pawn.HomeFaction == Faction.OfPlayer);
		}

		private void TrySkipTarget(Pawn target)
		{
			string phase = "init";
			try
			{
				SkipTarget(target, ref phase);
				//metUpdated = false;
			}
			catch (Exception arg)
			{
				Log.Error($"Failed sacrifice target pawn: {target.Name?.ToStringSafe()}. On phase: {phase}. Reason: {arg.Message}");
			}
		}

		private void SkipTarget(Pawn target, ref string phase)
		{
			if (target.HomeFaction == Faction.OfPlayer && PawnUtility.ShouldSendNotificationAbout(target) && !target.forceNoDeathNotification)
			{
				//Find.LetterStack.ReceiveLetter("WVC_Label_SacrificerSkip".Translate(), "WVC_Desc_SacrificerSkip".Translate(target), LetterDefOf.ThreatBig);
				Messages.Message("WVC_Desc_SacrificerSkip".Translate(target), null, MessageTypeDefOf.NegativeEvent, historical: false);
			}
			phase = "check pawn type";
			if (target.IsHuman())
			{
				phase = "human type";
				if (target.IsMutant)
				{
					MetOffset++;
				}
				CpxOffset++;
				bool isArchite = false;
				phase = "unlock pawn genes";
				foreach (Gene gene in target.genes.GenesListForReading)
				{
					if (gene.def.biostatArc != 0)
					{
						isArchite = true;
					}
					GameComponent.UnlockGeneDef(gene.def);
				}
				if (isArchite)
				{
					ArcOffset++;
				}
				phase = "clear pawn xenotype";
				ReimplanterUtility.SetXenotype(target, XenotypeDefOf.Baseliner);
			}
			else
			{
				phase = "non human type";
				if (Rand.Chance(0.05f) || target.RaceProps.IsMechanoid)
				{
					ArcOffset++;
				}
				if (target.RaceProps.IsFlesh)
				{
					MetOffset++;
				}
				CpxOffset++;
			}
			phase = "genesInteractions";
			SacrificerModeDef.Worker.GenesInteractions(this, target);
			IntVec3 postion = target.PositionHeld;
			Map map = target.MapHeld;
			phase = "effects";
			EffectsUtility.DoSkipEffects(postion, map);
			phase = "try kill";
			target.Kill();
			phase = "try destroy corpse";
			target.Corpse?.Destroy();
			phase = "change faction to entity";
			target.SetFactionDirect(Faction.OfEntities);
			//if (!EffectsUtility.SkipBodyFromMap(target))
			//{
			//	EffectsUtility.DoSkipEffects(postion, map);
			//}
			phase = "give reward";
			SpawnReward(postion, map);
		}

		private void SpawnReward(IntVec3 cell, Map map)
		{
			if (!cell.IsValid || map == null)
			{
				cell = pawn.PositionHeld;
				map = pawn.MapHeld;
			}
			if (!cell.IsValid || map == null)
			{
				return;
			}
			SacrificerModeDef.Worker.SpawnReward(pawn, cell, map, this);
		}

		public override void AddGene_Editor(GeneDef geneDef)
		{
			if (def.ConflictsWith(geneDef))
			{
				return;
			}
			pawn.genes.AddGene(geneDef, true);
		}

		public void SelfSactifice(Pawn caller)
		{
			caller.forceNoDeathNotification = true;
			TrySkipTarget(caller);
			caller.forceNoDeathNotification = false;
		}

		public override bool UseGeneline => true;
		public override bool ReqCooldown
		{
			get
			{
				if (Find.Anomaly?.LevelDef == MonolithLevelDefOf.Embraced)
				{
					return false;
				}
				return true;
			}
		}

		//public string RewardName => rewardMode.CapitalizeFirst().Translate();

		public override IntRange ReqMetRange => new(-MetOffset, MetOffset);

		public override int ComplexityLimit => CpxOffset;
		public override int ArchiteLimit => ArcOffset;

		//public int AllSacrifices => humanSacrifices + otherSacrifices;

		public int CpxOffset
		{
			get
			{
				return GameComponent.sacrificer_cpxOffset;
			}
			set
			{
				GameComponent.sacrificer_cpxOffset = value;
			}
		}

		public int ArcOffset
		{
			get
			{
				return GameComponent.sacrificer_arcOffset;
			}
			set
			{
				GameComponent.sacrificer_arcOffset = value;
			}
		}

		public int MetOffset
		{
			get
			{
				return GameComponent.sacrificer_metOffset;
			}
			set
			{
				GameComponent.sacrificer_metOffset = value;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref sacrificerModeDef, "sacrificerModeDef");
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				def.biostatMet = 0;
				update = true;
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			SacrificerModeDef?.Worker?.PostRemove(this);
		}

		public IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (!selPawn.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Deadly))
			{
				yield break;
			}
			if (pawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			if (!SacrificerModeDef.Worker.IsAllowed(selPawn))
			{
				yield return new FloatMenuOption("WVC_Sacrificer_SelfSacrificeWrongModeTip".Translate(selPawn.Named("PAWN")), null);
				yield break;
			}
			//if (selPawn.IsQuestLodger())
			//{
			//	yield return new FloatMenuOption("TemporaryFactionMember".Translate(selPawn.Named("PAWN")), null);
			//	yield break;
			//}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_Sacrificer_SelfSacrifice".Translate(), delegate
			{
				Job job = JobMaker.MakeJob(Extension_Giver.jobDef, pawn);
				selPawn.TryTakeOrderedJob(job, JobTag.Misc, false);
			}), selPawn, pawn);
		}

		public IEnumerable<FloatMenuOption> CompMultiSelectFloatMenuOptions(IEnumerable<Pawn> selPawns)
		{
			if (pawn.IsQuestLodger())
			{
				yield return new FloatMenuOption("TemporaryFactionMember".Translate(pawn.Named("PAWN")), null);
				yield break;
			}
			yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("WVC_Sacrificer_SelfSacrifice".Translate(), delegate
			{
				foreach (Pawn p in selPawns)
				{
					if (!SacrificerModeDef.Worker.IsAllowed(p))
					{
						continue;
					}
					if (!p.CanReach(pawn, PathEndMode.ClosestTouch, Danger.Deadly))
					{
						continue;
					}
					//if (p.IsQuestLodger())
					//{
					//	continue;
					//}
					Job job = JobMaker.MakeJob(Extension_Giver.jobDef, pawn);
					p.TryTakeOrderedJob(job, JobTag.Misc, false);
				}
			}), selPawns.RandomElement(), pawn);
		}

	}

}
