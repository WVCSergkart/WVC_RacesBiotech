using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace WVC_XenotypesAndGenes
{

	public class Gene_MorpherXenotypeTargeter : Gene_MorpherDependant
    {

        public GeneExtension_Giver Giver => def?.GetModExtension<GeneExtension_Giver>();

        public XenotypeHolder choosenXenotype = null;

        public virtual XenotypeHolder TargetedXenotype
        {
            get
            {
                if (choosenXenotype != null)
                {
                    return choosenXenotype;
                }
                //Log.Error("1");
                return new(XaG_GeneUtility.GetRandomXenotypeFromXenotypeChances(Giver?.morpherXenotypeChances, null));
            }
        }

    }

}
