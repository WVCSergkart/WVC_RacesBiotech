using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_PawnHaveWings : HediffCompProperties
	{

		public HediffCompProperties_PawnHaveWings()
		{
			compClass = typeof(HediffComp_PawnHaveWings);
		}

	}

	public class HediffComp_PawnHaveWings : HediffComp
	{

		public HediffCompProperties_PawnHaveWings Props => (HediffCompProperties_PawnHaveWings)props;

		public override bool CompShouldRemove => Wings == null;

		[Unsaved(false)]
		private Gene_Wings cachedWingsGene;

		public Gene_Wings Wings
		{
			get
			{
				if (cachedWingsGene == null || !cachedWingsGene.Active)
				{
					cachedWingsGene = Pawn?.genes?.GetFirstGeneOfType<Gene_Wings>();
				}
				return cachedWingsGene;
			}
		}

		// public override bool CompDisallowVisible()
		// {
			// return Wings == null;
		// }

		// public override void CompPostTick(ref float severityAdjustment)
		// {
			// if (Pawn.IsHashIntervalTick(120))
			// {
				// if (Wings == null)
				// {
					// Pawn.health.RemoveHediff(parent);
				// }
			// }
		// }

	}

}
