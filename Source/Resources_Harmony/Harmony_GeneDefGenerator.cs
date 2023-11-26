using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
// using static RimWorld.BaseGen.SymbolStack;

namespace WVC_XenotypesAndGenes
{
    namespace HarmonyPatches
    {

        [HarmonyPatch(typeof(GeneDefGenerator), "ImpliedGeneDefs")]
        public static class WVC_Genes_GeneDefGenerator_ImpliedGeneDefs_Patch
        {
            [HarmonyPostfix]
            public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
            {
                List<GeneDef> geneDefList = values.ToList();
                if (WVC_Biotech.settings.generateSkillGenes)
                {
                    foreach (SkillsGeneTemplateDef template in DefDatabase<SkillsGeneTemplateDef>.AllDefsListForReading)
                    {
                        List<SkillDef> skillDefs = DefDatabase<SkillDef>.AllDefsListForReading;
                        foreach (SkillDef skillDef in skillDefs)
                        {
                            geneDefList.Add(TemplatesUtility.GetFromTemplate_Skills(template, skillDef, skillDef.index * 1000));
                        }
                    }
                }
                // foreach (InheritableImmuneGeneTemplateDef template in DefDatabase<InheritableImmuneGeneTemplateDef>.AllDefsListForReading)
                // {
                    // geneDefList.Add(TemplatesUtility.GetFromTemplate_InheritableImmune(template));
                // }
                if (WVC_Biotech.settings.generateXenotypeForceGenes)
                {
                    foreach (XenotypeForcerGeneTemplateDef template in DefDatabase<XenotypeForcerGeneTemplateDef>.AllDefsListForReading)
                    {
                        foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true, true))
                        {
                            geneDefList.Add(TemplatesUtility.GetFromTemplate_XenotypeForcer(template, allDef, allDef.index * 1000));
                        }
                    }
                }
                return geneDefList;
            }
        }

    }

}
