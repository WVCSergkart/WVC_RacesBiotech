using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Scarshield : HediffWithComps
	{
		public int cachedScarsCount;
		public int refreshInterval = 12000;

		private HediffStage curStage;

		// public int AdditionalBandwidth => cachedSporesCount;

		public override bool ShouldRemove => false;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			RecacheScars();
		}

		public override void PostTick()
		{
			if (!pawn.IsHashIntervalTick(refreshInterval))
			{
				return;
			}
			RecacheScars();
		}

		public void RecacheScars()
		{
			cachedScarsCount = pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification);
			curStage = null;
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null && cachedScarsCount > 0)
				{
					StatModifier statModifier = new();
					statModifier.stat = StatDefOf.IncomingDamageFactor;
					statModifier.value = 1f - (0.15f * cachedScarsCount);
					curStage = new HediffStage
					{
						statFactors = new List<StatModifier> { statModifier }
					};
				}
				return curStage;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedScarsCount, "cachedScarsCount", 0);
		}
	}

}
