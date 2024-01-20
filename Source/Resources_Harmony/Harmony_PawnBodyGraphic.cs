using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{
	namespace HarmonyPatches
	{

		[HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
		public static class Patch_PawnGraphicSet_ResolveAllGraphics
		{
			[HarmonyPostfix]
			public static void Postfix(PawnGraphicSet __instance)
			{
				if (WVC_Biotech.settings.disableFurGraphic)
				{
					return;
				}
				Pawn pawn = __instance.pawn;
				if (!ModsConfig.BiotechActive || pawn == null || pawn.RaceProps?.Humanlike != true || pawn?.genes == null)
				{
					return;
				}
				if (pawn.story?.furDef == null)
				{
					return;
				}
				Gene_Exoskin gene_Exoskin = pawn.genes.GetFirstGeneOfType<Gene_Exoskin>();
				if (gene_Exoskin == null)
				{
					return;
				}
				GeneExtension_Graphic modExtension = gene_Exoskin.def.GetModExtension<GeneExtension_Graphic>();
				if (modExtension == null)
				{
					return;
				}
				__instance.furCoveredGraphic = null;
				string bodyPath = pawn.story.furDef.GetFurBodyGraphicPath(pawn);
				if (modExtension.furIsSkinWithHair)
				{
					__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
				}
				else if (modExtension.furIsSkin)
				{
					__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
				}
				if (modExtension.furCanRot)
				{
					__instance.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColorOverriden ? (PawnGraphicSet.RottingColorDefault * pawn.story.SkinColor) : PawnGraphicSet.RottingColorDefault);
				}
			}
		}

	}

}
