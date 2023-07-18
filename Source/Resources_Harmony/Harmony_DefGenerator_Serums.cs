using System;
using System.Collections.Generic;
// using System.Collections.IEnumerable;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
// using static RimWorld.BaseGen.SymbolStack;
using WVC;
using WVC_XenotypesAndGenes;


namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(ThingDefGenerator_Neurotrainer), "ImpliedThingDefs")]
		public static class WVC_ThingDefGenerator_Neurotrainer_ImpliedThingDefs_Patch
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
