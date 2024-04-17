using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
// using System.Collections.IEnumerable;
using System.Linq;
using Verse;
// using static RimWorld.BaseGen.SymbolStack;


namespace WVC_XenotypesAndGenes
{
    namespace HarmonyPatches
    {

        [Obsolete]
        [HarmonyPatch(typeof(ThingDefGenerator_Neurotrainer), "ImpliedThingDefs")]
        public static class Patch_ThingDefGenerator_Neurotrainer_ImpliedThingDefs
        {
            [HarmonyPostfix]
            public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values)
            {
                List<ThingDef> thingDefList = values.ToList();
                if (WVC_Biotech.settings.serumsForAllXenotypes)
                {
                    foreach (SerumTemplateDef serumTemplate in DefDatabase<SerumTemplateDef>.AllDefsListForReading)
                    {
                        if (serumTemplate.serumTagName.Contains("BaseSerumsGenerator") && WVC_Biotech.settings.serumsForAllXenotypes_GenBase)
                        {
                            foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
                            {
                                thingDefList.Add(TemplatesUtility.GetFromThingTemplate(serumTemplate, allDef, allDef.index * 1000));
                            }
                        }
                        if (serumTemplate.serumTagName.Contains("UltraSerumsGenerator") && WVC_Biotech.settings.serumsForAllXenotypes_GenUltra)
                        {
                            foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
                            {
                                thingDefList.Add(TemplatesUtility.GetFromThingTemplate(serumTemplate, allDef, allDef.index * 1000));
                            }
                        }
                        if (serumTemplate.serumTagName.Contains("HybridSerumsGenerator") && WVC_Biotech.settings.serumsForAllXenotypes_GenHybrid)
                        {
                            foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
                            {
                                thingDefList.Add(TemplatesUtility.GetFromThingTemplate(serumTemplate, allDef, allDef.index * 1000));
                            }
                        }
                        if (serumTemplate.serumTagName.Contains("AnySerum"))
                        {
                            foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
                            {
                                thingDefList.Add(TemplatesUtility.GetFromThingTemplate(serumTemplate, allDef, allDef.index * 1000));
                            }
                        }
                    }
                }
                return thingDefList;
            }
        }

    }

}
