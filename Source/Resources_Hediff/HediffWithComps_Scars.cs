using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

    public class HediffWithComps_Scars : HediffWithComps
	{

		// public XaG_HediffDef def;

		// public StatDef statDef;
		// public bool useFactorInsteadOffset = false;

		public int cachedScarsCount = 0;
		public int nextTick = 52988;

		private HediffStage curStage;

		// public int AdditionalBandwidth => cachedSporesCount;

		public override bool ShouldRemove => false;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			Reset();
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new HediffStage
					{
						statOffsets = new(),
						statFactors = new()
					};
					cachedScarsCount = pawn.health.hediffSet.GetHediffCount(HediffDefOf.Scarification);
					if (cachedScarsCount > 0 && def is XaG_HediffDef newDef && newDef.statModifiers != null)
					{
						if (!newDef.statModifiers.statFactors.NullOrEmpty())
						{
							foreach (StatModifier item in newDef.statModifiers.statFactors)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								float factor = 1f - (item.value * cachedScarsCount);
								statModifier.value = factor > 0f ? factor : 0f;
								curStage.statFactors.Add(statModifier);
							}
						}
						if (!newDef.statModifiers.statOffsets.NullOrEmpty())
						{
							foreach (StatModifier item in newDef.statModifiers.statOffsets)
							{
								StatModifier statModifier = new();
								statModifier.stat = item.stat;
								statModifier.value = item.value * cachedScarsCount;
								curStage.statOffsets.Add(statModifier);
							}
						}
					}
				}
				return curStage;
			}
		}

		public override void TickInterval(int delta)
        {
            if (GeneResourceUtility.CanTick(ref nextTick, 52988, delta))
            {
                Reset();
            }
        }

        public void Reset()
		{
			cachedScarsCount = 0;
			curStage = null;
		}

		//public override void ExposeData()
		//{
		//	base.ExposeData();
		//	Scribe_Values.Look(ref cachedScarsCount, "cachedScarsCount", 0);
		//}
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
