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
		private Gene_Resurgent cachedResurgentGene;

		private Gene_Resurgent Resurgent
		{
			get
			{
				if (cachedResurgentGene == null || !cachedResurgentGene.Active)
				{
					cachedResurgentGene = base.Pawn?.genes?.GetFirstGeneOfType<Gene_Resurgent>();
				}
				return cachedResurgentGene;
			}
		}

		public override void CompPostTickInterval(ref float severityAdjustment, int delta)
		{
			//base.CompPostTick(ref severityAdjustment);
			if (!Pawn.IsHashIntervalTick(2400, delta))
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
