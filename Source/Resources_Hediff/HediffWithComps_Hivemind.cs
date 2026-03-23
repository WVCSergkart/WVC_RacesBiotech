using RimWorld;
using System.Linq;
using Verse;

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

	//public class HediffWithComps_Hivemind_TempRange : HediffWithComps_Hivemind
	//{

	//	public static HediffStage curStage;

	//	public static void Recache()
	//	{
	//		curStage = null;
	//	}

	//	public override HediffStage CurStage
	//	{
	//		get
	//		{
	//			if (curStage == null)
	//			{
	//				curStage = new();
	//				float newLimit = HivemindUtility.HivemindPawns.Count;
	//				curStage.statOffsets = new();
	//				StatModifier statMod = new();
	//				statMod.stat = StatDefOf.GlobalLearningFactor;
	//				statMod.value = -0.9f + (newLimit * 0.18f);
	//				curStage.statOffsets.Add(statMod);
	//			}
	//			return curStage;
	//		}
	//	}

	//}

	//public class Hediff_HivemindHatred : Hediff
	//{

	//	public override bool Visible =>  false;

	//	public override string LabelBase => "WVC_XaG_HivemindHatred".Translate();

	//	public static HediffStage curStage;
	//	public override HediffStage CurStage
	//	{
	//		get
	//		{
	//			if (curStage == null)
	//			{
	//				curStage = new();
	//				float newLimit = Find.FactionManager.AllFactions.Where((faction) => faction.defeated).Count();
	//				aptitudes;
	//			}
	//			return curStage;
	//		}
	//	}

	//}
}
