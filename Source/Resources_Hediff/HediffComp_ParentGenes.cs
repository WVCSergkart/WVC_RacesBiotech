using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_TrueParentGenes : HediffCompProperties
	{

		public float daysBeforeCustomPregnancyFire = 3;

		public HediffCompProperties_TrueParentGenes()
		{
			compClass = typeof(HediffComp_NotifyPregnancyStarted);
		}

	}

	//public class HediffComp_TrueParentGenes : HediffComp
	//{

	//	public HediffCompProperties_TrueParentGenes Props => (HediffCompProperties_TrueParentGenes)props;

	//	public override void CompPostPostAdd(DamageInfo? dinfo)
	//	{
	//		if (parent is Hediff_Pregnant pregnancy)
	//		{
	//			GeneSet newGeneSet = pregnancy.geneSet;
	//			HediffUtility.AddParentGenes(pregnancy.Mother, newGeneSet);
	//			HediffUtility.AddParentGenes(pregnancy.Father, newGeneSet);
	//			if (!parent.pawn.Spawned || Props.addSurrogateGenes)
	//			{
	//				HediffUtility.AddParentGenes(parent.pawn, newGeneSet);
	//			}
	//			newGeneSet.SortGenes();
	//		}
	//	}

	//}

	public class HediffComp_NotifyPregnancyStarted : HediffComp
	{

		public HediffCompProperties_TrueParentGenes Props => (HediffCompProperties_TrueParentGenes)props;

		public override void CompPostPostAdd(DamageInfo? dinfo)
        {
			if (parent is Hediff_Pregnant pregnancy)
			{
				if (Pawn.genes == null)
				{
					return;
				}
				foreach (Gene gene in parent.pawn.genes.GenesListForReading.ToList())
				{
					try
					{
						if (gene is IGenePregnantHuman genePregnantHuman && gene.Active)
						{
							genePregnantHuman.Notify_PregnancyStarted(pregnancy);
						}
					}
					catch (Exception arg)
					{
						Log.Error("Failed Notify_PregnancyStarted for gene: " + gene.def.defName + ". For pawn: " + Pawn.NameFullColored + ". Reason: " + arg);
					}
				}
			}
        }


		/// <summary>
		/// Custom pregnancy hook. Egg layer, parasite implanter, etc. 
		/// </summary>
		private bool fired = false;
        public override void CompPostTickInterval(ref float severityAdjustment, int delta)
        {
            if (fired)
            {
                return;
            }
            if (parent.ageTicks <= Props.daysBeforeCustomPregnancyFire * 60000)
            {
                return;
			}
			fired = true;
			if (parent is Hediff_Pregnant pregnancy)
            {
                if (Pawn.genes == null)
                {
					return;
                }
                foreach (Gene gene in parent.pawn.genes.GenesListForReading.ToList())
                {
                    try
                    {
                        if (gene is not IGenePregnantHuman genePregnantHuman || !gene.Active)
                        {
                            continue;
                        }
                        if (genePregnantHuman.Notify_CustomPregnancy(pregnancy))
                        {
                            break;
                        }
                    }
                    catch (Exception arg)
                    {
                        Log.Error("Failed Notify_CustomPregnancy for gene: " + gene.def.defName + ". For pawn: " + Pawn.NameFullColored + ". Reason: " + arg);
                    }
                }
            }
        }

        public override void CompExposeData()
        {
			Scribe_Values.Look(ref fired, "fired", defaultValue: false);
		}

    }

}
