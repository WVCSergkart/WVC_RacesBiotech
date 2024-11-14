using System;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_SeverityFromResurgent : HediffCompProperties
	{

		[Obsolete]
		public int refreshTicks = 1500;

		public HediffCompProperties_SeverityFromResurgent()
		{
			compClass = typeof(HediffComp_SeverityFromResurgent);
		}

	}

	public class HediffComp_SeverityFromResurgent : HediffComp
	{

		public HediffCompProperties_SeverityFromResurgent Props => (HediffCompProperties_SeverityFromResurgent)props;

		public override bool CompShouldRemove => Resurgent == null;

		[Unsaved(false)]
		private Gene_ResurgentCells cachedResurgentGene;

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
			//base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(1501))
			{
				return;
			}
			if (Resurgent == null)
			{
				Pawn.health.RemoveHediff(parent);
			}
			else if (Resurgent.Value <= 0.01f)
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
