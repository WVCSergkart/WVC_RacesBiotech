using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenotypeShapeshifter : Gene
	{

		// public bool geneIsXenogene = false;

		public XenotypeDef Xenotype => pawn.genes?.Xenotype;

		public override void PostAdd()
		{
			base.PostAdd();
			// if (pawn.genes != null)
			// {
				// geneIsXenogene = pawn.genes.IsXenogene(this);
			// }
			// if (Xenotype != null)
			// {
				// XenotypeExtension_SubXenotype modExtension = Xenotype.GetModExtension<XenotypeExtension_SubXenotype>();
				// if (modExtension != null)
				// {
					// if (Rand.Chance(modExtension.subXenotypeChance))
					// {
						// SubXenotypeDef subXenotypeDef = modExtension.subXenotypeDefs.RandomElement();
						// RandomXenotype(pawn, subXenotypeDef, Xenotype);
					// }
					// if (modExtension.mainSubXenotypeDef != null && shouldAddMainGenes)
					// {
						// SubXenotypeDef subXenotypeDef = modExtension.mainSubXenotypeDef;
						// MainXenotype(pawn, subXenotypeDef, Xenotype);
					// }
				// }
			// }
			// if (Active && !Overridden)
			// {
			// }
			SubXenotypeUtility.ShapeShift(pawn, Xenotype, this);
			// if (this != null)
			// {
				// pawn.genes.RemoveGene(this);
			// }
		}

	}

}
