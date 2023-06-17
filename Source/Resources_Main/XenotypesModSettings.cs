using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using RimWorld;
using System;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class XenotypesFilterStartup
	{
		public static List<string> filterBlackListedXenotypesForSerums = new();
		// public static bool anyShieldItemPresent;
		static XenotypesFilterStartup()
		{
			// List<string> blackListedXenotypesForSerums = new();
			// foreach (XenotypesAndGenesListDef item in DefDatabase<XenotypesAndGenesListDef>.AllDefsListForReading)
			// {
				// blackListedXenotypesForSerums.AddRange(item.blackListedXenotypesForSerums);
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
			// anyShieldItemPresent = DefDatabase<ThingDef>.AllDefs.Any(x => x.GetCompProperties<CompProperties_Shield>() != null);
			// List<string> whiteListedXenotypesFromDef = XenotypeFilterUtility.WhiteListedXenotypesForFilter();
			WVC_Biotech.cachedXenotypesFilter ??= new Dictionary<string, bool>();
			WVC_Biotech.allXenotypes = XenotypeFilterUtility.WhiteListedXenotypes(false);
			SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
			foreach (XenotypeDef item in XenotypeFilterUtility.WhiteListedXenotypes(true, true))
			{
				WVC_GenesDefOf.WVC_XenotypeSerums_SupportedXenotypesList.descriptionHyperlinks.Add(item);
			}
		}

		public static void SetValues(List<string> whiteListedXenotypesFromDef)
		{
			foreach (XenotypeDef thingDef in WVC_Biotech.allXenotypes)
			{
				// bool flag = WVC_Biotech.cachedXenotypesFilter[thingDef.defName];
				// XenotypesFilterMod.cachedXenotypesFilter[thingDef.defName] = true;
				if (!WVC_Biotech.cachedXenotypesFilter.TryGetValue(thingDef.defName, out _))
				{
					float metabol = 0f;
					foreach (GeneDef item in thingDef.genes)
					{
						metabol += item.biostatMet;
					}
					// Whitelist from defs
					if (whiteListedXenotypesFromDef.Contains(thingDef.defName))
					{
						WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
					}
					// Immediately weed out all xenotypes that can cause bugs
					// Androids do not work correctly with sera
					// Random xenotypes should not be in sera
					else if (thingDef.defName.Contains("VREA_") || thingDef.defName.Contains("Android") || thingDef.defName.Contains("Random"))
					{
						WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
					}
					// Check that the metabolism are in the limit of the vanilla game.
					// To exclude xenotypes with millions and millions of metabolism.
					else if (metabol < -5 && metabol > 5)
					{
						WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
					}
					else if (!thingDef.inheritable)
					{
						if (thingDef.defName.Contains("WVC_"))
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
						}
						// else if (thingDef.doubleXenotypeChances != null)
						// {
							// flag = _ = false;
						// }
						// 7 genes is one line, 14 - two, and so on.
						// If a xenotype has less than two lines of genes, then it is quite small, it is probably not worth spending time on generation
						// And if there are too many xenotypes, this can affect the speed of the game launch
						else if (thingDef.genes.Count > 14)
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
						}
						else
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
						}
					}
					else if (thingDef.inheritable)
					{
						float archites = 0f;
						foreach (GeneDef item in thingDef.genes)
						{
							archites += item.biostatArc;
						}
						// If my xenotype is inheritable but not listed, it shouldn't be there by default.
						if (thingDef.defName.Contains("WVC_"))
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
						}
						// Check for the presence of archite xenotypes with a bunch of genes. 
						// Quantity is not a sign of quality, but they can be interesting.
						else if (archites > 0 && thingDef.genes.Count > 14)
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
						}
						// Check that the xenotype has more than a couple of genes. 
						// When there are many xenotypes, too small xenotypes are not important.
						else if (thingDef.genes.Count > 21)
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
						}
						else
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
						}
					}
					else
					{
						WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
					}
				}
				// else
				// {
					// filterBlackListedXenotypesForSerums.Add(thingDef.defName);
				// }

				if (WVC_Biotech.cachedXenotypesFilter[thingDef.defName] == false)
				{
					filterBlackListedXenotypesForSerums.Add(thingDef.defName);
				}
			}
		}
	}

	// public class XenotypesFilterSettings : ModSettings
	// {

		// public override void ExposeData()
		// {
		// }
	// }

	// public class XenotypesFilterMod : Mod
	// {
		// public static WVC_BiotechSettings settings;

		// public XenotypesFilterMod(ModContentPack content) : base(content)
		// {
			// settings = GetSettings<WVC_BiotechSettings>();
		// }

		// public override string SettingsCategory()
		// {
			// return "WVC - " + "TEST";
		// }
	// }

}
