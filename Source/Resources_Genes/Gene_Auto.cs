using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AutoResearch : Gene, IGeneNotifyGenesChanged
	{

		//public override void Tick()
		//{
		//	if (pawn.IsHashIntervalTick(3333))
		//	{
		//		DoResearch(3333);
		//	}
		//}

		private float? cachedResearchSpeed;

		public void DoResearch(int tick, float researchRate = 0.04f)
		{
			if (pawn.Faction != Faction.OfPlayer)
			{
				return;
			}
			if (!cachedResearchSpeed.HasValue)
			{
				if (StatDefOf.ResearchSpeed.Worker.IsDisabledFor(pawn))
				{
					cachedResearchSpeed = 0.01f;
				}
				else
				{
					cachedResearchSpeed = pawn.GetStatValue(StatDefOf.ResearchSpeed);
				}
			}
			ResearchManager researchManager = Find.ResearchManager;
			ResearchProjectDef currentProject = researchManager.GetProject();
			if (currentProject != null)
			{
				//pawn.skills.Learn(SkillDefOf.Intellectual, 0.01f * tick);
				//float amount = researchRate * (cachedResearchSpeed.Value * tick) - (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1 ? StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 12.8f : 0f);
				//float researchAmount = Mathf.Clamp(amount, currentProject.baseCost * 0.01f, (currentProject.baseCost * 0.2f));
				//researchManager.ResearchPerformed(researchAmount, pawn);
				float amount = cachedResearchSpeed.Value * researchRate * tick * (1f - (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1 ? StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.01f : 0f));
				researchManager.ResearchPerformed(amount, pawn);
				pawn.skills.Learn(SkillDefOf.Intellectual, 0.05f * researchRate * tick);
			}
		}

		public virtual void Notify_GenesChanged(Gene changedGene)
		{
			cachedResearchSpeed = null;
		}

	}

	public class Gene_SleepResearch : Gene_AutoResearch
	{

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(4343, delta))
			{
				return;
			}
			if (pawn.Downed || !pawn.Awake() || pawn.Deathresting)
			{
				DoResearch(4343, 0.22f);
			}
		}

	}

	public class Gene_ArchiverResearch : Gene_AutoResearch
	{

		[Unsaved(false)]
		private Gene_Archiver cachedMainGene;
		public Gene_Archiver MainGene
		{
			get
			{
				if (cachedMainGene == null || !cachedMainGene.Active)
				{
					cachedMainGene = pawn?.genes?.GetFirstGeneOfType<Gene_Archiver>();
				}
				return cachedMainGene;
			}
		}

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(14343, delta))
			{
				return;
			}
			if (MainGene == null)
			{
				return;
			}
			DoResearch(14343, 0.6f * MainGene.FormsCount);
		}

	}

}
