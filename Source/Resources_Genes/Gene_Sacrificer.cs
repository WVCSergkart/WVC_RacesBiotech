using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static UnityEngine.GraphicsBuffer;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Sacrificer : Gene_XenogenesEditor, IGeneShapeshifter
	{

		public XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;
		private static bool? sacrificerIsActive;

		public static void ResetCache()
		{
			sacrificerIsActive = null;
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

		private bool needMore = false;
		private int delay = 180;

		//private static bool metUpdated = false;

		public override void TickInterval(int delta)
		{
			if (needMore)
			{
				delay -= delta;
				if (delay <= 0)
				{
					Sacrifice();
					delay = new IntRange(3, 7).RandomInRange * 60;
				}
			}
			//if (!metUpdated)
			//{
			//	metUpdated = true;
			//	def.biostatMet = AllSacrifices;
			//}
			if (pawn.IsHashIntervalTick(59050, delta))
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
				needMore = false;
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
				try
				{
					SkipTarget(target);
					//metUpdated = false;
				}
				catch (Exception arg)
				{
					Log.Error($"Failed kill target pawn: {target.Name?.ToStringSafe()}. Reason: {arg.Message}");
				}
				break;
			}
			needMore = allPawns.Any(pawn => pawn.HomeFaction == Faction.OfPlayer);

			void SkipTarget(Pawn target)
			{
				if (target.HomeFaction == Faction.OfPlayer && PawnUtility.ShouldSendNotificationAbout(target))
				{
					//Find.LetterStack.ReceiveLetter("WVC_Label_SacrificerSkip".Translate(), "WVC_Desc_SacrificerSkip".Translate(target), LetterDefOf.ThreatBig);
					Messages.Message("WVC_Desc_SacrificerSkip".Translate(target), null, MessageTypeDefOf.NegativeEvent, historical: false);
				}
				if (target.IsHuman())
				{
					if (target.IsMutant)
					{
						metOffset++;
					}
					cpxOffset++;
					bool isArchite = false;
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
						arcOffset++;
					}
					ReimplanterUtility.SetXenotype(target, XenotypeDefOf.Baseliner);
				}
				else
				{
					if (Rand.Chance(0.05f) || target.RaceProps.IsMechanoid)
					{
						arcOffset++;
					}
					if (target.RaceProps.IsFlesh)
					{
						metOffset++;
					}
					cpxOffset++;
				}
				IntVec3 postion = target.PositionHeld;
				Map map = target.MapHeld;
				target.Kill();
				if (!EffectsUtility.SkipBodyFromMap(target))
				{
					EffectsUtility.DoSkipEffects(postion, map);
				}
				SpawnReward(postion, map);
			}
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
			SacrificerModeDef.Worker.SpawnReward(pawn, cell, map);
		}

		public override void AddGene_Editor(GeneDef geneDef)
		{
			if (def.ConflictsWith(geneDef))
			{
				return;
			}
			pawn.genes.AddGene(geneDef, true);
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

		public override IntRange ReqMetRange => new(-metOffset, metOffset);

		public override int ComplexityLimit => cpxOffset;
		public override int ArchiteLimit => arcOffset;

		//public int AllSacrifices => humanSacrifices + otherSacrifices;

		private int cpxOffset = 0;
		private int arcOffset = 0;
		private int metOffset = 0;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cpxOffset, "cpxOffset", defaultValue: 0);
			Scribe_Values.Look(ref arcOffset, "arcOffset", defaultValue: 0);
			Scribe_Values.Look(ref metOffset, "metOffset", defaultValue: 0);
			Scribe_Defs.Look(ref sacrificerModeDef, "sacrificerModeDef");
			//if (Scribe.mode == LoadSaveMode.PostLoadInit)
			//{
			//	metUpdated = false;
			//}
		}

	}

}
