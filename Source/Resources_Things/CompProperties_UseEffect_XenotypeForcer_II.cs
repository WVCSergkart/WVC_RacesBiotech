using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class CompProperties_UseEffect_XenotypeForcer_II : CompProperties
	{

		public XenotypeDef endotypeDef = null;
		public XenotypeDef xenotypeDef = null;

		public List<GeneDef> possibleGenes;

		public XenotypeType xenotypeType = (XenotypeType)0;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public JobDef retuneJob;

		[MustTranslate]
		public string jobString;

		public List<ResearchProjectDef> researchPrerequisites;

		public enum XenotypeType
		{
			Base,
			Archite
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			if (compClass == typeof(ThingComp))
			{
				Log.Error(parentDef.defName + " has CompProperties_UseEffect_XenotypeForcer_II with ThingComp compClass. Will be used CompUseEffect_XenotypeForcer_II instead.");
				compClass = typeof(CompUseEffect_XenotypeForcer_II);
			}
			if (compClass == typeof(CompUseEffect_GeneGiver) && possibleGenes.NullOrEmpty())
			{
				Log.Error(parentDef.defName + " has CompUseEffect_GeneGiver compClass with null possibleGenes.");
			}
		}

		// public CompProperties_UseEffect_XenotypeForcer_II()
		// {
			// compClass = typeof(CompUseEffect_XenotypeForcer_II);
		// }
	}

}
