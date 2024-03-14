using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_HiveSpawnAnimals : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(60000, 120000);

		public int maxNumber = 4;

		public int maxLivingThings = 20;

		public int maxNumberOfSpawns = 1;

		public bool ignoreFaction = true;

		public string uniqueTag = "PlayerHive";

		public string inspectString = "WVC_XaG_Label_CompHiveSpawnAnimals_WalkingCorpses";

		public List<PawnKindDef> pawnsList = new();

		public CompProperties_HiveSpawnAnimals()
		{
			compClass = typeof(CompHiveSpawnAnimals);
		}

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string item in base.ConfigErrors(parentDef))
			{
				yield return item;
			}
			if (maxNumberOfSpawns < 0)
			{
				yield return "Invalid maxNumberOfSpawns value. The valid range is zero or more.";
			}
			if (maxLivingThings < maxNumber)
			{
				yield return "Invalid maxLivingThings value. maxLivingThings must be greater than maxNumber.";
			}
			if (parentDef.tickerType == TickerType.Never)
			{
				yield return "has CompHiveSpawnAnimals, but its TickerType is set to Never";
			}
		}
	}

	public class CompHiveSpawnAnimals : ThingComp
	{
		public int tickCounter = 0;
		public int currentNumberOfSpawns = 0;

		// private int ticksBetweenSpawn = 0;

		private CompProperties_HiveSpawnAnimals Props => (CompProperties_HiveSpawnAnimals)props;

		// public override void PostPostMake()
		// {
		// if (parent.def.descriptionHyperlinks == null)
		// {
		// parent.def.descriptionHyperlinks = new List<DefHyperlink>();
		// }
		// foreach (PawnKindDef pawnKindDef in Props.pawnsList)
		// {
		// parent.def.descriptionHyperlinks.Add(new DefHyperlink(pawnKindDef));
		// }
		// }

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
			// if (parent.def.descriptionHyperlinks == null)
			// {
			// parent.def.descriptionHyperlinks = new List<DefHyperlink>();
			// }
			// foreach (PawnKindDef pawnKindDef in Props.pawnsList)
			// {
			// parent.def.descriptionHyperlinks.Add(new DefHyperlink(pawnKindDef));
			// }
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				ResetCounter();
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			Tick(1);
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			Tick(250);
		}

		public override void CompTickLong()
		{
			base.CompTickRare();
			Tick(2000);
		}

		public void Tick(int tick)
		{
			tickCounter -= tick;
			if (tickCounter > 0)
			{
				return;
			}
			if (currentNumberOfSpawns < Props.maxNumberOfSpawns)
			{
				currentNumberOfSpawns++;
			}
			TryDoSpawn();
			ResetCounter();
		}

		public void TryDoSpawn()
		{
			// int trySpawn = currentNumberOfSpawns;
			for (int i = currentNumberOfSpawns; i > 0; i--)
			{
				if (GetTotalNumber() < GetMaxNumber())
				{
					IntVec3 intVec = parent.Position.RandomAdjacentCell8Way();
					if (intVec.InBounds(parent.Map) && intVec.Walkable(parent.Map))
					{
						Pawn newThing = PawnGenerator.GeneratePawn(Props.pawnsList.RandomElement(), Faction.OfPlayer);
						GenSpawn.Spawn(newThing, intVec, parent.Map);
						newThing.ageTracker.AgeBiologicalTicks = 0;
						newThing.ageTracker.AgeChronologicalTicks = 0;
						newThing.ageTracker.PostResolveLifeStageChange();
						FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_Slime);
						WVC_GenesDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(parent));
					}
					currentNumberOfSpawns--;
				}
			}
		}

		public int GetMaxNumber()
		{
			int hiveNumber = 0;
			List<Thing> list = parent.Map.listerThings.ThingsOfDef(parent.def);
			foreach (Thing item in list)
			{
				if (item.Faction == Faction.OfPlayer || Props.ignoreFaction)
				{
					hiveNumber++;
				}
			}
			int maxNumber = Props.maxNumber + hiveNumber;
			if (maxNumber > Props.maxLivingThings)
			{
				maxNumber = Props.maxLivingThings;
			}
			return maxNumber;
		}

		public int GetTotalNumber()
		{
			int TotalNumber = 0;
			foreach (PawnKindDef pawns in Props.pawnsList)
			{
				List<Thing> list = parent.Map.listerThings.ThingsOfDef(pawns.race);
				foreach (Thing item in list)
				{
					if (item.Faction == Faction.OfPlayer)
					{
						TotalNumber++;
					}
				}
			}
			return TotalNumber;
		}

		public override string CompInspectStringExtra()
		{
			if (GetTotalNumber() < GetMaxNumber())
			{
				return "WVC_XaG_Label_CompHiveSpawnAnimals".Translate((tickCounter).ToStringTicksToPeriod()) + "\n" + Props.inspectString.Translate() + ": " + GetTotalNumber() + "/" + GetMaxNumber() + "\n" + "WVC_XaG_Label_CompHiveSpawnAnimals_currentNumberOfSpawns".Translate() + ": " + currentNumberOfSpawns + "/" + Props.maxNumberOfSpawns;
			}
			return Props.inspectString.Translate() + ": " + GetTotalNumber() + "/" + GetMaxNumber() + "\n" + "WVC_XaG_Label_CompHiveSpawnAnimals_currentNumberOfSpawns".Translate() + ": " + currentNumberOfSpawns + "/" + Props.maxNumberOfSpawns;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new()
				{
					defaultLabel = "DEV: Spawn creature",
					action = delegate
					{
						ResetCounter();
						TryDoSpawn();
					}
				};
				yield return command_Action;
				Command_Action command_Action1 = new()
				{
					defaultLabel = "DEV: Reset timer to 0",
					action = delegate
					{
						tickCounter = 0;
					}
				};
				yield return command_Action1;
			}
		}

		public void ResetCounter()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref tickCounter, "tickCounterNextSpawn_" + Props.uniqueTag, 0);
			Scribe_Values.Look(ref currentNumberOfSpawns, "currentNumberOfSpawns" + Props.uniqueTag, 0);
			// Scribe_Values.Look(ref ticksBetweenSpawn, "ticksBetweenSpawn", 0);
			// Scribe_Values.Look(ref TotalNumber, "TotalNumber", 0);
		}
	}

}
