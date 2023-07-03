using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Blank : Gene
	{

		public override bool Active
		{
			get
			{
				// if (Overridden)
				// {
					// return false;
				// }
				if (pawn?.genes != null)
				{
					List<Gene> genesListForReading = pawn.genes.Xenogenes;
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
