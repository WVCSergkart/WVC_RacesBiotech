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

	[HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
	public static class WVC_DefGenerator_GenerateImpliedDefs_PreResolve_Patch
	{
		[HarmonyPostfix]
		public static void Postfix()
		{
			// foreach (CustomXenotype customXenotype in ImpliedCustomXenotypeDefs())
			// {
				// DefGenerator.AddImpliedDef(customXenotype);
			// }
			foreach (ThingDef item in ImpliedThingDefs())
			{
				DefGenerator.AddImpliedDef(item);
			}
			// foreach (RecipeDef item1 in ImpliedRecipeDefs())
			// {
				// DefGenerator.AddImpliedDef(item1);
			// }
		}

		// =================================================================

		// public static IEnumerable<CustomXenotype> ImpliedCustomXenotypeDefs()
		// {
			// if (!ModsConfig.BiotechActive || !WVC_Biotech.settings.convertCustomXenotypesIntoXenotypes || !WVC_Biotech.settings.serumsForAllXenotypes)
			// {
				// yield break;
			// }
			// foreach (SerumTemplateDef g in DefDatabase<SerumTemplateDef>.AllDefs)
			// {
				// List<string> blackListedXenotypesForSerums = new List<string>();
				// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				// {
					// blackListedXenotypesForSerums.AddRange(item.blackListedXenotypesForSerums);
				// }
				// List<XenotypeDef> genesListForReading = DefDatabase<XenotypeDef>.AllDefs.ToList();
				// List<XenotypeDef> whiteListedXenotypes = new List<XenotypeDef>();
				// for (int i = 0; i < genesListForReading.Count; i++)
				// {
					// if (!blackListedXenotypesForSerums.Contains(genesListForReading[i].defName))
					// {
						// whiteListedXenotypes.Add(genesListForReading[i]);
					// }
				// }
				// foreach (XenotypeDef allDef in whiteListedXenotypes)
				// {
					// yield return TemplatesUtility.GetFromThingTemplate(g, allDef, allDef.index * 1000);
				// }
			// }
		// }

		// =================================================================

		public static IEnumerable<ThingDef> ImpliedThingDefs()
		{
			if (!ModsConfig.BiotechActive || !WVC_Biotech.settings.serumsForAllXenotypes)
			{
				yield break;
			}
			foreach (SerumTemplateDef g in DefDatabase<SerumTemplateDef>.AllDefsListForReading)
			{
				// List<string> blackListedXenotypesForSerums = new();
				// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
				// {
					// blackListedXenotypesForSerums.AddRange(item.blackListedXenotypesForSerums);
				// }
				// foreach (string item in XenotypesFilterStartup.filterBlackListedXenotypesForSerums)
				// {
					// blackListedXenotypesForSerums.Add(item);
				// }
				// List<XenotypeDef> genesListForReading = DefDatabase<XenotypeDef>.AllDefsListForReading;
				// List<XenotypeDef> whiteListedXenotypes = new();
				// for (int i = 0; i < genesListForReading.Count; i++)
				// {
					// if (!blackListedXenotypesForSerums.Contains(genesListForReading[i].defName))
					// {
						// whiteListedXenotypes.Add(genesListForReading[i]);
					// }
				// }
				// List<GeneDef> whiteListedGenes = new List<GeneDef>();
				foreach (XenotypeDef allDef in XenotypeFilterUtility.WhiteListedXenotypes(true))
				{
					yield return TemplatesUtility.GetFromThingTemplate(g, allDef, allDef.index * 1000);
				}
			}
		}
	}

}
