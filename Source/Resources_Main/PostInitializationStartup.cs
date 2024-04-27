using RimWorld;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class XaG_PostInitialization
	{

		static XaG_PostInitialization()
		{
			InitializeModSettings();
			GeneDefs();
			ThingDefs();
			// XenotypeDefs();
			HarmonyPatches.HarmonyUtility.PostInitialPatches();
		}

		public static void GeneDefs()
		{
			List<GeneDef> xenogenesGenes = new();
			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
			{
				List<GeneDef> inheritableGeneDefs = geneDef?.GetModExtension<GeneExtension_General>()?.inheritableGeneDefs;
				if (!inheritableGeneDefs.NullOrEmpty())
				{
					foreach (GeneDef inheritableGeneDef in inheritableGeneDefs)
					{
						MiscUtility.InheritGeneDefFrom(geneDef, inheritableGeneDef);
					}
				}
				if (!geneDef.IsXenoGenesDef())
				{
					continue;
				}
				xenogenesGenes.Add(geneDef);
				if (!WVC_Biotech.settings.hideXaGGenes)
				{
					continue;
				}
				if (geneDef.biostatArc != 0)
				{
					geneDef.displayCategory = GeneCategoryDefOf.Archite;
				}
				else
				{
					geneDef.displayCategory = GeneCategoryDefOf.Miscellaneous;
				}
			}
			if (ModsConfig.AnomalyActive)
			{
				List<MutantDef> exceptions_Mutants = XenotypeFilterUtility.MutantsExceptions();
				List<GeneDef> exceptions = XenotypeFilterUtility.AnomalyExceptions();
				foreach (MutantDef mutantDef in DefDatabase<MutantDef>.AllDefsListForReading)
				{
					if (mutantDef == null)
					{
						continue;
					}
					if (exceptions_Mutants.Contains(mutantDef))
					{
						continue;
					}
					if (mutantDef.disablesGenes.NullOrEmpty())
					{
						mutantDef.disablesGenes = new();
					}
					foreach (GeneDef geneDef in xenogenesGenes)
					{
						// if (geneDef.fur != null && !mutantDef.bodyTypeGraphicPaths.NullOrEmpty())
						// {
							// continue;
						// }
						GeneExtension_General modExtension = geneDef?.GetModExtension<GeneExtension_General>();
						if (modExtension?.supportMutants == false)
						{
							if (!mutantDef.disablesGenes.Contains(geneDef))
							{
								mutantDef.disablesGenes.Add(geneDef);
							}
							continue;
						}
						if (modExtension?.supportedMutantDefs?.Contains(mutantDef) == true)
						{
							continue;
						}
						if (exceptions.Contains(geneDef))
						{
							continue;
						}
						if (geneDef.geneClass == typeof(Gene))
						{
							continue;
						}
						if (!mutantDef.disablesGenes.Contains(geneDef))
						{
							mutantDef.disablesGenes.Add(geneDef);
						}
					}
				}
			}
		}

		public static void ThingDefs()
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (thingDef.IsXenoGenesDef())
				{
					if (WVC_Biotech.settings.onlyXenotypesMode)
					{
						if (thingDef.thingSetMakerTags != null)
						{
							// Log.Error(thingDef.LabelCap + " patched.");
							thingDef.thingSetMakerTags = null;
							thingDef.tradeTags = new() { "ExoticMisc" };
							thingDef.tradeability = Tradeability.Sellable;
						}
						if (thingDef.techHediffsTags != null)
						{
							thingDef.techHediffsTags = null;
						}
					}
				}
				if (thingDef?.race == null)
				{
					continue;
				}
				GeneExtension_General modExtension = thingDef?.GetModExtension<GeneExtension_General>();
				if (modExtension == null)
				{
					continue;
				}
				if (modExtension.removeRepairComp)
				{
					thingDef.comps.RemoveAll((CompProperties compProperties) => compProperties is CompProperties_MechRepairable);
				}
				if (modExtension.removeDormantComp)
				{
					thingDef.comps.RemoveAll((CompProperties compProperties) => compProperties is CompProperties_CanBeDormant);
					thingDef.comps.RemoveAll((CompProperties compProperties) => compProperties is CompProperties_WakeUpDormant);
				}
				ThingDef corpseDef = thingDef.race?.corpseDef;
				if (corpseDef != null)
				{
					if (modExtension.removeButcherRecipes)
					{
						corpseDef.thingCategories = new();
						// thingDef.race.corpseDef = null;
					}
					if (modExtension.shouldResurrect)
					{
						if (corpseDef.GetCompProperties<CompProperties_UndeadCorpse>() != null)
						{
							continue;
						}
						CompProperties_UndeadCorpse undead_comp = new();
						undead_comp.resurrectionDelay = modExtension.resurrectionDelay;
						undead_comp.uniqueTag = modExtension.uniqueTag;
						corpseDef.comps.Add(undead_comp);
					}
				}
			}
		}

		// public static void XenotypeDefs()
		// {
			// foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
			// {
			// }
		// }

		// public static void SubXenotypes()
		// {
			// foreach (SubXenotypeDef subXenotypeDef in DefDatabase<SubXenotypeDef>.AllDefsListForReading)
			// {
				// GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
				// if (subXenotypeDef.removeGenes.Contains(geneticShifter))
				// {
					// subXenotypeDef.removeGenes.Remove(geneticShifter);
					// Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in removeGenes. Fixing..");
				// }
				// if (subXenotypeDef.endogenes.Contains(geneticShifter))
				// {
					// subXenotypeDef.endogenes.Remove(geneticShifter);
					// Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in endogenes. Fixing..");
				// }
				// if (subXenotypeDef.genes.Contains(geneticShifter))
				// {
					// subXenotypeDef.genes.Remove(geneticShifter);
					// Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in genes. Fixing..");
				// }
				// if (!subXenotypeDef.genes.Contains(geneticShifter))
				// {
					// subXenotypeDef.genes.Add(geneticShifter);
				// }
				// if (subXenotypeDef.inheritable)
				// {
					// subXenotypeDef.inheritable = false;
					// Log.Warning(subXenotypeDef.defName + " is inheritable. Fixing..");
				// }
			// }
		// }

		public static void InitializeModSettings()
		{
			if (WVC_Biotech.settings.firstModLaunch || WVC_Biotech.settings.serumsForAllXenotypes)
			{
				WVC_Biotech.cachedXenotypesFilter ??= new Dictionary<string, bool>();
				WVC_Biotech.allXenotypes = XenotypeFilterUtility.WhiteListedXenotypes(false);
				SetValues(XenotypeFilterUtility.WhiteListedXenotypesForFilter());
				WVC_Biotech.settings.firstModLaunch = false;
				WVC_Biotech.settings.Write();
			}
			foreach (XenotypeDef item in XenotypeFilterUtility.WhiteListedXenotypes(true, true))
			{
				WVC_GenesDefOf.WVC_XenotypeSerums_SupportedXenotypesList.descriptionHyperlinks.Add(item);
			}
		}

		public static void SetValues(List<string> whiteListedXenotypesFromDef)
		{
			foreach (XenotypeDef thingDef in WVC_Biotech.allXenotypes)
			{

				if (!WVC_Biotech.settings.serumsForAllXenotypes)
				{
					if (!WVC_Biotech.cachedXenotypesFilter.TryGetValue(thingDef.defName, out _))
					{
						WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
					}
				}
				else
				{
					if (!WVC_Biotech.cachedXenotypesFilter.TryGetValue(thingDef.defName, out _))
					{
						float metabol = 0f;
						foreach (GeneDef item in thingDef.genes)
						{
							metabol += item.biostatMet;
						}
						if (whiteListedXenotypesFromDef.Contains(thingDef.defName))
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
						}
						else if (thingDef.defName.Contains("VREA_") || thingDef.defName.Contains("Android") || thingDef.defName.Contains("Random") || thingDef.defName.Contains("WVC_"))
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
						}
						else if (metabol < -5 && metabol > 5)
						{
							WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = false;
						}
						else if (!thingDef.inheritable)
						{
							if (thingDef.genes.Count > 14)
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
							if (archites > 0 && thingDef.genes.Count > 14)
							{
								WVC_Biotech.cachedXenotypesFilter[thingDef.defName] = _ = true;
							}
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
				}

			}
		}

	}

}
