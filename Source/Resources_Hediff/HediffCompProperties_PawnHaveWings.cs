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

		public override bool CompShouldRemove => false;

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

		//public override void CompPostTick(ref float severityAdjustment)
		//{
		//    if (Pawn.IsHashIntervalTick(120))
		//    {
		//        if (Wings == null)
		//        {
		//            Pawn.health.RemoveHediff(parent);
		//        }
		//    }
		//}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//base.CompPostTickInterval(ref severityAdjustment, delta);
			if (!Pawn.IsHashIntervalTick(9999, delta))
			{
				return;
			}
			if (Wings == null)
			{
				Pawn.health.RemoveHediff(parent);
			}
		}

	}

}
