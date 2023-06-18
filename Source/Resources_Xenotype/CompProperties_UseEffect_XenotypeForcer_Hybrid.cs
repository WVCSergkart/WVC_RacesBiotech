using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_XenotypeForcer_Hybrid : CompProperties
	{

		// public XenotypeForcerType xenotypeForcerType = (XenotypeForcerType)0;

		// public XenotypeType xenotypeType = (XenotypeType)0;

		public XenotypeDef endotypeDef = null;
		public XenotypeDef xenotypeDef = null;

		// public bool removeEndogenes = false;

		// public bool removeXenogenes = true;

		// public enum XenotypeForcerType
		// {
			// Base,
			// Hybrid,
			// Custom,
			// CustomHybrid
		// }

		// public enum XenotypeType
		// {
			// Base,
			// Archite
		// }

		public CompProperties_UseEffect_XenotypeForcer_Hybrid()
		{
			compClass = typeof(CompUseEffect_XenotypeForcer_Hybrid);
		}
	}

}
