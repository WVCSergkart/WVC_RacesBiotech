using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_XenotypesAndGenes
{

	public class PawnRenderNode_GlowingHair : PawnRenderNode_Hair
	{
		public PawnRenderNode_GlowingHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			if (base.GraphicFor(pawn) == null)
			{
				return null;
			}
			Color color = ColorFor(pawn);
			color.a = 0.6f;
			return GraphicDatabase.Get<Graphic_Multi>(base.GraphicFor(pawn).path, ShaderDatabase.MoteGlow, Vector2.one, color);
		}
	}

	public class PawnRenderNode_GlowingBeard : PawnRenderNode_Beard
	{
		public PawnRenderNode_GlowingBeard(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
			: base(pawn, props, tree)
		{
		}

		public override Graphic GraphicFor(Pawn pawn)
		{
			if (base.GraphicFor(pawn) == null)
			{
				return null;
			}
			Color color = ColorFor(pawn);
			color.a = 0.6f;
			return GraphicDatabase.Get<Graphic_Multi>(base.GraphicFor(pawn).path, ShaderDatabase.MoteGlow, Vector2.one, color);
		}
	}

	//public class PawnRenderNode_GlowingBodyHair : PawnRenderNode_Body
	//{
	//	public PawnRenderNode_GlowingBodyHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
	//		: base(pawn, props, tree)
	//	{
	//	}

	//	public override Graphic GraphicFor(Pawn pawn)
	//	{
	//		if (base.GraphicFor(pawn) == null)
	//		{
	//			return null;
	//		}
	//		Color color = pawn.story.HairColor;
	//		color.a = 0.6f;
	//		return GraphicDatabase.Get<Graphic_Multi>(base.GraphicFor(pawn).path, ShaderDatabase.MoteGlow, Vector2.one, Color.black, color);
	//	}
	//}


}
