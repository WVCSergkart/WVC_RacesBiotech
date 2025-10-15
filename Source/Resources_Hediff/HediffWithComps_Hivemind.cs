using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

    public class HediffWithComps_Hivemind : HediffWithComps
	{

		public override bool Visible => !WVC_Biotech.settings.hideGeneHediffs;

	}

	public class HediffWithComps_Hivemind_Beauty : HediffWithComps_Hivemind
	{

		public static HediffStage curStage;

		public static void Recache()
		{
			curStage = null;
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					float newLimit = HivemindUtility.HivemindPawns.Count;
					curStage.statOffsets = new();
					StatModifier statMod = new();
					statMod.stat = StatDefOf.PawnBeauty;
					statMod.value = -3 + newLimit;
					curStage.statOffsets.Add(statMod);
				}
				return curStage;
			}
		}

		//public override void TickInterval(int delta)
		//{

		//}

	}

	public class HediffWithComps_Hivemind_Learning : HediffWithComps_Hivemind
	{

		public static HediffStage curStage;

		public static void Recache()
		{
			curStage = null;
		}

		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					float newLimit = HivemindUtility.HivemindPawns.Count;
					curStage.statOffsets = new();
					StatModifier statMod = new();
					statMod.stat = StatDefOf.GlobalLearningFactor;
					statMod.value = -0.9f + (newLimit * 0.18f);
					curStage.statOffsets.Add(statMod);
				}
				return curStage;
			}
		}

	}

}
