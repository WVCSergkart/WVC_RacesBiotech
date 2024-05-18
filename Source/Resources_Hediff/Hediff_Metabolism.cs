using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Metabolism : HediffWithComps
	{

		public int cachedMetabolism;
		public int refreshInterval = 11982;

		private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => cachedMetabolism != 0f;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			RecacheScars();
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null && cachedMetabolism != 0f)
				{
					curStage = new();
					if (cachedMetabolism > 0f)
					{
						float factor = 1f - (cachedMetabolism * 0.1f);
						curStage.hungerRateFactor = factor > 0f ? factor : 0f;
					}
					else if (cachedMetabolism < 0f)
					{
						float factor = 1f + ((cachedMetabolism * -1) * 0.1112f);
						curStage.hungerRateFactor = factor;
					}
					// curStage.hungerRateFactor = newDef.curve.Evaluate(cachedMetabolism);
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
			int num2 = 0;
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (!item.Overridden)
				{
					num2 += item.def.biostatMet;
				}
			}
			if (num2 >= 5)
			{
				num2 -= 5;
			}
			else if (num2 <= -5)
			{
				num2 += 5;
			}
			else
			{
				num2 = 0;
			}
			// if (num2 < 0)
			// {
				// num2 *= -1;
			// }
			cachedMetabolism = num2;
			curStage = null;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedMetabolism, "cachedMetabolism", 0);
		}

	}

}
