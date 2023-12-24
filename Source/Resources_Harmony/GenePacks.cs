using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Grammar;

namespace WVC_XenotypesAndGenes
{

    [HarmonyPatch(typeof(Genepack))]
    [HarmonyPatch("PostMake")]
    public static class WVC_Genepack_PostMake_Patch
    {
        private struct WVC_GenesCount
        {
            public int genesCount;

            public int architeCount;

            public float chance;
        }

        // private struct WVC_GenesCount
        // {
        // public int genesCount;

        // public int architeCount;

        // public float chance;
        // }

        private static readonly WVC_GenesCount[] WVC_BaseGenesCountProbabilities;
        private static readonly WVC_GenesCount[] WVC_UltraGenesCountProbabilities;
        private static readonly WVC_GenesCount[] WVC_MechaGenesCountProbabilities;

        static WVC_Genepack_PostMake_Patch()
        {
            WVC_BaseGenesCountProbabilities = new WVC_GenesCount[4]
            {
                new WVC_GenesCount
                {
                    genesCount = 1,
                    chance = 0.63f
                },
                new WVC_GenesCount
                {
                    genesCount = 2,
                    chance = 0.22f
                },
                new WVC_GenesCount
                {
                    genesCount = 3,
                    chance = 0.11f
                },
                new WVC_GenesCount
                {
                    genesCount = 4,
                    chance = 0.08f
                }
            };
            WVC_UltraGenesCountProbabilities = new WVC_GenesCount[7]
            {
                new WVC_GenesCount
                {
                    genesCount = 1,
                    chance = 0.63f
                },
                new WVC_GenesCount
                {
                    genesCount = 2,
                    chance = 0.22f
                },
                new WVC_GenesCount
                {
                    genesCount = 3,
                    chance = 0.11f
                },
                new WVC_GenesCount
                {
                    genesCount = 4,
                    chance = 0.09f
                },
                new WVC_GenesCount
                {
                    architeCount = 1,
                    chance = 0.27f
                },
                new WVC_GenesCount
                {
                    genesCount = 1,
                    architeCount = 1,
                    chance = 0.09f
                },
                new WVC_GenesCount
                {
                    genesCount = 2,
                    architeCount = 2,
                    chance = 0.05f
                }
            };
            WVC_MechaGenesCountProbabilities = new WVC_GenesCount[4]
            {
                new WVC_GenesCount
                {
                    architeCount = 1,
                    chance = 0.66f
                },
                new WVC_GenesCount
                {
                    architeCount = 2,
                    chance = 0.15f
                },
                new WVC_GenesCount
                {
                    architeCount = 3,
                    chance = 0.09f
                },
                new WVC_GenesCount
                {
                    architeCount = 4,
                    chance = 0.03f
                }
            };
        }

        [HarmonyPostfix]
        public static void ChangeGenesInPack(Genepack __instance, ref GeneSet ___geneSet)
        {
            if (__instance.def.defName.Contains("WVC_"))
            {
                GeneSet geneSetWVC = new();
                int? seed = null;
                // Rand.PushState(GenTicks.TicksGame);
                if (seed.HasValue)
                {
                    Rand.PushState(seed.Value);
                }
                if (__instance.def == WVC_GenesDefOf.WVC_Genepack)
                {
                    WVC_GenesCount geneCountWVC = WVC_BaseGenesCountProbabilities.RandomElementByWeight((WVC_GenesCount x) => x.chance);
                    WVC_BaseChangeGenesInPack(geneCountWVC, geneSetWVC);
                    GenerateName(geneSetWVC, WVC_GenesDefOf.WVC_NamerGenepack);
                }
                else if (__instance.def == WVC_GenesDefOf.WVC_UltraGenepack)
                {
                    WVC_GenesCount geneCountWVC = WVC_UltraGenesCountProbabilities.RandomElementByWeight((WVC_GenesCount x) => x.chance);
                    WVC_BaseChangeGenesInPack(geneCountWVC, geneSetWVC);
                    GenerateName(geneSetWVC, WVC_GenesDefOf.WVC_NamerUltraGenepack);
                }
                else if (__instance.def == WVC_GenesDefOf.WVC_MechaGenepack)
                {
                    WVC_GenesCount geneCountWVC = WVC_MechaGenesCountProbabilities.RandomElementByWeight((WVC_GenesCount x) => x.chance);
                    WVC_BaseChangeGenesInPack(geneCountWVC, geneSetWVC);
                    GenerateName(geneSetWVC, WVC_GenesDefOf.WVC_NamerMechaGenepack);
                }
                if (seed.HasValue)
                {
                    Rand.PopState();
                }
                if (geneSetWVC.Empty)
                {
                    Log.Error("Generated genepack with no genes.");
                }
                geneSetWVC.SortGenes();
                ___geneSet = geneSetWVC;
            }
        }

