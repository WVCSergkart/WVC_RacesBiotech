using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class ShaperGeneSet
	{

		public ShaperGeneSetDef def;

		public virtual bool Allowed(Pawn pawn)
		{
			return true;
		}

	}

	public class FleshshaperGeneSet_Hivemind : ShaperGeneSet
	{

		public override bool Allowed(Pawn pawn)
		{
			if (pawn?.genes?.Endogenes == null)
			{
				return false;
			}
			return pawn.genes.Endogenes.Any(HivemindUtility.IsHivemindGene);
		}

	}

	public class FleshshaperGeneSet_SoloLeveling : ShaperGeneSet
	{

		public override bool Allowed(Pawn pawn)
		{
			return StaticCollectionsClass.oneManArmyMode;
		}

	}

}
