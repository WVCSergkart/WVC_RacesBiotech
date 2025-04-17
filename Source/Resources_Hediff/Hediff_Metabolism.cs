using RimWorld;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Metabolism : HediffWithComps
	{

		public int cachedMetabolism;
		public int nextTick = 333;

		private HediffStage curStage;

		public override bool ShouldRemove => false;

		public override bool Visible => cachedMetabolism != 0f;

		public override void PostAdd(DamageInfo? dinfo)
		{
			base.PostAdd(dinfo);
			RecacheHungerFactor();
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
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

		public override void Tick()
		{
			if (!GeneResourceUtility.CanTick(ref nextTick, 59389))
			{
				return;
			}
			RecacheHungerFactor();
		}

		public void RecacheHungerFactor()
		{
			int num2 = 0;
			if (WVC_Biotech.settings.enable_chimeraMetabolismHungerFactor)
			{
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
			}
			cachedMetabolism = num2;
			curStage = null;
		}

		public override void PostRemoved()
		{
			base.PostRemoved();
			if (pawn.genes.GenesListForReading.Any((gene) => gene is IGeneMetabolism))
			{
				if (HediffUtility.TryAddHediff(def, pawn, null, null))
				{
					if (DebugSettings.ShowDevGizmos)
					{
						Log.Warning("Trying to remove " + def.label + " hediff, but " + pawn.Name.ToString() + " has the required gene. Hediff is added back.");
					}
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedMetabolism, "cachedMetabolism", 0);
		}

	}

}
