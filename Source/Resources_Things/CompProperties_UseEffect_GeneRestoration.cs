using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class CompProperties_UseEffect_GeneRestoration : CompProperties
	{

		public int daysDelay = 8;

		public List<HediffDef> hediffsToRemove;

		public CompProperties_UseEffect_GeneRestoration()
		{
			compClass = typeof(CompUseEffect_GeneRestoration);
		}

	}

}
