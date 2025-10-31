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
					try
					{
						curStage = new();
						curStage.statOffsets = def.stages[CurStageIndex]?.statOffsets;
						curStage.statFactors = def.stages[CurStageIndex]?.statFactors;
						if (pawn.mutant?.Def?.breathesAir == false)
						{
							curStage.totalBleedFactor = 0f;
						}
						if (pawn.mutant?.Def?.entitledToMedicalCare == false)
						{
							curStage.regeneration = 33f;
						}
					}
					catch
					{
						try
						{
							curStage = def.stages[CurStageIndex];
							Log.Warning("Failed set curStage for Hediff_Thrall. On def: " + def.defName);
						}
						catch
						{
							curStage = new();
						}
					}
				}
				return curStage;
			}
		}

	}

}
