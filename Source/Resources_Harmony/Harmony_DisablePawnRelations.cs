using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace WVC_XenotypesAndGenes
{

    namespace HarmonyPatches
    {

        [HarmonyPatch(typeof(PawnGenerator))]
        [HarmonyPatch("GeneratePawnRelations")]
        public static class WVC_PawnGenerator_GeneratePawnRelations_Patch
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
