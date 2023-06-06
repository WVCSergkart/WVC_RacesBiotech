using System.Collections.Generic;
using RimWorld;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Golems : HediffWithComps
	{
		// private const int BandNodeCheckInterval = 60;

		private float cachedWorkSpeedFactor;

		private HediffStage curStage;

		// public int AdditionalBandwidth => cachedTunedBandNodesCount;

		// public override bool ShouldRemove => cachedTunedBandNodesCount == 0;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null && cachedWorkSpeedFactor > 0f)
				{
                    StatModifier statModifier = new StatModifier
                    {
                        stat = StatDefOf.WorkSpeedGlobal,
                        value = cachedWorkSpeedFactor
                    };
                    curStage = new HediffStage
                    {
                        statFactors = new List<StatModifier> { statModifier }
                    };
                }
				return curStage;
			}
		}

		public override void PostTick()
		{
			base.PostTick();
			// if (pawn.IsHashIntervalTick(60000))
			if (pawn.IsHashIntervalTick(60))
			{
				RecacheWork();
			}
		}

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			RecacheWork();
		}

		public void RecacheWork()
		{
			// cachedWorkSpeedFactor = 
			Pawn overseer = pawn.GetOverseer();
			if (overseer != null)
			{
				cachedWorkSpeedFactor = overseer.GetStatValue(StatDefOf.PsychicSensitivity);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedWorkSpeedFactor, "cachedWorkSpeedFactor");
		}
	}

}
