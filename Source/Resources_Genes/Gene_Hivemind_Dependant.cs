using Verse;

namespace WVC_XenotypesAndGenes
{

    /// <summary>
    /// Depends on hivemind, but is not included in it.
    /// In Dev
    /// </summary>
    public class Gene_Hivemind_Dependant : Gene
    {

        public override bool Active
        {
            get
            {
                if (!InHivemind)
                {
                    return false;
                }
                return base.Active;
            }
        }

        public virtual bool InHivemind
        {
            get
            {
                return HivemindUtility.InHivemind_Safe(pawn);
            }
        }

        /// <summary>
        /// For special calls
        /// </summary>
        public virtual void ResetCollection()
        {
            if (!HivemindUtility.SuitableForHivemind(pawn))
            {
                return;
            }
            HivemindUtility.ResetCollection();
        }

    }

    public class Gene_Hivemind_Denier : Gene_Hivemind_Dependant
    {

        public override bool InHivemind => !base.InHivemind;

    }


    /// <summary>
    /// Simple drone. If gene removed can call hivemind recache.
    /// </summary>
    public class Gene_DormantDrone : Gene_Hivemind_Dependant, IGeneHivemind
    {

        public override void PostRemove()
        {
            base.PostRemove();
            if (InHivemind)
            {
                ResetCollection();
            }
        }

    }

}
