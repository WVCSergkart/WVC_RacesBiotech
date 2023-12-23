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
		}
	}

}
