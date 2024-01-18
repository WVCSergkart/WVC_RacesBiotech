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
		public static class PawnGraphicSet_ResolveAllGraphics_Patch
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
				// List<Gene> genesListForReading = pawn.genes.GenesListForReading;
				// foreach (Gene item in genesListForReading)
				// {
					// if (!item.Active)
					// {
						// continue;
					// }
					// GeneExtension_Graphic modExtension = item.def.GetModExtension<GeneExtension_Graphic>();
					// if (modExtension == null || modExtension.furDef == null)
					// {
						// continue;
					// }
					// __instance.furCoveredGraphic = null;
					// if (modExtension.furIsSkinWithHair)
					// {
						// __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(modExtension.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
					// }
					// else if (modExtension.furIsSkin)
					// {
						// __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(modExtension.furDef.GetFurBodyGraphicPath(pawn), ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
					// }
				// }
				if (pawn.story?.furDef == null)
				{
					return;
				}
				Gene_Exoskin gene_Exoskin = pawn.genes.GetFirstGeneOfType<Gene_Exoskin>();
				if (gene_Exoskin != null)
				{
					GeneExtension_Graphic modExtension = gene_Exoskin.def.GetModExtension<GeneExtension_Graphic>();
					if (modExtension == null)
					{
						return;
					}
					__instance.furCoveredGraphic = null;
					if (modExtension.furIsSkinWithHair)
					{
						__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
					}
					else if (modExtension.furIsSkin)
					{
						__instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
					}
				}
			}
		}

	}

}
