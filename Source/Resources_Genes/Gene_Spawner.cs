using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Spawner : Gene, IGeneInspectInfo
	{
		// public ThingDef ThingDefToSpawn => def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;

		// private int StackCount => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		public int ticksUntilSpawn;

		// For info
		private int? cachedStackCount;

		public int FinalStackCount
		{
			get
			{
				if (cachedStackCount.HasValue)
				{
					return cachedStackCount.Value;
				}
				return GetStackModifier();
			}
		}
		// For info

		public override void PostAdd()
		{
			base.PostAdd();
			ResetInterval();
		}

		public override void Tick()
		{
			base.Tick();
			ticksUntilSpawn--;
			if (ticksUntilSpawn > 0)
			{
				return;
			}
			if (!XaG_GeneUtility.ActiveFactionMap(pawn, this) && Props != null)
			{
				SpawnItems();
			}
			ResetInterval();
		}

		private void SpawnItems()
		{
			MiscUtility.SpawnItems(pawn, Props.thingDefToSpawn, GetStackModifier(), Props.showMessageIfOwned, Props.spawnMessage);
		}

		private int GetStackModifier()
		{
			float modifier = 1f;
			List<Gene> genes = pawn?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				cachedStackCount = Props.stackCount;
				return cachedStackCount.Value;
			}
			int met = 0;
			foreach (Gene item in genes)
			{
				met += item.def.biostatMet;
			}
			if (met > 0f)
			{
				float factor = 1f + (met * 0.1f);
				modifier = factor > 0f ? factor : 0f;
			}
			else if (met < 0f)
			{
				float factor = 1f - ((met * -1) * 0.1f);
				modifier = factor;
			}
			if (modifier == 0)
			{
				cachedStackCount = Props.stackCount;
				return cachedStackCount.Value;
			}
			// modifier += met * 0.1f;
			int countedStack = (int)(Props.stackCount * modifier);
			cachedStackCount = countedStack <= 1 ? 1 : countedStack;
			return cachedStackCount.Value;
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: Spawn " + Props.thingDefToSpawn.label,
					defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					action = delegate
					{
						if (pawn.Map != null && Active)
						{
							SpawnItems();
						}
						ResetInterval();
					}
				};
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		}

		public string GetInspectInfo
		{
			get
			{
				if (pawn.Drafted)
				{
					return null;
				}
				return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(Props?.thingDefToSpawn, null, FinalStackCount)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
			}
		}

	}

	// Harvester

	public class Gene_BloodyGrowths : Gene_HemogenOffset
	{

		public GeneExtension_Spawner Props => def?.GetModExtension<GeneExtension_Spawner>();

		public override void Tick()
		{
			base.Tick();
			if (!pawn.IsHashIntervalTick(54623))
			{
				return;
			}
			if (Hemogen == null || Props == null)
			{
				return;
			}
			// if (XaG_GeneUtility.ActiveFactionMap(pawn, this))
			// {
				// return;
			// }
			if (pawn.Map == null)
			{
				return;
			}
			TrySpawnHemogenPack();
		}

		private bool TrySpawnHemogenPack()
		{
			if (Hemogen.Value >= (Hemogen.Max * Props.matchPercent))
			{
				SpawnItems(pawn, Props.thingDefToSpawn, Props.stackCount, styleDef: Props.styleDef);
				Hemogen.Value -= Props.hemogenPerThing * Props.stackCount;
				SoundDefOf.Execute_Cut.PlayOneShot(pawn);
				GeneFeaturesUtility.TrySpawnBloodFilth(pawn, new(1,2));
				return true;
			}
			return false;
		}

		public static void SpawnItems(Pawn pawn, ThingDef thingDef, int stack, bool showMessage = false, string message = "MessageCompSpawnerSpawnedItem", ThingStyleDef styleDef = null)
		{
			Thing thing = ThingMaker.MakeThing(thingDef);
			thing.stackCount = stack;
			if (styleDef != null)
			{
				thing.SetStyleDef(styleDef);
			}
			GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
			if (showMessage)
			{
				Messages.Message(message.Translate(thing.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
			}
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: ForceSpawn " + Props.thingDefToSpawn.label,
					action = delegate
					{
						if (!TrySpawnHemogenPack())
						{
							Log.Error("Failed spawn hemogen pack. Current hemogen level: " + Hemogen.Value.ToString());
						}
					}
				};
			}
		}

	}

	public class Gene_HorrorPlating : Gene
	{

		public bool horrorSpawned = false;

		public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
		{
			if (!Active)
			{
				return;
			}
			if (ModsConfig.AnomalyActive && !horrorSpawned && pawn.Corpse?.Map != null)
			{
				SpawnMetalhorrorWithoutHediff(pawn);
				horrorSpawned = true;
				MiscUtility.SpawnItems(pawn, ThingDefOf.Bioferrite, (int)((pawn.GetStatValue(StatDefOf.MeatAmount) + pawn.GetStatValue(StatDefOf.LeatherAmount)) * 0.5), false);
				// pawn?.Corpse?.Destroy();
			}
		}

		public static void SpawnMetalhorrorWithoutHediff(Pawn infected)
		{
			if (!ModLister.CheckAnomaly("Metalhorror"))
			{
				return;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Metalhorror, Faction.OfEntities));
			if (!GenAdj.TryFindRandomAdjacentCell8WayWithRoom(infected.SpawnedParentOrMe, out var result))
			{
				result = infected.PositionHeld;
			}
			CompMetalhorror compMetalhorror = pawn.TryGetComp<CompMetalhorror>();
			compMetalhorror.emergedFrom = infected;
			compMetalhorror.implantSource = null;
			int index = 2;
			int num = (int)infected.ageTracker.AgeBiologicalTicks / 2500;
			if (num <= 24)
			{
				index = 0;
			}
			else if (num <= 72)
			{
				index = 1;
			}
			pawn.ageTracker.LockCurrentLifeStageIndex(index);
			pawn.ageTracker.AgeBiologicalTicks = infected.ageTracker.AgeBiologicalTicks;
			pawn.ageTracker.AgeChronologicalTicks = infected.ageTracker.AgeChronologicalTicks;
			Pawn pawn2 = (Pawn)GenSpawn.Spawn(pawn, result, infected.Corpse.MapHeld);
			// compMetalhorror.FindOrCreateEmergedLord();
			Find.BattleLog.Add(new BattleLogEntry_Event(infected, RulePackDefOf.Event_MetalhorrorEmerged, pawn2));
			pawn2.stances.stunner.StunFor(60, null, addBattleLog: false);
			// return pawn2;
			// GeneFeaturesUtility.TrySpawnBloodFilth(pawn2, new(3,4));
			WVC_GenesDefOf.CocoonDestroyed.SpawnAttached(pawn2, pawn2.Map).Trigger(pawn2, null);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref horrorSpawned, "horrorSpawned", false);
		}

	}

}
