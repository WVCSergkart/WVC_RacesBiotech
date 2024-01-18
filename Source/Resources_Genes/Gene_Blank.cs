using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Blank : Gene
	{

		// private bool cachedResult = false;
		// private int nextRecache = -1;

		public override bool Active
		{
			get
			{
				if (base.Active && pawn?.genes != null)
				{
					// if (Find.TickManager.TicksGame < nextRecache)
					// {
						// return cachedResult;
					// }
					// cachedResult = !pawn.genes.Xenogenes.NullOrEmpty();
					// nextRecache = Find.TickManager.TicksGame + 60000;
					return !pawn.genes.Xenogenes.Any();
				}
				return base.Active;
			}
		}

	}

}
