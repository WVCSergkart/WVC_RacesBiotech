using RimWorld;
using System.Collections.Generic;
using Verse;

// namespace WVC
namespace WVC_XenotypesAndGenes
{

	[StaticConstructorOnStartup]
	public static class PostInitializationPerfectImmunityGenes
	{
		static PostInitializationPerfectImmunityGenes()
		{
			// List<string> filter = UltraFilterUtility.BlackListedTerrainDefs();
			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefsListForReading)
			{
				GeneExtension_General modExtension = geneDef.GetModExtension<GeneExtension_General>();
				if (modExtension != null && modExtension.perfectImmunity)
				{
					TemplatesUtility.InheritGeneImmunityFrom(geneDef, WVC_GenesDefOf.PerfectImmunity);
				}
				if (modExtension != null && modExtension.diseaseFree)
				{
					TemplatesUtility.InheritGeneImmunityFrom(geneDef, WVC_GenesDefOf.DiseaseFree);
				}
			}
		}
	}

}
