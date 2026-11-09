using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_GeneHolder : Hediff
	{

		private HediffStage curStage;

		public override bool ShouldRemove => Holder == null || Holder.Containers.Empty();

		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;

		[Unsaved(false)]
		private Gene_Holder cachedGene;
		public Gene_Holder Holder
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Holder>();
				}
				return cachedGene;
			}
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					if (ModsUtility.GameNotStarted())
					{
						return new();
					}
					curStage = new();
					curStage.statFactors = new();
					if (MainDefOf.VEF_BodySize_Multiplier != null)
					{
						StatModifier bodySize = new();
						bodySize.stat = MainDefOf.VEF_BodySize_Multiplier;
						float factor = 1f;
						foreach (PawnContainerHolder pawnContainer in Holder.Containers)
						{
							factor += pawnContainer.holded.BodySize * 0.2f;
						}
						bodySize.value = factor;
						curStage.statFactors.Add(bodySize);
					}
				}
				return curStage;
			}
		}

	}

}
