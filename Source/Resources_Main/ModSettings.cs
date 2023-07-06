using System;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC
{
	public class WVC_BiotechSettings : ModSettings
	{
		public bool disableFurGraphic = false;
		// public bool mechaskinTemperatureAdaptability;
		public bool minWastepacksPerRecharge = false;
		// public bool nodeskinVX_MechBandwidth;
		public bool enableGeneSpawnerGizmo = true;
		// public bool enableStatSkillFactor;
		// public bool canMechaskinBePredatorPrey;
		// public bool hideEncodingGenes = true;
		public bool serumsForAllXenotypes = true;
		public bool serumsForAllXenotypes_GenBase = true;
		public bool serumsForAllXenotypes_GenUltra = true;
		public bool serumsForAllXenotypes_GenHybrid = true;
		public bool serumsForAllXenotypes_Recipes = true;
		public bool serumsForAllXenotypes_Spawners = false;
		// public bool convertCustomXenotypesIntoXenotypes;
		// public bool fixAgelessAge;
		// public bool mecaXenotypeIsInheritable = false;
		// public bool mechanoidizationGenesPatch = true;
		// public float serumsCraftCost_ArchitesFactor;
		// public float serumsCraftCost_MetabolismFactor;
		// public float serumsCraftCost_ComplexityFactor;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
			where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
			select specificSetting.Name;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref disableFurGraphic, "disableFurGraphic", defaultValue: false);
			// Scribe_Values.Look(ref mechaskinTemperatureAdaptability, "mechaskinTemperatureAdaptability", defaultValue: false);
			Scribe_Values.Look(ref minWastepacksPerRecharge, "minWastepacksPerRecharge", defaultValue: false);
			// Scribe_Values.Look(ref nodeskinVX_MechBandwidth, "nodeskinVX_MechBandwidth", defaultValue: true);
			Scribe_Values.Look(ref enableGeneSpawnerGizmo, "enableGeneSpawnerGizmo", defaultValue: true);
			// Scribe_Values.Look(ref enableStatSkillFactor, "enableStatSkillFactor", defaultValue: true);
			// Scribe_Values.Look(ref canMechaskinBePredatorPrey, "canMechaskinBePredatorPrey", defaultValue: true);
			// Scribe_Values.Look(ref hideEncodingGenes, "hideEncodingGenes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes, "serumsForAllXenotypes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenBase, "serumsForAllXenotypes_GenBase", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenUltra, "serumsForAllXenotypes_GenUltra", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenHybrid, "serumsForAllXenotypes_GenHybrid", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_Recipes, "serumsForAllXenotypes_Recipes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_Spawners, "serumsForAllXenotypes_Spawners", defaultValue: false);
			// Scribe_Values.Look(ref serumsCraftCost_ArchitesFactor, "serumsCraftCost_ArchitesFactor", 1f);
			// Scribe_Values.Look(ref serumsCraftCost_ComplexityFactor, "serumsCraftCost_ComplexityFactor", 1f);
			// Scribe_Values.Look(ref serumsCraftCost_MetabolismFactor, "serumsCraftCost_MetabolismFactor", 1f);
			// Scribe_Values.Look(ref convertCustomXenotypesIntoXenotypes, "convertCustomXenotypesIntoXenotypes", defaultValue: false);
			// Scribe_Values.Look(ref fixAgelessAge, "fixAgelessAge", defaultValue: false);
			// Scribe_Values.Look(ref mecaXenotypeIsInheritable, "mecaXenotypeIsInheritable", defaultValue: false);
			// Scribe_Values.Look(ref mechanoidizationGenesPatch, "mechanoidizationGenesPatch", defaultValue: true);
			// base.ExposeData();
			base.ExposeData();
			Scribe_Collections.Look(ref WVC_Biotech.cachedXenotypesFilter, "cachedXenotypesFilter", LookMode.Value, LookMode.Value);
		}
	}

	public class WVC_Biotech : Mod
	{
		public static WVC_BiotechSettings settings;

		private int PageIndex = 0;

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
				}, PageIndex == 1)
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
			}
		}

		private void GeneralSettings(Rect inRect)
		{
			Listing_Standard listingStandard = new();
			listingStandard.Begin(inRect);
			// ===============
			listingStandard.Label("WVC_BiotechSettings_Label_Graphics".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Graphics".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_disableFurGraphic".Translate(), ref settings.disableFurGraphic, "WVC_ToolTip_disableFurGraphic".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_enableGeneSpawnerGizmo".Translate(), ref settings.enableGeneSpawnerGizmo, "WVC_ToolTip_enableGeneSpawnerGizmo".Translate());
			// listingStandard.None();
			listingStandard.Label("");
			// ===============
			// listingStandard.Label("WVC_BiotechSettings_Label_Genes".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Genes".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_mechaskinTemperatureAdaptability".Translate(), ref settings.mechaskinTemperatureAdaptability, "WVC_ToolTip_mechaskinTemperatureAdaptability".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_nodeskinVX_MechBandwidth".Translate(), ref settings.nodeskinVX_MechBandwidth, "WVC_ToolTip_nodeskinVX_MechBandwidth".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_canMechaskinBePredatorPrey".Translate(), ref settings.canMechaskinBePredatorPrey, "WVC_ToolTip_canMechaskinBePredatorPrey".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_hideEncodingGenes".Translate(), ref settings.hideEncodingGenes, "WVC_ToolTip_hideEncodingGenes".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_fixAgelessAge".Translate(), ref settings.fixAgelessAge, "WVC_ToolTip_fixAgelessAge".Translate());
			// listingStandard.None();
			// listingStandard.Label("");
			// ===============
			listingStandard.Label("WVC_BiotechSettings_Label_Other".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Other".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_minWastepacksPerRecharge".Translate(), ref settings.minWastepacksPerRecharge, "WVC_ToolTip_minWastepacksPerRecharge".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_enableStatSkillFactor".Translate(), ref settings.enableStatSkillFactor, "WVC_ToolTip_enableStatSkillFactor".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes".Translate(), ref settings.serumsForAllXenotypes, "WVC_ToolTip_serumsForAllXenotypes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenBase".Translate(), ref settings.serumsForAllXenotypes_GenBase);
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenUltra".Translate(), ref settings.serumsForAllXenotypes_GenUltra);
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenHybrid".Translate(), ref settings.serumsForAllXenotypes_GenHybrid);
			listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_Recipes".Translate(), ref settings.serumsForAllXenotypes_Recipes, "WVC_ToolTip_serumsForAllXenotypes_Recipes".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_serumsSpawnersForAllXenotypes".Translate(), ref settings.serumsForAllXenotypes_Spawners, "WVC_ToolTip_serumsSpawnersForAllXenotypes".Translate());
			// listingStandard.SliderLabeled("WVC_Label_serumsCraftCost_ArchitesFactor".Translate(), ref settings.serumsCraftCost_ArchitesFactor, 0, 10, 0.1f);
			// listingStandard.CheckboxLabeled("WVC_Label_convertCustomXenotypesIntoXenotypes".Translate(), ref settings.convertCustomXenotypesIntoXenotypes, "WVC_ToolTip_convertCustomXenotypesIntoXenotypes".Translate());
			// listingStandard.Label("");
			// ===============
			// listingStandard.Label("WVC_BiotechSettings_Label_Legend".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Legend".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_mecaXenotypeIsInheritable".Translate(), ref settings.mecaXenotypeIsInheritable, "WVC_ToolTip_mecaXenotypeIsInheritable".Translate());
			// listingStandard.CheckboxLabeled("WVC_Label_mechanoidizationGenesPatch".Translate(), ref settings.mechanoidizationGenesPatch, "WVC_ToolTip_mechanoidizationGenesPatch".Translate());
			listingStandard.End();
			// base.DoSettingsWindowContents(inRect);
		}

		private string searchKey;
		private static Vector2 scrollPosition = Vector2.zero;

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
					var iconRect = new Rect(outerPos.x + 5, outerPos.y + 5, 24, 24);
					Widgets.DefIcon(iconRect, def);
					// Log.Error("2");
					var labelRect = new Rect(iconRect.xMax + 15, outerPos.y + 5, viewArea.width - 80, 24f);
					Widgets.Label(labelRect, def.LabelCap);
					// Log.Error("3");
					var pos = new Vector2(viewArea.width - 40, labelRect.y);
					// Log.Error("4");
					bool value = cachedXenotypesFilter[def.defName];
					// bool value = true;
					Widgets.Checkbox(pos, ref value);
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

		public override string SettingsCategory()
		{
			return "WVC - " + "WVC_BiotechSettings".Translate();
		}
	}

	public class PatchOperationOptional : PatchOperation
	{
		public string settingName;
		public PatchOperation caseTrue;
		public PatchOperation caseFalse;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (WVC_Biotech.settings.GetEnabledSettings.Contains(settingName) != true && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}
}
