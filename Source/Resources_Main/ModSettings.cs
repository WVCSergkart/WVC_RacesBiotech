using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class WVC_BiotechSettings : ModSettings
	{

		public bool firstModLaunch = true;
		// public bool advancedDevMode = false;

		// Main
		// Graphic
		public bool hideXaGGenes = false;
		public bool disableFurGraphic = false;
		public bool enable_FurskinIsSkinAutopatch = false;
		public bool disableAllGraphic = false;
		public bool disableUniqueGeneInterface = false;
		public bool disableEyesGraphic = false;
		public bool useMaskForFurskinGenes = true;
		// Generator
		public bool generateSkillGenes = true;
		public bool generateXenotypeForceGenes = false;
		// Genes
		public bool onlyXenotypesMode = false;
		public bool canNonPlayerPawnResurrect = true;
		public bool totalHealingIgnoreScarification = true;
		public bool enableIncestLoverGene = true;
		public bool disableNonAcceptablePreyGenes = false;
		public bool enable_OverOverridableGenesMechanic = false;
		public bool disableUniqueXenotypeScenarios = false;
		public bool restoreBodyPartsWithFullHP = false;
		public bool thrallMaker_ThrallsInheritMasterGenes = true;
		// Learning
		public bool learningTelepathWorkForBothSides = false;
		public float learning_CyclicallySelfLearning_MaxSkillLevel = 20f;
		// Info
		public bool enable_xagHumanComponent = true;
		public bool enable_StartingFoodPolicies = true;
		// Fix
		public bool harmony_vanillaFixesTweaksAndCompatability = true;
		public bool spawnXenoForcerSerumsFromTraders = true;
		public bool resetGenesOnLoad = false;
		public bool fixGeneAbilitiesOnLoad = false;
		public bool fixGeneTypesOnLoad = false;
		// Gestator
		public bool enable_birthQualityOffsetFromGenes = true;
		public float xenotypeGestator_GestationTimeFactor = 1f;
		public float xenotypeGestator_GestationMatchPercent = 0.4f;
		// Reincarnation
		public float reincarnation_MinChronoAge = 200f;
		public float reincarnation_Chance = 0.12f;
		// Hemogenic
		public bool harmony_EnableGenesMechanicsTriggers = true;
		public bool bloodeater_SafeBloodfeed = false;
		public bool bloodfeeder_AutoBloodfeed = false;
		public float hemogenic_ImplanterFangsChanceFactor = 1f;
		// Thralls
		public float thrallMaker_cooldownOverride = 9f;
		// Invisibility
		public float invisibility_invisBonusTicks = 0;
		// Links
		public bool link_addedMechlinkWithGene = false;
		public bool link_addedPsylinkWithGene = false;
		public float mechlink_HediffFromGeneChance = 0.02f;
		public float psylink_HediffFromGeneChance = 0.02f;
		public float golemnoids_ShutdownRechargePerTick = 0.5f;
		public IntRange golemlink_spawnIntervalRange = new(240000, 420000);
		public IntRange golemlink_golemsToSpawnRange = new(1, 3);
		public IntRange falselink_spawnIntervalRange = new(480000, 960000);
		public IntRange falselink_mechsToSpawnRange = new(1, 6);
		public float voidlink_mechCostFactor = 2f;
		public float voidlink_mechCostLimit = 99f;
		public float voidlink_resourceGainFromMechsFactor = 0.25f;
		public bool voidlink_dynamicResourceLimit = true;
		// Shapeshifter Morpher Archiver Traitshifter
		public float shapeshifer_GeneCellularRegeneration = 1f;
		public float shapeshifer_BaseGenesMatch = 0.7f;
		public float shapeshifer_CooldownDurationFactor = 1f;
		public bool archiver_transferWornApparel = false;
		public bool archiver_transferEquipedWeapon = false;
		public float traitshifter_MaxTraits = 3f;
		// Chimera
		public bool enable_chimeraMetabolismHungerFactor = true;
		public float chimeraStartingGenes = 5f;
		public bool enable_chimeraStartingTools = true;
		public bool enable_chimeraXenogermCD = false;
		public bool enable_chimeraXenogenesLimit = true;
		public IntRange chimera_defaultReqMetabolismRange = new(-99, 99);
		// Duplicator
		public float duplicator_RandomOutcomeChance = 0.66f;
		public float duplicator_RandomGeneChance = 0.12f;
		// Fleshmass
		public float fleshmass_MaxMutationsLevel = 5f;
		public bool fleshmass_HideBodypartHediffs = false;
		// DryadQueen
		public bool enable_dryadQueenMechanicGenerator = true;
		public float gestatedDryads_FilthRateFactor = 0.1f;
		public float gestatedDryads_AnomalyRegeneration = 0f;
		// Xenotypes
		public bool enable_spawnXenotypesInFactions = false;
		public bool disableXenotypes_MainSwitch = false;
		public bool disableXenotypes_Undeads = false;
		public bool disableXenotypes_Psycasters = false;
		public bool disableXenotypes_Mechalike = false;
		public bool disableXenotypes_GolemMasters = false;
		public bool disableXenotypes_Bloodeaters = false;
		public bool disableXenotypes_MutantMakers = false;
		public bool disableXenotypes_Misc = false;
		// Misc
		public bool geneGizmosDefaultCollapse = false;
		public bool showGenesSettingsGizmo = true;
		public bool hideGeneHediffs = false;
		// ExtraSettings
		public bool enable_flatGenesSpawnChances = false;
		public bool offsetMarketPriceFromGenes = false;
		public bool enable_HideMechanitorButtonsPatch = false;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
														 where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
														 select specificSetting.Name;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref firstModLaunch, "firstModLaunch", defaultValue: true, forceSave: true);
			// Scribe_Values.Look(ref advancedDevMode, "advancedDevMode", defaultValue: false);
			// Main
			// Graphic
			Scribe_Values.Look(ref hideXaGGenes, "hideXaGGenes", defaultValue: false);
			Scribe_Values.Look(ref disableFurGraphic, "disableFurGraphic", defaultValue: false);
			Scribe_Values.Look(ref enable_FurskinIsSkinAutopatch, "enable_FurskinIsSkinAutopatch", defaultValue: false);
			Scribe_Values.Look(ref disableAllGraphic, "disableAllGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueGeneInterface, "disableUniqueGeneInterface", defaultValue: false);
			// Scribe_Values.Look(ref enableBodySizeGenes, "enableBodySizeGenes", defaultValue: true);
			Scribe_Values.Look(ref disableEyesGraphic, "disableEyesGraphic", defaultValue: false);
			Scribe_Values.Look(ref useMaskForFurskinGenes, "useMaskForFurskinGenes", defaultValue: true);
			// Generator
			Scribe_Values.Look(ref generateSkillGenes, "generateSkillGenes", defaultValue: true);
			Scribe_Values.Look(ref generateXenotypeForceGenes, "generateXenotypeForceGenes", defaultValue: false);
			//Scribe_Values.Look(ref generateResourceSpawnerGenes, "generateResourceSpawnerGenes", defaultValue: false);
			//Scribe_Values.Look(ref generateSkinHairColorGenes, "generateSkinHairColorGenes", defaultValue: false);
			// Genes
			Scribe_Values.Look(ref onlyXenotypesMode, "onlyXenotypesMode", defaultValue: false);
			Scribe_Values.Look(ref canNonPlayerPawnResurrect, "canNonPlayerPawnResurrect", defaultValue: true);
			//Scribe_Values.Look(ref allowShapeshiftAfterDeath, "allowShapeshiftAfterDeath", defaultValue: true);
			Scribe_Values.Look(ref totalHealingIgnoreScarification, "totalHealingIgnoreScarification", defaultValue: true);
			// Scribe_Values.Look(ref genesRemoveMechlinkUponDeath, "genesRemoveMechlinkUponDeath", defaultValue: false);
			// Scribe_Values.Look(ref enableCustomMechLinkName, "enableCustomMechLinkName", defaultValue: false);
			// Scribe_Values.Look(ref shapeshifterGeneUnremovable, "shapeshifterGeneUnremovable", defaultValue: true);
			Scribe_Values.Look(ref enableIncestLoverGene, "enableIncestLoverGene", defaultValue: true);
			Scribe_Values.Look(ref disableNonAcceptablePreyGenes, "disableNonAcceptablePreyGenes", defaultValue: false);
			//Scribe_Values.Look(ref enableHarmonyTelepathyGene, "enableHarmonyTelepathyGene", defaultValue: false);
			// Scribe_Values.Look(ref useAlternativeDustogenicFoodJob, "useAlternativeDustogenicFoodJob", defaultValue: true);
			Scribe_Values.Look(ref learningTelepathWorkForBothSides, "learningTelepathWorkForBothSides", defaultValue: false);
			Scribe_Values.Look(ref restoreBodyPartsWithFullHP, "restoreBodyPartsWithFullHP", defaultValue: false);
			// Scribe_Values.Look(ref reimplantResurrectionRecruiting, "reimplantResurrectionRecruiting", defaultValue: false);
			Scribe_Values.Look(ref thrallMaker_ThrallsInheritMasterGenes, "thrallMaker_ThrallsInheritMasterGenes", defaultValue: true);
			// Fix
			Scribe_Values.Look(ref harmony_vanillaFixesTweaksAndCompatability, "harmony_vanillaFixesTweaksAndCompatability", defaultValue: true);
			// Scribe_Values.Look(ref minWastepacksPerRecharge, "minWastepacksPerRecharge", defaultValue: false);
			// Scribe_Values.Look(ref validatorAbilitiesPatch, "validatorAbilitiesPatch", defaultValue: true);
			Scribe_Values.Look(ref spawnXenoForcerSerumsFromTraders, "spawnXenoForcerSerumsFromTraders", defaultValue: true);
			// Scribe_Values.Look(ref fixGenesOnLoad, "fixGenesOnLoad", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueXenotypeScenarios, "disableUniqueXenotypeScenarios", defaultValue: false);
			// Info
			Scribe_Values.Look(ref enable_xagHumanComponent, "enable_xagHumanComponent", defaultValue: true);
			Scribe_Values.Look(ref enable_StartingFoodPolicies, "enable_StartingFoodPolicies", defaultValue: true);
			Scribe_Values.Look(ref enable_OverOverridableGenesMechanic, "enable_OverOverridableGenesMechanic", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneSpawnerGizmo, "enableGeneSpawnerGizmo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneWingInfo, "enableGeneWingInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneBlesslinkInfo, "enableGeneBlesslinkInfo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneUndeadInfo, "enableGeneUndeadInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneScarifierInfo, "enableGeneScarifierInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneInstabilityInfo, "enableGeneInstabilityInfo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneGauranlenConnectionInfo, "enableGeneGauranlenConnectionInfo", defaultValue: true);
			// Serums
			// Scribe_Values.Look(ref serumsForAllXenotypes, "serumsForAllXenotypes", defaultValue: false, forceSave: true);
			// Scribe_Values.Look(ref serumsForAllXenotypes_GenBase, "serumsForAllXenotypes_GenBase", defaultValue: true);
			// Scribe_Values.Look(ref serumsForAllXenotypes_GenUltra, "serumsForAllXenotypes_GenUltra", defaultValue: false);
			// Scribe_Values.Look(ref serumsForAllXenotypes_GenHybrid, "serumsForAllXenotypes_GenHybrid", defaultValue: false);
			// Scribe_Values.Look(ref serumsForAllXenotypes_Recipes, "serumsForAllXenotypes_Recipes", defaultValue: true);
			// Scribe_Values.Look(ref serumsForAllXenotypes_Spawners, "serumsForAllXenotypes_Spawners", defaultValue: false);
			Scribe_Values.Look(ref learning_CyclicallySelfLearning_MaxSkillLevel, "learning_CyclicallySelfLearning_MaxSkillLevel", defaultValue: 20f);
			// ExtraSettings
			//Scribe_Values.Look(ref genesCanTickOnlyOnMap, "genesCanTickOnlyOnMap", defaultValue: false);
			Scribe_Values.Look(ref enable_flatGenesSpawnChances, "enable_flatGenesSpawnChances", defaultValue: false);
			Scribe_Values.Look(ref offsetMarketPriceFromGenes, "offsetMarketPriceFromGenes", defaultValue: false);
			Scribe_Values.Look(ref enable_HideMechanitorButtonsPatch, "enable_HideMechanitorButtonsPatch", defaultValue: false);
			// Scribe_Values.Look(ref autoPatchVanillaArchiteImmunityGenes, "autoPatchVanillaArchiteImmunityGenes", defaultValue: false);
			//Scribe_Values.Look(ref enable_ReplaceSimilarGenesAutopatch, "enable_ReplaceSimilarGenesAutopatch", defaultValue: false);
			// Gestator
			Scribe_Values.Look(ref enable_birthQualityOffsetFromGenes, "enable_birthQualityOffsetFromGenes", defaultValue: true);
			Scribe_Values.Look(ref xenotypeGestator_GestationTimeFactor, "xenotypeGestator_GestationTimeFactor", defaultValue: 1f);
			Scribe_Values.Look(ref xenotypeGestator_GestationMatchPercent, "xenotypeGestator_GestationMatchPercent", defaultValue: 0.4f);
			// Reincarnation
			//Scribe_Values.Look(ref reincarnation_EnableMechanic, "reincarnation_EnableMechanic", defaultValue: true);
			Scribe_Values.Look(ref reincarnation_MinChronoAge, "reincarnation_MinChronoAge", defaultValue: 200f);
			Scribe_Values.Look(ref reincarnation_Chance, "reincarnation_Chance", defaultValue: 0.12f);
			// Hemogenic
			Scribe_Values.Look(ref harmony_EnableGenesMechanicsTriggers, "harmony_EnableGenesMechanicsTriggers", defaultValue: true);
			Scribe_Values.Look(ref bloodeater_SafeBloodfeed, "bloodeater_SafeBloodfeed", defaultValue: false);
			//Scribe_Values.Look(ref bloodeater_disableAutoFeed, "bloodeater_disableAutoFeed", defaultValue: false);
			Scribe_Values.Look(ref bloodfeeder_AutoBloodfeed, "bloodfeeder_AutoBloodfeed", defaultValue: false);
			Scribe_Values.Look(ref hemogenic_ImplanterFangsChanceFactor, "hemogenic_ImplanterFangsChanceFactor", defaultValue: 1f);
			// Thralls
			Scribe_Values.Look(ref thrallMaker_cooldownOverride, "thrallMaker_cooldownOverride", defaultValue: 9f);
			Scribe_Values.Look(ref invisibility_invisBonusTicks, "invisibility_invisDurationFactor", defaultValue: 0f);
			//Scribe_Values.Look(ref enableInstabilityLastChanceMechanic, "enableInstabilityLastChanceMechanic", defaultValue: true);
			// Links
			Scribe_Values.Look(ref link_addedMechlinkWithGene, "link_addedMechlinkWithGene", defaultValue: false);
			Scribe_Values.Look(ref link_addedPsylinkWithGene, "link_addedPsylinkWithGene", defaultValue: false);
			Scribe_Values.Look(ref mechlink_HediffFromGeneChance, "mechlink_HediffFromGeneChance", defaultValue: 0.02f);
			Scribe_Values.Look(ref psylink_HediffFromGeneChance, "psylink_HediffFromGeneChance", defaultValue: 0.02f);
			//Scribe_Values.Look(ref link_removeMechlinkWithGene, "link_removeMechlinkWithGene", defaultValue: false);
			//Scribe_Values.Look(ref link_removePsylinkWithGene, "link_removePsylinkWithGene", defaultValue: false);
			Scribe_Values.Look(ref golemnoids_ShutdownRechargePerTick, "golemnoids_ShutdownRechargePerTick", defaultValue: 0.5f);
			//Scribe_Values.Look(ref golembond_ShrinesStatPartOffset, "golembond_ShrinesStatPartOffset", defaultValue: false);
			Scribe_Values.Look(ref golemlink_spawnIntervalRange, "golemlink_spawnIntervalRange", defaultValue: new(240000, 420000));
			Scribe_Values.Look(ref golemlink_golemsToSpawnRange, "golemlink_golemsToSpawnRange", defaultValue: new(1, 3));
			Scribe_Values.Look(ref falselink_spawnIntervalRange, "falselink_spawnIntervalRange", defaultValue: new(480000, 960000));
			Scribe_Values.Look(ref falselink_mechsToSpawnRange, "falselink_mechsToSpawnRange", defaultValue: new(1, 6));
			Scribe_Values.Look(ref voidlink_mechCostFactor, "voidlink_mechCostFactor", defaultValue: 2f);
			Scribe_Values.Look(ref voidlink_mechCostLimit, "voidlink_mechCostLimit", defaultValue: 99f);
			Scribe_Values.Look(ref voidlink_resourceGainFromMechsFactor, "voidlink_resourceGainFromMechsFactor", defaultValue: 0.25f);
			Scribe_Values.Look(ref voidlink_dynamicResourceLimit, "voidlink_dynamicResourceLimit", defaultValue: true);
			// shapeshifter
			Scribe_Values.Look(ref shapeshifer_GeneCellularRegeneration, "shapeshifer_GeneCellularRegeneration", defaultValue: 1f);
			Scribe_Values.Look(ref shapeshifer_BaseGenesMatch, "shapeshifer_BaseGenesMatch", defaultValue: 0.7f);
			Scribe_Values.Look(ref shapeshifer_CooldownDurationFactor, "shapeshifer_CooldownDurationFactor", defaultValue: 1f);
			//Scribe_Values.Look(ref enable_MorpherExperimentalMode, "enable_MorpherExperimentalMode", defaultValue: false);
			Scribe_Values.Look(ref archiver_transferWornApparel, "archiver_transferWornApparel", defaultValue: false);
			Scribe_Values.Look(ref archiver_transferEquipedWeapon, "archiver_transferEquipedWeapon", defaultValue: false);
			// Scribe_Values.Look(ref shapeshifter_enableStyleButton, "shapeshifter_enableStyleButton", defaultValue: true);
			Scribe_Values.Look(ref enable_spawnXenotypesInFactions, "enable_spawnXenotypesInFactions", defaultValue: false);
			Scribe_Values.Look(ref traitshifter_MaxTraits, "traitshifter_MaxTraits", defaultValue: 3f);
			// Chimera
			Scribe_Values.Look(ref enable_chimeraMetabolismHungerFactor, "enable_chimeraMetabolismHungerFactor", defaultValue: true);
			Scribe_Values.Look(ref chimera_defaultReqMetabolismRange, "chimera_defaultReqMetabolismRange", defaultValue: new(-99, 99));
			//Scribe_Values.Look(ref chimeraMinGeneCopyChance, "chimeraMinGeneCopyChance", defaultValue: 0.35f);
			Scribe_Values.Look(ref chimeraStartingGenes, "chimeraStartingGenes", defaultValue: 5f);
			Scribe_Values.Look(ref enable_chimeraStartingTools, "enable_chimeraStartingTools", defaultValue: true);
			Scribe_Values.Look(ref enable_chimeraXenogermCD, "enable_chimeraXenogermCD", defaultValue: false);
			Scribe_Values.Look(ref enable_chimeraXenogenesLimit, "enable_chimeraXenogenesLimit", defaultValue: true);
			// Duplicator
			Scribe_Values.Look(ref duplicator_RandomOutcomeChance, "duplicator_RandomOutcomeChance", defaultValue: 0.66f);
			Scribe_Values.Look(ref duplicator_RandomGeneChance, "duplicator_RandomGeneChance", defaultValue: 0.12f);
			// Fleshmass
			Scribe_Values.Look(ref fleshmass_MaxMutationsLevel, "fleshmass_MaxMutationsLevel", defaultValue: 5f);
			Scribe_Values.Look(ref fleshmass_HideBodypartHediffs, "fleshmass_HideBodypartHediffs", defaultValue: false);
			// DryadQueen
			Scribe_Values.Look(ref enable_dryadQueenMechanicGenerator, "enable_dryadQueenMechanicGenerator", defaultValue: true);
			Scribe_Values.Look(ref gestatedDryads_FilthRateFactor, "gestatedDryads_FilthRateFactor", defaultValue: 0.1f);
			Scribe_Values.Look(ref gestatedDryads_AnomalyRegeneration, "gestatedDryads_AnomalyRegeneration", defaultValue: 0f);
			// Rechargeable
			//Scribe_Values.Look(ref rechargeable_enablefoodPoisoningFromFood, "rechargeable_enablefoodPoisoningFromFood", defaultValue: true);
			// Reincarnation
			Scribe_Values.Look(ref disableXenotypes_MainSwitch, "disableXenotypes_MainSwitch", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Undeads, "disableXenotypes_Undeads", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Psycasters, "disableXenotypes_Psycasters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Mechalike, "disableXenotypes_Mechalike", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_GolemMasters, "disableXenotypes_GolemMasters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Bloodeaters, "disableXenotypes_Bloodeaters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_MutantMakers, "disableXenotypes_MutantMakers", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Misc, "disableXenotypes_Misc", defaultValue: false);
			// PregnantHuman
			// Scribe_Values.Look(ref pregnantHuman_TrueParentGenesPatch, "pregnantHuman_TrueParentGenesPatch", defaultValue: false);
			// Scribe_Values.Look(ref pregnantHuman_InheritSurrogateGenes, "pregnantHuman_InheritSurrogateGenes", defaultValue: false);
			// Scribe_Values.Look(ref pregnantHuman_InheritArchiteGenes, "pregnantHuman_InheritArchiteGenes", defaultValue: false);
			Scribe_Values.Look(ref geneGizmosDefaultCollapse, "geneGizmosDefaultCollapse", defaultValue: false);
			Scribe_Values.Look(ref showGenesSettingsGizmo, "showGenesSettingsGizmo", defaultValue: true);
			Scribe_Values.Look(ref hideGeneHediffs, "hideGeneHediffs", defaultValue: false);
			// End
			Scribe_Collections.Look(ref WVC_Biotech.cachedXenotypesFilter, "cachedXenotypesFilter", LookMode.Value, LookMode.Value);
		}
	}

	public class WVC_Biotech : Mod
	{
		public static WVC_BiotechSettings settings;

		private int PageIndex = 0;

		private static Vector2 scrollPosition = Vector2.zero;

		public WVC_Biotech(ModContentPack content) : base(content)
		{
			settings = GetSettings<WVC_BiotechSettings>();
			//new Harmony("wvc.sergkart.races.biotech").PatchAll();
			HarmonyPatches.HarmonyUtility.HarmonyPatches();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Rect rect = new(inRect);
			rect.y = inRect.y + 40f;
			// Rect baseRect = rect;
			rect = new Rect(inRect)
			{
				height = inRect.height - 40f,
				y = inRect.y + 40f
			};
			// Rect rect2 = rect;
			Widgets.DrawMenuSection(rect);
			List<TabRecord> tabs = new()
			{
				new TabRecord("WVC_BiotechSettings_Tab_General".Translate(), delegate
				{
					PageIndex = 0;
					WriteSettings();
				}, PageIndex == 0),
				new TabRecord("WVC_BiotechSettings_Label_Genes".Translate(), delegate
				{
					PageIndex = 1;
					WriteSettings();
				}, PageIndex == 1),
				new TabRecord("WVC_BiotechSettings_Tab_XenotypesSettings".Translate(), delegate
				{
					PageIndex = 2;
					WriteSettings();
				}, PageIndex == 2),
				new TabRecord("WVC_BiotechSettings_Tab_XenotypesFilter".Translate(), delegate
				{
					PageIndex = 3;
					WriteSettings();
				}, PageIndex == 3),
				new TabRecord("WVC_BiotechSettings_Tab_ExtraSettings".Translate(), delegate
				{
					PageIndex = 4;
					WriteSettings();
				}, PageIndex == 4)
			};
			TabDrawer.DrawTabs(rect, tabs);
			switch (PageIndex)
			{
				case 0:
					GeneralSettings(rect.ContractedBy(15f));
					break;
				case 1:
					GenesSettings(rect.ContractedBy(15f));
					break;
				case 2:
					XenotypesSettings(rect.ContractedBy(15f));
					break;
				case 3:
					XenotypeFilterSettings(rect.ContractedBy(15f));
					break;
				case 4:
					ExtraSettings(rect.ContractedBy(15f));
					break;
			}
		}

		public override string SettingsCategory()
		{
			return "WVC - " + "WVC_BiotechSettings".Translate();
		}

		// General Settings
		// General Settings
		// General Settings

		private void GeneralSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			// Rect rect = new(0f, 0f, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// Main
			// listingStandard.Label("WVC_BiotechSettings_Label_Graphics".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Graphics".Translate());
			// Graphic
			listingStandard.Label("WVC_BiotechSettings_Label_Graphics".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Graphics".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableFurGraphic".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableFurGraphic, "WVC_ToolTip_disableFurGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enable_FurskinIsSkinAutopatch".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enable_FurskinIsSkinAutopatch, "WVC_ToolTip_enable_FurskinIsSkinAutopatch".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableAllGraphic".Translate(), ref settings.disableAllGraphic, "WVC_ToolTip_disableAllGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableEyesGraphic".Translate(), ref settings.disableEyesGraphic, "WVC_ToolTip_disableEyesGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_useMaskForFurskinGenes".Translate(), ref settings.useMaskForFurskinGenes, "WVC_ToolTip_useMaskForFurskinGenes".Translate());
			// UI
			listingStandard.Gap();
			listingStandard.Label("WVC_BiotechSettings_Label_UI".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_UI".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_hideXaGGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.hideXaGGenes, "WVC_ToolTip_hideXaGGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableUniqueGeneInterface".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableUniqueGeneInterface, "WVC_ToolTip_disableUniqueGeneInterface".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_GeneGizmosDefaultCollapse".Translate().Colorize(ColorLibrary.LightBlue), ref settings.geneGizmosDefaultCollapse, "WVC_ToolTip_GeneGizmosDefaultCollapse".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_showGenesSettingsGizmo".Translate().Colorize(ColorLibrary.LightBlue), ref settings.showGenesSettingsGizmo, "WVC_ToolTip_showGenesSettingsGizmo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_hideGeneHediffs".Translate().Colorize(ColorLibrary.LightBlue), ref settings.hideGeneHediffs, "WVC_ToolTip_hideGeneHediffs".Translate());
			//listingStandard.CheckboxLabeled("WVC_Label_enableBodySizeGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableBodySizeGenes, "WVC_ToolTip_enableBodySizeGenes".Translate());
			// Info
			// listingStandard.Gap();
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneSpawnerGizmo".Translate(), ref settings.enableGeneSpawnerGizmo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneWingInfo".Translate(), ref settings.enableGeneWingInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneBlesslinkInfo".Translate(), ref settings.enableGeneBlesslinkInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneUndeadInfo".Translate(), ref settings.enableGeneUndeadInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneScarifierInfo".Translate(), ref settings.enableGeneScarifierInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneInstabilityInfo".Translate(), ref settings.enableGeneInstabilityInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableGeneGauranlenConnectionInfo".Translate(), ref settings.enableGeneGauranlenConnectionInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.Gap();
			//listingStandard.Label("WVC_BiotechSettings_Label_Generators".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Generators".Translate());
			//listingStandard.Gap();
			// Misc
			listingStandard.Label("WVC_BiotechSettings_Label_Other".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Other".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_generateSkillGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateSkillGenes, "WVC_ToolTip_generateTemplateGenes_Aptitudes".Translate());
			if (settings.generateXenotypeForceGenes || Prefs.DevMode)
			{
				listingStandard.CheckboxLabeled("WVC_Label_generateXenotypeForceGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateXenotypeForceGenes, "WVC_ToolTip_generateXenoForcerGenes".Translate());
			}
			// Outdated. No longer supported and maintained
			//if (settings.generateResourceSpawnerGenes)
			//{
			//	listingStandard.CheckboxLabeled("WVC_Label_generateResourceSpawnerGenes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.generateResourceSpawnerGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
			//}
			//if (settings.generateSkinHairColorGenes)
			//{
			//	listingStandard.CheckboxLabeled("WVC_Label_generateSkinHairColorGenes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.generateSkinHairColorGenes, "WVC_ToolTip_generateSkinHairColorGenes".Translate());
			//}
			listingStandard.CheckboxLabeled("WVC_Label_fixVanillaGeneImmunityCheck".Translate().Colorize(ColorLibrary.LightPurple), ref settings.harmony_vanillaFixesTweaksAndCompatability, "WVC_ToolTip_fixVanillaGeneImmunityCheck".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_spawnXenoForcerSerumsFromTraders".Translate(), ref settings.spawnXenoForcerSerumsFromTraders, "WVC_ToolTip_spawnXenoForcerSerumsFromTraders".Translate());
			listingStandard.GapLine();
			// Serums
			// if (settings.serumsForAllXenotypes)
			// {
			// listingStandard.Label("WVC_BiotechSettings_Label_Serums".Translate().Colorize(ColoredText.SubtleGrayColor) + ":", -1, "WVC_BiotechSettings_Tooltip_Serums".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes, "WVC_ToolTip_serumsForAllXenotypes".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenBase".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenBase, "WVC_ToolTip_serumsForAllXenotypes_GenBase".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenUltra".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenUltra, "WVC_ToolTip_serumsForAllXenotypes_GenUltra".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenHybrid".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenHybrid, "WVC_ToolTip_serumsForAllXenotypes_GenHybrid".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_Recipes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Recipes, "WVC_ToolTip_serumsForAllXenotypes_Recipes".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_serumsSpawnersForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Spawners, "WVC_ToolTip_serumsSpawnersForAllXenotypes".Translate());
			// listingStandard.GapLine();
			// }
			// =============== Buttons ===============
			ResetButton(listingStandard);
			// =============== Buttons ===============
			listingStandard.End();
			Widgets.EndScrollView();
		}

		// Xenotype Filter Settings
		// Xenotype Filter Settings
		// Xenotype Filter Settings

		private string searchKey;

		public static Dictionary<string, bool> cachedXenotypesFilter = new();
		public static List<XenotypeDef> allXenotypes = new();

		private void XenotypeFilterSettings(Rect inRect)
		{
			if (allXenotypes.NullOrEmpty())
			{
				// XaG_PostInitialization.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
				InitialUtility.SetValues();
				//allXenotypes = ListsUtility.GetWhiteListedXenotypes(false);
				return;
			}
			var rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
			Text.Anchor = TextAnchor.MiddleLeft;
			var searchLabel = new Rect(rect.x + 5, rect.y, 60, 24);
			Widgets.Label(searchLabel, "WVC_BiotechSettings_XenotypesFilter_Search".Translate());
			var searchRect = new Rect(searchLabel.xMax + 5, searchLabel.y, 200, 24f);
			searchKey = Widgets.TextField(searchRect, searchKey);
			Text.Anchor = TextAnchor.UpperLeft;

			IEnumerable<XenotypeDef> searchXenotypes;
			if (!searchKey.NullOrEmpty())
			{
				searchXenotypes = allXenotypes.Where((XenotypeDef x) => x.label.ToLower().Contains(searchKey.ToLower()));
			}
			else
			{
				IEnumerable<XenotypeDef> enumerable = allXenotypes;
				searchXenotypes = enumerable;
			}
			List<XenotypeDef> list = (from x in searchXenotypes
									  orderby x.label
									  select x).ToList();

			// Log.Error("0");
			var resetRect = new Rect(searchLabel.x, searchLabel.yMax + 5, 265, 24f);
			if (Widgets.ButtonText(resetRect, "WVC_BiotechSettings_XenotypesFilter_Reset".Translate()))
			{
				cachedXenotypesFilter.Clear();
				InitialUtility.SetValues();
			}

			var explanationTitleRect = new Rect(resetRect.xMax + 15, resetRect.y, inRect.width - (resetRect.width + 35), 24f);
			Widgets.Label(explanationTitleRect, "WVC_BiotechSettings_XenotypesFilter_Title".Translate());

			// Log.Error("1");
			float height = GetScrollHeight(list);
			var outerRect = new Rect(rect.x, searchRect.yMax + 35, rect.width, rect.height - 70);
			var viewArea = new Rect(rect.x, outerRect.y, rect.width - 16, height);
			// Log.Error("2");
			Widgets.BeginScrollView(outerRect, ref scrollPosition, viewArea);
			var outerPos = new Vector2(rect.x + 5, outerRect.y);
			float num = 0;
			int entryHeight = 200;
			// Log.Error("3");
			foreach (XenotypeDef def in list)
			{
				bool canDrawGroup = num >= scrollPosition.y - entryHeight && num <= (scrollPosition.y + outerRect.height);
				float curNum = outerPos.y;
				if (canDrawGroup)
				{
					// Log.Error("1");
					var infoRect = new Rect(outerPos.x + 5, outerPos.y + 5, 24, 24);
					Widgets.InfoCardButton(infoRect, def);
					var iconRect = new Rect(infoRect.xMax + 5, outerPos.y + 5, 24, 24);
					Widgets.DefIcon(iconRect, def);
					// Widgets.HyperlinkWithIcon(iconRect, new Dialog_InfoCard.Hyperlink(def));
					// Log.Error("2");
					var labelRect = new Rect(iconRect.xMax + 15, outerPos.y + 5, viewArea.width - 85, 24f);
					Widgets.DrawHighlightIfMouseover(labelRect);
					// Widgets.Label(labelRect, def.LabelCap);
					// Log.Error("3");
					// var pos = new Vector2(viewArea.width - 40, labelRect.y);
					// Log.Error("4");
					bool value = !cachedXenotypesFilter.TryGetValue(def.defName, out bool canSpawn) || canSpawn;
					// bool value = cachedXenotypesFilter[def.defName];
					Widgets.CheckboxLabeled(labelRect, def.LabelCap, ref value);
					// Widgets.InfoCardButton(pos, def);
					// bool value = true;
					// Widgets.Checkbox(pos, ref value);
					// Log.Error("5");
					cachedXenotypesFilter[def.defName] = value;
				}
				var innerPos = new Vector2(outerPos.x + 10, outerPos.y);
				outerPos.y += 24;
				num += outerPos.y - curNum;
			}
			// Log.Error("4");
			Widgets.EndScrollView();
		}

		private float GetScrollHeight(List<XenotypeDef> defs)
		{
			float num = 0;
			foreach (var def in defs)
			{
				num += 24;
			}
			return num + 5;
		}

		// Extra Settings
		// Extra Settings
		// Extra Settings

		public void ExtraSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			// Rect rect = new(0f, 0f, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// Extra
			listingStandard.Label("WVC_BiotechSettings_Label_Genes".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Genes".Translate());
			//listingStandard.CheckboxLabeled("WVC_Label_genesCanTickOnlyOnMap".Translate().Colorize(ColorLibrary.LightPurple), ref settings.genesCanTickOnlyOnMap, "WVC_ToolTip_genesCanTickOnlyOnMap".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_flatGenesSpawnChances".Translate(), ref settings.enable_flatGenesSpawnChances, "WVC_ToolTip_flatGenesSpawnChances".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_offsetMarketPriceFromGenes".Translate(), ref settings.offsetMarketPriceFromGenes, "WVC_ToolTip_offsetMarketPriceFromGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enable_HideMechanitorButtonsPatch".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enable_HideMechanitorButtonsPatch, "WVC_ToolTip_enable_HideMechanitorButtonsPatch".Translate());
			//listingStandard.CheckboxLabeled("WVC_Label_enable_ReplaceSimilarGenesAutopatch".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_ReplaceSimilarGenesAutopatch, "WVC_ToolTip_enable_ReplaceSimilarGenesAutopatch".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_autoPatchVanillaArchiteImmunityGenes".Translate().Colorize(ColorLibrary.LightBlue), ref settings.autoPatchVanillaArchiteImmunityGenes, "WVC_ToolTip_autoPatchVanillaArchiteImmunityGenes".Translate());
			listingStandard.Gap();
			// =============== Dev ===============
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGenesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.resetGenesOnLoad, "WVC_ToolTip_fixGenesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneAbilitiesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneAbilitiesOnLoad, "WVC_ToolTip_fixGeneAbilitiesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneTypesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneTypesOnLoad, "WVC_ToolTip_fixGeneTypesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			// listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixThrallTypesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixThrallTypesOnLoad, "WVC_ToolTip_fixThrallTypesOnLoad".Translate());
			listingStandard.Gap();
			// listingStandard.CheckboxLabeled("DEV MODE", ref settings.advancedDevMode, "Dev tool.");
			listingStandard.GapLine();
			// =============== Buttons ===============
			ResetButton(listingStandard);
			// =============== Dev Mode ===============
			if (Prefs.DevMode)
			{
				if (listingStandard.ButtonText("DEV: Count active WVC_ genes"))
				{
					List<GeneDef> genes = new();
					int genesCount = 0;
					foreach (Def def in Content.AllDefs)
					{
						if (def is GeneDef geneDef)
						{
							genesCount++;
							genes.Add(geneDef);
						}
					}
					Log.Error("WVC Genes: " + genesCount.ToString());
					if (!genes.NullOrEmpty())
					{
						Log.Error("All active XaG genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
					}
					else
					{
						Log.Error("Genes list is null");
					}
				}
				if (listingStandard.ButtonText("DEV: Count used in xenotypes genes"))
				{
					List<GeneDef> genes = new();
					int genesCount = 0;
					foreach (Def def in Content.AllDefs)
					{
						if (def is XenotypeDef xenotypeDef)
						{
							foreach (GeneDef geneDef in xenotypeDef.genes)
							{
								if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
								{
									genesCount ++;
									genes.Add(geneDef);
								}
							}
						}
					}
					Log.Error("WVC Genes: " + genesCount.ToString());
					if (!genes.NullOrEmpty())
					{
						Log.Error("All xenotype genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
					}
					else
					{
						Log.Error("Genes list is null");
					}
				}
				if (listingStandard.ButtonText("DEV: Count used in thrallDefs genes"))
				{
					List<GeneDef> genes = new();
					int genesCount = 0;
					foreach (Def def in Content.AllDefs)
					{
						if (def is ThrallDef xenotypeDef)
						{
							foreach (GeneDef geneDef in xenotypeDef.genes)
							{
								if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
								{
									genesCount ++;
									genes.Add(geneDef);
								}
							}
						}
					}
					Log.Error("WVC Genes: " + genesCount.ToString());
					if (!genes.NullOrEmpty())
					{
						Log.Error("All thrall genes:" + "\n" + genes.Select((GeneDef x) => x.defName).ToLineList(" - "));
					}
					else
					{
						Log.Error("Genes list is null");
					}
				}
				if (listingStandard.ButtonText("DEV: Log genes weights"))
				{
					// foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
					// {
						// Log.Error(geneDef.defName + " | " + geneDef.LabelCap + ": " + geneDef.selectionWeight.ToString());
					// }
					Log.Error("Genes weights:" + "\n" + DefDatabase<GeneDef>.AllDefsListForReading.Select((GeneDef x) => x.defName + " | " + x.LabelCap + ": " + x.selectionWeight).ToLineList(" - "));
				}
				if (listingStandard.ButtonText("DEV: Count unused in xenotypes genes"))
				{
					List<GeneDef> genes = new();
					int genesCount = 0;
					foreach (Def def in Content.AllDefs)
					{
						if (def is XenotypeDef xenotypeDef)
						{
							foreach (GeneDef geneDef in xenotypeDef.genes)
							{
								if (geneDef.IsXenoGenesDef() && !genes.Contains(geneDef))
								{
									genesCount++;
									genes.Add(geneDef);
								}
							}
						}
					}
					Log.Error("WVC used genes: " + genesCount.ToString());
					if (!genes.NullOrEmpty())
					{
						Log.Error("All unused genes:" + "\n" + Content.AllDefs.Where((Def x) => x is GeneDef geneDef && !genes.Contains(geneDef) && geneDef.GetModExtension<GeneExtension_Obsolete>() == null).Select((Def x) => x.defName).ToLineList(" - "));
					}
					else
					{
						Log.Error("Genes list is null");
					}
				}
				if (listingStandard.ButtonText("DEV: Log used genes"))
				{
					Dictionary<GeneDef, int> genes = new();
					foreach (Def def in Content.AllDefs)
					{
						if (def is XenotypeDef xenotypeDef)
						{
							foreach (GeneDef geneDef in xenotypeDef.genes)
							{
								if (geneDef.IsXenoGenesDef())
								{
									if (!genes.TryGetValue(geneDef, out int dgene))
									{
										genes[geneDef] = 1;
									}
									else
									{
										// Log.Error(geneDef.defName + " +1");
										genes[geneDef] += 1;
									}
								}
							}
						}
					}
					if (!genes.NullOrEmpty())
					{
						string text = "";
						foreach (var item in genes)
						{
							text += "\n" + item.Key.defName + ": " + item.Value.ToString();
						}
						Log.Error("Genes:" + text);
					}
					else
					{
						Log.Error("Genes list is null");
					}
				}
				if (listingStandard.ButtonText("DEV: Log obsolete genes"))
				{
					Log.Error("Obsolete genes:" + "\n" + DefDatabase<GeneDef>.AllDefsListForReading.Where((GeneDef x) => x.GetModExtension<GeneExtension_Obsolete>()?.logInDevMode == true).Select((GeneDef x) => x.defName + " | " + x.LabelCap + ": " + x.selectionWeight).ToLineList(" - "));
				}
			}
			listingStandard.End();
			Widgets.EndScrollView();
		}

		// Genes Settings
		// Genes Settings
		// Genes Settings
		private bool collapse_genesSettings_general = false;
		private bool collapse_genesSettings_Learning = false;
		private bool collapse_genesSettings_MechAndPsyLinks = false;
		private bool collapse_genesSettings_Hemogenic = false;
		//private bool collapse_genesSettings_Energy = false;
		private bool collapse_genesSettings_XenotypeGestator = false;
		private bool collapse_genesSettings_Undead = false;
		private bool collapse_genesSettings_TotalHealing = false;
		private bool collapse_genesSettings_Shapeshifer = false;
		private bool collapse_genesSettings_Anomaly = false;
		private bool collapse_genesSettings_DryadQueen = false;


		public void GenesSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 3.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_BiotechSettings_Tab_General".Translate(), tooltip: "WVC_BiotechSettings_Tooltip_GenesMechanicsGeneral".Translate()))
            {
				collapse_genesSettings_general = !collapse_genesSettings_general;
			}
			if (collapse_genesSettings_general)
			{
				//listingStandard.Label("WVC_BiotechSettings_Tab_General".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_GenesMechanicsGeneral".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_xagHumanComponent".Translate(), ref settings.enable_xagHumanComponent, "WVC_ToolTip_enable_xagHumanComponent".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_birthQualityOffsetFromGenes".Translate(), ref settings.enable_birthQualityOffsetFromGenes, "WVC_ToolTip_enable_birthQualityOffsetFromGenes".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_harmony_EnableGenesMechanicsTriggers".Translate().Colorize(ColorLibrary.LightPurple), ref settings.harmony_EnableGenesMechanicsTriggers, "WVC_ToolTip_harmony_EnableGenesMechanicsTriggers".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_StartingFoodPolicies".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_StartingFoodPolicies, "WVC_ToolTip_enable_StartingFoodPolicies".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enableIncestLoverGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableIncestLoverGene, "WVC_ToolTip_enableIncestLoverGene".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_disableNonAcceptablePreyGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableNonAcceptablePreyGenes, "WVC_ToolTip_disableNonAcceptablePreyGenes".Translate());
				//listingStandard.CheckboxLabeled("WVC_Label_enableHarmonyTelepathyGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableHarmonyTelepathyGene, "WVC_ToolTip_enableHarmonyTelepathyGene".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_OverOverridableGenesMechanic".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enable_OverOverridableGenesMechanic, "WVC_ToolTip_enable_OverOverridableGenesMechanic".Translate());
			}
			listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Learning".Translate()))
			{
				collapse_genesSettings_Learning = !collapse_genesSettings_Learning;
			}
			if (collapse_genesSettings_Learning)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_XenotypeGestator".Translate() + ":", -1);
				listingStandard.CheckboxLabeled("WVC_Label_learningTelepathWorkForBothSides".Translate().Colorize(ColorLibrary.LightBlue), ref settings.learningTelepathWorkForBothSides, "WVC_ToolTip_learningTelepathWorkForBothSides".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_learning_CyclicallySelfLearning_MaxSkillLevel".Translate((settings.learning_CyclicallySelfLearning_MaxSkillLevel).ToString()), ref settings.learning_CyclicallySelfLearning_MaxSkillLevel, 1f, 100f, "WVC_ToolTip_learning_CyclicallySelfLearning_MaxSkillLevel".Translate(), 0);
			}
			listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_MechAndPsyLinks".Translate()))
			{
				collapse_genesSettings_MechAndPsyLinks = !collapse_genesSettings_MechAndPsyLinks;
			}
			if (collapse_genesSettings_MechAndPsyLinks)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_MechAndPsyLinks".Translate() + ":", -1);
				listingStandard.CheckboxLabeled("WVC_Label_link_addedMechlinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_addedMechlinkWithGene, "WVC_ToolTip_link_addedMechlinkWithGene".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_link_addedPsylinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_addedPsylinkWithGene, "WVC_ToolTip_link_addedPsylinkWithGene".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_mechlink_HediffFromGeneChance".Translate((settings.mechlink_HediffFromGeneChance * 100).ToString()), ref settings.mechlink_HediffFromGeneChance, 0f, 1f, round: 2);
				listingStandard.SliderLabeledWithRef("WVC_Label_psylink_HediffFromGeneChance".Translate((settings.psylink_HediffFromGeneChance * 100).ToString()), ref settings.psylink_HediffFromGeneChance, 0f, 1f, round: 2);
				//listingStandard.CheckboxLabeled("WVC_Label_link_removeMechlinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_removeMechlinkWithGene, "WVC_ToolTip_link_removeMechlinkWithGene".Translate());
				//listingStandard.CheckboxLabeled("WVC_Label_link_removePsylinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_removePsylinkWithGene, "WVC_ToolTip_link_removePsylinkWithGene".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_golemnoids_ShutdownRechargePerTick".Translate((settings.golemnoids_ShutdownRechargePerTick).ToString()), ref settings.golemnoids_ShutdownRechargePerTick, 0f, 10f, round: 1);
				//listingStandard.CheckboxLabeled("WVC_Label_golembond_ShrinesStatPartOffset".Translate(), ref settings.golembond_ShrinesStatPartOffset, "WVC_ToolTip_golembond_ShrinesStatPartOffset".Translate());
				listingStandard.IntRangeLabeledWithRef("WVC_Label_golemlink_spawnIntervalRange".Translate((int)settings.golemlink_spawnIntervalRange.min.TicksToDays() + "~" + (int)settings.golemlink_spawnIntervalRange.max.TicksToDays()), ref settings.golemlink_spawnIntervalRange, 60000, 1800000);
				listingStandard.IntRangeLabeledWithRef("WVC_Label_golemlink_golemsToSpawnRange".Translate(settings.golemlink_golemsToSpawnRange.ToString()), ref settings.golemlink_golemsToSpawnRange, 1, 10);
				listingStandard.IntRangeLabeledWithRef("WVC_Label_falselink_spawnIntervalRange".Translate((int)settings.falselink_spawnIntervalRange.min.TicksToDays() + "~" + (int)settings.falselink_spawnIntervalRange.max.TicksToDays()), ref settings.falselink_spawnIntervalRange, 60000, 1800000);
				listingStandard.IntRangeLabeledWithRef("WVC_Label_falselink_mechsToSpawnRange".Translate(settings.falselink_mechsToSpawnRange.ToString()), ref settings.falselink_mechsToSpawnRange, 1, 10);
				listingStandard.SliderLabeledWithRef("WVC_Label_voidlink_mechCostFactor".Translate((settings.voidlink_mechCostFactor).ToString()), ref settings.voidlink_mechCostFactor, 0f, 5f, "WVC_ToolTip_voidlink_mechCostFactor".Translate(), round: 1);
				listingStandard.SliderLabeledWithRef("WVC_Label_voidlink_mechCostLimit".Translate((settings.voidlink_mechCostLimit).ToString()), ref settings.voidlink_mechCostLimit, 99f, 200f, "WVC_ToolTip_voidlink_mechCostLimit".Translate(), round: 0);
				listingStandard.SliderLabeledWithRef("WVC_Label_voidlink_resourceGainFromMechsFactor".Translate((settings.voidlink_resourceGainFromMechsFactor).ToStringPercent()), ref settings.voidlink_resourceGainFromMechsFactor, 0f, 1f, "WVC_ToolTip_voidlink_resourceGainFromMechsFactor".Translate(), round: 2);
				listingStandard.CheckboxLabeled("WVC_Label_voidlink_dynamicResourceLimit".Translate().Colorize(ColorLibrary.LightBlue), ref settings.voidlink_dynamicResourceLimit, "WVC_ToolTip_voidlink_dynamicResourceLimit".Translate());
			}
			listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Hemogenic".Translate()))
			{
				collapse_genesSettings_Hemogenic = !collapse_genesSettings_Hemogenic;
			}
			if (collapse_genesSettings_Hemogenic)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_Hemogenic".Translate() + ":", -1);
				listingStandard.CheckboxLabeled("WVC_Label_bloodeater_SafeBloodfeed".Translate(), ref settings.bloodeater_SafeBloodfeed, "WVC_ToolTip_bloodeater_SafeBloodfeed".Translate());
				//listingStandard.CheckboxLabeled("WVC_Label_bloodeater_disableAutoFeed".Translate().Colorize(ColorLibrary.LightBlue), ref settings.bloodeater_disableAutoFeed, "WVC_ToolTip_bloodeater_disableAutoFeed".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_bloodfeeder_AutoBloodfeed".Translate(), ref settings.bloodfeeder_AutoBloodfeed, "WVC_ToolTip_bloodfeeder_AutoBloodfeed".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_hemogenic_ImplanterFangsChanceFactor".Translate((settings.hemogenic_ImplanterFangsChanceFactor * 100f).ToString()), ref settings.hemogenic_ImplanterFangsChanceFactor, 0f, 10f, null, 2);
			}
			listingStandard.GapLine();
			// =
			//if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Energy".Translate()))
			//{
			//	collapse_genesSettings_Energy = !collapse_genesSettings_Energy;
			//}
			//if (collapse_genesSettings_Energy)
			//{
			//	listingStandard.CheckboxLabeled("WVC_Label_rechargeable_enablefoodPoisoningFromFood".Translate().Colorize(ColorLibrary.LightBlue), ref settings.rechargeable_enablefoodPoisoningFromFood, "WVC_ToolTip_rechargeable_enablefoodPoisoningFromFood".Translate());
			//}
			//listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_XenotypeGestator".Translate()))
			{
				collapse_genesSettings_XenotypeGestator = !collapse_genesSettings_XenotypeGestator;
			}
			if (collapse_genesSettings_XenotypeGestator)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_XenotypeGestator".Translate() + ":", -1);
				listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationTimeFactor".Translate((settings.xenotypeGestator_GestationTimeFactor * 100f).ToString()), ref settings.xenotypeGestator_GestationTimeFactor, 0f, 2f);
				listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationMatchPercent".Translate((settings.xenotypeGestator_GestationMatchPercent * 100f).ToString()), ref settings.xenotypeGestator_GestationMatchPercent, 0f, 1f);
			}
			listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Undead".Translate()))
			{
				collapse_genesSettings_Undead = !collapse_genesSettings_Undead;
			}
			if (collapse_genesSettings_Undead)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_Undead".Translate() + ":", -1);
				listingStandard.SliderLabeledWithRef("WVC_Label_invisibility_invisDurationFactor".Translate(settings.invisibility_invisBonusTicks.ToString()), ref settings.invisibility_invisBonusTicks, 0f, 60000f, round: 0, tooltip: "WVC_ToolTip_invisibility_invisDurationFactor".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_canNonPlayerPawnResurrect".Translate().Colorize(ColorLibrary.LightBlue), ref settings.canNonPlayerPawnResurrect, "WVC_ToolTip_canNonPlayerPawnResurrect".Translate());
				//listingStandard.CheckboxLabeled("WVC_Label_allowShapeshiftAfterDeath".Translate().Colorize(ColorLibrary.LightBlue), ref settings.allowShapeshiftAfterDeath, "WVC_ToolTip_allowShapeshiftAfterDeath".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_ThrallMaker_ThrallsInheritMasterGenes".Translate().Colorize(ColorLibrary.LightBlue), ref settings.thrallMaker_ThrallsInheritMasterGenes, "WVC_ToolTip_ThrallMaker_ThrallsInheritMasterGenes".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_thrallMaker_cooldownOverride".Translate((settings.thrallMaker_cooldownOverride).ToString()), ref settings.thrallMaker_cooldownOverride, 0f, 30f, round: 0);
				//listingStandard.CheckboxLabeled("WVC_Label_enableInstabilityLastChanceMechanic".Translate(), ref settings.enableInstabilityLastChanceMechanic, "WVC_ToolTip_enableInstabilityLastChanceMechanic".Translate());
				//listingStandard.CheckboxLabeled("WVC_Label_reincarnation_EnableMechanic".Translate().Colorize(ColorLibrary.LightBlue), ref settings.reincarnation_EnableMechanic, "WVC_ToolTip_reincarnation_EnableMechanic".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_Reincarnation_MinChronoAge".Translate((settings.reincarnation_MinChronoAge).ToString()), ref settings.reincarnation_MinChronoAge, 50f, 2000f, round: 0);
				listingStandard.SliderLabeledWithRef("WVC_Label_reincarnation_Chance".Translate((settings.reincarnation_Chance * 100f).ToString()), ref settings.reincarnation_Chance, 0f, 1f, round: 2);
			}
			listingStandard.GapLine();
			// =
			//listingStandard.Label("WVC_XaGGeneSettings_Thralls".Translate() + ":", -1);
			//listingStandard.GapLine();
			// =
			//listingStandard.Label("WVC_XaGGeneSettings_Reincarnation".Translate() + ":", -1);
			//listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_TotalHealing".Translate()))
			{
				collapse_genesSettings_TotalHealing = !collapse_genesSettings_TotalHealing;
			}
			if (collapse_genesSettings_TotalHealing)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_TotalHealing".Translate() + ":", -1);
				listingStandard.CheckboxLabeled("WVC_Label_restoreBodyPartsWithFullHP".Translate().Colorize(ColorLibrary.LightBlue), ref settings.restoreBodyPartsWithFullHP, "WVC_ToolTip_restoreBodyPartsWithFullHP".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_totalHealingIgnoreScarification".Translate().Colorize(ColorLibrary.LightBlue), ref settings.totalHealingIgnoreScarification, "WVC_ToolTip_totalHealingIgnoreScarification".Translate());
			}
			listingStandard.GapLine();
			// =
			if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Shapeshifer".Translate()))
			{
				collapse_genesSettings_Shapeshifer = !collapse_genesSettings_Shapeshifer;
			}
			if (collapse_genesSettings_Shapeshifer)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_Shapeshifer".Translate() + ":", -1);
				// listingStandard.CheckboxLabeled("WVC_Label_ShapeshifterGeneUnremovable".Translate().Colorize(ColorLibrary.LightBlue), ref settings.shapeshifterGeneUnremovable, "WVC_ToolTip_ShapeshifterGeneUnremovable".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_shapeshifer_CooldownDurationFactor".Translate(settings.shapeshifer_CooldownDurationFactor.ToStringPercent()), ref settings.shapeshifer_CooldownDurationFactor, 0f, 5f, round: 1, tooltip: "WVC_Tooltip_shapeshifer_CooldownDurationFactor".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_shapeshifer_BaseGenesMatch".Translate(settings.shapeshifer_BaseGenesMatch.ToStringPercent()), ref settings.shapeshifer_BaseGenesMatch, 0f, 1f, round: 2, tooltip: "WVC_Tooltip_shapeshifer_BaseGenesMatch".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_shapeshifer_GeneCellularRegeneration".Translate((settings.shapeshifer_GeneCellularRegeneration).ToString()), ref settings.shapeshifer_GeneCellularRegeneration, 1f, 100f, round: 0);
				// listingStandard.CheckboxLabeled("WVC_Label_shapeshifter_enableStyleButton".Translate().Colorize(ColorLibrary.LightBlue), ref settings.shapeshifter_enableStyleButton, "WVC_ToolTip_shapeshifter_enableStyleButton".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_chimeraMetabolismHungerFactor".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_chimeraMetabolismHungerFactor, "WVC_ToolTip_enable_chimeraMetabolismHungerFactor".Translate());
				//listingStandard.SliderLabeledWithRef("WVC_Label_chimeraMinGeneCopyChance".Translate((settings.chimeraMinGeneCopyChance * 100f).ToString()), ref settings.chimeraMinGeneCopyChance, 0.01f, 1f, round: 2);
				listingStandard.SliderLabeledWithRef("WVC_Label_chimeraStartingGenes".Translate((settings.chimeraStartingGenes).ToString()), ref settings.chimeraStartingGenes, 0f, 50f, round: 0);
				listingStandard.CheckboxLabeled("WVC_Label_enable_chimeraStartingTools".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_chimeraStartingTools, "WVC_ToolTip_enable_chimeraStartingTools".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_chimeraXenogermCD".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_chimeraXenogermCD, "WVC_ToolTip_enable_chimeraXenogermCD".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_enable_chimeraXenogenesLimit".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_chimeraXenogenesLimit, "WVC_ToolTip_enable_chimeraXenogenesLimit".Translate());
				listingStandard.IntRangeLabeledWithRef("WVC_Label_chimera_defaultReqMetabolismRange".Translate((int)settings.chimera_defaultReqMetabolismRange.min + "~" + (int)settings.chimera_defaultReqMetabolismRange.max), ref settings.chimera_defaultReqMetabolismRange, -99, 99, tooltip: "WVC_Tooltip_chimera_defaultReqMetabolismRange".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_duplicator_RandomOutcomeChance".Translate((settings.duplicator_RandomOutcomeChance).ToString()), ref settings.duplicator_RandomOutcomeChance, 0f, 1f, round: 2, tooltip: "WVC_Tooltip_duplicator_RandomOutcomeChance".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_duplicator_RandomGeneChance".Translate((settings.duplicator_RandomGeneChance).ToString()), ref settings.duplicator_RandomGeneChance, 0f, 1f, round: 2, tooltip: "WVC_Tooltip_duplicator_RandomGeneChance".Translate());
				//if (settings.enable_MorpherExperimentalMode || Prefs.DevMode)
				//{
				//	listingStandard.CheckboxLabeled("WVC_Label_enable_MorpherExperimentalMode".Translate().Colorize(ColorLibrary.RedReadable), ref settings.enable_MorpherExperimentalMode, "WVC_ToolTip_enable_MorpherExperimentalMode".Translate().ToString());
				//}
				listingStandard.CheckboxLabeled("WVC_Label_archiver_transferWornApparel".Translate().Colorize(ColorLibrary.LightBlue), ref settings.archiver_transferWornApparel, "WVC_ToolTip_archiver_transferWornApparel".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_archiver_transferEquipedWeapon".Translate().Colorize(ColorLibrary.LightBlue), ref settings.archiver_transferEquipedWeapon, "WVC_ToolTip_archiver_transferEquipedWeapon".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_traitshifter_MaxTraits".Translate((settings.traitshifter_MaxTraits).ToString()), ref settings.traitshifter_MaxTraits, 1f, 50f, round: 0, tooltip: "WVC_ToolTip_traitshifter_MaxTraits".Translate());
			}
			listingStandard.GapLine();
            // =
            if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_Fleshmass".Translate()))
            {
				collapse_genesSettings_Anomaly = !collapse_genesSettings_Anomaly;
            }
            if (collapse_genesSettings_Anomaly)
            {
                listingStandard.SliderLabeledWithRef("WVC_Label_fleshmass_MaxMutationsLevel".Translate(settings.fleshmass_MaxMutationsLevel), ref settings.fleshmass_MaxMutationsLevel, 0f, 100f, "WVC_ToolTip_fleshmass_MaxMutationsLevel".Translate(), 0);
				listingStandard.CheckboxLabeled("WVC_Label_fleshmass_HideBodypartHediffs".Translate().Colorize(ColorLibrary.LightBlue), ref settings.fleshmass_HideBodypartHediffs, "WVC_ToolTip_fleshmass_HideBodypartHediffs".Translate());
			}
            listingStandard.GapLine();
            // =
            //listingStandard.Label("WVC_XaGGeneSettings_IncestLover".Translate() + ":", -1);
            //listingStandard.GapLine();
            //listingStandard.Label("WVC_XaGGeneSettings_AcceptablePrey".Translate() + ":", -1);
            //listingStandard.GapLine();
            //listingStandard.Label("WVC_XaGGeneSettings_Telepath".Translate() + ":", -1);
            //listingStandard.GapLine();
            //listingStandard.Label("WVC_XaGGeneSettings_TelepathStudy".Translate() + ":", -1);
            //listingStandard.GapLine();
            // =
            if (listingStandard.ButtonTextWithTooltip("WVC_XaGGeneSettings_DryadQueen".Translate()))
			{
				collapse_genesSettings_DryadQueen = !collapse_genesSettings_DryadQueen;
			}
			if (collapse_genesSettings_DryadQueen)
			{
				//listingStandard.Label("WVC_XaGGeneSettings_DryadQueen".Translate() + ":", -1);
				listingStandard.CheckboxLabeled("WVC_Label_enable_dryadQueenMechanicGenerator".Translate().Colorize(ColorLibrary.LightOrange), ref settings.enable_dryadQueenMechanicGenerator, "WVC_ToolTip_enable_dryadQueenMechanicGenerator".Translate());
				listingStandard.SliderLabeledWithRef("WVC_Label_gestatedDryads_FilthRateFactor".Translate((settings.gestatedDryads_FilthRateFactor * 100f).ToString()), ref settings.gestatedDryads_FilthRateFactor, 0f, 1f, round: 2);
				if (ModsConfig.AnomalyActive)
				{
					listingStandard.SliderLabeledWithRef("WVC_Label_gestatedDryads_AnomalyRegeneration".Translate((settings.gestatedDryads_AnomalyRegeneration).ToString()), ref settings.gestatedDryads_AnomalyRegeneration, 0f, 50f, round: 0);
				}
			}
			// Reset Button
			listingStandard.GapLine();
			// =============== Buttons ===============
			ResetButton(listingStandard);
			// =============== Buttons ===============
			listingStandard.End();
			Widgets.EndScrollView();
		}

		// Xenotypes Settings
		// Xenotypes Settings
		// Xenotypes Settings

		public void XenotypesSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// =
			listingStandard.CheckboxLabeled("WVC_Label_enable_spawnXenotypesInFactions".Translate(), ref settings.enable_spawnXenotypesInFactions, "WVC_ToolTip_enable_spawnXenotypesInFactions".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_onlyXenotypesMode".Translate(), ref settings.onlyXenotypesMode, "WVC_ToolTip_onlyXenotypesMode".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableUniqueXenotypeScenarios".Translate(), ref settings.disableUniqueXenotypeScenarios, "WVC_ToolTip_disableUniqueXenotypeScenarios".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_MainSwitch".Translate(), ref settings.disableXenotypes_MainSwitch, "WVC_ToolTip_disableXenotypes_MainSwitch".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Undeads".Translate(), ref settings.disableXenotypes_Undeads, "WVC_ToolTip_disableXenotypes_Undeads".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Psycasters".Translate(), ref settings.disableXenotypes_Psycasters, "WVC_ToolTip_disableXenotypes_Psycasters".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Mechalike".Translate(), ref settings.disableXenotypes_Mechalike, "WVC_ToolTip_disableXenotypes_Mechalike".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_GolemMasters".Translate(), ref settings.disableXenotypes_GolemMasters, "WVC_ToolTip_disableXenotypes_GolemMasters".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Bloodeaters".Translate(), ref settings.disableXenotypes_Bloodeaters, "WVC_ToolTip_disableXenotypes_Bloodeaters".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_MutantMakers".Translate(), ref settings.disableXenotypes_MutantMakers, "WVC_ToolTip_disableXenotypes_MutantMakers".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Misc".Translate(), ref settings.disableXenotypes_Misc, "WVC_ToolTip_disableXenotypes_Misc".Translate());
			listingStandard.GapLine();
			// =============== Xenotypes Chance Settings ===============
			// listingStandard.GapLine();
			// =============== Buttons ===============
			ResetButton(listingStandard);
			// if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			// {
			// Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
			// {
			// ResetSettings_MyChoice();
			// Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
			// });
			// Find.WindowStack.Add(window);
			// }
			// =============== Buttons ===============
			listingStandard.End();
			Widgets.EndScrollView();
		}

		private static void ResetButton(Listing_Standard listingStandard)
		{
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				List<FloatMenuOption> list = new();
				List<SettingsDef> allSettings = DefDatabase<SettingsDef>.AllDefsListForReading;
				for (int i = 0; i < allSettings.Count; i++)
				{
					SettingsDef preset = allSettings[i];
					list.Add(new FloatMenuOption(preset.LabelCap, delegate
					{
						Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(preset.description), delegate
						{
							ResetSettings_ByTemplate(preset);
							Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
						});
						Find.WindowStack.Add(window);
					}, orderInPriority: 0 - preset.presetOrder));
				}
				if (!list.Any())
				{
					list.Add(new FloatMenuOption("ERROR: SETTINGS PRESET IS NULL", null));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		// Reset

		public static void ResetSettings_ByTemplate(SettingsDef settingsDef)
		{
			WVC_Biotech.settings.onlyXenotypesMode = settingsDef.onlyXenotypesMode;
			// Graphic
			WVC_Biotech.settings.hideXaGGenes = settingsDef.hideXaGGenes;
			WVC_Biotech.settings.disableFurGraphic = settingsDef.disableFurGraphic;
			WVC_Biotech.settings.enable_FurskinIsSkinAutopatch = settingsDef.enable_FurskinIsSkinAutopatch;
			WVC_Biotech.settings.disableAllGraphic = settingsDef.disableAllGraphic;
			WVC_Biotech.settings.disableUniqueGeneInterface = settingsDef.disableUniqueGeneInterface;
			WVC_Biotech.settings.disableEyesGraphic = settingsDef.disableEyesGraphic;
			WVC_Biotech.settings.useMaskForFurskinGenes = settingsDef.useMaskForFurskinGenes;
			// Generator
			WVC_Biotech.settings.generateSkillGenes = settingsDef.generateSkillGenes;
			WVC_Biotech.settings.generateXenotypeForceGenes = settingsDef.generateXenotypeForceGenes;
			//WVC_Biotech.settings.generateResourceSpawnerGenes = settingsDef.generateResourceSpawnerGenes;
			//WVC_Biotech.settings.generateSkinHairColorGenes = settingsDef.generateSkinHairColorGenes;
			// Misc
			WVC_Biotech.settings.disableUniqueXenotypeScenarios = settingsDef.disableUniqueXenotypeScenarios;
			// Fix
			WVC_Biotech.settings.harmony_vanillaFixesTweaksAndCompatability = settingsDef.harmony_vanillaFixesTweaksAndCompatability;
			WVC_Biotech.settings.spawnXenoForcerSerumsFromTraders = settingsDef.spawnXenoForcerSerumsFromTraders;
			// Info
			WVC_Biotech.settings.enable_xagHumanComponent = settingsDef.enable_xagHumanComponent;
			WVC_Biotech.settings.enable_StartingFoodPolicies = settingsDef.enable_StartingFoodPolicies;
			// WVC_Biotech.settings.enableGeneSpawnerGizmo = true;
			// WVC_Biotech.settings.enableGeneWingInfo = false;
			// WVC_Biotech.settings.enableGeneBlesslinkInfo = true;
			// WVC_Biotech.settings.enableGeneUndeadInfo = false;
			// WVC_Biotech.settings.enableGeneScarifierInfo = false;
			// WVC_Biotech.settings.enableGeneInstabilityInfo = true;
			// WVC_Biotech.settings.enableGeneGauranlenConnectionInfo = true;
			// Serums
			// WVC_Biotech.settings.serumsForAllXenotypes = false;
			// WVC_Biotech.settings.serumsForAllXenotypes_GenBase = false;
			// WVC_Biotech.settings.serumsForAllXenotypes_GenUltra = false;
			// WVC_Biotech.settings.serumsForAllXenotypes_GenHybrid = false;
			// WVC_Biotech.settings.serumsForAllXenotypes_Recipes = false;
			// WVC_Biotech.settings.serumsForAllXenotypes_Spawners = false;
			// =
			WVC_Biotech.settings.canNonPlayerPawnResurrect = settingsDef.canNonPlayerPawnResurrect;
			//WVC_Biotech.settings.enable_MorpherExperimentalMode = settingsDef.enable_MorpherExperimentalMode;
			//WVC_Biotech.settings.allowShapeshiftAfterDeath = settingsDef.allowShapeshiftAfterDeath;
			// WVC_Biotech.settings.shapeshifter_enableStyleButton = true;
			WVC_Biotech.settings.enable_chimeraMetabolismHungerFactor = settingsDef.enable_chimeraMetabolismHungerFactor;
			//WVC_Biotech.settings.chimeraMinGeneCopyChance = settingsDef.chimeraMinGeneCopyChance;
			WVC_Biotech.settings.shapeshifer_BaseGenesMatch = settingsDef.shapeshifer_BaseGenesMatch;
			WVC_Biotech.settings.shapeshifer_CooldownDurationFactor = settingsDef.shapeshifer_CooldownDurationFactor;
			WVC_Biotech.settings.shapeshifer_GeneCellularRegeneration = settingsDef.shapeshifer_GeneCellularRegeneration;
			WVC_Biotech.settings.chimeraStartingGenes = settingsDef.chimeraStartingGenes;
			WVC_Biotech.settings.enable_chimeraStartingTools = settingsDef.enable_chimeraStartingTools;
			WVC_Biotech.settings.enable_chimeraXenogermCD = settingsDef.enable_chimeraXenogermCD;
			WVC_Biotech.settings.enable_chimeraXenogenesLimit = settingsDef.enable_chimeraXenogenesLimit;
			WVC_Biotech.settings.chimera_defaultReqMetabolismRange = settingsDef.chimera_defaultReqMetabolismRange;
			// =
			WVC_Biotech.settings.duplicator_RandomOutcomeChance = settingsDef.duplicator_RandomOutcomeChance;
			WVC_Biotech.settings.duplicator_RandomGeneChance = settingsDef.duplicator_RandomGeneChance;
			// =
			WVC_Biotech.settings.thrallMaker_cooldownOverride = settingsDef.thrallMaker_cooldownOverride;
			WVC_Biotech.settings.thrallMaker_ThrallsInheritMasterGenes = settingsDef.thrallMaker_ThrallsInheritMasterGenes;
			// =
			WVC_Biotech.settings.totalHealingIgnoreScarification = settingsDef.totalHealingIgnoreScarification;
			WVC_Biotech.settings.restoreBodyPartsWithFullHP = settingsDef.restoreBodyPartsWithFullHP;
			// =
			// WVC_Biotech.settings.shapeshifterGeneUnremovable = true;
			// =
			WVC_Biotech.settings.enableIncestLoverGene = settingsDef.enableIncestLoverGene;
			WVC_Biotech.settings.disableNonAcceptablePreyGenes = settingsDef.disableNonAcceptablePreyGenes;
			// =
			//WVC_Biotech.settings.enableHarmonyTelepathyGene = settingsDef.enableHarmonyTelepathyGene;
			// =
			WVC_Biotech.settings.invisibility_invisBonusTicks = settingsDef.invisibility_invisBonusTicks;
			// =
			WVC_Biotech.settings.enable_OverOverridableGenesMechanic = settingsDef.enable_OverOverridableGenesMechanic;
			// WVC_Biotech.settings.useAlternativeDustogenicFoodJob = true;
			// =
			WVC_Biotech.settings.learningTelepathWorkForBothSides = settingsDef.learningTelepathWorkForBothSides;
			WVC_Biotech.settings.learning_CyclicallySelfLearning_MaxSkillLevel = settingsDef.learning_CyclicallySelfLearning_MaxSkillLevel;
			// =
			WVC_Biotech.settings.enable_birthQualityOffsetFromGenes = settingsDef.enable_birthQualityOffsetFromGenes;
			WVC_Biotech.settings.xenotypeGestator_GestationTimeFactor = settingsDef.xenotypeGestator_GestationTimeFactor;
			WVC_Biotech.settings.xenotypeGestator_GestationMatchPercent = settingsDef.xenotypeGestator_GestationMatchPercent;
			// =
			//WVC_Biotech.settings.reincarnation_EnableMechanic = settingsDef.reincarnation_EnableMechanic;
			WVC_Biotech.settings.reincarnation_MinChronoAge = settingsDef.reincarnation_MinChronoAge;
			WVC_Biotech.settings.reincarnation_Chance = settingsDef.reincarnation_Chance;
			// =
			WVC_Biotech.settings.harmony_EnableGenesMechanicsTriggers = settingsDef.harmony_EnableGenesMechanicsTriggers;
			WVC_Biotech.settings.bloodeater_SafeBloodfeed = settingsDef.bloodeater_SafeBloodfeed;
			//WVC_Biotech.settings.bloodeater_disableAutoFeed = settingsDef.bloodeater_disableAutoFeed;
			WVC_Biotech.settings.bloodfeeder_AutoBloodfeed = settingsDef.bloodfeeder_AutoBloodfeed;
			WVC_Biotech.settings.hemogenic_ImplanterFangsChanceFactor = settingsDef.hemogenic_ImplanterFangsChanceFactor;
			// =
			//WVC_Biotech.settings.enableInstabilityLastChanceMechanic = settingsDef.enableInstabilityLastChanceMechanic;
			// =
			WVC_Biotech.settings.archiver_transferEquipedWeapon = settingsDef.archiver_transferEquipedWeapon;
			WVC_Biotech.settings.archiver_transferWornApparel = settingsDef.archiver_transferWornApparel;
			// =
			WVC_Biotech.settings.traitshifter_MaxTraits = settingsDef.traitshifter_MaxTraits;
			// =
			WVC_Biotech.settings.enable_dryadQueenMechanicGenerator = settingsDef.enable_dryadQueenMechanicGenerator;
			WVC_Biotech.settings.gestatedDryads_FilthRateFactor = settingsDef.gestatedDryads_FilthRateFactor;
			WVC_Biotech.settings.gestatedDryads_AnomalyRegeneration = settingsDef.gestatedDryads_AnomalyRegeneration;
			// =
			WVC_Biotech.settings.link_addedMechlinkWithGene = settingsDef.link_addedMechlinkWithGene;
			WVC_Biotech.settings.link_addedPsylinkWithGene = settingsDef.link_addedPsylinkWithGene;
			WVC_Biotech.settings.mechlink_HediffFromGeneChance = settingsDef.mechlink_HediffFromGeneChance;
			WVC_Biotech.settings.psylink_HediffFromGeneChance = settingsDef.psylink_HediffFromGeneChance;
			//WVC_Biotech.settings.link_removeMechlinkWithGene = settingsDef.link_removeMechlinkWithGene;
			//WVC_Biotech.settings.link_removePsylinkWithGene = settingsDef.link_removePsylinkWithGene;
			WVC_Biotech.settings.golemnoids_ShutdownRechargePerTick = settingsDef.golemnoids_ShutdownRechargePerTick;
			//WVC_Biotech.settings.golembond_ShrinesStatPartOffset = settingsDef.golembond_ShrinesStatPartOffset;
			WVC_Biotech.settings.golemlink_spawnIntervalRange = settingsDef.golemlink_spawnIntervalRange;
			WVC_Biotech.settings.golemlink_golemsToSpawnRange = settingsDef.golemlink_golemsToSpawnRange;
			WVC_Biotech.settings.falselink_spawnIntervalRange = settingsDef.falselink_spawnIntervalRange;
			WVC_Biotech.settings.falselink_mechsToSpawnRange = settingsDef.falselink_mechsToSpawnRange;
			WVC_Biotech.settings.voidlink_mechCostFactor = settingsDef.voidlink_mechCostFactor;
			WVC_Biotech.settings.voidlink_mechCostLimit = settingsDef.voidlink_mechCostLimit;
			WVC_Biotech.settings.voidlink_resourceGainFromMechsFactor = settingsDef.voidlink_resourceGainFromMechsFactor;
			WVC_Biotech.settings.voidlink_dynamicResourceLimit = settingsDef.voidlink_dynamicResourceLimit;
			// =
			//WVC_Biotech.settings.rechargeable_enablefoodPoisoningFromFood = settingsDef.rechargeable_enablefoodPoisoningFromFood;
			WVC_Biotech.settings.fleshmass_MaxMutationsLevel = settingsDef.fleshmass_MaxMutationsLevel;
			WVC_Biotech.settings.fleshmass_HideBodypartHediffs = settingsDef.fleshmass_HideBodypartHediffs;
			// =
			// Extra
			//WVC_Biotech.settings.genesCanTickOnlyOnMap = settingsDef.genesCanTickOnlyOnMap;
			WVC_Biotech.settings.enable_flatGenesSpawnChances = settingsDef.enable_flatGenesSpawnChances;
			WVC_Biotech.settings.offsetMarketPriceFromGenes = settingsDef.offsetMarketPriceFromGenes;
			WVC_Biotech.settings.enable_HideMechanitorButtonsPatch = settingsDef.enable_HideMechanitorButtonsPatch;
			//WVC_Biotech.settings.enable_ReplaceSimilarGenesAutopatch = settingsDef.enable_ReplaceSimilarGenesAutopatch;
			// Xenotypes
			WVC_Biotech.settings.enable_spawnXenotypesInFactions = settingsDef.enable_spawnXenotypesInFactions;
			WVC_Biotech.settings.disableXenotypes_MainSwitch = settingsDef.disableXenotypes_MainSwitch;
			WVC_Biotech.settings.disableXenotypes_Undeads = settingsDef.disableXenotypes_Undeads;
			WVC_Biotech.settings.disableXenotypes_Psycasters = settingsDef.disableXenotypes_Psycasters;
			WVC_Biotech.settings.disableXenotypes_Mechalike = settingsDef.disableXenotypes_Mechalike;
			WVC_Biotech.settings.disableXenotypes_GolemMasters = settingsDef.disableXenotypes_GolemMasters;
			WVC_Biotech.settings.disableXenotypes_Bloodeaters = settingsDef.disableXenotypes_Bloodeaters;
			WVC_Biotech.settings.disableXenotypes_MutantMakers = settingsDef.disableXenotypes_MutantMakers;
			WVC_Biotech.settings.disableXenotypes_Misc = settingsDef.disableXenotypes_Misc;
			// PregnantHuman
			// WVC_Biotech.settings.pregnantHuman_TrueParentGenesPatch = false;
			// WVC_Biotech.settings.pregnantHuman_InheritSurrogateGenes = false;
			// WVC_Biotech.settings.pregnantHuman_InheritArchiteGenes = false;
			// XenotypesSettings
			// WVC_Biotech.cachedXenotypesFilter.Clear();
			// XaG_PostInitialization.SetValues();
			WVC_Biotech.settings.geneGizmosDefaultCollapse = settingsDef.geneGizmosDefaultCollapse;
			WVC_Biotech.settings.showGenesSettingsGizmo = settingsDef.showGenesSettingsGizmo;
			WVC_Biotech.settings.hideGeneHediffs = settingsDef.hideGeneHediffs;
			// Initial
			WVC_Biotech.settings.firstModLaunch = false;
			WVC_Biotech.settings.Write();
		}

	}

}
