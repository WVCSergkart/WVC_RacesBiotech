using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_TrueParentGenes : HediffCompProperties
	{

		public bool addSurrogateGenes = false;

		public HediffCompProperties_TrueParentGenes()
		{
			compClass = typeof(HediffComp_TrueParentGenes);
		}

	}

	public class HediffComp_TrueParentGenes : HediffComp
	{

		public HediffCompProperties_TrueParentGenes Props => (HediffCompProperties_TrueParentGenes)props;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			// Log.Error("0");
			if (parent is Hediff_Pregnant pregnancy)
			{
				// Log.Error("1");
				GeneSet newGeneSet = pregnancy.geneSet;
				HediffUtility.AddParentGenes(pregnancy.Mother, newGeneSet);
				HediffUtility.AddParentGenes(pregnancy.Father, newGeneSet);
				if (!parent.pawn.Spawned || Props.addSurrogateGenes)
				{
					HediffUtility.AddParentGenes(parent.pawn, newGeneSet);
				}
				// Log.Error("Genes: " + newGeneSet.GenesListForReading.Count.ToString());
				newGeneSet.SortGenes();
				// if (!newGeneSet.Empty)
				// {
					// pregnancy.geneSet = newGeneSet;
				// }
			}
		}

	}

	public class HediffComp_NotifyPregnancyStarted : HediffComp
	{

		// public HediffCompProperties_TrueParentGenes Props => (HediffCompProperties_TrueParentGenes)props;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			if (parent is Hediff_Pregnant pregnancy)
			{
				foreach (Gene gene in parent.pawn.genes.GenesListForReading)
				{
					if (gene is IGenePregnantHuman genePregnantHuman && gene.Active)
					{
						genePregnantHuman.Notify_PregnancyStarted(pregnancy);
					}
				}
			}
		}

	}

}
