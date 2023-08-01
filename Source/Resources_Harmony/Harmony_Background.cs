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
    // [StaticConstructorOnStartup]
    // public static class WVC_XenotypesAndGenes_Main
    // {
    // static WVC_XenotypesAndGenes_Main()
    // {
    // new Harmony("wvc.sergkart.races.biotech").PatchAll();
    // }
    // }

    // ===============================
    namespace HarmonyPatches
    {

        [StaticConstructorOnStartup]
        public static class GraphicsCache
        {
			// public static readonly object GeneBackground_Endogene = Activator.CreateInstance(WVC_GeneUIUtility_DrawGeneIcon_Patch.cachedTextureType, "UI/Icons/Genes/GeneBackground_Endogene");
			// public static readonly object GeneBackground_Xenogene = Activator.CreateInstance(WVC_GeneUIUtility_DrawGeneIcon_Patch.cachedTextureType, "UI/Icons/Genes/GeneBackground_Xenogene");
			public static readonly object GeneBackground_Archite = Activator.CreateInstance(WVC_GeneUIUtility_DrawGeneIcon_Patch.cachedTextureType, "UI/Icons/Genes/GeneBackground_ArchiteGene");
        }

        [HarmonyPatch(typeof(GeneUIUtility))]
        [HarmonyPatch("DrawGeneBasics")]
        public static class WVC_GeneUIUtility_DrawGeneIcon_Patch
        {
			public static Type cachedTextureType = AccessTools.TypeByName("Verse.CachedTexture");

            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
				// FieldInfo loadsFieldEndo = AccessTools.Field(typeof(GeneUIUtility), "GeneBackground_Endogene");
				// FieldInfo loadsFieldXeno = AccessTools.Field(typeof(GeneUIUtility), "GeneBackground_Xenogene");
                FieldInfo loadsFieldArch = AccessTools.Field(typeof(GeneUIUtility), "GeneBackground_Archite");
                List<CodeInstruction> codes = instructions.ToList();
                for (int i = 0; i < codes.Count; i++)
                {
                    CodeInstruction code = codes[i];
                    // if (codes[i].opcode == OpCodes.Ldsfld && codes[i].LoadsField(loadsFieldEndo))
                    // {
                        // yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(codes[i]);
                        // yield return new CodeInstruction(OpCodes.Call, typeof(WVC_GeneUIUtility_DrawGeneIcon_Patch).GetMethod("ChooseEndogeneBackground"));
                    // }
					// else if (codes[i].opcode == OpCodes.Ldsfld && codes[i].LoadsField(loadsFieldXeno))
					// {
						// yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(codes[i]);
						// yield return new CodeInstruction(OpCodes.Call, typeof(WVC_GeneUIUtility_DrawGeneIcon_Patch).GetMethod("ChooseXenogeneBackground"));
					// }
					if (codes[i].opcode == OpCodes.Ldsfld && codes[i].LoadsField(loadsFieldArch))
					{
						yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(codes[i]);
						yield return new CodeInstruction(OpCodes.Call, typeof(WVC_GeneUIUtility_DrawGeneIcon_Patch).GetMethod("ChooseArchiteBackground"));
					}
					else
					{
						yield return code;
					}
                }
            }

			// public static object ChooseEndogeneBackground(GeneDef gene)
			// {
				// if (gene.GetModExtension<GeneExtension_Background>()?.backgroundPathEndogenes != null)
				// {
					// return Activator.CreateInstance(cachedTextureType, gene.GetModExtension<GeneExtension_Background>().backgroundPathEndogenes);
				// }
				// return GraphicsCache.GeneBackground_Endogene;
			// }
			// public static object ChooseXenogeneBackground(GeneDef gene)
			// {
				// if (gene.GetModExtension<GeneExtension_Background>()?.backgroundPathXenogenes != null)
				// {
					// return Activator.CreateInstance(cachedTextureType, gene.GetModExtension<GeneExtension_Background>().backgroundPathXenogenes);
				// }
				// return GraphicsCache.GeneBackground_Xenogene;
			// }
			public static object ChooseArchiteBackground(GeneDef gene)
			{
				if (gene.GetModExtension<GeneExtension_Background>()?.backgroundPathArchite != null)
				{
					return Activator.CreateInstance(cachedTextureType, gene.GetModExtension<GeneExtension_Background>().backgroundPathArchite);
				}
				return GraphicsCache.GeneBackground_Archite;
			}
        }

    }

}
