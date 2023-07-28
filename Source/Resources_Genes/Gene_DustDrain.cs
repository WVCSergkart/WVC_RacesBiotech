using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_DustDrain : Gene, IGeneResourceDrain
    {
        [Unsaved(false)]
        private Gene_Dust cachedHemogenGene;

        // private const float MinAgeForDrain = 3f;

        public Gene_Resource Resource
        {
            get
            {
                if (cachedHemogenGene == null || !cachedHemogenGene.Active)
                {
                    cachedHemogenGene = pawn.genes.GetFirstGeneOfType<Gene_Dust>();
                }
                return cachedHemogenGene;
            }
        }

        public bool CanOffset
        {
            get
            {
                if (Active)
                {
                    return true;
                }
                return false;
            }
        }

        // public float ResourceLossPerDay => ResourceLoss();
        public float ResourceLossPerDay => Gene_Dust.ResourceLoss(pawn, def.resourceLossPerDay);

        // public float ResourceLoss()
        // {
        // if (cachedHemogenGene.PawnUnconscious())
        // {
        // return -1 * def.resourceLossPerDay;
        // }
        // return def.resourceLossPerDay;
        // }

        public Pawn Pawn => pawn;

        public string DisplayLabel => Label + " (" + "Gene".Translate() + ")";

        public override void Tick()
        {
            base.Tick();
            UndeadUtility.TickResourceDrain(this);
        }
    }

}
