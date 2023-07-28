using Verse;

namespace WVC_XenotypesAndGenes
{

    public class HediffComp_SeverityFromResurgent : HediffComp
    {

        private Gene_ResurgentCells cachedResurgentGene;

        public HediffCompProperties_SeverityFromResurgent Props => (HediffCompProperties_SeverityFromResurgent)props;

        public override bool CompShouldRemove => base.Pawn.genes?.GetFirstGeneOfType<Gene_ResurgentCells>() == null;

        private Gene_ResurgentCells Resurgent
        {
            get
            {
                if (cachedResurgentGene == null)
                {
                    cachedResurgentGene = base.Pawn.genes.GetFirstGeneOfType<Gene_ResurgentCells>();
                }
                return cachedResurgentGene;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            // severityAdjustment += ((Resurgent.Value > 0f) ? Props.severityPerHour : Props.severityPerHourEmpty) / 2500f;
            if (!Pawn.IsHashIntervalTick(Props.refreshTicks))
            {
                return;
            }
            if (Resurgent.Value <= 0.01f)
            {
                parent.Severity = 0.01f;
            }
            else
            {
                parent.Severity = Resurgent.Value;
            }
        }

    }

}
