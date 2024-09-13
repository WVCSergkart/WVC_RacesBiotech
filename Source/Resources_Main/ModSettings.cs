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

		// Main
		// Graphic
		public bool hideXaGGenes = false;
		public bool disableFurGraphic = false;
		public bool disableAllGraphic = false;
		public bool disableUniqueGeneInterface = false;
		public bool disableEyesGraphic = false;
		// public bool enableBodySizeGenes = true;
		// Generator
		public bool generateSkillGenes = true;
		public bool generateXenotypeForceGenes = false;
		public bool generateResourceSpawnerGenes = false;
		public bool generateSkinHairColorGenes = false;
		// Genes
		public bool onlyXenotypesMode = false;
		public bool canNonPlayerPawnResurrect = false;
		public bool allowShapeshiftAfterDeath = true;
		public bool totalHealingIgnoreScarification = true;
		// public bool genesRemoveMechlinkUponDeath = false;
		// public bool enableCustomMechLinkName = false;
		public bool shapeshifterGeneUnremovable = true;
		public bool enableIncestLoverGene = true;
		public bool disableNonAcceptablePreyGenes = false;
		public bool enableHarmonyTelepathyGene = false;
		// public bool useAlternativeDustogenicFoodJob = true;
		public bool learningTelepathWorkForBothSides = false;
		public bool disableUniqueXenotypeScenarios = false;
		public bool restoreBodyPartsWithFullHP = false;
		// public bool reimplantResurrectionRecruiting = false;
		public bool thrallMaker_ThrallsInheritMasterGenes = true;
		// Info
		public bool enableGenesInfo = true;
		// public bool enableGeneSpawnerGizmo = true;
		// public bool enableGeneWingInfo = false;
		// public bool enableGeneBlesslinkInfo = true;
		// public bool enableGeneUndeadInfo = false;
		// public bool enableGeneScarifierInfo = false;
		// public bool enableGeneInstabilityInfo = true;
		// public bool enableGeneGauranlenConnectionInfo = true;
		// Serums
		public bool serumsForAllXenotypes = false;
		public bool serumsForAllXenotypes_GenBase = true;
		public bool serumsForAllXenotypes_GenUltra = false;
		public bool serumsForAllXenotypes_GenHybrid = false;
		public bool serumsForAllXenotypes_Recipes = true;
		public bool serumsForAllXenotypes_Spawners = false;
		// ExtraSettings
		public bool genesCanTickOnlyOnMap = false;
		public bool enable_flatGenesSpawnChances = false;
		// public float flatGenesSpawnChances_selectionWeight = 0.001f;
		// Fix
		public bool fixVanillaGeneImmunityCheck = true;
		public bool spawnXenoForcerSerumsFromTraders = true;
		public bool fixGenesOnLoad = false;
		public bool fixGeneAbilitiesOnLoad = false;
		public bool fixGeneTypesOnLoad = false;
		// public bool fixThrallTypesOnLoad = false;
		// Gestator
		public bool enable_birthQualityOffsetFromGenes = true;
		public float xenotypeGestator_GestationTimeFactor = 1f;
		public float xenotypeGestator_GestationMatchPercent = 0.4f;
		// Reincarnation
		public bool reincarnation_EnableMechanic = true;
		public float reincarnation_MinChronoAge = 200f;
		public float reincarnation_Chance = 0.08f;
		// Hemogenic
		public bool harmony_EnableGenesMechanicsTriggers = true;
		public bool bloodeater_SafeBloodfeed = false;
		public bool bloodeater_disableAutoFeed = false;
		public bool bloodfeeder_AutoBloodfeed = false;
		public float hemogenic_ImplanterFangsChanceFactor = 1f;
		// Thralls
		public bool enableInstabilityLastChanceMechanic = true;
		// Links
		public bool link_addedMechlinkWithGene = true;
		public bool link_addedPsylinkWithGene = true;
		public bool link_removeMechlinkWithGene = false;
		public bool link_removePsylinkWithGene = false;
		// Shapeshifter
		// public bool shapeshifter_enableStyleButton = true;
		public float shapeshifer_GeneCellularRegeneration = 1f;
		// Chimera
		public bool enable_chimeraMetabolismHungerFactor = true;
		public float chimeraMinGeneCopyChance = 0.01f;
		public float chimeraStartingGenes = 5f;
		// DryadQueen
		public bool enable_dryadQueenMechanicGenerator = true;
		// Xenotypes
		public bool enable_spawnXenotypesInFactions = false;
		// public bool increasedXenotypesFactionlessGenerationWeight_MainSwitch = false;
		public bool disableXenotypes_MainSwitch = false;
		public bool disableXenotypes_Undeads = false;
		public bool disableXenotypes_Psycasters = false;
		public bool disableXenotypes_Mechalike = false;
		public bool disableXenotypes_GolemMasters = false;
		public bool disableXenotypes_Bloodeaters = false;
		public bool disableXenotypes_Misc = false;
		// PregnantHuman
		// public bool pregnantHuman_TrueParentGenesPatch = false;
		// public bool pregnantHuman_InheritSurrogateGenes = false;
		// public bool pregnantHuman_InheritArchiteGenes = false;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
														 where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
														 select specificSetting.Name;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref firstModLaunch, "firstModLaunch", defaultValue: true, forceSave: true);
			// Main
			// Graphic
			Scribe_Values.Look(ref hideXaGGenes, "hideXaGGenes", defaultValue: false);
			Scribe_Values.Look(ref disableFurGraphic, "disableFurGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableAllGraphic, "disableAllGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueGeneInterface, "disableUniqueGeneInterface", defaultValue: false);
			// Scribe_Values.Look(ref enableBodySizeGenes, "enableBodySizeGenes", defaultValue: true);
			Scribe_Values.Look(ref disableEyesGraphic, "disableEyesGraphic", defaultValue: false);
			// Generator
			Scribe_Values.Look(ref generateSkillGenes, "generateSkillGenes", defaultValue: true);
			Scribe_Values.Look(ref generateXenotypeForceGenes, "generateXenotypeForceGenes", defaultValue: false);
			Scribe_Values.Look(ref generateResourceSpawnerGenes, "generateResourceSpawnerGenes", defaultValue: false);
			Scribe_Values.Look(ref generateSkinHairColorGenes, "generateSkinHairColorGenes", defaultValue: false);
			// Genes
			Scribe_Values.Look(ref onlyXenotypesMode, "onlyXenotypesMode", defaultValue: false);
			Scribe_Values.Look(ref canNonPlayerPawnResurrect, "canNonPlayerPawnResurrect", defaultValue: true);
			Scribe_Values.Look(ref allowShapeshiftAfterDeath, "allowShapeshiftAfterDeath", defaultValue: true);
			Scribe_Values.Look(ref totalHealingIgnoreScarification, "totalHealingIgnoreScarification", defaultValue: true);
			// Scribe_Values.Look(ref genesRemoveMechlinkUponDeath, "genesRemoveMechlinkUponDeath", defaultValue: false);
			// Scribe_Values.Look(ref enableCustomMechLinkName, "enableCustomMechLinkName", defaultValue: false);
			Scribe_Values.Look(ref shapeshifterGeneUnremovable, "shapeshifterGeneUnremovable", defaultValue: true);
			Scribe_Values.Look(ref enableIncestLoverGene, "enableIncestLoverGene", defaultValue: true);
			Scribe_Values.Look(ref disableNonAcceptablePreyGenes, "disableNonAcceptablePreyGenes", defaultValue: false);
			Scribe_Values.Look(ref enableHarmonyTelepathyGene, "enableHarmonyTelepathyGene", defaultValue: false);
			// Scribe_Values.Look(ref useAlternativeDustogenicFoodJob, "useAlternativeDustogenicFoodJob", defaultValue: true);
			Scribe_Values.Look(ref learningTelepathWorkForBothSides, "learningTelepathWorkForBothSides", defaultValue: false);
			Scribe_Values.Look(ref restoreBodyPartsWithFullHP, "restoreBodyPartsWithFullHP", defaultValue: false);
			// Scribe_Values.Look(ref reimplantResurrectionRecruiting, "reimplantResurrectionRecruiting", defaultValue: false);
			Scribe_Values.Look(ref thrallMaker_ThrallsInheritMasterGenes, "thrallMaker_ThrallsInheritMasterGenes", defaultValue: true);
			// Fix
			Scribe_Values.Look(ref fixVanillaGeneImmunityCheck, "fixVanillaGeneImmunityCheck", defaultValue: true);
			// Scribe_Values.Look(ref minWastepacksPerRecharge, "minWastepacksPerRecharge", defaultValue: false);
			// Scribe_Values.Look(ref validatorAbilitiesPatch, "validatorAbilitiesPatch", defaultValue: true);
			Scribe_Values.Look(ref spawnXenoForcerSerumsFromTraders, "spawnXenoForcerSerumsFromTraders", defaultValue: true);
			// Scribe_Values.Look(ref fixGenesOnLoad, "fixGenesOnLoad", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueXenotypeScenarios, "disableUniqueXenotypeScenarios", defaultValue: false);
			// Info
			Scribe_Values.Look(ref enableGenesInfo, "enableGenesInfo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneSpawnerGizmo, "enableGeneSpawnerGizmo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneWingInfo, "enableGeneWingInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneBlesslinkInfo, "enableGeneBlesslinkInfo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneUndeadInfo, "enableGeneUndeadInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneScarifierInfo, "enableGeneScarifierInfo", defaultValue: false);
			// Scribe_Values.Look(ref enableGeneInstabilityInfo, "enableGeneInstabilityInfo", defaultValue: true);
			// Scribe_Values.Look(ref enableGeneGauranlenConnectionInfo, "enableGeneGauranlenConnectionInfo", defaultValue: true);
			// Serums
			Scribe_Values.Look(ref serumsForAllXenotypes, "serumsForAllXenotypes", defaultValue: false, forceSave: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenBase, "serumsForAllXenotypes_GenBase", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenUltra, "serumsForAllXenotypes_GenUltra", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenHybrid, "serumsForAllXenotypes_GenHybrid", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_Recipes, "serumsForAllXenotypes_Recipes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_Spawners, "serumsForAllXenotypes_Spawners", defaultValue: false);
			// ExtraSettings
			Scribe_Values.Look(ref genesCanTickOnlyOnMap, "genesCanTickOnlyOnMap", defaultValue: false);
			Scribe_Values.Look(ref enable_flatGenesSpawnChances, "enable_flatGenesSpawnChances", defaultValue: false);
			// Scribe_Values.Look(ref autoPatchVanillaArchiteImmunityGenes, "autoPatchVanillaArchiteImmunityGenes", defaultValue: false);
			// Gestator
			Scribe_Values.Look(ref enable_birthQualityOffsetFromGenes, "enable_birthQualityOffsetFromGenes", defaultValue: true);
			Scribe_Values.Look(ref xenotypeGestator_GestationTimeFactor, "xenotypeGestator_GestationTimeFactor", defaultValue: 1f);
			Scribe_Values.Look(ref xenotypeGestator_GestationMatchPercent, "xenotypeGestator_GestationMatchPercent", defaultValue: 0.4f);
			// Reincarnation
			Scribe_Values.Look(ref reincarnation_EnableMechanic, "reincarnation_EnableMechanic", defaultValue: true);
			Scribe_Values.Look(ref reincarnation_MinChronoAge, "reincarnation_MinChronoAge", defaultValue: 200f);
			Scribe_Values.Look(ref reincarnation_Chance, "reincarnation_Chance", defaultValue: 0.08f);
			// Hemogenic
			Scribe_Values.Look(ref harmony_EnableGenesMechanicsTriggers, "harmony_EnableGenesMechanicsTriggers", defaultValue: true);
			Scribe_Values.Look(ref bloodeater_SafeBloodfeed, "bloodeater_SafeBloodfeed", defaultValue: false);
			Scribe_Values.Look(ref bloodeater_disableAutoFeed, "bloodeater_disableAutoFeed", defaultValue: false);
			Scribe_Values.Look(ref bloodfeeder_AutoBloodfeed, "bloodfeeder_AutoBloodfeed", defaultValue: false);
			Scribe_Values.Look(ref hemogenic_ImplanterFangsChanceFactor, "hemogenic_ImplanterFangsChanceFactor", defaultValue: 1f);
			// Thralls
			Scribe_Values.Look(ref enableInstabilityLastChanceMechanic, "enableInstabilityLastChanceMechanic", defaultValue: true);
			// Links
			Scribe_Values.Look(ref link_addedMechlinkWithGene, "link_addedMechlinkWithGene", defaultValue: true);
			Scribe_Values.Look(ref link_addedPsylinkWithGene, "link_addedPsylinkWithGene", defaultValue: true);
			Scribe_Values.Look(ref link_removeMechlinkWithGene, "link_removeMechlinkWithGene", defaultValue: false);
			Scribe_Values.Look(ref link_removePsylinkWithGene, "link_removePsylinkWithGene", defaultValue: false);
			// shapeshifter
			Scribe_Values.Look(ref shapeshifer_GeneCellularRegeneration, "shapeshifer_GeneCellularRegeneration", defaultValue: 1f);
			// Scribe_Values.Look(ref shapeshifter_enableStyleButton, "shapeshifter_enableStyleButton", defaultValue: true);
			Scribe_Values.Look(ref enable_spawnXenotypesInFactions, "enable_spawnXenotypesInFactions", defaultValue: false);
			// Chimera
			Scribe_Values.Look(ref enable_chimeraMetabolismHungerFactor, "enable_chimeraMetabolismHungerFactor", defaultValue: true);
			Scribe_Values.Look(ref chimeraMinGeneCopyChance, "chimeraMinGeneCopyChance", defaultValue: 0.01f);
			Scribe_Values.Look(ref chimeraStartingGenes, "chimeraStartingGenes", defaultValue: 5f);
			// DryadQueen
			Scribe_Values.Look(ref enable_dryadQueenMechanicGenerator, "enable_dryadQueenMechanicGenerator", defaultValue: true);
			// Reincarnation
			Scribe_Values.Look(ref disableXenotypes_MainSwitch, "disableXenotypes_MainSwitch", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Undeads, "disableXenotypes_Undeads", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Psycasters, "disableXenotypes_Psycasters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Mechalike, "disableXenotypes_Mechalike", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_GolemMasters, "disableXenotypes_GolemMasters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Bloodeaters, "disableXenotypes_Bloodeaters", defaultValue: false);
			Scribe_Values.Look(ref disableXenotypes_Misc, "disableXenotypes_Misc", defaultValue: false);
			// PregnantHuman
			// Scribe_Values.Look(ref pregnantHuman_TrueParentGenesPatch, "pregnantHuman_TrueParentGenesPatch", defaultValue: false);
			// Scribe_Values.Look(ref pregnantHuman_InheritSurrogateGenes, "pregnantHuman_InheritSurrogateGenes", defaultValue: false);
			// Scribe_Values.Look(ref pregnantHuman_InheritArchiteGenes, "pregnantHuman_InheritArchiteGenes", defaultValue: false);
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
			listingStandard.CheckboxLabeled("WVC_Label_hideXaGGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.hideXaGGenes, "WVC_ToolTip_hideXaGGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableFurGraphic".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableFurGraphic, "WVC_ToolTip_disableFurGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableAllGraphic".Translate(), ref settings.disableAllGraphic, "WVC_ToolTip_disableAllGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableUniqueGeneInterface".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableUniqueGeneInterface, "WVC_ToolTip_disableUniqueGeneInterface".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableEyesGraphic".Translate(), ref settings.disableEyesGraphic, "WVC_ToolTip_disableEyesGraphic".Translate());
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
			// Generator
			listingStandard.Label("WVC_BiotechSettings_Label_Generators".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Generators".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_generateSkillGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateSkillGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_generateXenotypeForceGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateXenotypeForceGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_generateResourceSpawnerGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateResourceSpawnerGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_generateSkinHairColorGenes".Translate().Colorize(ColorLibrary.LightOrange), ref settings.generateSkinHairColorGenes, "WVC_ToolTip_generateSkinHairColorGenes".Translate());
			listingStandard.Gap();
			// Fix
			listingStandard.Label("WVC_BiotechSettings_Label_Other".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Other".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_fixVanillaGeneImmunityCheck".Translate().Colorize(ColorLibrary.LightPurple), ref settings.fixVanillaGeneImmunityCheck, "WVC_ToolTip_fixVanillaGeneImmunityCheck".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_spawnXenoForcerSerumsFromTraders".Translate(), ref settings.spawnXenoForcerSerumsFromTraders, "WVC_ToolTip_spawnXenoForcerSerumsFromTraders".Translate());
			listingStandard.GapLine();
			// Serums
			if (settings.serumsForAllXenotypes)
			{
				listingStandard.Label("WVC_BiotechSettings_Label_Serums".Translate().Colorize(ColoredText.SubtleGrayColor) + ":", -1, "WVC_BiotechSettings_Tooltip_Serums".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes, "WVC_ToolTip_serumsForAllXenotypes".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenBase".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenBase, "WVC_ToolTip_serumsForAllXenotypes_GenBase".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenUltra".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenUltra, "WVC_ToolTip_serumsForAllXenotypes_GenUltra".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenHybrid".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenHybrid, "WVC_ToolTip_serumsForAllXenotypes_GenHybrid".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_Recipes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Recipes, "WVC_ToolTip_serumsForAllXenotypes_Recipes".Translate());
				listingStandard.CheckboxLabeled("WVC_Label_serumsSpawnersForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Spawners, "WVC_ToolTip_serumsSpawnersForAllXenotypes".Translate());
				listingStandard.GapLine();
			}
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_Default();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_MyChoice();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
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
				allXenotypes = XenotypeFilterUtility.WhiteListedXenotypes(false);
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
				XaG_PostInitialization.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
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
			listingStandard.CheckboxLabeled("WVC_Label_genesCanTickOnlyOnMap".Translate().Colorize(ColorLibrary.LightPurple), ref settings.genesCanTickOnlyOnMap, "WVC_ToolTip_genesCanTickOnlyOnMap".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_flatGenesSpawnChances".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enable_flatGenesSpawnChances, "WVC_ToolTip_flatGenesSpawnChances".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_autoPatchVanillaArchiteImmunityGenes".Translate().Colorize(ColorLibrary.LightBlue), ref settings.autoPatchVanillaArchiteImmunityGenes, "WVC_ToolTip_autoPatchVanillaArchiteImmunityGenes".Translate());
			listingStandard.Gap();
			// =============== Dev ===============
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGenesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGenesOnLoad, "WVC_ToolTip_fixGenesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneAbilitiesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneAbilitiesOnLoad, "WVC_ToolTip_fixGeneAbilitiesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneTypesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneTypesOnLoad, "WVC_ToolTip_fixGeneTypesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			// listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixThrallTypesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixThrallTypesOnLoad, "WVC_ToolTip_fixThrallTypesOnLoad".Translate());
			listingStandard.GapLine();
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_Default();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_MyChoice();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
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
			}
			listingStandard.End();
			Widgets.EndScrollView();
		}

		// Genes Settings
		// Genes Settings
		// Genes Settings

		public void GenesSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 3.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// =
			listingStandard.Label("WVC_BiotechSettings_Tab_General".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_GenesMechanicsGeneral".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGenesInfo".Translate(), ref settings.enableGenesInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enable_birthQualityOffsetFromGenes".Translate(), ref settings.enable_birthQualityOffsetFromGenes, "WVC_ToolTip_enable_birthQualityOffsetFromGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_harmony_EnableGenesMechanicsTriggers".Translate().Colorize(ColorLibrary.LightPurple), ref settings.harmony_EnableGenesMechanicsTriggers, "WVC_ToolTip_harmony_EnableGenesMechanicsTriggers".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_MechAndPsyLinks".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_link_addedMechlinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_addedMechlinkWithGene, "WVC_ToolTip_link_addedMechlinkWithGene".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_link_addedPsylinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_addedPsylinkWithGene, "WVC_ToolTip_link_addedPsylinkWithGene".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_link_removeMechlinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_removeMechlinkWithGene, "WVC_ToolTip_link_removeMechlinkWithGene".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_link_removePsylinkWithGene".Translate().Colorize(ColorLibrary.LightBlue), ref settings.link_removePsylinkWithGene, "WVC_ToolTip_link_removePsylinkWithGene".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Hemogenic".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_bloodeater_SafeBloodfeed".Translate(), ref settings.bloodeater_SafeBloodfeed, "WVC_ToolTip_bloodeater_SafeBloodfeed".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_bloodeater_disableAutoFeed".Translate().Colorize(ColorLibrary.LightBlue), ref settings.bloodeater_disableAutoFeed, "WVC_ToolTip_bloodeater_disableAutoFeed".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_bloodfeeder_AutoBloodfeed".Translate(), ref settings.bloodfeeder_AutoBloodfeed, "WVC_ToolTip_bloodfeeder_AutoBloodfeed".Translate());
			listingStandard.SliderLabeledWithRef("WVC_Label_hemogenic_ImplanterFangsChanceFactor".Translate((settings.hemogenic_ImplanterFangsChanceFactor * 100f).ToString()), ref settings.hemogenic_ImplanterFangsChanceFactor, 0f, 10f, null, 2);
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_XenotypeGestator".Translate() + ":", -1);
			listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationTimeFactor".Translate((settings.xenotypeGestator_GestationTimeFactor * 100f).ToString()), ref settings.xenotypeGestator_GestationTimeFactor, 0f, 2f);
			listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationMatchPercent".Translate((settings.xenotypeGestator_GestationMatchPercent * 100f).ToString()), ref settings.xenotypeGestator_GestationMatchPercent, 0f, 1f);
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Undead".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_canNonPlayerPawnResurrect".Translate().Colorize(ColorLibrary.LightBlue), ref settings.canNonPlayerPawnResurrect, "WVC_ToolTip_canNonPlayerPawnResurrect".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_allowShapeshiftAfterDeath".Translate().Colorize(ColorLibrary.LightBlue), ref settings.allowShapeshiftAfterDeath, "WVC_ToolTip_allowShapeshiftAfterDeath".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Thralls".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_ThrallMaker_ThrallsInheritMasterGenes".Translate().Colorize(ColorLibrary.LightBlue), ref settings.thrallMaker_ThrallsInheritMasterGenes, "WVC_ToolTip_ThrallMaker_ThrallsInheritMasterGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableInstabilityLastChanceMechanic".Translate(), ref settings.enableInstabilityLastChanceMechanic, "WVC_ToolTip_enableInstabilityLastChanceMechanic".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Reincarnation".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_reincarnation_EnableMechanic".Translate().Colorize(ColorLibrary.LightBlue), ref settings.reincarnation_EnableMechanic, "WVC_ToolTip_reincarnation_EnableMechanic".Translate());
			listingStandard.SliderLabeledWithRef("WVC_Label_Reincarnation_MinChronoAge".Translate((settings.reincarnation_MinChronoAge).ToString()), ref settings.reincarnation_MinChronoAge, 50f, 2000f, round: 0);
			listingStandard.SliderLabeledWithRef("WVC_Label_reincarnation_Chance".Translate((settings.reincarnation_Chance * 100f).ToString()), ref settings.reincarnation_Chance, 0f, 1f, round: 2);
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_TotalHealing".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_restoreBodyPartsWithFullHP".Translate().Colorize(ColorLibrary.LightBlue), ref settings.restoreBodyPartsWithFullHP, "WVC_ToolTip_restoreBodyPartsWithFullHP".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_totalHealingIgnoreScarification".Translate().Colorize(ColorLibrary.LightBlue), ref settings.totalHealingIgnoreScarification, "WVC_ToolTip_totalHealingIgnoreScarification".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Shapeshifer".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_ShapeshifterGeneUnremovable".Translate().Colorize(ColorLibrary.LightBlue), ref settings.shapeshifterGeneUnremovable, "WVC_ToolTip_ShapeshifterGeneUnremovable".Translate());
			listingStandard.SliderLabeledWithRef("WVC_Label_shapeshifer_GeneCellularRegeneration".Translate((settings.shapeshifer_GeneCellularRegeneration).ToString()), ref settings.shapeshifer_GeneCellularRegeneration, 1f, 100f, round: 0);
			// listingStandard.CheckboxLabeled("WVC_Label_shapeshifter_enableStyleButton".Translate().Colorize(ColorLibrary.LightBlue), ref settings.shapeshifter_enableStyleButton, "WVC_ToolTip_shapeshifter_enableStyleButton".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enable_chimeraMetabolismHungerFactor".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enable_chimeraMetabolismHungerFactor, "WVC_ToolTip_enable_chimeraMetabolismHungerFactor".Translate());
			listingStandard.SliderLabeledWithRef("WVC_Label_chimeraMinGeneCopyChance".Translate((settings.chimeraMinGeneCopyChance * 100f).ToString()), ref settings.chimeraMinGeneCopyChance, 0.01f, 1f, round: 2);
			listingStandard.SliderLabeledWithRef("WVC_Label_chimeraStartingGenes".Translate((settings.chimeraStartingGenes).ToString()), ref settings.chimeraStartingGenes, 0f, 50f, round: 0);
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_IncestLover".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_enableIncestLoverGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableIncestLoverGene, "WVC_ToolTip_enableIncestLoverGene".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_AcceptablePrey".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_disableNonAcceptablePreyGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableNonAcceptablePreyGenes, "WVC_ToolTip_disableNonAcceptablePreyGenes".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Telepath".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_enableHarmonyTelepathyGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableHarmonyTelepathyGene, "WVC_ToolTip_enableHarmonyTelepathyGene".Translate());
			listingStandard.GapLine();
			// =
			// listingStandard.Label("WVC_XaGGeneSettings_Dustogenic".Translate() + ":", -1);
			// listingStandard.CheckboxLabeled("WVC_Label_useAlternativeDustogenicFoodJob".Translate().Colorize(ColorLibrary.LightBlue), ref settings.useAlternativeDustogenicFoodJob, "WVC_ToolTip_useAlternativeDustogenicFoodJob".Translate());
			// listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_TelepathStudy".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_learningTelepathWorkForBothSides".Translate().Colorize(ColorLibrary.LightBlue), ref settings.learningTelepathWorkForBothSides, "WVC_ToolTip_learningTelepathWorkForBothSides".Translate());
			listingStandard.GapLine();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_DryadQueen".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_enable_dryadQueenMechanicGenerator".Translate().Colorize(ColorLibrary.LightOrange), ref settings.enable_dryadQueenMechanicGenerator, "WVC_ToolTip_enable_dryadQueenMechanicGenerator".Translate());
			// Reset Button
			listingStandard.GapLine();
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_Default();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_MyChoice();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
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
			listingStandard.CheckboxLabeled("WVC_Label_disableXenotypes_Misc".Translate(), ref settings.disableXenotypes_Misc, "WVC_ToolTip_disableXenotypes_Misc".Translate());
			listingStandard.GapLine();
			// =============== Xenotypes Chance Settings ===============
			// listingStandard.GapLine();
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_Default();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					ResetSettings_MyChoice();
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			// =============== Buttons ===============
			listingStandard.End();
			Widgets.EndScrollView();
		}

		// Reset

		public static void ResetSettings_Default()
		{
			WVC_Biotech.settings.onlyXenotypesMode = false;
			// Graphic
			WVC_Biotech.settings.hideXaGGenes = false;
			WVC_Biotech.settings.disableFurGraphic = false;
			WVC_Biotech.settings.disableAllGraphic = false;
			WVC_Biotech.settings.disableUniqueGeneInterface = false;
			WVC_Biotech.settings.disableEyesGraphic = false;
			// Generator
			WVC_Biotech.settings.generateSkillGenes = true;
			WVC_Biotech.settings.generateXenotypeForceGenes = false;
			WVC_Biotech.settings.generateResourceSpawnerGenes = false;
			WVC_Biotech.settings.generateSkinHairColorGenes = false;
			// Misc
			WVC_Biotech.settings.disableUniqueXenotypeScenarios = false;
			// Fix
			WVC_Biotech.settings.fixVanillaGeneImmunityCheck = true;
			WVC_Biotech.settings.spawnXenoForcerSerumsFromTraders = true;
			// Info
			WVC_Biotech.settings.enableGenesInfo = true;
			// WVC_Biotech.settings.enableGeneSpawnerGizmo = true;
			// WVC_Biotech.settings.enableGeneWingInfo = false;
			// WVC_Biotech.settings.enableGeneBlesslinkInfo = true;
			// WVC_Biotech.settings.enableGeneUndeadInfo = false;
			// WVC_Biotech.settings.enableGeneScarifierInfo = false;
			// WVC_Biotech.settings.enableGeneInstabilityInfo = true;
			// WVC_Biotech.settings.enableGeneGauranlenConnectionInfo = true;
			// Serums
			WVC_Biotech.settings.serumsForAllXenotypes = false;
			WVC_Biotech.settings.serumsForAllXenotypes_GenBase = true;
			WVC_Biotech.settings.serumsForAllXenotypes_GenUltra = false;
			WVC_Biotech.settings.serumsForAllXenotypes_GenHybrid = false;
			WVC_Biotech.settings.serumsForAllXenotypes_Recipes = true;
			WVC_Biotech.settings.serumsForAllXenotypes_Spawners = false;
			// =
			WVC_Biotech.settings.canNonPlayerPawnResurrect = true;
			WVC_Biotech.settings.allowShapeshiftAfterDeath = true;
			// WVC_Biotech.settings.shapeshifter_enableStyleButton = true;
			WVC_Biotech.settings.enable_chimeraMetabolismHungerFactor = true;
			WVC_Biotech.settings.chimeraMinGeneCopyChance = 0.01f;
			WVC_Biotech.settings.shapeshifer_GeneCellularRegeneration = 1f;
			WVC_Biotech.settings.chimeraStartingGenes = 5f;
			// =
			WVC_Biotech.settings.thrallMaker_ThrallsInheritMasterGenes = true;
			// =
			WVC_Biotech.settings.totalHealingIgnoreScarification = true;
			WVC_Biotech.settings.restoreBodyPartsWithFullHP = false;
			// =
			WVC_Biotech.settings.shapeshifterGeneUnremovable = true;
			// =
			WVC_Biotech.settings.enableIncestLoverGene = true;
			WVC_Biotech.settings.disableNonAcceptablePreyGenes = false;
			// =
			WVC_Biotech.settings.enableHarmonyTelepathyGene = false;
			// =
			// WVC_Biotech.settings.useAlternativeDustogenicFoodJob = true;
			// =
			WVC_Biotech.settings.learningTelepathWorkForBothSides = false;
			// =
			WVC_Biotech.settings.enable_birthQualityOffsetFromGenes = true;
			WVC_Biotech.settings.xenotypeGestator_GestationTimeFactor = 1f;
			WVC_Biotech.settings.xenotypeGestator_GestationMatchPercent = 0.4f;
			// =
			WVC_Biotech.settings.reincarnation_EnableMechanic = true;
			WVC_Biotech.settings.reincarnation_MinChronoAge = 200f;
			WVC_Biotech.settings.reincarnation_Chance = 0.08f;
			// =
			WVC_Biotech.settings.harmony_EnableGenesMechanicsTriggers = true;
			WVC_Biotech.settings.bloodeater_SafeBloodfeed = false;
			WVC_Biotech.settings.bloodeater_disableAutoFeed = false;
			WVC_Biotech.settings.bloodfeeder_AutoBloodfeed = false;
			WVC_Biotech.settings.hemogenic_ImplanterFangsChanceFactor = 1f;
			// =
			WVC_Biotech.settings.enableInstabilityLastChanceMechanic = true;
			// =
			WVC_Biotech.settings.enable_dryadQueenMechanicGenerator = true;
			// =
			WVC_Biotech.settings.link_addedMechlinkWithGene = true;
			WVC_Biotech.settings.link_addedPsylinkWithGene = true;
			WVC_Biotech.settings.link_removeMechlinkWithGene = false;
			WVC_Biotech.settings.link_removePsylinkWithGene = false;
			// Extra
			WVC_Biotech.settings.genesCanTickOnlyOnMap = false;
			WVC_Biotech.settings.enable_flatGenesSpawnChances = false;
			// WVC_Biotech.settings.autoPatchVanillaArchiteImmunityGenes = false;
			// Xenotypes
			WVC_Biotech.settings.enable_spawnXenotypesInFactions = false;
			WVC_Biotech.settings.disableXenotypes_MainSwitch = false;
			WVC_Biotech.settings.disableXenotypes_Undeads = false;
			WVC_Biotech.settings.disableXenotypes_Psycasters = false;
			WVC_Biotech.settings.disableXenotypes_Mechalike = false;
			WVC_Biotech.settings.disableXenotypes_GolemMasters = false;
			WVC_Biotech.settings.disableXenotypes_Bloodeaters = false;
			WVC_Biotech.settings.disableXenotypes_Misc = false;
			// PregnantHuman
			// WVC_Biotech.settings.pregnantHuman_TrueParentGenesPatch = false;
			// WVC_Biotech.settings.pregnantHuman_InheritSurrogateGenes = false;
			// WVC_Biotech.settings.pregnantHuman_InheritArchiteGenes = false;
			// XenotypesSettings
			WVC_Biotech.cachedXenotypesFilter.Clear();
			XaG_PostInitialization.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
			// Initial
			WVC_Biotech.settings.firstModLaunch = false;
			WVC_Biotech.settings.Write();
		}

		public static void ResetSettings_MyChoice()
		{
			WVC_Biotech.settings.onlyXenotypesMode = false;
			// Graphic
			WVC_Biotech.settings.hideXaGGenes = true;
			WVC_Biotech.settings.disableFurGraphic = false;
			WVC_Biotech.settings.disableAllGraphic = false;
			WVC_Biotech.settings.disableUniqueGeneInterface = false;
			WVC_Biotech.settings.disableEyesGraphic = false;
			// Generator
			WVC_Biotech.settings.generateSkillGenes = true;
			WVC_Biotech.settings.generateXenotypeForceGenes = false;
			WVC_Biotech.settings.generateResourceSpawnerGenes = false;
			WVC_Biotech.settings.generateSkinHairColorGenes = false;
			// Misc
			WVC_Biotech.settings.disableUniqueXenotypeScenarios = false;
			// Fix
			WVC_Biotech.settings.fixVanillaGeneImmunityCheck = true;
			WVC_Biotech.settings.spawnXenoForcerSerumsFromTraders = true;
			// Info
			WVC_Biotech.settings.enableGenesInfo = true;
			// WVC_Biotech.settings.enableGeneSpawnerGizmo = true;
			// WVC_Biotech.settings.enableGeneWingInfo = false;
			// WVC_Biotech.settings.enableGeneBlesslinkInfo = true;
			// WVC_Biotech.settings.enableGeneUndeadInfo = false;
			// WVC_Biotech.settings.enableGeneScarifierInfo = false;
			// WVC_Biotech.settings.enableGeneInstabilityInfo = true;
			// WVC_Biotech.settings.enableGeneGauranlenConnectionInfo = true;
			// Serums
			WVC_Biotech.settings.serumsForAllXenotypes = false;
			WVC_Biotech.settings.serumsForAllXenotypes_GenBase = false;
			WVC_Biotech.settings.serumsForAllXenotypes_GenUltra = false;
			WVC_Biotech.settings.serumsForAllXenotypes_GenHybrid = false;
			WVC_Biotech.settings.serumsForAllXenotypes_Recipes = false;
			WVC_Biotech.settings.serumsForAllXenotypes_Spawners = false;
			// =
			WVC_Biotech.settings.canNonPlayerPawnResurrect = true;
			WVC_Biotech.settings.allowShapeshiftAfterDeath = true;
			// WVC_Biotech.settings.shapeshifter_enableStyleButton = true;
			WVC_Biotech.settings.enable_chimeraMetabolismHungerFactor = true;
			WVC_Biotech.settings.chimeraMinGeneCopyChance = 0.01f;
			WVC_Biotech.settings.shapeshifer_GeneCellularRegeneration = 1f;
			WVC_Biotech.settings.chimeraStartingGenes = 5f;
			// =
			WVC_Biotech.settings.thrallMaker_ThrallsInheritMasterGenes = true;
			// =
			WVC_Biotech.settings.totalHealingIgnoreScarification = true;
			WVC_Biotech.settings.restoreBodyPartsWithFullHP = false;
			// =
			WVC_Biotech.settings.shapeshifterGeneUnremovable = true;
			// =
			WVC_Biotech.settings.enableIncestLoverGene = true;
			WVC_Biotech.settings.disableNonAcceptablePreyGenes = false;
			// =
			WVC_Biotech.settings.enableHarmonyTelepathyGene = false;
			// =
			// WVC_Biotech.settings.useAlternativeDustogenicFoodJob = true;
			// =
			WVC_Biotech.settings.learningTelepathWorkForBothSides = true;
			// =
			WVC_Biotech.settings.enable_birthQualityOffsetFromGenes = true;
			WVC_Biotech.settings.xenotypeGestator_GestationTimeFactor = 1f;
			WVC_Biotech.settings.xenotypeGestator_GestationMatchPercent = 0.4f;
			// =
			WVC_Biotech.settings.reincarnation_EnableMechanic = true;
			WVC_Biotech.settings.reincarnation_MinChronoAge = 500f;
			WVC_Biotech.settings.reincarnation_Chance = 0.08f;
			// =
			WVC_Biotech.settings.harmony_EnableGenesMechanicsTriggers = true;
			WVC_Biotech.settings.bloodeater_SafeBloodfeed = false;
			WVC_Biotech.settings.bloodeater_disableAutoFeed = false;
			WVC_Biotech.settings.bloodfeeder_AutoBloodfeed = true;
			WVC_Biotech.settings.hemogenic_ImplanterFangsChanceFactor = 1f;
			// =
			WVC_Biotech.settings.enableInstabilityLastChanceMechanic = true;
			// =
			WVC_Biotech.settings.enable_dryadQueenMechanicGenerator = true;
			// =
			WVC_Biotech.settings.link_addedMechlinkWithGene = true;
			WVC_Biotech.settings.link_addedPsylinkWithGene = true;
			WVC_Biotech.settings.link_removeMechlinkWithGene = false;
			WVC_Biotech.settings.link_removePsylinkWithGene = false;
			// =
			// Extra
			WVC_Biotech.settings.genesCanTickOnlyOnMap = false;
			WVC_Biotech.settings.enable_flatGenesSpawnChances = false;
			// WVC_Biotech.settings.autoPatchVanillaArchiteImmunityGenes = false;
			// Xenotypes
			WVC_Biotech.settings.enable_spawnXenotypesInFactions = false;
			WVC_Biotech.settings.disableXenotypes_MainSwitch = false;
			WVC_Biotech.settings.disableXenotypes_Undeads = false;
			WVC_Biotech.settings.disableXenotypes_Psycasters = false;
			WVC_Biotech.settings.disableXenotypes_Mechalike = false;
			WVC_Biotech.settings.disableXenotypes_GolemMasters = false;
			WVC_Biotech.settings.disableXenotypes_Bloodeaters = false;
			WVC_Biotech.settings.disableXenotypes_Misc = false;
			// PregnantHuman
			// WVC_Biotech.settings.pregnantHuman_TrueParentGenesPatch = false;
			// WVC_Biotech.settings.pregnantHuman_InheritSurrogateGenes = false;
			// WVC_Biotech.settings.pregnantHuman_InheritArchiteGenes = false;
			// XenotypesSettings
			WVC_Biotech.cachedXenotypesFilter.Clear();
			XaG_PostInitialization.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
			// Initial
			WVC_Biotech.settings.firstModLaunch = false;
			WVC_Biotech.settings.Write();
		}

	}

}
