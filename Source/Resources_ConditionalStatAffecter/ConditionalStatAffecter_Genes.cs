using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

namespace WVC_XenotypesAndGenes
{
	public class ConditionalStatAffecter_Genes : ConditionalStatAffecter
	{
		public GeneDef geneDef;
		public bool invert = false;
		// public bool pawnShouldBeImmortal = false;

		public override string Label => Invert();

		public override bool Applies(StatRequest req)
		{
			if (!ModsConfig.BiotechActive)
			{
				return false;
			}
			 // && ((MechanoidizationUtility.PawnIsImmortal(pawn) && pawnShouldBeImmortal == true) || (pawnShouldBeImmortal == false))
			if (req.HasThing && req.Thing is Pawn pawn && ((!pawn.genes.HasGene(geneDef) && invert == true) || (pawn.genes.HasGene(geneDef) && invert == false)))
			{
				return true;
			}
			return false;
		}

		private string Invert()
		{
			// if (pawnShouldBeImmortal == true)
			// {
				// if (invert == true)
				// {
					// return "WVC_StatsReport_Immortal".Translate() + "-" + "WVC_StatsReport_NonGenes".Translate() + geneDef.label;
				// }
				// return "WVC_StatsReport_Immortal".Translate() + "-"  + geneDef.label;
			// }
			if (invert == true)
			{
				return "WVC_StatsReport_NonGenes".Translate() + geneDef.label;
			}
			return geneDef.label;
		}
	}
}
