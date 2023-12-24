using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
			{
				List<GeneDef> inheritableGeneDefs = geneDef?.GetModExtension<GeneExtension_General>()?.inheritableGeneDefs;
				if (!inheritableGeneDefs.NullOrEmpty())
				{
					foreach (GeneDef inheritableGeneDef in inheritableGeneDefs)
					{
						TemplatesUtility.InheritGeneImmunityFrom(geneDef, inheritableGeneDef);
					}
				}
			}
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				ThingExtension_Golems modExtension = thingDef?.GetModExtension<ThingExtension_Golems>();
				if (modExtension != null)
				{
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
					}
				}
				ThingExtension_Undead deadExtension = thingDef?.GetModExtension<ThingExtension_Undead>();
				if (deadExtension != null && deadExtension.shouldResurrect)
				{
					ThingDef corpseDef = thingDef.race?.corpseDef;
					if (corpseDef != null)
					{
						CompProperties_UndeadCorpse undead_comp = new();
						undead_comp.resurrectionDelay = deadExtension.resurrectionDelay;
						undead_comp.uniqueTag = deadExtension.uniqueTag;
						corpseDef.comps.Add(undead_comp);
					}
				}
			}
			// SubXenotypes Debug
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
				// if (!subXenotypeDef.doubleXenotypeChances.NullOrEmpty())
				// {
					// subXenotypeDef.doubleXenotypeChances = new();
					// Log.Warning(subXenotypeDef.defName + " doubleXenotypeChances is not empty. Fixing..");
				// }
				// Add if it was not added in XML. This option is desirable, since genes from the list are taken in the order they were added during generation.
				// In theory, it should help avoid a couple of bugs. In practice, it has still not been possible to repeat these bugs with non-custom xenotypes.
				// if (!subXenotypeDef.genes.NullOrEmpty())
				// {
					// Log.Warning(subXenotypeDef.defName + " genes is not empty. Fixing..");
				// }
				// subXenotypeDef.genes = new();
				if (!subXenotypeDef.genes.Contains(geneticShifter))
				{
					subXenotypeDef.genes.Add(geneticShifter);
				}
				if (subXenotypeDef.inheritable)
				{
					subXenotypeDef.inheritable = false;
					Log.Warning(subXenotypeDef.defName + " is inheritable. Fixing..");
				}
				// if (subXenotypeDef.doubleXenotypeChances.NullOrEmpty() || subXenotypeDef.doubleXenotypeChances.Sum((XenotypeChance x) => x.chance) != 1f)
				// {
					// Log.Error(subXenotypeDef.defName + " has null doubleXenotypeChances. doubleXenotypeChances must contain at least one xenotype with a chance 1.0");
				// }
			}
		}
	}

}
