using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_Sacrificer : Gene_XenogenesEditor, IGeneShapeshifter
	{

		public XaG_GameComponent GameComponent => StaticCollectionsClass.GameComponent;
		private static bool? sacrificerIsActive;

		public static void ResetCache()
		{
			sacrificerIsActive = null;
		}

		public static bool IsActive
		{
			get
			{
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

		public override List<GeneDef> GenelineGenes => GameComponent.UnlockedGeneDefs;

		private string rewardMode = "biotech";
		public void SetRewardMode(string modeName)
		{
			rewardMode = modeName;
		}

		public override void TickInterval(int delta)
		{
			if (pawn.IsHashIntervalTick(59050, delta))
			{
				Sacrifice();
			}
		}

		private Gizmo gizmo;
		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (XaG_GeneUtility.SelectorActiveFactionMap(pawn, this))
			{
				yield break;
			}
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Sacrifice",
					action = delegate
					{
						Sacrifice();
					}
				};
			}
			if (gizmo == null)
			{
				gizmo = (Gizmo)Activator.CreateInstance(def.resourceGizmoType, this);
			}
			yield return gizmo;
		}

		private void Sacrifice()
		{
			foreach (Pawn colonist in PawnsFinder.AllMaps_FreeColonistsSpawned)
			{
				if (colonist == pawn)
				{
					continue;
				}
				if (colonist.HomeFaction == Faction.OfPlayer)
				{
					try
					{
						SkipTarget(colonist);
					}
					catch (Exception arg)
					{
						Log.Error($"Failed kill target pawn: {colonist.Name?.ToStringSafe()}. Reason: {arg.Message}");
					}
					return;
				}
			}

			void SkipTarget(Pawn target)
			{
				if (PawnUtility.ShouldSendNotificationAbout(target))
				{
					//Find.LetterStack.ReceiveLetter("WVC_Label_SacrificerSkip".Translate(), "WVC_Desc_SacrificerSkip".Translate(target), LetterDefOf.ThreatBig);
					Messages.Message("WVC_Desc_SacrificerSkip".Translate(target), null, MessageTypeDefOf.NegativeEvent, historical: false);
				}
				if (target.IsHuman())
				{
					if (target.IsMutant)
					{
						otherSacrifices++;
					}
					humanSacrifices++;
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
						architeSacrifices++;
					}
					ReimplanterUtility.SetXenotype(target, XenotypeDefOf.Baseliner);
				}
				else
				{
					otherSacrifices++;
				}
				IntVec3 postion = target.PositionHeld;
				Map map = target.MapHeld;
				target.Kill();
				EffectsUtility.SkipBodyFromMap(target);
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
			switch (rewardMode)
			{
				case "biotech":
					if (Rand.Chance(0.75f) || !HediffUtility.TryAddMechlink(pawn))
					{
						MiscUtility.SpawnItems(ThingDefOf.Steel, 40, cell, map);
						MiscUtility.SpawnItems(ThingDefOf.ComponentIndustrial, 1, cell, map);
					}
					break;
				case "anomaly":
					MiscUtility.SpawnItems(ThingDefOf.Bioferrite, 40, cell, map);
					if (Rand.Chance(0.25f))
					{
						MiscUtility.SpawnItems(ThingDefOf.Shard, 1, cell, map);
					}
					break;
				case "odyssey":
					MiscUtility.SpawnItems(ThingDefOf.Steel, 40, cell, map);
					MiscUtility.SpawnItems(ThingDefOf.GravlitePanel, 10, cell, map);
					if (Rand.Chance(0.5f))
					{
						MiscUtility.SpawnItems(ThingDefOf.ComponentIndustrial, 1, cell, map);
					}
					if (Rand.Chance(0.25f))
					{
						MiscUtility.SpawnItems(ThingDefOf.Gravcore, 1, cell, map);
					}
					break;
				default:
					MiscUtility.SpawnItems(ThingDefOf.Steel, 40, cell, map);
					break;
			}
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

		public string RewardName => rewardMode.CapitalizeFirst().Translate();

		public override IntRange ReqMetRange => new(-otherSacrifices, otherSacrifices);

		public override int ComplexityLimit => humanSacrifices;
		public override int ArchiteLimit => architeSacrifices;

		private int humanSacrifices = 0;
		private int architeSacrifices = 0;
		private int otherSacrifices = 5;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref humanSacrifices, "humanSacrifices", defaultValue: 0);
			Scribe_Values.Look(ref architeSacrifices, "architeSacrifices", defaultValue: 0);
			Scribe_Values.Look(ref otherSacrifices, "otherSacrifices", defaultValue: 5);
			Scribe_Values.Look(ref rewardMode, "rewardMode", defaultValue: "biotech");
		}

	}

}
