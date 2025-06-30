using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_Thrall : Hediff
	{

		private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => true;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					curStage.statOffsets = def.stages[CurStageIndex].statOffsets;
					curStage.statFactors = def.stages[CurStageIndex].statFactors;
					if (!pawn.mutant.Def.breathesAir)
                    {
						curStage.totalBleedFactor = 0f;
                    }
					if (!pawn.mutant.Def.entitledToMedicalCare)
					{
						curStage.regeneration = 33f;
					}
				}
				return curStage;
			}
		}

	}

}
