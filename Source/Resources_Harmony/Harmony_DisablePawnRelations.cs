using HarmonyLib;
using RimWorld;
using Verse;

namespace WVC_XenotypesAndGenes
{

    namespace HarmonyPatches
    {

        [HarmonyPatch(typeof(PawnGenerator))]
        [HarmonyPatch("GeneratePawnRelations")]
        public static class Patch_PawnGenerator_GeneratePawnRelations
        {
            [HarmonyPrefix]
            public static bool DisablePawnRelations(Pawn pawn)
            {
                Pawn_GeneTracker genes = pawn.genes;
                if (genes == null || !genes.HasGene(WVC_GenesDefOf.WVC_FemaleOnly))
                {
                    Pawn_GeneTracker genes2 = pawn.genes;
                    if (genes2 == null || !genes2.HasGene(WVC_GenesDefOf.WVC_MaleOnly))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

    }

}
