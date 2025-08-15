namespace WVC_XenotypesAndGenes
{

    public class Gene_Duplicator_MeDuplicate : Gene_DuplicatorSubGene
    {

        public override bool Active
        {
            get
            {
                if (IsDuplicate)
                {
                    return base.Active;
                }
                return false;
            }
        }

    }

    // =====================================

}
