using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_Faceless : Gene
	{

		public override bool Active
		{
			get
			{
				// if (Overridden)
				// {
					// return false;
				// }
				// if (pawn?.ageTracker != null && (float)pawn.ageTracker.AgeBiologicalYears < def.minAgeActive)
				// {
					// return false;
				// }
				// GeneExtension_EyeDraws modExtension = def.GetModExtension<GeneExtension_EyeDraws>();
				// if (modExtension == null || !modExtension.eyesShouldBeInvisble)
				// {
					// return false;
				// }
				// if (!HasActiveGene(pawn))
				// {
					// return false;
				// }
				if (base.Active)
				{
					if (pawn?.story != null && !pawn.story.headType.defName.Contains("WVC_Faceless"))
					{
						return false;
					}
				}
				return base.Active;
			}
		}

		// private static bool HasActiveGene(Pawn pawn)
		// {
			// if (pawn?.genes == null)
			// {
				// return false;
			// }
			// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (genesListForReading[i].pawn.story.headType.defName.Contains("WVC_Faceless"))
				// {
					// return true;
				// }
			// }
			// return false;
		// }

	}
}
