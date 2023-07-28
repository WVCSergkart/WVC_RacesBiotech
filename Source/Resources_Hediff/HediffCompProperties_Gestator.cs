using Verse;


namespace WVC_XenotypesAndGenes
{

    public class HediffCompProperties_Gestator : HediffCompProperties
    {
        public int gestationIntervalDays = 1;

        public string completeMessage = "WVC_RB_Gene_MechaGestator";

        public bool endogeneTransfer = true;

        public bool xenogeneTransfer = true;

        public HediffCompProperties_Gestator()
        {
            compClass = typeof(HediffComp_Gestator);
        }
    }

}
