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
		}
	}

}
