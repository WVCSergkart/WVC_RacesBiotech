using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Blank : Gene
	{

		private List<Gene> genesListForReading;

		public override bool Active
		{
			get
			{
				// if (Overridden)
				// {
				// return false;
				// }
				// Log.Error("Active");
				if (base.Active && pawn?.genes != null)
				{
					if (genesListForReading.NullOrEmpty())
					{
						// Log.Error("Active");
						genesListForReading = pawn.genes.Xenogenes;
					}
					if (genesListForReading.Count > 0)
					{
						return false;
					}
				}
				return base.Active;
			}
		}

	}

}
