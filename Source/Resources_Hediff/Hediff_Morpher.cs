using Verse;


namespace WVC_XenotypesAndGenes
{
    public class Hediff_Morpher : Hediff
	{

		public override bool ShouldRemove => false;

		public override bool Visible => false;

		[Unsaved(false)]
		private Gene_Morpher cachedGene;

		public Gene_Morpher Gene
		{
			get
			{
				if (cachedGene == null || !cachedGene.Active)
				{
					cachedGene = pawn?.genes?.GetFirstGeneOfType<Gene_Morpher>();
				}
				return cachedGene;
			}
		}

        public override void TickInterval(int delta)
        {
			if (pawn.IsHashIntervalTick(2500, delta))
            {
                Cast();
            }
        }

        private void Cast()
        {
            if (Gene?.Trigger == null)
            {
                pawn.health.RemoveHediff(this);
            }
            else
            {
                Gene.Trigger?.MorphAutoCast();
            }
        }

    }

}
