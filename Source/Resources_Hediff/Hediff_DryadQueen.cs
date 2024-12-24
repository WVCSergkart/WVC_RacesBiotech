using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_DryadQueen : HediffWithComps
	{

		public int refreshInterval = 41982;

		private HediffStage curStage;

		public override bool ShouldRemove => Gauranlen == null;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_DryadQueen cachedDryadsQueenGene;

		public Gene_DryadQueen Gauranlen
		{
			get
			{
				if (cachedDryadsQueenGene == null || !cachedDryadsQueenGene.Active)
				{
					cachedDryadsQueenGene = pawn?.genes?.GetFirstGeneOfType<Gene_DryadQueen>();
				}
				return cachedDryadsQueenGene;
			}
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					if (Gauranlen != null && Gauranlen.DryadsListForReading.Count > 0)
					{
						StatModifier statModifier = new()
						{
							stat = StatDefOf.MaxNutrition,
							value = 0.1f * Gauranlen.DryadsListForReading.Count
						};
						curStage.statOffsets = new List<StatModifier> { statModifier };
						curStage.hungerRateFactor = 1f + (0.1f * Gauranlen.DryadsListForReading.Count);
					}
				}
				return curStage;
			}
		}

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(refreshInterval))
			{
				return;
			}
			Recache();
		}

		public void Recache()
		{
			if (Gauranlen == null)
			{
				pawn?.health?.RemoveHediff(this);
			}
			curStage = null;
		}

	}

}
