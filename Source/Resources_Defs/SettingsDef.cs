using Verse;

namespace WVC_XenotypesAndGenes
{

    public class SettingsDef : Def
	{

		public int presetOrder = 0;

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
		public bool onlyXenotypesMode = true;
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
		//public bool spawnXenoForcerSerumsFromTraders = true;
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
		public float voidlink_resourceGainFromMechsFactor = 0.05f;
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

	}

}
