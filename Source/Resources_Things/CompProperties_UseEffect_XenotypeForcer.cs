using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_XenotypeForcer : CompProperties
	{

		// public bool hybrid = false;

		public XenotypeForcerType xenotypeForcerType = (XenotypeForcerType)0;

		public XenotypeDef xenotypeDef;

		// public XenotypeDef perfectCandidate;

		// public bool setXenotypeDirect = false;

		public bool removeEndogenes = false;

		public bool removeXenogenes = true;

		// public bool useCustomXenotype = false;

		public CompProperties_UseEffect_XenotypeForcer()
		{
			compClass = typeof(CompUseEffect_XenotypeForcer);
		}

		public enum XenotypeForcerType
		{
			Base,
			Hybrid,
			Custom,
			CustomHybrid
		}
	}

}
