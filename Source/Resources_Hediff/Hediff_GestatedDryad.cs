using RimWorld;
using System.Collections.Generic;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class Hediff_GestatedDryad : Hediff
	{

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		private HediffStage curStage;

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
                    StatModifier statModifier = new()
                    {
                        stat = StatDefOf.FilthRate,
                        value = WVC_Biotech.settings.gestatedDryads_FilthRateFactor
                    };
                    curStage = new HediffStage
                    {
                        statFactors = new List<StatModifier> { statModifier }
                    };
                }
				return curStage;
			}
		}

	}

}