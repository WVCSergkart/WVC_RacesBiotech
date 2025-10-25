using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class Gene_Hivemind_Gestator : Gene_XenotypeGestator, IGeneHivemind
    {

        //public override bool Active
        //{
        //    get
        //    {
        //        if (!HivemindUtility.InHivemind_Safe(pawn))
        //        {
        //            return false;
        //        }
        //        return base.Active;
        //    }
        //}

        public override float ReqMatch => 1f;

        //public override bool RemoteControl_Hide => !HivemindUtility.InHivemind(pawn) || base.RemoteControl_Hide;

        public override List<Gene> GetPawnGenes()
        {
            List<Gene> genes = new();
            foreach (Pawn hiver in HivemindUtility.HivemindPawns)
            {
                if (hiver.genes == null)
                {
                    continue;
                }
                genes.AddRangeSafe(hiver.genes.GenesListForReading);
            }
            return genes;
        }

        public override void Notify_GestatorStart(XenotypeHolder holder)
        {
            holder.genes.AddRangeSafe(Giver.geneDefs);
        }

    }

}
