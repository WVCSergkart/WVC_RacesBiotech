using RimWorld;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class PostInitializationInheritFromGenes
	{

		static PostInitializationInheritFromGenes()
		{
			// List<string> filter = UltraFilterUtility.BlackListedTerrainDefs();
			Genes();
			// Race Patches
			GolemsAndMechs();
			// SubXenotypes Debug
			SubXenotypes();
			// Patches
			HarmonyPatches.HarmonyUtility.PostInitialPatches();
		}

		public static void Genes()
		{
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
				if (!WVC_Biotech.settings.hideXaGGenes)
				{
					continue;
				}
				if (geneDef.IsXenoGenesGene())
				{
					if (geneDef.biostatArc != 0)
					{
						geneDef.displayCategory = GeneCategoryDefOf.Archite;
					}
					else
					{
						geneDef.displayCategory = GeneCategoryDefOf.Miscellaneous;
					}
				}
			}
		}

		public static void GolemsAndMechs()
		{
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
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
					}
					if (modExtension.shouldResurrect)
					{
						CompProperties_UndeadCorpse undead_comp = new();
						undead_comp.resurrectionDelay = modExtension.resurrectionDelay;
						undead_comp.uniqueTag = modExtension.uniqueTag;
						corpseDef.comps.Add(undead_comp);
					}
				}
			}
		}

		public static void SubXenotypes()
		{
			foreach (SubXenotypeDef subXenotypeDef in DefDatabase<SubXenotypeDef>.AllDefsListForReading)
			{
				GeneDef geneticShifter = WVC_GenesDefOf.WVC_XenotypesAndGenes_SubXenotypeShapeshifter;
				if (subXenotypeDef.removeGenes.Contains(geneticShifter))
				{
					subXenotypeDef.removeGenes.Remove(geneticShifter);
					Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in removeGenes. Fixing..");
				}
				if (subXenotypeDef.endogenes.Contains(geneticShifter))
				{
					subXenotypeDef.endogenes.Remove(geneticShifter);
					Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in endogenes. Fixing..");
				}
				if (subXenotypeDef.genes.Contains(geneticShifter))
				{
					subXenotypeDef.genes.Remove(geneticShifter);
					Log.Warning(subXenotypeDef.defName + " contains " + geneticShifter.defName + " in genes. Fixing..");
				}
				if (!subXenotypeDef.genes.Contains(geneticShifter))
				{
					subXenotypeDef.genes.Add(geneticShifter);
				}
				if (subXenotypeDef.inheritable)
				{
					subXenotypeDef.inheritable = false;
					Log.Warning(subXenotypeDef.defName + " is inheritable. Fixing..");
				}
			}
		}

	}

}
