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
        public bool disableFurGraphic = false;
        public bool disableAllGraphic = false;
        public bool disableUniqueGeneInterface = false;
        // Genes
        public bool generateSkillGenes = true;
        public bool generateXenotypeForceGenes = true;
        public bool canNonPlayerPawnResurrect = false;
        public bool allowShapeshiftAfterDeath = true;
        // Fix
        public bool minWastepacksPerRecharge = false;
        public bool validatorAbilitiesPatch = true;
        // Info
        public bool enableGenesInfo = true;
        public bool enableGeneSpawnerGizmo = true;
        public bool enableGeneWingInfo = false;
        public bool enableGeneBlesslinkInfo = true;
        public bool enableGeneUndeadInfo = false;
        public bool enableGeneScarifierInfo = false;
        // Serums
        public bool serumsForAllXenotypes = true;
        public bool serumsForAllXenotypes_GenBase = true;
        public bool serumsForAllXenotypes_GenUltra = false;
        public bool serumsForAllXenotypes_GenHybrid = false;
        public bool serumsForAllXenotypes_Recipes = true;
        public bool serumsForAllXenotypes_Spawners = false;

        public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
                                                         where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
                                                         select specificSetting.Name;

		public override void ExposeData()
		{
			// Graphic
			Scribe_Values.Look(ref disableFurGraphic, "disableFurGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableAllGraphic, "disableAllGraphic", defaultValue: false);
			Scribe_Values.Look(ref disableUniqueGeneInterface, "disableUniqueGeneInterface", defaultValue: false);
			// Genes
			Scribe_Values.Look(ref generateSkillGenes, "generateSkillGenes", defaultValue: true);
			Scribe_Values.Look(ref generateXenotypeForceGenes, "generateXenotypeForceGenes", defaultValue: true);
			Scribe_Values.Look(ref canNonPlayerPawnResurrect, "canNonPlayerPawnResurrect", defaultValue: false);
			Scribe_Values.Look(ref allowShapeshiftAfterDeath, "allowShapeshiftAfterDeath", defaultValue: true);
			// Fix
			Scribe_Values.Look(ref minWastepacksPerRecharge, "minWastepacksPerRecharge", defaultValue: false);
			Scribe_Values.Look(ref validatorAbilitiesPatch, "validatorAbilitiesPatch", defaultValue: true);
			// Info
			Scribe_Values.Look(ref enableGenesInfo, "enableGenesInfo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneSpawnerGizmo, "enableGeneSpawnerGizmo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneWingInfo, "enableGeneWingInfo", defaultValue: false);
			Scribe_Values.Look(ref enableGeneBlesslinkInfo, "enableGeneBlesslinkInfo", defaultValue: true);
			Scribe_Values.Look(ref enableGeneUndeadInfo, "enableGeneUndeadInfo", defaultValue: false);
			Scribe_Values.Look(ref enableGeneScarifierInfo, "enableGeneScarifierInfo", defaultValue: false);
			// Serums
			Scribe_Values.Look(ref serumsForAllXenotypes, "serumsForAllXenotypes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenBase, "serumsForAllXenotypes_GenBase", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenUltra, "serumsForAllXenotypes_GenUltra", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_GenHybrid, "serumsForAllXenotypes_GenHybrid", defaultValue: false);
			Scribe_Values.Look(ref serumsForAllXenotypes_Recipes, "serumsForAllXenotypes_Recipes", defaultValue: true);
			Scribe_Values.Look(ref serumsForAllXenotypes_Spawners, "serumsForAllXenotypes_Spawners", defaultValue: false);
			// End
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
            Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
            // Rect rect = new(0f, 0f, inRect.width, inRect.height);
            Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2);
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
            Listing_Standard listingStandard = new();
            listingStandard.Begin(rect);
            // ===============
            listingStandard.Label("WVC_BiotechSettings_Label_Graphics".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Graphics".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_disableFurGraphic".Translate(), ref settings.disableFurGraphic, "WVC_ToolTip_disableFurGraphic".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_disableAllGraphic".Translate(), ref settings.disableAllGraphic, "WVC_ToolTip_disableAllGraphic".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_disableUniqueGeneInterface".Translate(), ref settings.disableUniqueGeneInterface, "WVC_ToolTip_disableUniqueGeneInterface".Translate());
            // ===============
            listingStandard.Gap();
            listingStandard.Label("WVC_BiotechSettings_Label_Info".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Info".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGenesInfo".Translate(), ref settings.enableGenesInfo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGeneSpawnerGizmo".Translate(), ref settings.enableGeneSpawnerGizmo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGeneWingInfo".Translate(), ref settings.enableGeneWingInfo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGeneBlesslinkInfo".Translate(), ref settings.enableGeneBlesslinkInfo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGeneUndeadInfo".Translate(), ref settings.enableGeneUndeadInfo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_enableGeneScarifierInfo".Translate(), ref settings.enableGeneScarifierInfo, "WVC_ToolTip_enableGenesInfo".Translate());
            listingStandard.Gap();
            // ===============
            listingStandard.Label("WVC_BiotechSettings_Label_Genes".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Genes".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_generateSkillGenes".Translate(), ref settings.generateSkillGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_generateXenotypeForceGenes".Translate(), ref settings.generateXenotypeForceGenes, "WVC_ToolTip_generateTemplateGenes".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_canNonPlayerPawnResurrect".Translate(), ref settings.canNonPlayerPawnResurrect, "WVC_ToolTip_canNonPlayerPawnResurrect".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_allowShapeshiftAfterDeath".Translate(), ref settings.allowShapeshiftAfterDeath, "WVC_ToolTip_allowShapeshiftAfterDeath".Translate());
            listingStandard.Gap();
            // ===============
            listingStandard.Label("WVC_BiotechSettings_Label_Other".Translate() + ":", -1, "WVC_BiotechSettings_Tooltip_Other".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_minWastepacksPerRecharge".Translate(), ref settings.minWastepacksPerRecharge, "WVC_ToolTip_minWastepacksPerRecharge".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_validatorAbilitiesPatch".Translate(), ref settings.validatorAbilitiesPatch, "WVC_ToolTip_validatorAbilitiesPatch".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes".Translate(), ref settings.serumsForAllXenotypes, "WVC_ToolTip_serumsForAllXenotypes".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenBase".Translate(), ref settings.serumsForAllXenotypes_GenBase, "WVC_ToolTip_serumsForAllXenotypes_GenBase".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenUltra".Translate(), ref settings.serumsForAllXenotypes_GenUltra, "WVC_ToolTip_serumsForAllXenotypes_GenUltra".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_GenHybrid".Translate(), ref settings.serumsForAllXenotypes_GenHybrid, "WVC_ToolTip_serumsForAllXenotypes_GenHybrid".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsForAllXenotypes_Recipes".Translate(), ref settings.serumsForAllXenotypes_Recipes, "WVC_ToolTip_serumsForAllXenotypes_Recipes".Translate());
            listingStandard.CheckboxLabeled("WVC_Label_serumsSpawnersForAllXenotypes".Translate(), ref settings.serumsForAllXenotypes_Spawners, "WVC_ToolTip_serumsSpawnersForAllXenotypes".Translate());
            listingStandard.End();
            Widgets.EndScrollView();
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
