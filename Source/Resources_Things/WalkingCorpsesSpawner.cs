using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_WalkingCorpsesSpawner : CompProperties
	{
		public IntRange ticksBetweenSpawn = new(60000, 120000);

		public bool canBeCustomized = false;

		public string uniqueTag = "PlayerMechHive";

		public string inspectString = "WVC_XaG_Label_CompHiveSpawnAnimals";

		public string uiIconAssign = "WVC/UI/XaG_General/Ui_Overseer";

		public string uiIconRand = "WVC/UI/XaG_General/UiRandomize";

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

		public Pawn chosenMechanitor;

		public PawnKindDef chosenWalker;

		public bool alwaysRandomize = true;

		private CompProperties_WalkingCorpsesSpawner Props => (CompProperties_WalkingCorpsesSpawner)props;

		public CompToxifier Toxifier => parent.TryGetComp<CompToxifier>();

		public CompSpawnSubplantDuration Subplant => parent.TryGetComp<CompSpawnSubplantDuration>();

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
			if (!cachedMechanitors.NullOrEmpty())
			{
				if (chosenMechanitor == null || !Props.canBeCustomized)
				{
					chosenMechanitor = cachedMechanitors.RandomElementByWeight((Pawn p) => WalkingUtility.GetLichWeight(p));
				}
				if (chosenWalker == null || !Props.canBeCustomized)
				{
					chosenWalker = Props.summonsList.RandomElement();
				}
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
			ResetCounter();
		}

		public void TryDoSpawn()
		{
			if (cachedMechanitors.NullOrEmpty() || !Props.canBeCustomized)
			{
				ResetMechanitors();
			}
			if (!cachedMechanitors.NullOrEmpty())
			{
				if (chosenMechanitor == null)
				{
					chosenMechanitor = cachedMechanitors.RandomElementByWeight((Pawn p) => WalkingUtility.GetLichWeight(p));
				}
				// Log.Error("2");
				PawnGenerationRequest generateNewPawn = new(chosenWalker, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Newborn);
				Pawn summon = PawnGenerator.GeneratePawn(generateNewPawn);
				// Pawn summon = PawnGenerator.GeneratePawn(Props.summonsList.RandomElement(), Faction.OfPlayer);
				if (WalkingUtility.CanSpawnMoreCorpses(chosenMechanitor, summon))
				{
					IntVec3 intVec = parent.Position.RandomAdjacentCell8Way();
					if (intVec.InBounds(parent.Map) && intVec.Walkable(parent.Map))
					{
						chosenMechanitor.relations.AddDirectRelation(PawnRelationDefOf.Overseer, summon);
						GenSpawn.Spawn(summon, intVec, parent.Map);
						FilthMaker.TryMakeFilth(parent.Position, parent.Map, Props.filthDef);
						SoundDefOf.Hive_Spawn.PlayOneShot(new TargetInfo(parent));
						if (Toxifier != null)
						{
							Toxifier.PolluteNextCell();
						}
						if (Subplant != null)
						{
							Subplant.DoGrowSubplant();
						}
					}
				}
			}
			if (alwaysRandomize || !Props.canBeCustomized)
			{
				chosenMechanitor = cachedMechanitors.RandomElementByWeight((Pawn p) => WalkingUtility.GetLichWeight(p));
				chosenWalker = Props.summonsList.RandomElement();
			}
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new();
			// stringBuilder.AppendLine();
			if (chosenMechanitor != null && Props.canBeCustomized)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", "WVC_XaG_LichTreeCurrentOwnerLabel".Translate().Resolve(), chosenMechanitor.Name.ToStringFull.Colorize(ColoredText.NameColor)));
			}
			if (chosenWalker != null && Props.canBeCustomized)
			{
				stringBuilder.AppendLine(string.Format("{0}: {1}", "WVC_XaG_LichTreeCurrentSpawnLabel".Translate().Resolve(), chosenWalker.race.label.CapitalizeFirst().Colorize(ColorLibrary.LightGreen)));
			}
			stringBuilder.Append(string.Format("{0}", Props.inspectString.Translate((tickCounter).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor))));
			return stringBuilder.ToString();
			// return Props.inspectString.Translate((tickCounter).ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor));
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
			if (!Props.canBeCustomized)
			{
				yield break;
			}
			ResetMechanitors();
			if (cachedMechanitors.NullOrEmpty())
			{
				yield break;
			}
			if (cachedMechanitors.Count > 1)
			{
				yield return new Command_Action
				{
					defaultLabel = "WVC_XaG_ChosenLichLabel".Translate(),
					defaultDesc = "WVC_XaG_ChosenLichDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get(Props.uiIconAssign),
					shrinkable = true,
					action = delegate
					{
						List<FloatMenuOption> list = new();
						for (int i = 0; i < cachedMechanitors.Count; i++)
						{
							Pawn localPawn = cachedMechanitors[i];
							if (localPawn != chosenMechanitor || Find.Selector.SelectedPawns.Count > 1)
							{
								list.Add(new FloatMenuOption(localPawn.Name.ToStringFull, delegate
								{
									chosenMechanitor = localPawn;
									Messages.Message("WVC_XaG_MechanitorIsChosen".Translate(localPawn.Name.ToStringFull, parent.def.label.CapitalizeFirst()), localPawn, MessageTypeDefOf.NeutralEvent, historical: false);
								}));
							}
						}
						if (list.Any())
						{
							Find.WindowStack.Add(new FloatMenu(list));
						}
					}
				};
			}
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_ChosenWalkerLabel".Translate(),
				defaultDesc = "WVC_XaG_ChosenWalkerDesc".Translate(),
				// icon = ContentFinder<Texture2D>.Get(chosenWalker.race.uiIcon),
				icon = chosenWalker.race.uiIcon,
				shrinkable = true,
				action = delegate
				{
					List<FloatMenuOption> list = new();
					List<PawnKindDef> walkers = Props.summonsList;
					for (int i = 0; i < walkers.Count; i++)
					{
						PawnKindDef walker = walkers[i];
						if (WalkingUtility.CanChoseMechPawnKindDef(chosenMechanitor, walker))
						{
							list.Add(new FloatMenuOption(walker.label.CapitalizeFirst() + " | " + "WVC_XaG_WalkerCost".Translate(walker.race.statBases.GetStatValueFromList(WVC_GenesDefOf.WVC_SporesBandwidthCost, 1f).ToString()).Colorize(ColorLibrary.LightGreen), delegate
							{
								chosenWalker = walker;
								Messages.Message("WVC_XaG_WalkerIsChosen".Translate(parent.def.label.CapitalizeFirst(), walker.label.CapitalizeFirst()), null, MessageTypeDefOf.NeutralEvent, historical: false);
							}));
						}
					}
					if (list.Any())
					{
						Find.WindowStack.Add(new FloatMenu(list));
					}
				}
			};
			yield return new Command_Action
			{
				defaultLabel = "WVC_XaG_AlwaysRandomizeLabel".Translate(GeneUiUtility.OnOrOff(alwaysRandomize)),
				defaultDesc = "WVC_XaG_AlwaysRandomizeDesc".Translate(),
				icon = ContentFinder<Texture2D>.Get(Props.uiIconRand),
				shrinkable = true,
				action = delegate
				{
					alwaysRandomize = !alwaysRandomize;
				}
			};
		}

		public void ResetCounter()
		{
			tickCounter = Props.ticksBetweenSpawn.RandomInRange;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref alwaysRandomize, "alwaysRandomize_" + Props.uniqueTag, true);
			Scribe_Values.Look(ref tickCounter, "tickCounterNextSpawn_" + Props.uniqueTag, 0);
			Scribe_Defs.Look(ref chosenWalker, "chosenWalker_" + Props.uniqueTag);
			Scribe_References.Look(ref chosenMechanitor, "chosenMechanitor_" + Props.uniqueTag);
		}
	}

}
