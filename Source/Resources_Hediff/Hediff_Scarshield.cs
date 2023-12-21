using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Scars : HediffWithComps
	{

		// public XaG_HediffDef def;

		// public StatDef statDef;
		// public bool useFactorInsteadOffset = false;

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

		public override HediffStage CurStage
		{
			get
			{
                if (def is XaG_HediffDef newDef && newDef.statDef != null && curStage == null && cachedScarsCount > 0)
                {
                    StatModifier statModifier = new();
                    statModifier.stat = newDef.statDef;
                    if (newDef.useFactorInsteadOffset)
                    {
                        statModifier.value = 1f - (0.15f * cachedScarsCount);
                        curStage = new HediffStage
                        {
                            statFactors = new List<StatModifier> { statModifier }
                        };
                    }
                    else
                    {
                        statModifier.value = 0.2f * cachedScarsCount;
                        curStage = new HediffStage
                        {
                            statOffsets = new List<StatModifier> { statModifier }
                        };
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
			RecacheScars();
		}

		public void RecacheScars()
		{
			cachedScarsCount = pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification);
			curStage = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedScarsCount, "cachedScarsCount", 0);
		}
	}

	// public class HediffWithComps_Scarshield : HediffWithComps_Scars
	// {

		// public override HediffStage CurStage
		// {
			// get
			// {
				// if (curStage == null && cachedScarsCount > 0)
				// {
					// StatModifier statModifier = new();
					// statModifier.stat = StatDefOf.IncomingDamageFactor;
					// statModifier.value = 1f - (0.15f * cachedScarsCount);
					// curStage = new HediffStage
					// {
						// statFactors = new List<StatModifier> { statModifier }
					// };
				// }
				// return curStage;
			// }
		// }

	// }

	// public class HediffWithComps_Scarstomach : HediffWithComps_Scars
	// {

		// public override HediffStage CurStage
		// {
			// get
			// {
				// if (curStage == null && cachedScarsCount > 0)
				// {
					// StatModifier statModifier = new();
					// statModifier.stat = StatDefOf.MaxNutrition;
					// statModifier.value = 0.2f * cachedScarsCount;
					// curStage = new HediffStage
					// {
						// statOffsets = new List<StatModifier> { statModifier }
					// };
				// }
				// return curStage;
			// }
		// }

	// }

}
