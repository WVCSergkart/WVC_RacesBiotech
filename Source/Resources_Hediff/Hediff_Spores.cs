using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;


namespace WVC_XenotypesAndGenes
{

	public class HediffWithComps_Spores : HediffWithComps
	{
		public int cachedSporesCount;

		private HediffStage curStage;

		// public int AdditionalBandwidth => cachedSporesCount;

		public override bool ShouldRemove => cachedSporesCount == 0;

		public override string LabelBase
		{
			get
			{
				StringBuilder stringBuilder = new();
				stringBuilder.Append(base.LabelBase + " + " + cachedSporesCount);
				return stringBuilder.ToString();
			}
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null && cachedSporesCount > 0)
				{
					StatModifier statModifier = new();
					statModifier.stat = WVC_GenesDefOf.WVC_SporesBandwidth;
					statModifier.value = cachedSporesCount;
					curStage = new HediffStage
					{
						statOffsets = new List<StatModifier> { statModifier }
					};
				}
				return curStage;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref cachedSporesCount, "cachedSporesCount", 0);
		}
	}

}
