using Verse;


namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_GolemGestator : HediffCompProperties
    {
        public int gestationIntervalDays = 1;

        public string completeMessage = "WVC_RB_Gene_MechaGestator";

        public bool endogeneTransfer = true;

        public bool xenogeneTransfer = true;

        public GeneDef geneDef;

        public HediffCompProperties_GolemGestator()
        {
            compClass = typeof(HediffComp_GolemGestator);
        }
    }

}
