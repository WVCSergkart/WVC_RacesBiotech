using Verse;

namespace WVC_XenotypesAndGenes
{

	public class Gene_ResurgentDependent : Gene
	{

		[Unsaved(false)]
		private Gene_ResurgentCells cachedResurgentGene;

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					if (pawn?.genes != null)
					{
						return ResurgentCells(pawn);
					}
				}
				return base.Active;
			}
		}

		public bool ResurgentCells(Pawn pawn)
		{
			if (cachedResurgentGene == null)
			{
				cachedResurgentGene = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			}
			if (cachedResurgentGene != null)
			{
				if (cachedResurgentGene.Value >= cachedResurgentGene.MinLevelForAlert)
				{
					return true;
				}
			}
			return false;
		}

	}

	public class Gene_ResurgentActive : Gene
	{

		[Unsaved(false)]
		private Gene_ResurgentCells cachedResurgentGene;

		public override bool Active
		{
			get
			{
				if (base.Active)
				{
					if (pawn?.genes != null)
					{
						return ResurgentCells(pawn);
					}
				}
				return false;
			}
		}

		public bool ResurgentCells(Pawn pawn)
		{
			if (cachedResurgentGene == null)
			{
				cachedResurgentGene = pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>();
			}
			if (cachedResurgentGene != null)
			{
				if (cachedResurgentGene.Value >= def.resourceLossPerDay)
				{
					return true;
				}
			}
			return false;
		}

	}
}
