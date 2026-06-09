using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class XaG_GameComponent : GameComponent
	{

		public override void StartedNewGame()
		{
			//StaticCollectionsClass.currentGameComponent = this;
			//HivemindUtility.ResetSafeCollection();
			StaticCollectionsClass.ResetCollection();
			//HivemindUtility.ResetCollection();
			if (!WVC_Biotech.settings.enable_StartingFoodPolicies)
			{
				return;
			}
			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where((ThingDef x) => x.GetStatValueAbstract(StatDefOf.Nutrition) > 0f).ToList();
			FoodPolicy bloodEaterFoodPolicy = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
			bloodEaterFoodPolicy.label = "WVC_XaG_BloodEaterFoodPolicy".Translate();
			FoodPolicy energyFoodPolicy = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
			energyFoodPolicy.label = "WVC_XaG_EnergyFoodPolicy".Translate();
			FoodPolicy rawMeatFoodPolicy = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
			rawMeatFoodPolicy.label = "WVC_XaG_RawMeatFoodPolicy".Translate();
			foreach (ThingDef item in thingDefs)
			{
				bloodEaterFoodPolicy.filter.SetAllow(item, allow: false);
				energyFoodPolicy.filter.SetAllow(item, allow: false);
				rawMeatFoodPolicy.filter.SetAllow(item, allow: false);
				if (item.IsRawMeat() || item.IsCorpse)
				{
					rawMeatFoodPolicy.filter.SetAllow(item, allow: true);
				}
				if (item.IsHemogenPack())
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
					rawMeatFoodPolicy.filter.SetAllow(item, allow: true);
				}
				if (item.IsDrug)
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
					energyFoodPolicy.filter.SetAllow(item, allow: true);
					rawMeatFoodPolicy.filter.SetAllow(item, allow: true);
				}
				if (item.ingestible?.foodType == FoodTypeFlags.Fluid)
				{
					bloodEaterFoodPolicy.filter.SetAllow(item, allow: true);
				}
			}
			ApparelPolicy thrallApparelPolicy = Current.Game.outfitDatabase.MakeNewOutfit();
			thrallApparelPolicy.label = "WVC_XaG_ThrallOutfitPolicy".Translate();
			thrallApparelPolicy.filter.SetAllow(SpecialThingFilterDefOf.AllowDeadmansApparel, allow: true);
			thrallApparelPolicy.filter.SetAllow(SpecialThingFilterDefOf.AllowNonDeadmansApparel, allow: false);
			//UpdateSavedVersion();
		}

		public Game currentGame;

		public XaG_GameComponent(Game game)
		{
			currentGame = game;
		}

		public override void LoadedGame()
		{
			StaticCollectionsClass.ResetStaticCache_PerSave();
			HivemindUtility.ResetCollection();
			GeneshiftUtility.ResetXenotypesCollection();
			DevFixes();
		}

		// ========================================

		protected List<string> unlockedXenotypes;

		public void UnlockXenotype(string xenotypeName)
		{
			if (unlockedXenotypes == null)
			{
				unlockedXenotypes = new();
			}
			unlockedXenotypes.AddSafe(xenotypeName.UncapitalizeFirst());
		}

		public List<string> UnlcokedXenotypes
		{
			get
			{
				if (unlockedXenotypes == null)
				{
					unlockedXenotypes = ["baseliner"];
				}
				return unlockedXenotypes;
			}
		}

		protected List<string> collectedGeneDefs;
		public void UnlockGeneDef(GeneDef geneDef)
		{
			if (collectedGeneDefs == null)
			{
				collectedGeneDefs = new();
			}
			collectedGeneDefs.AddSafe(geneDef.defName);
			cachedUnlockedGeneDefs = null;
		}

		private List<GeneDef> cachedUnlockedGeneDefs;
		public List<GeneDef> UnlockedGeneDefs
		{
			get
			{
				if (cachedUnlockedGeneDefs == null)
				{
					if (collectedGeneDefs == null)
					{
						collectedGeneDefs = new();
					}
					cachedUnlockedGeneDefs = collectedGeneDefs.ConvertToDefs<GeneDef>();
				}
				return cachedUnlockedGeneDefs;
			}
		}

		public List<ReincarnationSet> reincarnations;

		private bool backup_shouldSummon = false;
		private int backup_GeneCooldown = -1;

		public bool BackupOnCooldown => backup_GeneCooldown > Find.TickManager.TicksGame;
		public int BackupCooldownTicks => backup_GeneCooldown - Find.TickManager.TicksGame;

		public void Backup_InitSummon()
		{
			backup_shouldSummon = true;
			backup_GeneCooldown = Find.TickManager.TicksGame + 1800000;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref reincarnations, "reincarnations", LookMode.Deep);
			Scribe_Values.Look(ref backup_shouldSummon, "backup_shouldSummon", false);
			Scribe_Values.Look(ref backup_GeneCooldown, "backup_GeneCooldown", -1);
			Scribe_Collections.Look(ref unlockedXenotypes, "unlockedXenotypeDefs", LookMode.Value);
			Scribe_Collections.Look(ref collectedGeneDefs, "collectedGeneDefs", LookMode.Value);
		}

		// ========================================

		public override void GameComponentTick()
		{
			nextRecache--;
			if (nextRecache > 0)
			{
				return;
			}
			XaG_General();
			ResetCounter(new(40000, 70000));
		}

		private int nextRecache = 33;
		//private int nextSecondRecache = 0;

		public void XaG_General()
		{
			MiscUtility.UpdateStaticCollection();
			TryReincarnate();
			TrySummon();
		}

		private void TrySummon()
		{
			if (!backup_shouldSummon)
			{
				return;
			}
			if (MechanoidsUtility.CerebrexCoreDefeated)
			{
				backup_shouldSummon = false;
				Messages.Message("WVC_XaG_GeneBackup_CerebrexCoreDefeated_Message".Translate(), MessageTypeDefOf.NegativeEvent);
				return;
			}
			string phase = "";
			try
			{
				phase = "init";
				List<Map> homeMaps = Find.Maps.Where(map => map.IsPlayerHome).ToList();
				if (homeMaps.NullOrEmpty())
				{
					return;
				}
				phase = "reset cache";
				Gene_Backup.ResetCache();
				if (Gene_Backup.BackupPawns.NullOrEmpty())
				{
					return;
				}
				List<Thing> summonList = new();
				phase = "get possible dead pawns";
				foreach (Pawn pawn in Gene_Backup.BackupPawns)
				{
					if (pawn.HomeFaction != Faction.OfPlayer)
					{
						continue;
					}
					if (!pawn.Dead || pawn.Corpse != null)
					{
						continue;
					}
					phase = "try resurrect: " + pawn.Name;
					if (ResurrectionUtility.TryResurrect(pawn))
					{
						phase = "add in summon list: " + pawn.Name;
						summonList.Add(pawn);
					}
				}
				phase = "try summon";
				if (MiscUtility.TrySummonDropPod(homeMaps.RandomElement(), summonList))
				{
					//Find.LetterStack.ReceiveLetter("WVC_XaG_MechanoidSummon_Label".Translate(), "WVC_XaG_MechanoidSummon_Letter".Translate(), LetterDefOf.PositiveEvent, new LookTargets(summonList));
					Messages.Message("WVC_XaG_GeneBackup_Message".Translate(), new LookTargets(summonList), MessageTypeDefOf.PositiveEvent);
				}
				backup_shouldSummon = false;
			}
			catch (Exception arg)
			{
				Log.Error($"Failed summon pawn. On phase {phase}. Reason: {arg.Message}");
			}
		}

		private void TryReincarnate()
		{
			if (reincarnations == null)
			{
				return;
			}
			try
			{
				foreach (ReincarnationSet reincarnation in reincarnations.ToList())
				{
					if (reincarnation.asker == null || reincarnation.questScriptDefs.NullOrEmpty())
					{
						reincarnations.Remove(reincarnation);
						continue;
					}
					if (!reincarnation.asker.Dead)
					{
						reincarnations.Remove(reincarnation);
						continue;
					}
					if (reincarnation.asker.Corpse == null || reincarnation.asker.Corpse.Age > WVC_Biotech.settings.reincarnation_DelayDays * 60000)
					{
						foreach (QuestScriptDef item in reincarnation.questScriptDefs)
						{
							DeathlessUtility.ReincarnationQuest(reincarnation.asker, item);
						}
						reincarnations.Remove(reincarnation);
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error("Failed trigger reincarnation. Reason: " + arg.Message);
			}
			if (reincarnations.Empty())
			{
				reincarnations = null;
			}
		}

		public void DelayRecache(int delay = 1500)
		{
			nextRecache += delay;
		}

		// DEV
		public void DevFixes()
		{
			FixGenesClasses();
			ResetGenes();
			FixGeneAbilities();
		}

		private static void FixGeneAbilities()
		{
			if (WVC_Biotech.settings.fixGeneAbilitiesOnLoad)
			{
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						// List<AbilityDef> fixedAbilities = new();
						Pawn_AbilityTracker abilities = item.abilities;
						if (abilities != null && !abilities.AllAbilitiesForReading.NullOrEmpty())
						{
							foreach (Ability ability in abilities.AllAbilitiesForReading.ToList())
							{
								if (XaG_GeneUtility.AbilityIsGeneAbility(ability))
								{
									abilities.RemoveAbility(ability.def);
									abilities.GainAbility(ability.def);
									Log.Message(item.Name + ": ABILITY FIXED: " + ability.def.label);
									// fixedAbilities.Add(ability.def);
								}
							}
						}
						AddMissingGeneAbilities(item);
					}
				}
				WVC_Biotech.settings.fixGeneAbilitiesOnLoad = false;
			}
		}

		public static void AddMissingGeneAbilities(Pawn item)
		{
			List<AbilityDef> pawnAbilities = MiscUtility.ConvertToDefs(item.abilities.AllAbilitiesForReading);
			foreach (Gene gene in item.genes.GenesListForReading)
			{
				if (!ModsUtility.VanillaExpandedFrameworkActive || !gene.Overridden)
				{
					AddMissingGeneAbilities(item, pawnAbilities, gene);
				}
			}
		}

		public static void AddMissingGeneAbilities(Pawn item, List<AbilityDef> pawnAbilities, Gene gene)
		{
			if (gene.def.abilities.NullOrEmpty() || pawnAbilities == null)
			{
				return;
			}
			foreach (AbilityDef ability in gene.def.abilities)
			{
				if (!pawnAbilities.Contains(ability))
				{
					if (item.mutant?.Def?.abilityWhitelist != null && !item.mutant.Def.abilityWhitelist.Contains(ability))
					{
						continue;
					}
					item.abilities.GainAbility(ability);
				}
			}
		}

		private static void ResetGenes()
		{
			if (WVC_Biotech.settings.resetGenesOnLoad)
			{
				// foreach (Pawn item in currentGame.World.worldPawns.AllPawnsAliveOrDead)
				// List<Pawn> pawns = currentGame.CurrentMap.mapPawns.AllPawns;
				// Log.Error("1");
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						Pawn_GeneTracker genes = item.genes;
						if (!genes.Endogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Endogenes.ToList())
							{
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: false);
							}
							Log.Message(item.Name + ": ENDOGENES FIXED: " + "\n" + genes.Endogenes.Select((Gene x) => x.def.label).ToLineList("	 - ", capitalizeItems: true));
						}
						if (!genes.Xenogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Xenogenes.ToList())
							{
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: true);
							}
							Log.Message(item.Name + ": XENOGENES FIXED: " + "\n" + genes.Xenogenes.Select((Gene x) => x.def.label).ToLineList("	 - ", capitalizeItems: true));
						}
						ReimplanterUtility.PostImplantDebug(item);
					}
				}
				WVC_Biotech.settings.resetGenesOnLoad = false;
			}
		}

		private static void FixGenesClasses()
		{
			if (WVC_Biotech.settings.fixGeneTypesOnLoad)
			{
				foreach (Pawn item in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists.ToList())
				{
					if (item != null && item.RaceProps.Humanlike && item.genes != null)
					{
						Pawn_GeneTracker genes = item.genes;
						if (!genes.Endogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Endogenes.ToList())
							{
								if (gene.GetType() == gene.def.geneClass)
								{
									continue;
								}
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: false);
								Log.Message(item.Name + ": ENDOGENE TYPE FIXED: " + gene.def.defName);
							}
						}
						if (!genes.Xenogenes.NullOrEmpty())
						{
							foreach (Gene gene in genes.Xenogenes.ToList())
							{
								if (gene.GetType() == gene.def.geneClass)
								{
									continue;
								}
								genes.RemoveGene(gene);
								genes.AddGene(gene.def, xenogene: true);
								Log.Message(item.Name + ": XENOGENE TYPE FIXED: " + gene.def.defName);
							}
						}
						ReimplanterUtility.PostImplantDebug(item);
					}
				}
				WVC_Biotech.settings.fixGeneTypesOnLoad = false;
			}
		}

		public void ResetCounter(IntRange interval)
		{
			nextRecache = interval.RandomInRange;
		}

	}

}