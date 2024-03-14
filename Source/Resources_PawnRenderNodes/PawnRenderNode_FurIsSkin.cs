using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_FurIsSkin : PawnRenderNode
	{

		protected override Shader DefaultShader => ShaderDatabase.CutoutSkinOverlay;

		public PawnRenderNode_FurIsSkin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			string bodyPath = TexPathFor(pawn);
			if (bodyPath == null)
			{
				return null;
			}
			if (gene is not Gene_Exoskin gene_Exoskin)
			{
				return DefaultGraphic(pawn, bodyPath);
			}
			GeneExtension_Graphic modExtension = gene_Exoskin.Graphic;
			if (modExtension == null)
			{
				return DefaultGraphic(pawn, bodyPath);
			}
			if (modExtension.furIsSkinWithHair)
			{
				return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor, pawn.story.HairColor);
			}
			return DefaultGraphic(pawn, bodyPath);
		}

		protected override string TexPathFor(Pawn pawn)
		{
			return pawn?.story?.furDef?.GetFurBodyGraphicPath(pawn);
		}

		public override Color ColorFor(Pawn pawn)
		{
			return pawn.story.SkinColor;
		}

		public Graphic DefaultGraphic(Pawn pawn, string bodyPath)
		{
			return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderFor(pawn), Vector2.one, ColorFor(pawn));
		}

	}


}
