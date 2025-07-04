using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_SkeletonSkin : PawnRenderNode
	{

		//protected override Shader DefaultShader => ShaderDatabase.CutoutSkinOverlay;

		public PawnRenderNode_SkeletonSkin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
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
			return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderDatabase.Cutout, Vector2.one, color: new(1f, 1f, 1f, 0.5f));
		}

		// public Graphic DefaultGraphic(Pawn pawn, string bodyPath)
		// {
			// return GraphicDatabase.Get<Graphic_Multi>(bodyPath, ShaderFor(pawn), Vector2.one, ColorFor(pawn));
		// }

		protected override string TexPathFor(Pawn pawn)
		{
			return pawn.story.bodyType?.bodyDessicatedGraphicPath;
		}

		//public override Color ColorFor(Pawn pawn)
		//{
		//	Color color = Color.white;
		//	color.a = 0.6f;
		//	return color;
		//}

	}


}
