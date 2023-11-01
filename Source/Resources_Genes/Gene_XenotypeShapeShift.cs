using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_XenotypeShapeshifter : Gene
	{

		// public bool geneIsXenogene = false;

		public XenotypeDef Xenotype => pawn.genes?.Xenotype;

		public override void PostAdd()
		{
			base.PostAdd();
			// SubXenotypeUtility.ShapeShift(pawn, Xenotype, this);
			SubXenotypeUtility.XenotypeShapeShift(pawn, Xenotype, this);
		}

	}

}
