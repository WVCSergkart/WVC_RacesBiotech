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
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				ThingExtension_Golems modExtension = thingDef?.GetModExtension<ThingExtension_Golems>();
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
				if (corpseDef == null)
				{
					continue;
				}
				if (modExtension.removeButcherRecipes)
				{
					corpseDef.thingCategories = new();
				}
			}
		}
	}

}
