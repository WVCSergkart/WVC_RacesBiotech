using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_WalkingCorpsesSpawner : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(60000, 120000);

		public string uniqueTag = "PlayerMechHive";

		public string inspectString = "WVC_XaG_Label_CompHiveSpawnAnimals";

		public List<PawnKindDef> summonsList = new();

		// public ThingDef filthDef = ThingDefOf.Filth_CorpseBile;
		public ThingDef filthDef;

		public CompProperties_WalkingCorpsesSpawner()
		{
			compClass = typeof(CompWalkingCorpsesSpawner);
		}

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string item in base.ConfigErrors(parentDef))
			{
				yield return item;
			}
			if (parentDef.tickerType == TickerType.Never)
			{
				yield return "has CompWalkingCorpsesSpawner, but its TickerType is set to Never";
			}
		}
	}

	public class CompWalkingCorpsesSpawner : ThingComp
	{
		public int tickCounter = 0;

		public List<Pawn> cachedMechanitors = new();

		public Pawn cachedMechanitor;

		private CompProperties_WalkingCorpsesSpawner Props => (CompProperties_WalkingCorpsesSpawner)props;

		public CompToxifier Toxifier => parent.TryGetComp<CompToxifier>();

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			ResetCounter();
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				ResetCounter();
			}
			ResetMechanitors();
		}

		public void ResetMechanitors()
		{
			cachedMechanitors = WalkingUtility.GetAllLichs(parent.Map);
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
			ResetCounter();
		}

		public void TryDoSpawn()
		{
			if (cachedMechanitors.NullOrEmpty())
			{
				// Log.Error("0");
				ResetMechanitors();
			}
			// Log.Error("1");
			if (!cachedMechanitors.NullOrEmpty())
			{
				// Log.Error("2");
				cachedMechanitor = cachedMechanitors.RandomElementByWeight((Pawn p) => WalkingUtility.GetLichWeight(p));
				PawnGenerationRequest generateNewPawn = new(Props.summonsList.RandomElement(), Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
				Pawn summon = PawnGenerator.GeneratePawn(generateNewPawn);
				// Pawn summon = PawnGenerator.GeneratePawn(Props.summonsList.RandomElement(), Faction.OfPlayer);
				if (WalkingUtility.CanSpawnMoreCorpses(cachedMechanitor, summon))
				{
					// Log.Error("3");
					IntVec3 intVec = parent.Position.RandomAdjacentCell8Way();
					if (intVec.InBounds(parent.Map) && intVec.Walkable(parent.Map))
					{
						// summon.relations.AddDirectRelation(PawnRelationDefOf.Overseer, cachedMechanitor);
						cachedMechanitor.relations.AddDirectRelation(PawnRelationDefOf.Overseer, summon);
						// summon.ageTracker.AgeBiologicalTicks = 0;
						// summon.ageTracker.AgeChronologicalTicks = 0;
						// summon.ageTracker.PostResolveLifeStageChange();
						GenSpawn.Spawn(summon, intVec, parent.Map);
						FilthMaker.TryMakeFilth(parent.Position, parent.Map, Props.filthDef);
						SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(parent));
						if (Toxifier != null)
						{
							Toxifier.PolluteNextCell();
						}
					}
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			return Props.inspectString.Translate((tickCounter).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor));
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
		}
	}

}
