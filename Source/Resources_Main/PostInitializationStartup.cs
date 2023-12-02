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
			foreach (ThingDef allDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				ThingExtension_Golems modExtension = allDef.GetModExtension<ThingExtension_Golems>();
				if (modExtension == null)
				{
					continue;
				}
				ThingDef thingDef = allDef;
				if (thingDef == null)
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
				if (corpseDef == null)
				{
					continue;
				}
				if (modExtension.removeButcherRecipes)
				{
					corpseDef.thingCategories = new();
					// corpseDef.smeltable = false;
					// corpseDef.burnableByRecipe = false;
					// Log.Error(corpseDef.defName + ": " + corpseDef.thingCategories.ToString());
					// corpseDef.comps.RemoveAll((CompProperties compProperties) => compProperties is CompProperties_SpawnerFilth);
					// corpseDef.SetStatBaseValue(StatDefOf.Nutrition, 0.0f);
					// foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
					// {
						// TemplatesUtility.InheritGeneImmunityFrom(geneDef, inheritableGeneDef);
					// }
				}
			}
		}
	}

}
