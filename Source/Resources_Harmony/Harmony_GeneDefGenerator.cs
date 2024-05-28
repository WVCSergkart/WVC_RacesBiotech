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
		public static class Patch_GeneDefGenerator_ImpliedGeneDefs
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
							geneDefList.Add(GeneratorUtility.GetFromTemplate_Skills(template, skillDef, skillDef.index * 1000));
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
							geneDefList.Add(GeneratorUtility.GetFromTemplate_XenotypeForcer(template, allDef, allDef.index * 1000));
						}
					}
				}
				if (WVC_Biotech.settings.generateResourceSpawnerGenes)
				{
					foreach (SpawnerGeneTemplateDef template in DefDatabase<SpawnerGeneTemplateDef>.AllDefsListForReading)
					{
						foreach (ThingDef allDef in DefDatabase<ThingDef>.AllDefsListForReading)
						{
							if (allDef.stuffProps != null && allDef.stackLimit > 0)
							{
								geneDefList.Add(GeneratorUtility.GetFromTemplate_SpawnerGenes_Resources(template, allDef, allDef.index * 1000));
							}
						}
					}
				}
				if (WVC_Biotech.settings.generateSkinHairColorGenes)
				{
					foreach (ColorGeneTemplateDef template in DefDatabase<ColorGeneTemplateDef>.AllDefsListForReading)
					{
						foreach (ThingDef allDef in DefDatabase<ThingDef>.AllDefsListForReading)
						{
							if (allDef.stuffProps != null && allDef.stuffProps.color != null)
							{
								geneDefList.Add(GeneratorUtility.GetFromTemplate_SkinHairColorGenes_FromResources(template, allDef, allDef.index * 1000));
							}
						}
					}
				}
				// GeneratorUtility.GauranlenTreeModeDef();
				return geneDefList;
			}
		}

	}

}
