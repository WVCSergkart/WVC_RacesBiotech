using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentDependent : Gene
	{

		[Unsaved(false)]
		private Gene_ResurgentCells cachedResurgentGene;

		public Gene_ResurgentCells Resurgent
		{
			get
			{
				if (cachedResurgentGene == null || !cachedResurgentGene.Active)
				{
					cachedResurgentGene = pawn.genes.GetFirstGeneOfType<Gene_ResurgentCells>();
				}
				return cachedResurgentGene;
			}
		}

	}

	// public class Gene_ResurgentActive : Gene
	// {


	// }

}
