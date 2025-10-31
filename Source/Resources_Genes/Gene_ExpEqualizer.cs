using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_ExpEqualizer : Gene
	{

		public GeneExtension_General General => def?.GetModExtension<GeneExtension_General>();

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(6000, delta))
			{
				return;
			}
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			EqualizeSkillsExp(pawn, 6000);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			if (DebugSettings.ShowDevGizmos)
			{
				yield return new Command_Action
				{
					defaultLabel = "DEV: EqualizeSkillsExp",
					action = delegate
					{
						EqualizeSkillsExp(pawn, 6000);
					}
				};
			}
		}

		public void EqualizeSkillsExp(Pawn pawn, int tick, float expPercentPerDay = 0.1f)
		{
			float expPercent = (expPercentPerDay / 60000) * tick;
			SkillRecord lowestSkill = null;
			SkillRecord higherSkill = null;
			foreach (SkillRecord skill in pawn.skills.skills)
			{
				if (skill.TotallyDisabled)
				{
					continue;
				}
				if (lowestSkill == null)
				{
					lowestSkill = skill;
				}
				if (higherSkill == null)
				{
					higherSkill = skill;
				}
				if (skill.XpTotalEarned > higherSkill.XpTotalEarned)
				{
					higherSkill = skill;
				}
				if (skill.XpTotalEarned < lowestSkill.XpTotalEarned)
				{
					lowestSkill = skill;
				}
			}
			if (higherSkill == null || lowestSkill == null || lowestSkill == higherSkill || lowestSkill.levelInt == higherSkill.levelInt)
			{
				return;
			}
			if (lowestSkill.XpTotalEarned + higherSkill.XpTotalEarned * expPercent > higherSkill.XpTotalEarned - higherSkill.XpTotalEarned * expPercent)
			{
				return;
			}
			lowestSkill.Learn(higherSkill.XpTotalEarned * expPercent, true, true);
			higherSkill.Learn(-higherSkill.XpTotalEarned * expPercent, true, true);
		}

	}

}
