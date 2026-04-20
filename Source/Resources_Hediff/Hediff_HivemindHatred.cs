using RimWorld;
using System;
using Verse;

namespace WVC_XenotypesAndGenes
{
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

	public class Hediff_HivemindHatred : Hediff
	{

		public override bool Visible => true;

		public override bool ShouldRemove => !HivemindUtility.InHivemind(pawn);

		public override string LabelBase => "WVC_XaG_HivemindHatred".Translate();

		public HediffStage curStage;
		public override HediffStage CurStage
		{
			get
			{
				if (curStage == null)
				{
					curStage = new();
					//curStage.blocksInspirations = true;
					Upd();
				}
				return curStage;
			}
		}

		private void Upd()
		{
			try
			{
				def.aptitudes = new();
				int offset = -8;
				foreach (Faction faction in Find.FactionManager.AllFactions)
				{
					if (faction.defeated)
					{
						offset++;
					}
				}
				foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
				{
					Aptitude aptitude = new();
					aptitude.skill = skillDef;
					aptitude.level = offset;
					def.aptitudes.Add(aptitude);
				}
				pawn.skills?.DirtyAptitudes();
			}
			catch (Exception arg)
			{
				Log.Error("Failed set aptitudes. For pawn: " + pawn.Name + ". Reason: " + arg.Message);
			}
		}

	}
}
