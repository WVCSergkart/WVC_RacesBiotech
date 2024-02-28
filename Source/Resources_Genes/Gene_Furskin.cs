using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	// WIP
	public class Gene_Exoskin : Gene
	{

		public GeneExtension_Graphic Graphic => def?.GetModExtension<GeneExtension_Graphic>();

		// public override void PostAdd()
		// {
			// base.PostAdd();
			// if (!Active)
			// {
				// return;
			// }
			// GeneExtension_Graphic modExtension = def.GetModExtension<GeneExtension_Graphic>();
			// if (modExtension == null)
			// {
				// PawnGraphicSet __instance = pawn.Drawer.renderer.graphics;
				// if (!__instance.AllResolved)
				// {
					// __instance.ResolveAllGraphics();
				// }
				// __instance.furCoveredGraphic = null;
				// string bodyPath = pawn.story.furDef.GetFurBodyGraphicPath(pawn);
				// if (modExtension.furIsSkinWithHair)
				// {
					// __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
				// }
				// else if (modExtension.furIsSkin)
				// {
					// __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
				// }
				// if (modExtension.furCanRot)
				// {
					// __instance.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColorOverriden ? (PawnGraphicSet.RottingColorDefault * pawn.story.SkinColor) : PawnGraphicSet.RottingColorDefault);
				// }
				// __instance.ClearCache();
			// }
		// }


	}

}
