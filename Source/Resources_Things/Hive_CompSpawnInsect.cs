using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompHiveSpawnAnimals : ThingComp
	{
		public int tickCounter = 0;

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
			TryDoSpawn();
			if (Props.resetTimerIfCannotSpawn)
			{
				ResetCounter();
			}
		}

		public void TryDoSpawn()
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
					SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(parent));
				}
				if (!Props.resetTimerIfCannotSpawn)
				{
					ResetCounter();
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
				maxNumber = 20;
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
				return "WVC_XaG_Label_CompHiveSpawnAnimals".Translate((tickCounter).ToStringTicksToPeriod()) + "\n" + Props.inspectString.Translate() + ": " + GetTotalNumber() + "/" + GetMaxNumber();
			}
			return Props.inspectString.Translate() + ": " + GetTotalNumber() + "/" + GetMaxNumber();
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
			// Scribe_Values.Look(ref ticksBetweenSpawn, "ticksBetweenSpawn", 0);
			// Scribe_Values.Look(ref TotalNumber, "TotalNumber", 0);
		}
	}

}
