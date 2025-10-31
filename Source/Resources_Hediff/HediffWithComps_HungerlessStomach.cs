using Verse;


namespace WVC_XenotypesAndGenes
{
	public class HediffWithComps_HungerlessStomach : HediffWithComps
	{

		public int cachedMetabolism = 0;
		public int nextTick = 71211;

		private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => false;

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
					curStage = new();
					foreach (Gene item in pawn.genes.GenesListForReading)
					{
						if (!item.Overridden)
						{
							cachedMetabolism += item.def.biostatMet;
						}
					}
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
					curStage.hungerRateFactor *= 0.1f;
				}
				return curStage;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 71211, delta))
			{
				return;
			}
			Reset();
		}

		public void Reset()
		{
			cachedMetabolism = 0;
			curStage = null;
		}

	}

}
