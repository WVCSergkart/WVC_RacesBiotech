using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AngelicStability : Gene_DustDrain
	{

		public HediffDef HediffDefName => def.GetModExtension<GeneExtension_Giver>().hediffDefName;

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(HediffDefName, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!pawn.health.hediffSet.HasHediff(HediffDefName))
			{
				pawn.health.AddHediff(HediffDefName);
			}
		}

	}

	public class Gene_ResurgentStability : Gene_ResurgentCellsGain
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff);
			}
		}

	}

	public class Gene_GeneticStability : Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			// if (CheckShapeshift(pawn))
			// {
				// pawn.health.AddHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff);
			// }
			if (!pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticStabilityDebuff);
			}
		}

		// public static bool CheckShapeshift(Pawn pawn)
		// {
			// List<Gene> genesListForReading = pawn?.genes?.GenesListForReading;
			// for (int i = 0; i < genesListForReading.Count; i++)
			// {
				// if (GetAllShapeShiftGeneClasses().Contains(gene.def.geneClass))
				// {
					// return true;
				// }
			// }
			// return false;
		// }

	}

	public class Gene_GeneticInstability : Gene
	{

		public override void PostAdd()
		{
			base.PostAdd();
			if (pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticInStabilityBuff))
			{
				Gene_AddOrRemoveHediff.RemoveHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticInStabilityBuff, pawn);
			}
		}

		public override void PostRemove()
		{
			base.PostRemove();
			if (!pawn.health.hediffSet.HasHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticInStabilityBuff))
			{
				pawn.health.AddHediff(WVC_GenesDefOf.WVC_XenotypesAndGenes_GeneticInStabilityBuff);
			}
		}

	}

}
