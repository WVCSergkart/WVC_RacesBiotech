using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_AutoResearch : XaG_Gene, IGeneRecacheable
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
			//SimpleCurve curve = new()
			//{
			//	new CurvePoint(0, 3),
			//	new CurvePoint(1000, 5),
			//	new CurvePoint(2000, 7),
			//	new CurvePoint(3000, 12),
			//	new CurvePoint(10000, 40),
			//	new CurvePoint(100000, 60)
			//};
			if (currentProject != null)
			{
				//float amount = cachedResearchSpeed.Value * researchRate * tick * (1f - (StaticCollectionsClass.cachedNonDeathrestingColonistsCount > 1 ? StaticCollectionsClass.cachedNonDeathrestingColonistsCount * 0.01f : 0f));
				//float amount = (currentProject.baseCost / (60000 * curve.Evaluate(currentProject.baseCost))) * researchRate * cachedResearchSpeed.Value * tick;
				//researchManager.ResearchPerformed(amount, pawn);
				//pawn.skills.Learn(SkillDefOf.Intellectual, 0.05f * researchRate * tick);
				// Vanilla-like. Hmm..
				Pawn actor = pawn;
				float statValue = cachedResearchSpeed.Value;
				statValue *= researchRate;
				Find.ResearchManager.ResearchPerformed(statValue * (float)tick, actor);
				actor.skills.Learn(SkillDefOf.Intellectual, 0.05f * (float)tick);
			}
		}

		public virtual void Notify_GenesRecache(Gene changedGene)
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
			DoResearch(14343, 0.2f * MainGene.FormsCount);
		}

	}

}
