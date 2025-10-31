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
			StaticCollectionsClass.ResetStaticRecacheTick();
			//HivemindUtility.ResetSafeCollection();
			HivemindUtility.ResetCollection();
			//ThoughtWorker_Precept_PreferredXenotype_Social.UpdCollection();
			ThoughtWorker_Precept_Shapeshifter.ResetXenotypesCollection();
			//UpdateSavedVersion();
			//StaticCollectionsClass.currentGameComponent = this;
			DevFixes();
			//StaticCollectionsClass.ResetCollection();
		}

		//public List<ReferencableXenotypeHolder> knownXenotypeDefs = new() { new(XenotypeDefOf.Baseliner) };

		//public bool HasKnownXenotype(XenotypeDef xenotypeDef)
		//{
		//	foreach (ReferencableXenotypeHolder holder in knownXenotypeDefs)
		//	{
		//		if (holder.XenotypeIsSameXenotype(xenotypeDef))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//public bool HasKnownXenotype(CustomXenotype xenotypeDef)
		//{
		//	foreach (ReferencableXenotypeHolder holder in knownXenotypeDefs)
		//	{
		//		if (holder.XenotypeIsSameXenotype(xenotypeDef))
		//		{
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//public void TryUpdateKnownXenotype(Pawn pawn)
		//{
		//	bool addXenotype = true;
		//	foreach (ReferencableXenotypeHolder holder in knownXenotypeDefs)
		//	{
		//		if (holder.PawnIsSameXenotype(pawn))
		//		{
		//			addXenotype = false;
		//		}
		//	}
		//	if (addXenotype)
		//	{
		//		knownXenotypeDefs.Add(new(pawn));
		//	}
		//}

		//public override void ExposeData()
		//{
		//	Scribe_Deep.Look(ref knownXenotypeDefs, "knownXenotypeDefs");
		//	if (knownXenotypeDefs == null)
		//          {
		//		knownXenotypeDefs = new();
		//	}
		//}

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
		private int nextSecondRecache = 0;

		public void XaG_General()
		{
			MiscUtility.UpdateStaticCollection();
			//StaticCollectionsClass.cachedPawnsCount = MiscUtility.CountAllPlayerControlledColonistsExceptClonesAndQuests();
			//StaticCollectionsClass.cachedXenotypesCount = MiscUtility.CountAllPlayerXenos();
			//StaticCollectionsClass.cachedNonHumansCount = MiscUtility.CountAllPlayerNonHumanlikes();
			//Log.Error("Colonists: " + colonists + ". Xenos: " + xenos + ". Non-humans: " + nonHumans);
			nextSecondRecache++;
			if (nextSecondRecache >= 2)
			{
				MiscUtility.ForeverAloneDevelopmentPoints();
				nextSecondRecache = 1;
			}
			//HealingUtility.UpdRegenCollection();
		}

		public void DelayRecache(int delay = 1500)
		{
			nextRecache += delay;
			if (delay > 26666)
			{
				nextSecondRecache++;
			}
		}

		//private string savedModVersion = null;

		//public void UpdateSavedVersion(bool sendLetter = false)
		//{
		//	if (sendLetter && !savedModVersion.Contains(WVC_Biotech.settings.Mod.Content.ModMetaData.ModVersion))
		//          {

		//          }
		//	savedModVersion = WVC_Biotech.settings.Mod.Content.ModMetaData.ModVersion;
		//}

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
				AddMissingGeneAbilities(item, pawnAbilities, gene);
			}
		}

		public static void AddMissingGeneAbilities(Pawn item, List<AbilityDef> pawnAbilities, Gene gene)
		{
			if (gene.def?.abilities != null)
			{
				foreach (AbilityDef ability in gene.def.abilities)
				{
					if (!pawnAbilities.Contains(ability))
					{
						item.abilities.GainAbility(ability);
					}
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