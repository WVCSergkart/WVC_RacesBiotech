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

    /// <summary>
    /// Synchronizes dormant drones. Serves as a trigger for safe synchronization, which must be loop-proof.
    /// </summary>
    public class Gene_Hivemind_Resyncer : Gene, IGeneHivemind, IGeneNonSync
    {

        private static bool syncUpdated = false;
        public static void Recache()
        {
            syncUpdated = false;
        }

        public override void TickInterval(int delta)
        {
            if (!syncUpdated)
            {
                syncUpdated = true;
                _ = HivemindUtility.HivemindPawns;
            }
        }

    }


    /// <summary>
    /// Simple drone. If gene removed can call hivemind recache.
    /// </summary>
    public class Gene_DormantDrone : Gene, IGeneHivemind, IGeneNonSync
    {

        public override void PostRemove()
        {
            base.PostRemove();
            if (HivemindUtility.InHivemind_Safe(pawn))
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
