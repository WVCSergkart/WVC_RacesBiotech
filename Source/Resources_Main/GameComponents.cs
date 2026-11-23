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
			StaticCollectionsClass.ResetCollection_GameStart();
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
			XaG_GeneUtility.DevFixes();
		}

		// ========================================

		protected List<string> unlockedXenotypes;

		public void UnlockXenotype(string xenotypeName)
		{
			//if (unlockedXenotypes == null)
			//{
			//	unlockedXenotypes = new();
			//}
			UnlcokedXenotypes.AddSafe(xenotypeName.UncapitalizeFirst());
			//Gene_Energyshifter_XenotypesUnlocker.ResetCollectedCache();
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
			//Gene_Energyshifter_XenotypesUnlocker.ResetCollectedCache();
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
			Scribe_Values.Look(ref sacrificer_cpxOffset, "sacrificer_cpxOffset", defaultValue: 0);
			Scribe_Values.Look(ref sacrificer_arcOffset, "sacrificer_arcOffset", defaultValue: 0);
			Scribe_Values.Look(ref sacrificer_metOffset, "sacrificer_metOffset", defaultValue: 0);
			Scribe_Values.Look(ref sacrificer_defMet, "sacrificer_defMet");
			Scribe_Collections.Look(ref holder_Containers, "holder_Containers", LookMode.Deep);
		}

		// =======================================

		private List<PawnContainerHolder> holder_Containers;
		public virtual List<PawnContainerHolder> HolderContainers
		{
			get
			{
				if (holder_Containers == null)
				{
					holder_Containers = new();
				}
				return holder_Containers;
			}
		}

		// ========================================

		public int sacrificer_cpxOffset = 0;
		public int sacrificer_arcOffset = 0;
		public int sacrificer_metOffset = 0;
		public int sacrificer_defMet = 0;

		// ========================================

		public override void GameComponentTick()
		{
			if (warningTimeout > 0)
			{
				warningTimeout--;
			}
			nextRecache--;
			if (nextRecache > 0)
			{
				return;
			}
			XaG_General();
			ResetCounter(new(40000, 70000));
		}

		private int nextRecache = 33;

		public static int warningTimeout = 0;
		//private int nextSecondRecache = 0;

		public void XaG_General()
		{
			StaticCollectionsClass.UpdateStaticCollection();
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
				if (!summonList.Empty() && MiscUtility.TrySummonDropPod(homeMaps.RandomElement(), summonList))
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

		public void ResetCounter(IntRange interval)
		{
			nextRecache = interval.RandomInRange;
		}

	}

}