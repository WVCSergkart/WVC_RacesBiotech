using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using UnityEngine;
using Verse;
using WVC;
using WVC_XenotypesAndGenes;

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
	public class WVC_XenotypesAndGenes_Main : Mod
	{
		public WVC_XenotypesAndGenes_Main(ModContentPack content)
			: base(content)
		{
			new Harmony("wvc.sergkart.races.biotech").PatchAll();
		}
	}

	// ===============================

	[StaticConstructorOnStartup]
	public static class GraphicsCache
	{
		public static readonly CachedTexture GeneBackground_Archite = new("UI/Icons/Genes/GeneBackground_ArchiteGene");
	}

	[HarmonyPatch(typeof(GeneUIUtility))]
	[HarmonyPatch("DrawGeneBasics")]
	public static class WVC_GeneUIUtility_DrawGeneArchite_Patch
	{
#pragma warning disable IDE0051 // Remove unused private members
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
#pragma warning restore IDE0051 // Remove unused private members
        {
			FieldInfo loadsField = AccessTools.Field(typeof(GeneUIUtility), "GeneBackground_Archite");
			List<CodeInstruction> codes = instructions.ToList();
			for (int i = 0; i < codes.Count; i++)
			{
				CodeInstruction code = codes[i];
				if (codes[i].opcode == OpCodes.Ldsfld && codes[i].LoadsField(loadsField))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(codes[i]);
					yield return new CodeInstruction(OpCodes.Call, typeof(WVC_GeneUIUtility_DrawGeneArchite_Patch).GetMethod("WVC_ChooseArchiteBackground"));
				}
				else
				{
					yield return code;
				}
			}
		}

		public static CachedTexture WVC_ChooseArchiteBackground(GeneDef gene)
		{
			if (gene.GetModExtension<GeneExtension_Background>()?.backgroundPathArchite != null)
			{
				return new CachedTexture(gene.GetModExtension<GeneExtension_Background>().backgroundPathArchite);
			}
			return GraphicsCache.GeneBackground_Archite;
		}
	}

	// ===============================

	[HarmonyPatch(typeof(PawnGenerator))]
	[HarmonyPatch("GeneratePawnRelations")]
	public static class WVC_PawnGenerator_GeneratePawnRelations_Patch
	{
		[HarmonyPrefix]
		public static bool DisablePawnRelations(Pawn pawn)
		{
			// Pawn_GeneTracker genes = pawn.genes;
			// if (genes == null || !genes.HasGene(WVC_GenesDefOf.WVC_MaleOnly) || !genes.HasGene(WVC_GenesDefOf.WVC_FemaleOnly) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Blue) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Red) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Green) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Violet) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Yellow) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_Orange) || !genes.HasGene(WVC_GenesDefOf.WVC_NodeSkin_AltBlue))
			// {
				// return true;
			// }
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
