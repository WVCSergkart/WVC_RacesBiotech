using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_XenotypeForcer_II : CompProperties
	{

		public XenotypeForcerType xenotypeForcerType = (XenotypeForcerType)0;

		public XenotypeType xenotypeType = (XenotypeType)0;

		public XenotypeDef xenotypeDef;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		public enum XenotypeForcerType
		{
			Base,
			Hybrid,
			Custom,
			CustomHybrid
		}

		public enum XenotypeType
		{
			Base,
			Archite,
			Hybrid
		}

		public CompProperties_UseEffect_XenotypeForcer_II()
		{
			compClass = typeof(CompUseEffect_XenotypeForcer_II);
		}
	}

}
