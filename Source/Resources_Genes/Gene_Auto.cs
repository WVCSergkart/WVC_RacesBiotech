using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AutoResearch : Gene
	{

		//public override void Tick()
		//{
		//	if (pawn.IsHashIntervalTick(3333))
		//	{
		//		DoResearch(3333);
		//	}
		//}

		private float? cachedResearchSpeed;

		public void DoResearch(int tick)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (!cachedResearchSpeed.HasValue)
			{
				cachedResearchSpeed = pawn.GetStatValue(StatDefOf.ResearchSpeed);
			}
			ResearchManager researchManager = Find.ResearchManager;
			ResearchProjectDef currentProject = researchManager.GetProject();
			if (currentProject != null)
			{
				pawn.skills.Learn(SkillDefOf.Intellectual, 0.04f * tick);
				float researchAmount = (cachedResearchSpeed.Value * tick) / (StaticCollectionsClass.cachedColonistsCount > 0 ? StaticCollectionsClass.cachedColonistsCount : 1f);
				researchManager.ResearchPerformed(researchAmount, pawn);
			}
		}

	}

	public class Gene_SleepResearch : Gene_AutoResearch
	{

        public override void Tick()
        {
            if (!pawn.IsHashIntervalTick(4343))
            {
                return;
			}
			if (pawn.Downed || !pawn.Awake() || pawn.Deathresting)
			{
				DoResearch(4343);
			}
        }

    }

}