        private static void GenerateName(GeneSet geneSet, RulePackDef rule)
        {
            if (ModsConfig.BiotechActive && geneSet.GenesListForReading.Any())
            {
                GrammarRequest request = default;
                request.Includes.Add(rule);
                request.Rules.Add(new Rule_String("geneWord", geneSet.Label));
                request.Rules.Add(new Rule_String("geneCountMinusOne", (geneSet.GenesListForReading.Count - 1).ToString()));
                request.Constants.Add("geneCount", geneSet.GenesListForReading.Count.ToString());
                Type typeFromHandle = typeof(GeneSet);
                FieldInfo field = typeFromHandle.GetField("name", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(geneSet, GrammarResolver.Resolve("r_name", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false));
                // New
                // GrammarRequest request = default;
                // request.Includes.Add(rule);
                // request.Rules.Add(new Rule_String("geneWord", geneSet.LabelShortAdj));
                // request.Rules.Add(new Rule_String("geneCountMinusOne", (geneSet.GenesListForReading.Count - 1).ToString()));
                // request.Constants.Add("geneCount", geneSet.GenesListForReading.Count.ToString());
                // geneSet.name = GrammarResolver.Resolve("r_name", request, null, forceLog: false, null, null, null, capitalizeFirstSentence: false);
            }
        }

        private static void WVC_BaseChangeGenesInPack(WVC_GenesCount geneCountWVC, GeneSet geneSetWVC)
        {
            for (int i = 0; i < geneCountWVC.architeCount; i++)
            {
                if (DefDatabase<GeneDef>.AllDefs.Where((GeneDef x) => x.biostatArc > 0 && WVC_SpecialCanAddGeneDuringGeneration(x) && x.defName.Contains("WVC_")).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
                {
                    geneSetWVC.AddGene(result);
                }
            }
            for (int j = 0; j < geneCountWVC.genesCount; j++)
            {
                if (DefDatabase<GeneDef>.AllDefs.Where((GeneDef x) => x.biostatArc == 0 && WVC_SpecialCanAddGeneDuringGeneration(x) && x.defName.Contains("WVC_")).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out var result))
                {
                    geneSetWVC.AddGene(result);
                }
            }
        }

        private static bool WVC_SpecialCanAddGeneDuringGeneration(GeneDef gene)
        {
            List<GeneDef> genes = new();
            if (!ModsConfig.BiotechActive)
            {
                return false;
            }
            if (genes.Contains(gene))
            {
                return false;
            }
            // if (genes.Count > 0 && !GeneTuning.BiostatRange.Includes(gene.biostatMet + MetabolismTotal))
            // {
            // return false;
            // }
            if (!gene.canGenerateInGeneSet)
            {
                return false;
            }
            // if (gene.prerequisite != null && !genes.Contains(gene.prerequisite))
            // {
            // return false;
            // }
            for (int i = 0; i < genes.Count; i++)
            {
                if (gene.ConflictsWith(genes[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
