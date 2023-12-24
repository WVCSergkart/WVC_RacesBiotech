using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class HediffCompProperties_SeverityFromDust : HediffCompProperties
	{

		// public float severityPerHourEmpty;

		// public float severityPerHour;

		// 1500 = 1 hour
		public int refreshTicks = 1500;

		public HediffCompProperties_SeverityFromDust()
		{
			compClass = typeof(HediffComp_SeverityFromDust);
		}

	}

	public class HediffComp_SeverityFromDust : HediffComp
	{

		private Need_Food cachedDustGene;

		public HediffCompProperties_SeverityFromDust Props => (HediffCompProperties_SeverityFromDust)props;

		public override bool CompShouldRemove => base.Pawn.genes?.GetFirstGeneOfType<Gene_Dust>() == null;

		private Need_Food Dust
		{
			get
			{
				if (cachedDustGene == null)
				{
					cachedDustGene = base.Pawn.needs?.food;
					// Need_Food need_Food = pawn.needs?.food;
				}
				return cachedDustGene;
			}
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			// severityAdjustment += ((Dust.Value > 0f) ? Props.severityPerHour : Props.severityPerHourEmpty) / 2500f;
			if (!Pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			if (Dust.CurLevel <= 0.01f)
			{
				parent.Severity = 0.01f;
			}
			else
			{
				parent.Severity = Dust.CurLevel;
			}
		}

	}

}
