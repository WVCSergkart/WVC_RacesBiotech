// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    public class StatPart_XenotypeSerum : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (ModsConfig.BiotechActive && req.HasThing && req.Thing is XenotypeSerum xenotypeSerum)
            {
                float num = 0f;
                if (xenotypeSerum?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotype != null)
                {
                    XenotypeDef xenotypeDef = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotype;
                    num = XaG_GeneUtility.XenotypeCost(xenotypeDef);
                }
                if (xenotypeSerum?.TryGetComp<CompTargetEffect_DoJobOnTarget>()?.xenotypeDef != null)
                {
                    XenotypeDef xenotypeDef = xenotypeSerum.TryGetComp<CompTargetEffect_DoJobOnTarget>().xenotypeDef;
                    num = XaG_GeneUtility.XenotypeCost(xenotypeDef);
                }
                if (xenotypeSerum?.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>() != null)
                {
                    XenotypeDef xenotypeA = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().endotype;
                    XenotypeDef xenotypeB = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_Hybrid>().xenotype;
                    num = XaG_GeneUtility.XenotypeCost(xenotypeA) + XaG_GeneUtility.XenotypeCost(xenotypeB);
                }
                // if (xenotypeSerum?.TryGetComp<CompUseEffect_XenotypeForcer_II>()?.xenotypeDef != null)
                // {
                // XenotypeDef xenotypeDef = xenotypeSerum.TryGetComp<CompUseEffect_XenotypeForcer_II>().xenotypeDef;
                // }
                val += (100 * num);
                // val *= NumGenesFactor(genepack.GeneSet);
                // val *= ArchiteCostFactor(genepack.GeneSet);
                // val *= GenesFactorsDefined(genepack.GeneSet);
            }
        }

        // private float NumGenesFactor(GeneSet geneSet)
        // {
        // return Mathf.Max(3.5f - 0.5f * (float)geneSet.GenesListForReading.Count, 0.5f);
        // }

        // private float ArchiteCostFactor(GeneSet geneSet)
        // {
        // return 1f + 3f * (float)geneSet.ArchitesTotal;
        // }

        // private float GenesFactorsDefined(GeneSet geneSet)
        // {
        // float num = 1f;
        // for (int i = 0; i < geneSet.GenesListForReading.Count; i++)
        // {
        // num *= geneSet.GenesListForReading[i].marketValueFactor;
        // }
        // return num;
        // }

        public override string ExplanationPart(StatRequest req)
        {
            return null;
        }
    }

}
