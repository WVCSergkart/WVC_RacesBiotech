using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_SeverityFromResurgent : HediffCompProperties
	{

		public int refreshTicks = 1500;

		public HediffCompProperties_SeverityFromResurgent()
		{
			compClass = typeof(HediffComp_SeverityFromResurgent);
		}

	}

	public class HediffComp_SeverityFromResurgent : HediffComp
	{

		private Gene_ResurgentCells cachedResurgentGene;

		public HediffCompProperties_SeverityFromResurgent Props => (HediffCompProperties_SeverityFromResurgent)props;

		public override bool CompShouldRemove => Resurgent == null;

		private Gene_ResurgentCells Resurgent
		{
			get
			{
				if (cachedResurgentGene == null || !cachedResurgentGene.Active)
				{
					cachedResurgentGene = base.Pawn?.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
				}
				return cachedResurgentGene;
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			if (Resurgent.Value <= 0.01f)
			{
				parent.Severity = 0.01f;
			}
			else
			{
				parent.Severity = Resurgent.Value;
			}
		}

	}

}
