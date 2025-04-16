using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public static class InitialUtility
	{


		// public static void Hediffs()
		// {
		// if (!WVC_Biotech.settings.autoPatchVanillaArchiteImmunityGenes)
		// {
		// return;
		// }
		// GeneDef perfectImmunity = WVC_GenesDefOf.PerfectImmunity;
		// GeneDef diseaseFree = WVC_GenesDefOf.DiseaseFree;
		// foreach (HediffDef hediffDef in DefDatabase<HediffDef>.AllDefsListForReading)
		// {
		// if (!perfectImmunity.makeImmuneTo.NullOrEmpty())
		// {
		// if (hediffDef.isInfection && !perfectImmunity.makeImmuneTo.Contains(hediffDef))
		// {
		// perfectImmunity.makeImmuneTo.Add(hediffDef);
		// }
		// }
		// if (!diseaseFree.hediffGiversCannotGive.NullOrEmpty())
		// {
		// if (hediffDef.chronic && !diseaseFree.hediffGiversCannotGive.Contains(hediffDef))
		// {
		// diseaseFree.hediffGiversCannotGive.Add(hediffDef);
		// }
		// }
		// }
		// foreach (HediffGiver hediffGivers in WVC_GenesDefOf.OrganicStandard.hediffGivers)
		// {
		// if (!diseaseFree.hediffGiversCannotGive.Contains(hediffGivers.hediff))
		// {
		// diseaseFree.hediffGiversCannotGive.Add(hediffGivers.hediff);
		// }
		// }
		// }

		public static void GenesAndMutants()
		{
			List<GeneDef> xenogenesGenes = new();
			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
			{
				GeneExtension_General geneExtension_General = geneDef?.GetModExtension<GeneExtension_General>();
				if (geneExtension_General != null)
				{
					InheritableGeneStats(geneDef, geneExtension_General);
					BirthQuality(geneDef, geneExtension_General);
				}
                //GeneExtension_Giver geneExtension_Giver = geneDef?.GetModExtension<GeneExtension_Giver>();
                //if (geneExtension_Giver != null)
                //{
                //    GeneExtension_Giver(geneDef, geneExtension_Giver);
                //}
                FurskinIsSkin(geneDef);
				XenoGenesDef(geneDef, xenogenesGenes);
			}
			AnomalyPatch(xenogenesGenes);
			FlatGenesChances(xenogenesGenes);
		}

		//public static void GeneExtension_Giver(GeneDef geneDef, GeneExtension_Giver geneExtension_Giver)
		//{
		//	if (geneDef.customEffectDescriptions == null)
		//	{
		//		geneDef.customEffectDescriptions = new();
		//	}
		//	int scarsCount = geneExtension_Giver.scarsCount;
		//	if (scarsCount != 0)
		//	{
		//		geneDef.customEffectDescriptions.Add("WVC_XaG_ScarifierScars".Translate().Resolve() + ": " + (scarsCount > 0 ? "+" : "") + scarsCount);
		//	}
		//}

        public static void BirthQuality(GeneDef geneDef, GeneExtension_General geneExtension_General)
		{
			if (!WVC_Biotech.settings.enable_birthQualityOffsetFromGenes)
			{
				return;
			}
			if (geneExtension_General.birthQualityOffset == 0)
			{
				return;
			}
			if (geneDef.customEffectDescriptions == null)
			{
				geneDef.customEffectDescriptions = new();
			}
			string birthQualityOffset = (geneExtension_General.birthQualityOffset > 0 ? "+" : "") + (geneExtension_General.birthQualityOffset * 100f).ToString();
			geneDef.customEffectDescriptions.Add("WVC_XaG_BirthQuality".Translate(birthQualityOffset));
		}

		//public static void OverOverridable(GeneDef geneDef)
		//{
		//	if (!WVC_Biotech.settings.enable_OverOverridableGenesMechanic)
		//	{
		//		return;
		//	}
		//	if (geneDef.geneClass != typeof(Gene_OverOverridable))
		//	{
		//		return;
		//	}
		//	if (geneDef.customEffectDescriptions.NullOrEmpty())
		//	{
		//		geneDef.customEffectDescriptions = new();
		//	}
		//	geneDef.customEffectDescriptions.Add("WVC_XaG_OverOverrideGene".Translate().ToString());
		//}

		private static void InheritableGeneStats(GeneDef geneDef, GeneExtension_General geneExtension_General)
		{
			List<GeneDef> inheritableGeneDefs = geneExtension_General?.inheritableGeneDefs;
			if (!inheritableGeneDefs.NullOrEmpty())
			{
				foreach (GeneDef inheritableGeneDef in inheritableGeneDefs)
				{
					MiscUtility.CopyStatsFromGeneDef(geneDef, inheritableGeneDef);
				}
			}
		}

		private static void FurskinIsSkin(GeneDef geneDef)
		{
			if (geneDef.fur == null)
			{
				return;
			}
			if (!WVC_Biotech.settings.enable_FurskinIsSkinAutopatch || WVC_Biotech.settings.disableFurGraphic)
			{
				return;
			}
			GeneExtension_Graphic modExtension = geneDef.fur?.GetModExtension<GeneExtension_Graphic>();
			if (modExtension != null)
			{
				return;
			}
			geneDef.renderNodeProperties = null;
			if (geneDef.fur.modExtensions == null)
			{
				geneDef.fur.modExtensions = new();
			}
			GeneExtension_Graphic geneExtension_Graphic = new();
			if (MiscUtility.FurskinHasMask(geneDef.fur))
			{
				geneExtension_Graphic.furIsSkinWithHair = true;
			}
			geneExtension_Graphic.furIsSkin = true;
			geneDef.fur.modExtensions.Add(geneExtension_Graphic);
		}

		private static void XenoGenesDef(GeneDef geneDef, List<GeneDef> xenogenesGenes)
		{
			if (!geneDef.IsXenoGenesDef())
			{
				return;
			}
			if (geneDef.selectionWeight > 0.01f)
			{
				geneDef.selectionWeight = 0.001f;
			}
			UniqueDescAutopatch(geneDef);
			xenogenesGenes.Add(geneDef);
			if (!WVC_Biotech.settings.hideXaGGenes)
			{
				if (geneDef.displayCategory == GeneCategoryDefOf.Miscellaneous)
				{
					geneDef.displayCategory = WVC_GenesDefOf.WVC_Miscellaneous;
				}
				return;
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

		public static void UniqueDescAutopatch(GeneDef geneDef)
		{
			if (WVC_Biotech.settings.showGenesSettingsGizmo && geneDef.IsGeneDefOfType<IGeneRemoteControl>())
			{
				geneDef.description += "\n\n" + "WVC_XaG_GenesSettings_DescTip".Translate().ToString();
			}
			if (!WVC_Biotech.settings.enable_OverOverridableGenesMechanic)
			{
				return;
			}
			if (!geneDef.IsGeneDefOfType<Gene_OverOverridable>())
			{
				return;
			}
			if (geneDef.customEffectDescriptions == null)
			{
				geneDef.customEffectDescriptions = new();
			}
			geneDef.customEffectDescriptions.Add("WVC_XaG_OverOverrideGene".Translate().ToString());
		}

		private static void AnomalyPatch(List<GeneDef> xenogenesGenes)
		{
			if (ModsConfig.AnomalyActive)
			{
				List<MutantDef> exceptions_Mutants = ListsUtility.GetMutantsExceptions();
				List<GeneDef> exceptions = ListsUtility.GetAnomalyExceptions();
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

		private static void FlatGenesChances(List<GeneDef> xenogenesGenes)
		{
			if (!WVC_Biotech.settings.enable_flatGenesSpawnChances)
			{
				return;
			}
			foreach (GeneDef geneDef in xenogenesGenes)
			{
				if (geneDef.selectionWeight > 0f)
				{
					geneDef.selectionWeight = 1f / xenogenesGenes.Count;
					if (geneDef.prerequisite != null)
					{
						geneDef.selectionWeight *= 0.1f;
					}
					GeneExtension_General extension = geneDef?.GetModExtension<GeneExtension_General>();
					if (extension != null && extension.isAptitude)
					{
						geneDef.selectionWeight *= 0.02f;
					}
				}
			}
		}

		//[Obsolete]
		//public static void ThingDefs()
		//{
		//	foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
		//	{
		//		if (thingDef.IsXenoGenesDef())
		//		{
		//			if (WVC_Biotech.settings.onlyXenotypesMode)
		//			{
		//				if (thingDef.thingSetMakerTags != null)
		//				{
		//					thingDef.thingSetMakerTags = null;
		//				}
		//				if (thingDef.tradeTags != null)
		//				{
		//					thingDef.tradeTags = new() { "ExoticMisc" };
		//					thingDef.tradeability = Tradeability.Sellable;
		//				}
		//				if (thingDef.techHediffsTags != null)
		//				{
		//					thingDef.techHediffsTags = null;
		//				}
		//			}
		//		}
		//	}
		//}

		//[Obsolete]
		//public static void XenotypeDefs()
		//{
		//	if (!WVC_Biotech.settings.enable_ReplaceSimilarGenesAutopatch)
		//	{
		//		return;
		//	}
		//	List<XaG_CountWithChance> similarGenes = ListsUtility.GetIdenticalGeneDefs();
		//	foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
		//	{
		//		foreach (GeneDef geneDef in xenotypeDef.genes.ToList())
		//		{
		//			foreach (XaG_CountWithChance similar in similarGenes)
		//			{
		//				if (similar.sourceGeneDef != null && !similar.dupGeneDefs.NullOrEmpty() && similar.dupGeneDefs.Contains(geneDef))
		//				{
		//					xenotypeDef.genes.Remove(geneDef);
		//					geneDef.selectionWeight = 0f;
		//					geneDef.canGenerateInGeneSet = false;
		//					if (!similar.sourceGeneDef.randomChosen)
		//					{
		//						foreach (GeneDef geneDef2 in xenotypeDef.genes.ToList())
		//						{
		//							if (geneDef2.ConflictsWith(similar.sourceGeneDef))
		//							{
		//								xenotypeDef.genes.Remove(geneDef2);
		//							}
		//						}
		//					}
		//					xenotypeDef.genes.Add(similar.sourceGeneDef);
		//				}
		//			}
		//		}
		//	}
		//}

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
			if (WVC_Biotech.settings.firstModLaunch)
			{
				WVC_Biotech.cachedXenotypesFilter ??= new Dictionary<string, bool>();
				SetValues();
				WVC_Biotech.settings.firstModLaunch = false;
				WVC_Biotech.settings.Write();
			}
			// foreach (XenotypeDef item in ListsUtility.GetWhiteListedXenotypes(true, true))
			// {
			// WVC_GenesDefOf.WVC_XenotypeSerums_SupportedXenotypesList.descriptionHyperlinks.Add(item);
			// }
		}

		public static void SetValues()
		{
			WVC_Biotech.allXenotypes = ListsUtility.GetWhiteListedXenotypes(false);
			foreach (XenotypeDef xenotypeDef in WVC_Biotech.allXenotypes)
			{

				if (!WVC_Biotech.cachedXenotypesFilter.TryGetValue(xenotypeDef.defName, out _))
				{
					if (xenotypeDef.modContentPack?.IsOfficialMod == true)
					{
						WVC_Biotech.cachedXenotypesFilter[xenotypeDef.defName] = _ = true;
					}
					else
					{
						WVC_Biotech.cachedXenotypesFilter[xenotypeDef.defName] = _ = false;
					}
				}

			}
		}


	}
}
