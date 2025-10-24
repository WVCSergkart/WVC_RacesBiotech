using System.Collections.Generic;
using Verse;

namespace WVC_XenotypesAndGenes
{
    /// <summary>
    /// Depends on hivemind, but is not included in it.
    /// </summary>
    public class Gene_Hivemind_Dependant : Gene
    {

        //public override bool Active
        //{
        //    get
        //    {
        //        if (!InHivemind)
        //        {
        //            return false;
        //        }
        //        return base.Active;
        //    }
        //}

        //public bool? cachedInHivemind;
        //public virtual bool InHivemind
        //{
        //    get
        //    {
        //        if (!cachedInHivemind.HasValue)
        //        {
        //            cachedInHivemind = HivemindUtility.InHivemind(pawn);
        //        }
        //        return cachedInHivemind.Value;
        //    }
        //}

        //public virtual void Notify_OverriddenBy(Gene overriddenBy)
        //{
        //    Notify_GenesChanged(null);
        //}

        //public virtual void Notify_Override()
        //{
        //    Notify_GenesChanged(null);
        //}

        //public virtual void Notify_GenesChanged(Gene changedGene)
        //{
        //    cachedInHivemind = null;
        //}

        public virtual bool InHivemind
        {
            get
            {
                return HivemindUtility.InHivemind(pawn);
            }
        }

    }

    public class Gene_Hivemind_Denier : Gene_Hivemind_Dependant
    {

        public override bool InHivemind => !base.InHivemind;

    }

    public class Gene_Hivemind_Gestator : Gene_XenotypeGestator
    {

        //public override bool Active
        //{
        //    get
        //    {
        //        if (!HivemindUtility.InHivemind(pawn))
        //        {
        //            return false;
        //        }
        //        return base.Active;
        //    }
        //}

        public override float ReqMatch => 1f;

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
