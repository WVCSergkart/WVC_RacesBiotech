using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_XenotypesAndGenes
{
	public class Gene_ArchiverDependant : Gene
	{

		[Unsaved(false)]
		private Gene_Archiver cachedArchiverGene;
		public Gene_Archiver Archiver
		{
			get
			{
				if (cachedArchiverGene == null || !cachedArchiverGene.Active)
				{
					cachedArchiverGene = pawn?.genes?.GetFirstGeneOfType<Gene_Archiver>();
				}
				return cachedArchiverGene;
			}
		}

		public override void TickInterval(int delta)
		{

		}

	}

	public class Gene_Archiver_SkillsSync : Gene_ArchiverDependant
	{

		public override void TickInterval(int delta)
		{
			if (!pawn.IsHashIntervalTick(59994, delta))
			{
				return;
			}
			SyncSkills();
		}
		public void SyncSkills()
		{
			List<Pawn> archivedPawns = Archiver?.ArchivedPawns.ToList();
			if (!archivedPawns.Contains(pawn))
			{
				archivedPawns.Add(pawn);
			}
			Gene_HiveMind_Skills.SyncSkills(archivedPawns);
		}

	}

}
