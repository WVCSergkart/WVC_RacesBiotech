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

    }

    public class Gene_Hivemind_Denier : Gene_Hivemind_Dependant
    {

        public override bool InHivemind => !base.InHivemind;

    }

    //public class Gene_HiveDep_Sync : Gene, IGeneHivemind
    //{

    //    public static bool syncUpdated = false;

    //    public override void TickInterval(int delta)
    //    {
    //        if (!syncUpdated)
    //        {
    //            syncUpdated = true;
    //            _ = HivemindUtility.HivemindPawns;
    //        }
    //    }

    //}


    /// <summary>
    /// Simple drone. If gene removed can call hivemind recache.
    /// </summary>
    public class Gene_DormantDrone : Gene, IGeneHivemind, IGeneNonSync
    {

        public override void PostRemove()
        {
            base.PostRemove();
            if (pawn.InHivemind())
            {
                ResetCollection();
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

}
