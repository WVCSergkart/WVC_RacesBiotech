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

	[HarmonyPatch(typeof(ThingDefGenerator_Neurotrainer), "ImpliedThingDefs")]
	public static class WVC_ThingDefGenerator_Neurotrainer_ImpliedThingDefs_Patch
	{
		[HarmonyPostfix]
		public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values)
		{
			List<ThingDef> thingDefList = values.ToList();
			if (WVC_Biotech.settings.serumsForAllXenotypes)
			{
				foreach (SerumTemplateDef g in DefDatabase<SerumTemplateDef>.AllDefsListForReading)
				{
					foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
					{
						thingDefList.Add(TemplatesUtility.GetFromThingTemplate(g, allDef, allDef.index * 1000));
					}
				}
			}
			return thingDefList;
		}
	}

}
