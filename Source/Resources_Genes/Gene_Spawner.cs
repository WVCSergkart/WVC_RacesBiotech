using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Spawner : Gene_InspectInfo
	{
		// public ThingDef ThingDefToSpawn => def.GetModExtension<GeneExtension_Spawner>().thingDefToSpawn;

		// private int StackCount => def.GetModExtension<GeneExtension_Spawner>().stackCount;

		public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		public int ticksUntilSpawn;

		// For info
		private int cachedStackCount;

		public int FinalStackCount
		{
			get
			{
				if (cachedStackCount != 0)
				{
					return cachedStackCount;
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
			if (pawn.Map != null && Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && Props != null)
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
				cachedStackCount = (int)modifier;
				return cachedStackCount;
			}
			int met = 0;
			foreach (Gene item in genes)
			{
				met += item.def.biostatMet;
			}
			modifier += met * 0.1f;
			int countedStack = (int)(Props.stackCount * modifier);
			cachedStackCount = countedStack <= 1 ? 1 : countedStack;
			return cachedStackCount;
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

		public override string GetInspectInfo()
		{
			return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(Props?.thingDefToSpawn, null, FinalStackCount)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
		}

	}

	[Obsolete]
	public class Gene_SpawnerStuff : Gene
	{

		public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		public int ticksUntilSpawn;
		public ThingDef productDef;

		// For info
		public int FinalStackCount => GetStackCount();
		private int cachedStackCount;
		// For info

		private List<ThingDef> cachedProductDefs;

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
			if (pawn.Map != null && Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && Props != null)
			{
				SpawnItems();
			}
			ResetInterval();
		}

		private void SpawnItems()
		{
			if (productDef == null)
			{
				productDef = Props.thingDefToSpawn;
			}
			if (productDef == null)
			{
				return;
			}
			Thing thing = ThingMaker.MakeThing(productDef);
			thing.stackCount = GetStackModifier();
			GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near, null, null, default);
			if (Props.showMessageIfOwned)
			{
				Messages.Message(Props.spawnMessage.Translate(thing.LabelCap), thing, MessageTypeDefOf.PositiveEvent);
			}
		}

		private int GetStackCount()
		{
			if (cachedStackCount != 0)
			{
				return cachedStackCount;
			}
			// Log.Error("_1");
			return GetStackModifier();
		}

		private int GetStackModifier()
		{
			float modifier = 1f;
			List<Gene> genes = pawn?.genes?.GenesListForReading;
			if (genes.NullOrEmpty())
			{
				cachedStackCount = (int)modifier;
				return cachedStackCount;
			}
			int met = 0;
			foreach (Gene item in genes)
			{
				met += item.def.biostatMet;
			}
			modifier += met * 0.1f;
			int countedStack = (int)(Props.stackCount * modifier);
			cachedStackCount = countedStack <= 1 ? 1 : countedStack;
			if (Props.stuffCategoryDef != null && productDef != null)
			{
				cachedStackCount = (int)(productDef.stackLimit * Props.stackPercent * modifier);
			}
			return cachedStackCount;
		}

		private void ResetInterval()
		{
			ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
			productDef = Props.thingDefToSpawn;
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
			if (!Props.customizable)
			{
				yield break;
			}
			if (Find.Selector.SelectedPawns.Count > 1 || pawn.Drafted || !Active || !MiscUtility.PawnIsColonistOrSlave(pawn, true))
			{
				yield break;
			}
			if (Props.stuffCategoryDef != null)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_XaG_GeneSpawner_CustomButtonLabel".Translate(),
					defaultDesc = "WVC_XaG_GeneSpawner_CustomButtonDesc".Translate(),
					icon = GetIcon(),
					action = delegate
					{
						if (Props.stuffCategoryDef != null && cachedProductDefs.NullOrEmpty())
						{
							cachedProductDefs = MiscUtility.GetAllThingInStuffCategory(Props.stuffCategoryDef);
						}
						List<FloatMenuOption> list = new();
						for (int i = 0; i < cachedProductDefs.Count; i++)
						{
							ThingDef selectedProduct = cachedProductDefs[i];
							// if (selectedProduct != null)
							// {
							// }
							list.Add(new FloatMenuOption(selectedProduct.LabelCap, delegate
							{
								productDef = selectedProduct;
								Messages.Message("WVC_XaG_GeneSpawner_CustomButtonProductChanged".Translate(pawn.Name.ToStringFull, selectedProduct.label), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
							}));
						}
						if (list.Any())
						{
							Find.WindowStack.Add(new FloatMenu(list));
						}
					}
				};
			}
		}

		private Texture2D GetIcon()
		{
			if (productDef != null)
			{
				return productDef.uiIcon;
			}
			return ContentFinder<Texture2D>.Get(def.iconPath);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
			Scribe_Defs.Look(ref productDef, "productDef");
		}
	}

	// Harvester

	// public class Gene_Harvester : Gene
	// {

		// public GeneExtension_Spawner Props => def.GetModExtension<GeneExtension_Spawner>();

		// public int ticksUntilSpawn;

		// private int cachedStackCount;

		// public int FinalStackCount
		// {
			// get
			// {
				// if (cachedStackCount != 0)
				// {
					// return cachedStackCount;
				// }
				// return GetStackModifier();
			// }
		// }

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// ResetInterval();
		// }

		// public override void Tick()
		// {
			// base.Tick();
			// ticksUntilSpawn--;
			// if (ticksUntilSpawn > 0)
			// {
				// return;
			// }
			// if (pawn.Map != null && Active && pawn.Faction != null && pawn.Faction == Faction.OfPlayer && Props != null)
			// {
				// SpawnItems();
			// }
			// ResetInterval();
		// }

		// private void ResetInterval()
		// {
			// ticksUntilSpawn = Props.spawnIntervalRange.RandomInRange;
		// }

		// public override IEnumerable<Gizmo> GetGizmos()
		// {
			// if (DebugSettings.ShowDevGizmos)
			// {
				// yield return new Command_Action
				// {
					// defaultLabel = "DEV: Harvest " + Props.thingDefToSpawn.label,
					// defaultDesc = "NextSpawnedResourceIn".Translate() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor),
					// action = delegate
					// {
						// if (pawn.Map != null && Active)
						// {
							// SpawnItems();
						// }
						// ResetInterval();
					// }
				// };
			// }
		// }

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref ticksUntilSpawn, "ticksToSpawnThing", 0);
		// }
	// }

}
