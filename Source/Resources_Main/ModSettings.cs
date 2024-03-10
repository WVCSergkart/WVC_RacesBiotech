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
		// Graphic
		public bool hideXaGGenes = false;
		public bool disableFurGraphic = false;
		public bool disableAllGraphic = false;
		public bool disableUniqueGeneInterface = false;
		// Generator
		public bool generateSkillGenes = true;
		public bool generateXenotypeForceGenes = false;
		public bool generateResourceSpawnerGenes = false;
		public bool generateSkinHairColorGenes = false;
		// Genes
		public bool canNonPlayerPawnResurrect = false;
		public bool allowShapeshiftAfterDeath = true;
		public bool totalHealingIgnoreScarification = true;
		// public bool genesRemoveMechlinkUponDeath = false;
		// public bool enableCustomMechLinkName = false;
		public bool shapeshifterGeneUnremovable = false;
		public bool enableIncestLoverGene = true;
		public bool enableHarmonyTelepathyGene = false;
		public bool useAlternativeDustogenicFoodJob = false;
		public bool learningTelepathWorkForBothSides = false;
		public bool disableUniqueXenotypeScenarios = false;
		public bool restoreBodyPartsWithFullHP = false;
		// public bool reimplantResurrectionRecruiting = false;
		// Info
		public bool enableGenesInfo = true;
		public bool enableGeneSpawnerGizmo = true;
		public bool enableGeneWingInfo = false;
		public bool enableGeneBlesslinkInfo = true;
		public bool enableGeneUndeadInfo = false;
		public bool enableGeneScarifierInfo = false;
		public bool enableGeneInstabilityInfo = true;
		public bool enableGolemsInfo = true;
		// Serums
		public bool serumsForAllXenotypes = false;
		public bool serumsForAllXenotypes_GenBase = true;
		public bool serumsForAllXenotypes_GenUltra = false;
		public bool serumsForAllXenotypes_GenHybrid = false;
		public bool serumsForAllXenotypes_Recipes = true;
		public bool serumsForAllXenotypes_Spawners = false;
		// ExtraSettings
		public bool genesCanTickOnlyOnMap = false;
		// Fix
		public bool fixVanillaGeneImmunityCheck = true;
		public bool spawnXenoForcerSerumsFromTraders = true;
		public bool fixGenesOnLoad = false;
		public bool fixGeneAbilitiesOnLoad = false;
		public bool fixGeneTypesOnLoad = false;
		// Gestator
		public float xenotypeGestator_GestationTimeFactor = 1f;
		public float xenotypeGestator_GestationMatchPercent = 0.4f;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
														 where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
														 select specificSetting.Name;

		public override void ExposeData()
		{
			base.ExposeData();
			// Graphic
			Scribe_Values.Look(ref hideXaGGenes, "hideXaGGenes", defaultValue: false);
			Scribe_Values.Look(ref disableFurGraphic, "disableFurGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableAllGraphic, "disableAllGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueGeneInterface, "disableUniqueGeneInterface", defaultValue: false);
			// Generator
			Scribe_Values.Look(ref generateSkillGenes, "generateSkillGenes", defaultValue: true);
			Scribe_Values.Look(ref generateXenotypeForceGenes, "generateXenotypeForceGenes", defaultValue: false);
			Scribe_Values.Look(ref generateResourceSpawnerGenes, "generateResourceSpawnerGenes", defaultValue: false);
			Scribe_Values.Look(ref generateSkinHairColorGenes, "generateSkinHairColorGenes", defaultValue: false);
			// Genes
			Scribe_Values.Look(ref canNonPlayerPawnResurrect, "canNonPlayerPawnResurrect", defaultValue: false);
			Scribe_Values.Look(ref allowShapeshiftAfterDeath, "allowShapeshiftAfterDeath", defaultValue: true);
			Scribe_Values.Look(ref totalHealingIgnoreScarification, "totalHealingIgnoreScarification", defaultValue: true);
			// Scribe_Values.Look(ref genesRemoveMechlinkUponDeath, "genesRemoveMechlinkUponDeath", defaultValue: false);
			// Scribe_Values.Look(ref enableCustomMechLinkName, "enableCustomMechLinkName", defaultValue: false);
			Scribe_Values.Look(ref shapeshifterGeneUnremovable, "shapeshifterGeneUnremovable", defaultValue: false);
			Scribe_Values.Look(ref enableIncestLoverGene, "enableIncestLoverGene", defaultValue: true);
			Scribe_Values.Look(ref enableHarmonyTelepathyGene, "enableHarmonyTelepathyGene", defaultValue: false);
			Scribe_Values.Look(ref useAlternativeDustogenicFoodJob, "useAlternativeDustogenicFoodJob", defaultValue: false);
			Scribe_Values.Look(ref learningTelepathWorkForBothSides, "learningTelepathWorkForBothSides", defaultValue: false);
			Scribe_Values.Look(ref restoreBodyPartsWithFullHP, "restoreBodyPartsWithFullHP", defaultValue: false);
			// Scribe_Values.Look(ref reimplantResurrectionRecruiting, "reimplantResurrectionRecruiting", defaultValue: false);
			// Fix
			Scribe_Values.Look(ref fixVanillaGeneImmunityCheck, "fixVanillaGeneImmunityCheck", defaultValue: true);
			// Scribe_Values.Look(ref minWastepacksPerRecharge, "minWastepacksPerRecharge", defaultValue: false);
			// Scribe_Values.Look(ref validatorAbilitiesPatch, "validatorAbilitiesPatch", defaultValue: true);
			Scribe_Values.Look(ref spawnXenoForcerSerumsFromTraders, "spawnXenoForcerSerumsFromTraders", defaultValue: true);
			// Scribe_Values.Look(ref fixGenesOnLoad, "fixGenesOnLoad", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueXenotypeScenarios, "disableUniqueXenotypeScenarios", defaultValue: false);
			// Info
			Scribe_Values.Look(ref enableGenesInfo, "enableGenesInfo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneSpawnerGizmo, "enableGeneSpawnerGizmo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneWingInfo, "enableGeneWingInfo", defaultValue: false);
			Scribe_Values.Look(ref enableGeneBlesslinkInfo, "enableGeneBlesslinkInfo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneUndeadInfo, "enableGeneUndeadInfo", defaultValue: false);
			Scribe_Values.Look(ref enableGeneScarifierInfo, "enableGeneScarifierInfo", defaultValue: false);
			Scribe_Values.Look(ref enableGeneInstabilityInfo, "enableGeneInstabilityInfo", defaultValue: true);
			Scribe_Values.Look(ref enableGolemsInfo, "enableGolemsInfo", defaultValue: true);
			// Serums
			Scribe_Values.Look(ref serumsForAllXenotypes, "serumsForAllXenotypes", defaultValue: false, forceSave: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenBase, "serumsForAllXenotypes_GenBase", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenUltra, "serumsForAllXenotypes_GenUltra", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenHybrid, "serumsForAllXenotypes_GenHybrid", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_Recipes, "serumsForAllXenotypes_Recipes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_Spawners, "serumsForAllXenotypes_Spawners", defaultValue: false);
			// ExtraSettings
			Scribe_Values.Look(ref genesCanTickOnlyOnMap, "genesCanTickOnlyOnMap", defaultValue: false);
			// Gestator
			Scribe_Values.Look(ref xenotypeGestator_GestationTimeFactor, "xenotypeGestator_GestationTimeFactor", defaultValue: 1f);
			Scribe_Values.Look(ref xenotypeGestator_GestationMatchPercent, "xenotypeGestator_GestationMatchPercent", defaultValue: 0.4f);
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
			Rect baseRect = rect;
			rect = new Rect(inRect)
			{
				height = inRect.height - 40f,
				y = inRect.y + 40f
			};
			Rect rect2 = rect;
			Widgets.DrawMenuSection(rect2);
			List<TabRecord> tabs = new()
			{
				new TabRecord("WVC_BiotechSettings_Tab_General".Translate(), delegate
				{
					PageIndex = 0;
					WriteSettings();
				}, PageIndex == 0),
				new TabRecord("WVC_BiotechSettings_Tab_XenotypesFilter".Translate(), delegate
				{
					PageIndex = 1;
					WriteSettings();
				}, PageIndex == 1),
				new TabRecord("WVC_BiotechSettings_Tab_ExtraSettings".Translate(), delegate
				{
					PageIndex = 2;
					WriteSettings();
				}, PageIndex == 2),
				new TabRecord("WVC_BiotechSettings_Label_Genes".Translate(), delegate
				{
					PageIndex = 3;
					WriteSettings();
				}, PageIndex == 3)
			};
			TabDrawer.DrawTabs(baseRect, tabs);
			switch (PageIndex)
			{
				case 0:
					GeneralSettings(rect2.ContractedBy(15f));
					break;
				case 1:
					XenotypeFilterSettings(rect2.ContractedBy(15f));
					break;
				case 2:
					ExtraSettings(rect2.ContractedBy(15f));
					break;
				case 3:
					GenesSettings(rect2.ContractedBy(15f));
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
			// Graphic
			listingStandard.Label("WVC_BiotechSettings_Label_Graphics".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Graphics".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_hideXaGGenes".Translate().Colorize(ColorLibrary.LightPurple), ref settings.hideXaGGenes, "WVC_ToolTip_hideXaGGenes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableFurGraphic".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableFurGraphic, "WVC_ToolTip_disableFurGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableAllGraphic".Translate(), ref settings.disableAllGraphic, "WVC_ToolTip_disableAllGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableUniqueGeneInterface".Translate().Colorize(ColorLibrary.LightPurple), ref settings.disableUniqueGeneInterface, "WVC_ToolTip_disableUniqueGeneInterface".Translate());
			// Info
			listingStandard.Gap();
			listingStandard.Label("WVC_BiotechSettings_Label_Info".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Info".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGenesInfo".Translate().Colorize(ColorLibrary.LightBlue), ref settings.enableGenesInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneSpawnerGizmo".Translate(), ref settings.enableGeneSpawnerGizmo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneWingInfo".Translate(), ref settings.enableGeneWingInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneBlesslinkInfo".Translate(), ref settings.enableGeneBlesslinkInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneUndeadInfo".Translate(), ref settings.enableGeneUndeadInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneScarifierInfo".Translate(), ref settings.enableGeneScarifierInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneInstabilityInfo".Translate(), ref settings.enableGeneInstabilityInfo, "WVC_ToolTip_enableGenesInfo".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGolemsInfo".Translate(), ref settings.enableGolemsInfo, "WVC_ToolTip_enableGenesInfo".Translate());
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
			listingStandard.CheckboxLabeled("WVC_Label_disableUniqueXenotypeScenarios".Translate(), ref settings.disableUniqueXenotypeScenarios, "WVC_ToolTip_disableUniqueXenotypeScenarios".Translate());
			listingStandard.Gap();
			// Serums
			listingStandard.Label("WVC_BiotechSettings_Label_Serums".Translate().Colorize(ColoredText.SubtleGrayColor) + ":", -1, "WVC_BiotechSettings_Tooltip_Serums".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes, "WVC_ToolTip_serumsForAllXenotypes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenBase".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenBase, "WVC_ToolTip_serumsForAllXenotypes_GenBase".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenUltra".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenUltra, "WVC_ToolTip_serumsForAllXenotypes_GenUltra".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenHybrid".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_GenHybrid, "WVC_ToolTip_serumsForAllXenotypes_GenHybrid".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_Recipes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Recipes, "WVC_ToolTip_serumsForAllXenotypes_Recipes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsSpawnersForAllXenotypes".Translate().Colorize(ColoredText.SubtleGrayColor), ref settings.serumsForAllXenotypes_Spawners, "WVC_ToolTip_serumsSpawnersForAllXenotypes".Translate());
			listingStandard.GapLine();
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					// Graphic
					settings.hideXaGGenes = false;
					settings.disableFurGraphic = false;
					settings.disableAllGraphic = false;
					settings.disableUniqueGeneInterface = false;
					// Generator
					settings.generateSkillGenes = true;
					settings.generateXenotypeForceGenes = false;
					settings.generateResourceSpawnerGenes = false;
					settings.generateSkinHairColorGenes = false;
					// Misc
					settings.disableUniqueXenotypeScenarios = false;
					// Fix
					settings.fixVanillaGeneImmunityCheck = true;
					// settings.minWastepacksPerRecharge = false;
					// settings.validatorAbilitiesPatch = true;
					settings.spawnXenoForcerSerumsFromTraders = true;
					// Info
					settings.enableGenesInfo = true;
					settings.enableGeneSpawnerGizmo = true;
					settings.enableGeneWingInfo = false;
					settings.enableGeneBlesslinkInfo = true;
					settings.enableGeneUndeadInfo = false;
					settings.enableGeneScarifierInfo = false;
					settings.enableGeneInstabilityInfo = true;
					settings.enableGolemsInfo = true;
					// Serums
					settings.serumsForAllXenotypes = false;
					settings.serumsForAllXenotypes_GenBase = true;
					settings.serumsForAllXenotypes_GenUltra = false;
					settings.serumsForAllXenotypes_GenHybrid = false;
					settings.serumsForAllXenotypes_Recipes = true;
					settings.serumsForAllXenotypes_Spawners = false;
					// XenotypesSettings
					cachedXenotypesFilter.Clear();
					XenotypesFilterStartup.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
					// Message
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					// Graphic
					settings.hideXaGGenes = true;
					settings.disableFurGraphic = false;
					settings.disableAllGraphic = false;
					settings.disableUniqueGeneInterface = false;
					// Generator
					settings.generateSkillGenes = true;
					settings.generateXenotypeForceGenes = false;
					settings.generateResourceSpawnerGenes = false;
					settings.generateSkinHairColorGenes = false;
					// Misc
					settings.disableUniqueXenotypeScenarios = false;
					// Fix
					settings.fixVanillaGeneImmunityCheck = true;
					// settings.minWastepacksPerRecharge = false;
					// settings.validatorAbilitiesPatch = true;
					settings.spawnXenoForcerSerumsFromTraders = true;
					// Info
					settings.enableGenesInfo = true;
					settings.enableGeneSpawnerGizmo = true;
					settings.enableGeneWingInfo = false;
					settings.enableGeneBlesslinkInfo = true;
					settings.enableGeneUndeadInfo = false;
					settings.enableGeneScarifierInfo = false;
					settings.enableGeneInstabilityInfo = true;
					settings.enableGolemsInfo = true;
					// Serums
					settings.serumsForAllXenotypes = false;
					settings.serumsForAllXenotypes_GenBase = false;
					settings.serumsForAllXenotypes_GenUltra = false;
					settings.serumsForAllXenotypes_GenHybrid = false;
					settings.serumsForAllXenotypes_Recipes = false;
					settings.serumsForAllXenotypes_Spawners = false;
					// XenotypesSettings
					cachedXenotypesFilter.Clear();
					XenotypesFilterStartup.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
					// Message
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
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
				XenotypesFilterStartup.SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
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
					bool value = cachedXenotypesFilter[def.defName];
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
			listingStandard.Gap();
			// =============== Dev ===============
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGenesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGenesOnLoad, "WVC_ToolTip_fixGenesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneAbilitiesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneAbilitiesOnLoad, "WVC_ToolTip_fixGeneAbilitiesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.CheckboxLabeled("DEV: ".Colorize(ColorLibrary.RedReadable) + "WVC_Label_fixGeneTypesOnLoad".Translate().Colorize(ColorLibrary.LightPink), ref settings.fixGeneTypesOnLoad, "WVC_ToolTip_fixGeneTypesOnLoad".Translate() + "\n\n" + "WVC_Alert_fixBrokenShit".Translate());
			listingStandard.GapLine();
			// =============== Buttons ===============
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					// Extra
					settings.genesCanTickOnlyOnMap = false;
					// Message
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
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2.0f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// =
			listingStandard.Label("WVC_XaGGeneSettings_XenotypeGestator".Translate() + ":", -1);
			listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationTimeFactor".Translate((settings.xenotypeGestator_GestationTimeFactor * 100f).ToString()), ref settings.xenotypeGestator_GestationTimeFactor, 0f, 2f);
			listingStandard.SliderLabeledWithRef("WVC_Label_xenotypeGestator_GestationMatchPercent".Translate((settings.xenotypeGestator_GestationMatchPercent * 100f).ToString()), ref settings.xenotypeGestator_GestationMatchPercent, 0f, 1f);
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Undead".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_canNonPlayerPawnResurrect".Translate().Colorize(ColorLibrary.LightBlue), ref settings.canNonPlayerPawnResurrect, "WVC_ToolTip_canNonPlayerPawnResurrect".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_allowShapeshiftAfterDeath".Translate().Colorize(ColorLibrary.LightBlue), ref settings.allowShapeshiftAfterDeath, "WVC_ToolTip_allowShapeshiftAfterDeath".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_TotalHealing".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_restoreBodyPartsWithFullHP".Translate().Colorize(ColorLibrary.LightBlue), ref settings.restoreBodyPartsWithFullHP, "WVC_ToolTip_restoreBodyPartsWithFullHP".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_totalHealingIgnoreScarification".Translate().Colorize(ColorLibrary.LightBlue), ref settings.totalHealingIgnoreScarification, "WVC_ToolTip_totalHealingIgnoreScarification".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Shapeshifer".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_ShapeshifterGeneUnremovable".Translate().Colorize(ColorLibrary.LightBlue), ref settings.shapeshifterGeneUnremovable, "WVC_ToolTip_ShapeshifterGeneUnremovable".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_IncestLover".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_enableIncestLoverGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableIncestLoverGene, "WVC_ToolTip_enableIncestLoverGene".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Telepath".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_enableHarmonyTelepathyGene".Translate().Colorize(ColorLibrary.LightPurple), ref settings.enableHarmonyTelepathyGene, "WVC_ToolTip_enableHarmonyTelepathyGene".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_Dustogenic".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_useAlternativeDustogenicFoodJob".Translate().Colorize(ColorLibrary.LightBlue), ref settings.useAlternativeDustogenicFoodJob, "WVC_ToolTip_useAlternativeDustogenicFoodJob".Translate());
			listingStandard.Gap();
			// =
			listingStandard.Label("WVC_XaGGeneSettings_TelepathStudy".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_learningTelepathWorkForBothSides".Translate().Colorize(ColorLibrary.LightBlue), ref settings.learningTelepathWorkForBothSides, "WVC_ToolTip_learningTelepathWorkForBothSides".Translate());
			// Reset Button
			listingStandard.GapLine();
			if (listingStandard.ButtonText("WVC_XaG_ResetButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					// =
					settings.canNonPlayerPawnResurrect = false;
					settings.allowShapeshiftAfterDeath = true;
					// =
					settings.totalHealingIgnoreScarification = true;
					settings.restoreBodyPartsWithFullHP = false;
					// =
					settings.shapeshifterGeneUnremovable = false;
					// =
					settings.enableIncestLoverGene = true;
					// =
					settings.enableHarmonyTelepathyGene = false;
					// =
					settings.useAlternativeDustogenicFoodJob = false;
					// =
					settings.learningTelepathWorkForBothSides = false;
					// =
					settings.xenotypeGestator_GestationTimeFactor = 1f;
					settings.xenotypeGestator_GestationMatchPercent = 0.4f;
					// Message
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			if (listingStandard.ButtonText("WVC_XaG_ModDeveloperRecommendationButton".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("WVC_XaG_ResetButtonWarning".Translate(), delegate
				{
					// =
					settings.canNonPlayerPawnResurrect = false;
					settings.allowShapeshiftAfterDeath = true;
					// =
					settings.totalHealingIgnoreScarification = true;
					settings.restoreBodyPartsWithFullHP = false;
					// =
					settings.shapeshifterGeneUnremovable = true;
					// =
					settings.enableIncestLoverGene = true;
					// =
					settings.enableHarmonyTelepathyGene = false;
					// =
					settings.useAlternativeDustogenicFoodJob = true;
					// =
					settings.learningTelepathWorkForBothSides = true;
					// =
					settings.xenotypeGestator_GestationTimeFactor = 1f;
					settings.xenotypeGestator_GestationMatchPercent = 0.4f;
					// Message
					Messages.Message("WVC_XaG_ResetButton_SettingsChanged".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				});
				Find.WindowStack.Add(window);
			}
			listingStandard.End();
			Widgets.EndScrollView();
		}

	}

}
