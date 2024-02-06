using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_GeneRestoration : CompProperties
	{

		// public bool hybrid = false;

		public List<HediffDef> hediffsToRemove;

		public CompProperties_UseEffect_GeneRestoration()
		{
			compClass = typeof(CompUseEffect_GeneRestoration);
		}
	}

}
