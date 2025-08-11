using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DuplicatorDependant : Gene
	{


		[Unsaved(false)]
		private Gene_Duplicator cachedDuplicatorGene;
		public Gene_Duplicator Duplicator
		{
			get
			{
				if (cachedDuplicatorGene == null || !cachedDuplicatorGene.Active)
				{
					cachedDuplicatorGene = pawn?.genes?.GetFirstGeneOfType<Gene_Duplicator>();
				}
				return cachedDuplicatorGene;
			}
		}

		public override void TickInterval(int delta)
        {

        }

    }

	public class Gene_Duplicator_Skills : Gene_DuplicatorDependant
    {

        public override void TickInterval(int delta)
        {
			if (!pawn.IsHashIntervalTick(60912, delta))
            {
				return;
            }
			DoSync();
		}

		public void DoSync()
        {
			if (Duplicator == null)
            {
				return;
            }
			Gene_HiveMind_Skills.SyncSkills(Duplicator.PawnDuplicates);
        }

    }

}
